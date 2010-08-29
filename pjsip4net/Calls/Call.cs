using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using pjsip.Interop;
using pjsip4net.Accounts;
using pjsip4net.Calls.Dsl;
using pjsip4net.Utils;

namespace pjsip4net.Calls
{
    public class Call : Initializable, IIdentifiable<Call>
    {
        #region Private data

        private readonly object _lock = new object();
        private readonly MediaSession _mediaSession;
        private Account _account;
        private IDisposable _accountLock;
        internal InviteSession _inviteSession;

        #endregion

        #region Properties

        private string _destinationUri;

        public Account Account
        {
            get
            {
                GuardDisposed();
                return _account;
            }
            private set { _account = value; }
        }

        public string DestinationUri
        {
            get
            {
                GuardDisposed();
                return _destinationUri;
            }
            internal set
            {
                GuardDisposed();
                GuardNotInitializing();
                _destinationUri = value;
            }
        }

        public CallInviteState InviteState
        {
            get { return _inviteSession.InviteState; }
        }

        public CallMediaState MediaState
        {
            get { return _mediaSession.MediaState; }
        }

        public bool IsIncoming { get; private set; }

        public virtual bool IsActive
        {
            get
            {
                GuardDisposed();
                return Id != NativeConstants.PJSUA_INVALID_ID &&
                       SipUserAgent.ApiFactory.GetCallApi().pjsua_call_is_active(Id);
            }
        }

        public bool HasMedia
        {
            get
            {
                GuardDisposed();
                return Id != -1 && SipUserAgent.ApiFactory.GetCallApi().pjsua_call_has_media(Id) &&
                       _mediaSession.IsActive;
            }
        }

        public string LocalInfo
        {
            get
            {
                var info = GetCallInfo();
                return info.local_info;
            }
        }

        public string LocalContact
        {
            get
            {
                var info = GetCallInfo();
                return info.local_contact;
            }
        }

        public string RemoteInfo
        {
            get
            {
                var info = GetCallInfo();
                return info.remote_info;
            }
        }

        public string RemoteContact
        {
            get
            {
                var info = GetCallInfo();
                return info.remote_contact;
            }
        }

        public string DialogId
        {
            get
            {
                var info = GetCallInfo();
                return info.call_id;
            }
        }

        public string StateText
        {
            get
            {
                var info = GetCallInfo();
                return info.state_text;
            }
        }

        public uint LastStatusCode
        {
            get
            {
                var info = GetCallInfo();
                return (uint) info.last_status;
            }
        }

        public string LastStatusText
        {
            get
            {
                var info = GetCallInfo();
                return info.last_status_text;
            }
        }

        public int ConferenceSlotId
        {
            get
            {
                GuardDisposed();
                return GetCallInfo().conf_slot;
            }
        }

        public double RxLevel
        {
            get
            {
                GuardDisposed();
                uint rxLevel = 0, txLevel = 0;
                Helper.GuardError(SipUserAgent.ApiFactory.GetMediaApi().pjsua_conf_get_signal_level(ConferenceSlotId,
                                                                                                    ref txLevel,
                                                                                                    ref rxLevel));
                return rxLevel/255.0;
            }
            set
            {
                GuardDisposed();
                Helper.GuardInRange(0.0d, 1.0d, value);
                Helper.GuardError(SipUserAgent.ApiFactory.GetMediaApi().pjsua_conf_adjust_rx_level(ConferenceSlotId,
                                                                                                   (float) value));
            }
        }

        public double TxLevel
        {
            get
            {
                GuardDisposed();
                uint rxLevel = 0, txLevel = 0;
                Helper.GuardError(SipUserAgent.ApiFactory.GetMediaApi().pjsua_conf_get_signal_level(ConferenceSlotId,
                                                                                                    ref txLevel,
                                                                                                    ref rxLevel));
                return txLevel/255.0;
            }
            set
            {
                GuardDisposed();
                Helper.GuardInRange(0.0d, 1.0d, value);
                Helper.GuardError(SipUserAgent.ApiFactory.GetMediaApi().pjsua_conf_adjust_tx_level(ConferenceSlotId,
                                                                                                   (float) value));
            }
        }

        public TimeSpan ConnectDuration
        {
            get
            {
                var info = GetCallInfo();
                return info.connect_duration;
            }
        }

        public TimeSpan TotalDuration
        {
            get
            {
                var info = GetCallInfo();
                return info.total_duration;
            }
        }

        public int Id { get; internal set; }

        #endregion

        #region Methods

        internal Call(Account account)
            : this(account, NativeConstants.PJSUA_INVALID_ID)
        {
        }

        internal Call(Account account, int id)
        {
            Helper.GuardNotNull(account);
            Account = account;
            Id = id;

            _inviteSession = new InviteSession(this);
            _inviteSession.StateChanged += delegate { OnStateChanged(); };
            _mediaSession = new MediaSession(this);
            _mediaSession.StateChanged += delegate { OnStateChanged(); };

            pjsua_call_info info = GetCallInfo();
            IsIncoming = info.role == pjsip_role_e.PJSIP_ROLE_UAS;

            if (IsIncoming)
                _accountLock = Account.Lock();
        }

        public bool Equals(IIdentifiable<Call> other)
        {
            return EqualsTemplate.Equals(this, other);
        }

        bool IIdentifiable<Call>.DataEquals(Call other)
        {
            return true;
        }

        public static ToCallBuilderExpression MakeCall()
        {
            return new ToCallBuilderExpression(new CallBuilder());
        }

        public override void BeginInit()
        {
            GuardInitialized();
            base.BeginInit();
        }

        public override void EndInit()
        {
            GuardNotInitializing();
            base.EndInit();
            if (!IsIncoming)
                Helper.GuardError(SipUserAgent.ApiFactory.GetBasicApi().pjsua_verify_sip_url(DestinationUri));

            _accountLock = Account.Lock(); //if everything is ok
        }

        public void Hold()
        {
            GuardDisposed();
            if (_inviteSession.IsConfirmed && IsActive)
                Helper.GuardError(SipUserAgent.ApiFactory.GetCallApi().pjsua_call_set_hold(Id, null));
        }

        public void ReleaseHold()
        {
            GuardDisposed();
            if (_mediaSession.IsHeld) // media state should reflect correct state [unknown for now]
                Helper.GuardError(SipUserAgent.ApiFactory.GetCallApi().pjsua_call_reinvite(Id, Convert.ToInt32(true),
                                                                                           null));
        }

        public void Hangup()
        {
            Hangup("");
        }

        public void Hangup(string reason)
        {
            GuardDisposed();
            if (!_inviteSession.IsDisconnected)
            {
                var r = new pj_str_t(reason);
                Helper.GuardError(SipUserAgent.ApiFactory.GetCallApi().pjsua_call_hangup(Id,
                                                                                         (uint)
                                                                                         pjsip_status_code.
                                                                                             PJSIP_SC_DECLINE, ref r,
                                                                                         null));
            }
        }

        public void Answer(bool accept)
        {
            Answer(accept, "");
        }

        public void Answer(bool accept, string reason)
        {
            GuardDisposed();
            if (!IsIncoming)
                throw new InvalidOperationException("Can not answer on outgoing call");

            if (!_inviteSession.IsConfirmed)
            {
                var r = new pj_str_t(reason);
                var code = (uint)
                           (accept
                                ? pjsip_status_code.PJSIP_SC_OK
                                : pjsip_status_code.PJSIP_SC_DECLINE);
                Helper.GuardError(SipUserAgent.ApiFactory.GetCallApi().pjsua_call_answer(Id, code, ref r, null));
            }
        }

        public void ConnectToConference()
        {
            GuardDisposed();
            if (!_mediaSession.IsInConference)
                _mediaSession.ConnectToConference();
        }

        public void DisconnectFromConference()
        {
            GuardDisposed();
            if (_mediaSession.IsInConference)
                _mediaSession.DisconnectFromConference();
        }

        public void SendDtmf(string digits)
        {
            GuardDisposed();
            if (IsActive)
            {
                var digits1 = new pj_str_t(digits);
                Helper.GuardError(SipUserAgent.ApiFactory.GetCallApi().pjsua_call_dial_dtmf(Id, ref digits1));
            }
        }

        internal virtual pjsua_call_info GetCallInfo()
        {
            GuardDisposed();
            //lock (_lock)
            {
                var info = new pjsua_call_info();
                if (Id == NativeConstants.PJSUA_INVALID_ID)
                    return info;
                try
                {
                    Helper.GuardError(SipUserAgent.ApiFactory.GetCallApi().pjsua_call_get_info(Id, ref info));
                }
                catch (PjsipErrorException)
                {
                    Helper.GuardError(SipUserAgent.ApiFactory.GetCallApi().pjsua_call_get_info(Id, ref info));
                }
                return info;
            }
        }

        internal void HandleInviteStateChanged()
        {
            _inviteSession.HandleStateChanged();
        }

        internal void HandleMediaStateChanged()
        {
            _mediaSession.HandleStateChanged();
        }

        private void OnStateChanged()
        {
            SingletonHolder<ICallManagerInternal>.Instance.RaiseCallStateChanged(this);
        }

        public override string ToString()
        {
            return ToString(false);
        }

        public virtual string ToString(bool withMedia)
        {
            if (Id == NativeConstants.PJSUA_INVALID_ID)
                return base.ToString();

            IntPtr ptr = IntPtr.Zero;
            try
            {
                ptr = Marshal.AllocHGlobal(withMedia ? 2000 : 1000);
                Helper.GuardError(SipUserAgent.ApiFactory.GetCallApi().pjsua_call_dump(Id, Convert.ToInt32(withMedia),
                                                                                       ptr,
                                                                                       (uint) (withMedia ? 2000 : 1000),
                                                                                       " "));
                return "call data: " + Marshal.PtrToStringAnsi(ptr, withMedia ? 2000 : 1000);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

        #endregion

        #region Implementation of IDisposable

        protected override void CleanUp()
        {
            Debug.WriteLine("Call " + Id + " disposed");
            _accountLock.Dispose();
            _account = null;
            _accountLock = null;
            _inviteSession.Dispose();
            _mediaSession.Dispose();
        }

        #endregion
    }
}