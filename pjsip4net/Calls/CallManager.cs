using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using pjsip.Interop;
using pjsip4net.Accounts;
using pjsip4net.Utils;

namespace pjsip4net.Calls
{
    internal class CallManager : ICallManagerInternal
    {
        //#region Singleton

        private static readonly object _lock = new object();
        //private static CallManager _instance;

        public CallManager()
        {
            _barrier = new ManualResetEvent(true);
            //_syncContext = SynchronizationContext.Current;
        }

        #region Private data

        //private SynchronizationContext _syncContext;
        private readonly SortedDictionary<int, Call> _activeCalls = new SortedDictionary<int, Call>();
        private readonly ManualResetEvent _barrier;
        private MruCache<ValueWrapper<int>, CallStateChangedEventArgs> _eaCache;
        //private CallStateChangedEventArgs _ea = new CallStateChangedEventArgs();

        #endregion

        #region Properties

        private uint _maxCalls;
        public event EventHandler<EventArgs<Call>> IncomingCall = delegate { };
        public event EventHandler<CallStateChangedEventArgs> CallStateChanged = delegate { };
        public event EventHandler<DtmfEventArgs> IncomingDtmfDigit = delegate { };
        public event EventHandler<RingEventArgs> Ring = delegate { };
        public event EventHandler<CallTransferEventArgs> CallTransfer = delegate { };

        public ReadOnlyCollection<Call> Calls
        {
            get
            {
                lock (_lock)
                    return new ReadOnlyCollection<Call>(new List<Call>(_activeCalls.Values));
            }
        }

        public uint MaxCalls
        {
            get { return _maxCalls; }
            set
            {
                _maxCalls = value;
                _eaCache = new MruCache<ValueWrapper<int>, CallStateChangedEventArgs>((int) _maxCalls);
            }
        }

        #endregion

        #region Methods

        public Call MakeCall(string destinationUri)
        {
            return MakeCall(SingletonHolder<IAccountManager>.Instance.DefaultAccount, destinationUri);
        }

        public Call MakeCall(Account account, string destinationUri)
        {
            lock (_lock)
            {
                Helper.GuardNotNull(account);
                Helper.GuardInRange(0u, MaxCalls - 1, (uint) _activeCalls.Count);

                var result = new Call(account);
                using (result.InitializationScope())
                    result.DestinationUri = destinationUri;

                var uri = new pj_str_t(result.DestinationUri);
                int id = -1;
                Helper.GuardError(SipUserAgent.ApiFactory.GetCallApi().pjsua_call_make_call(result.Account.Id, ref uri,
                                                                                            0, IntPtr.Zero, null, ref id));
                result.Id = id;

                AddCallAndUpdateEaCache(destinationUri, result);

                result.HandleInviteStateChanged();

                _barrier.Reset();
                return result;
            }
        }

        public void HangupAll()
        {
            foreach (Call call in Calls)
                try
                {
                    call.Hangup();
                }
                catch (ObjectDisposedException)
                {
                }

            if (Calls.Count > 0)
                if (!_barrier.WaitOne(TimeSpan.FromSeconds(10.0)))
                    //throw new InvalidOperationException("Time out to wait for all calls to be deleted");
                    return; //close silently
        }

        public Call GetCallById(int id)
        {
            lock (_lock)
                if (_activeCalls.ContainsKey(id))
                    return _activeCalls[id];
            return null;
        }

        public void TerminateCall(Call call)
        {
            Helper.GuardNotNull(call);
            lock (_lock)
            {
                if (_activeCalls.ContainsKey(call.Id))
                    _activeCalls.Remove(call.Id);
                else
                    throw new InvalidOperationException("There is no call with id = " + call.Id +
                                                        " in active calls. Can not terminate.");

                call.InternalDispose();
                if (_activeCalls.Count == 0)
                    _barrier.Set();
            }
        }

        public void RaiseCallStateChanged(Call call)
        {
            CallStateChangedEventArgs ea;
            if (_eaCache.TryGetValue(new ValueWrapper<int>(call.Id), out ea))
            {
                ea.InviteState = call.InviteState;
                ea.MediaState = call.MediaState;
                //try
                //{
                //    if (_syncContext != null)
                //        _syncContext.Post(s => CallStateChanged(this, ea), null);
                //    else
                //        CallStateChanged(this, ea);
                //}
                //catch (InvalidOperationException)
                //{
                CallStateChanged(this, ea);
                //}
            }
        }

        public void RaiseRingEvent(Call call, bool ringOn)
        {
            //try
            //{
            call._inviteSession.IsRinging = true;
            //    if (_syncContext != null)
            //        _syncContext.Post(s => Ring(this, new RingEventArgs(ringOn, call)), null);
            //    else
            //        Ring(this, new RingEventArgs(ringOn, call));
            //}
            //catch (InvalidOperationException)
            //{
            Ring(this, new RingEventArgs(ringOn, call));
            //}
        }

        public void OnCallState(int call_id, ref IntPtr e)
        {
            lock (_lock)
                if (_activeCalls.ContainsKey(call_id) && _activeCalls[call_id] != null)
                {
                    CallStateChangedEventArgs ea;
                    if (_eaCache.TryGetValue(new ValueWrapper<int>(call_id), out ea))
                    {
                        ea.DestinationUri = _activeCalls[call_id].DestinationUri;
                        ea.Duration = _activeCalls[call_id].TotalDuration;
                    }
                    _activeCalls[call_id].HandleInviteStateChanged();
                }
        }

        public void OnIncomingCall(int acc_id, int call_id, IntPtr rdata)
        {
            Account account = SingletonHolder<IAccountManager>.Instance.Accounts.SingleOrDefault(a => a.Id == acc_id);
            if (account == null) // || !account.IsRegistered)
                account = SingletonHolder<IAccountManager>.Instance.DefaultAccount;

            Debug.WriteLine("incoming call for account " + account.AccountId);

            if (account != null)
            {
                Monitor.Enter(_lock);
                if (_activeCalls.Count < MaxCalls)
                {
                    var call = new Call(account, call_id);
                    AddCallAndUpdateEaCache(account.AccountId, call);
                    Monitor.Exit(_lock);
                    if (SipUserAgent.Instance.Config.AutoAnswer)
                    {
                        call.Answer(true);
                        if (_activeCalls.Count > 0)
                            _barrier.Reset();
                        return;
                    }

                    RaiseRingEvent(call, true);
                    var ea = new EventArgs<Call>(call);
                    //try
                    //{
                    //    if (_syncContext != null)
                    //        _syncContext.Post(delegate { IncomingCall(this, ea); }, null);
                    //    else
                    //        IncomingCall(this, ea);
                    //}
                    //catch (InvalidOperationException)
                    //{
                    IncomingCall(this, ea);
                    //}

                    if (_activeCalls.Count > 0)
                        _barrier.Reset();
                }
                else
                {
                    Monitor.Exit(_lock);
                    //Don't have to create an object here - simply closing this session
                    var reason = new pj_str_t("Too much calls");
                    Helper.GuardError(SipUserAgent.ApiFactory.GetCallApi().pjsua_call_answer(call_id,
                                                                                             (uint)
                                                                                             pjsip_status_code.
                                                                                                 PJSIP_SC_DECLINE,
                                                                                             ref reason, null));
                }
            }
        }

        public void OnCallMediaState(int call_id)
        {
            lock (_lock)
                if (_activeCalls.ContainsKey(call_id) && _activeCalls[call_id] != null)
                {
                    CallStateChangedEventArgs ea;
                    if (_eaCache.TryGetValue(new ValueWrapper<int>(call_id), out ea))
                    {
                        ea.DestinationUri = _activeCalls[call_id].DestinationUri;
                        ea.Duration = _activeCalls[call_id].TotalDuration;
                    }
                    _activeCalls[call_id].HandleMediaStateChanged();
                }
        }

        public void OnStreamDestroyed(int call_id, IntPtr sess, uint stream_idx)
        {
            lock (_lock)
                if (_activeCalls.ContainsKey(call_id) && _activeCalls[call_id] != null)
                    _activeCalls[call_id].HandleMediaStateChanged();
        }

        public void OnDtmfDigit(int call_id, int digit)
        {
            lock (_lock)
                if (_activeCalls.ContainsKey(call_id) && _activeCalls[call_id] != null)
                {
                    var ea = new DtmfEventArgs(_activeCalls[call_id], digit);
                    //try
                    //{
                    //    if (_syncContext != null)
                    //        _syncContext.Post(s => IncomingDtmfDigit(this, ea), null);
                    //    else
                    //        IncomingDtmfDigit(this, ea);
                    //}
                    //catch (InvalidOperationException)
                    //{
                    IncomingDtmfDigit(this, ea);
                    //}
                }
        }

        public void OnCallTransfer(int call_id, ref pj_str_t dst, ref pjsip_status_code code)
        {
            //string s = dst;
            //dst.ptr = new IntPtr(dst.ptr.ToInt32() + s.IndexOf("sip:"));
            //dst.slen = dst.slen - s.IndexOf("sip:") - 1;
            lock (_lock)
            {
                var ea = new CallTransferEventArgs
                             {Destination = dst, StatusCode = (int) code, Call = _activeCalls[call_id]};
                CallTransfer(this, ea);
                code = (pjsip_status_code) ea.StatusCode;
                //Marshal.FreeHGlobal(dst.ptr);
                //dst.ptr = Marshal.StringToHGlobalAnsi(ea.Destination);
                //dst.slen = ea.Destination.Length;
            }
        }

        public void OnCallTransferStatus(int call_id, int st_code, ref pj_str_t st_text, int final, ref int p_cont)
        {
            p_cont = 1;
        }

        private void AddCallAndUpdateEaCache(string destinationUri, Call call)
        {
            _activeCalls.Add(call.Id, call);
            CallStateChangedEventArgs ea;
            if (!_eaCache.TryGetValue(new ValueWrapper<int>(call.Id), out ea))
            {
                ea = new CallStateChangedEventArgs {Id = call.Id};
                _eaCache.Add(new ValueWrapper<int>(call.Id), ea);
            }
            ea.DestinationUri = destinationUri;
        }

        #endregion

        //internal static CallManager Instance
        //{
        //    get
        //    {
        //        if (_instance == null)
        //            lock(_lock)
        //                if (_instance == null)
        //                    _instance = new CallManager();
        //        return _instance;
        //    }
        //}

        //#endregion
    }
}