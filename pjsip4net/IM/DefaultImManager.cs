using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using pjsip4net.Accounts;
using pjsip4net.Calls;
using pjsip4net.Core;
using pjsip4net.Core.Data.Events;
using pjsip4net.Core.Interfaces.ApiProviders;
using pjsip4net.Core.Utils;
using pjsip4net.Interfaces;

namespace pjsip4net.IM
{
    internal class DefaultImManager : Initializable, IImManager
    {
        #region Private Data

        private readonly IBasicApiProvider _basicApi;
        private readonly IIMApiProvider _imApi;
        private readonly ICallApiProvider _callApi;
        private readonly IEventsProvider _eventsProvider;
        private ILocalRegistry _localRegistry;

        private readonly SortedDictionary<int, IBuddyInternal> _buddies = new SortedDictionary<int, IBuddyInternal>();
        private readonly object _instLock = new object();
        private IBuddyInternal _pendingBuddy;
        //private IVoIPTransport _rtpTransport;
        //private IVoIPTransport _sipTransport; //move to build

        #endregion

        public DefaultImManager(ILocalRegistry localRegistry, IBasicApiProvider basicApi,
                           IIMApiProvider imApi, ICallApiProvider callApi, IEventsProvider eventsProvider)
        {
            Helper.GuardNotNull(localRegistry);
            Helper.GuardNotNull(basicApi);
            Helper.GuardNotNull(imApi);
            Helper.GuardNotNull(callApi);
            Helper.GuardNotNull(eventsProvider);
            _imApi = imApi;
            _callApi = callApi;
            _basicApi = basicApi;
            _eventsProvider = eventsProvider;
            _localRegistry = localRegistry; 
        }

        #region Properties

        public ReadOnlyCollection<IBuddy> Buddies
        {
            get
            {
                lock (_instLock)
                {
                    return _buddies.Values.Cast<IBuddy>().ToList().AsReadOnly();
                }
            }
        }

        public event EventHandler<BuddyStateChangedEventArgs> BuddyStateChanged = delegate { };
        public event EventHandler<PagerEventArgs> IncomingMessage = delegate { };
        public event EventHandler<TypingEventArgs> TypingAlert = delegate { };
        public event EventHandler<NatEventArgs> NatDetected = delegate { };

        #endregion

        #region Methods

        public void Dispose()
        {
            GuardDisposed();
            GuardNotInitialized();
            Dispose(true);
        }

        public override void BeginInit()
        {
            GuardInitialized();
            base.BeginInit();

            //Config.BeginInit();
            //LoggingConfig.BeginInit();
            //MediaConfig.BeginInit();

            //_sipTransport.BeginInit();

            //if (ConfigurationProvider != null)
            //    ConfigurationProvider.Configure(Config, MediaConfig, LoggingConfig);
        }

        public override void EndInit()
        {
            base.EndInit();

            //move to account mgr
            //Config._config.cb.on_reg_state = SingletonHolder<IAccountManagerInternal>.Instance.OnRegistrationState;
            //call state callbacks
            

            _eventsProvider.Subscribe<NatDetected>(e => OnNatDetect(e));
            _eventsProvider.Subscribe<BuddyStateChanged>(e => OnBuddyState(e));
            _eventsProvider.Subscribe<IncomingSubscribeRecieved>(e => OnIncomingSubscribe(e));
            _eventsProvider.Subscribe<IncomingImRecieved>(e => OnPager(e));
            _eventsProvider.Subscribe<ImStatusChanged>(e => OnPagerStatus(e));
            _eventsProvider.Subscribe<IncomingTypingRecieved>(e => OnTyping(e));
        }

        private void OnNatDetect(NatDetected e)
        {
            NatDetected(this, new NatEventArgs(e));
        }

        private void OnBuddyState(BuddyStateChanged e)
        {
            Debug.WriteLine("SipUa.OnBuddyState entering. Thread id = " + Thread.CurrentThread.ManagedThreadId);
            lock (_instLock)
            {
                Debug.WriteLine("SipUa.OnBuddyState locked");
                Debug.WriteLine("SipUa.OnBuddyState about to raise event");
                if (_buddies.ContainsKey(e.Id) && _buddies[e.Id] != null)
                    RaiseBuddyState(_buddies[e.Id]);
                else if (_pendingBuddy != null) //this is a new buddy it still does not have an Id
                    RaiseBuddyState(_pendingBuddy);
                Debug.WriteLine("SipUa.OnBuddyState raised event");
            }
            Debug.WriteLine("SipUa.OnBuddyState unlocked");
        }

        private void RaiseBuddyState(IBuddyInternal buddy)
        {
            BuddyStateChanged(this, buddy.GetEventArgs());
        }

        private void OnPager(IncomingImRecieved e)
        {
            var ea = new PagerEventArgs(e.From, e.To, e.Contact, e.MimeType, e.CallId, e.Body);
            IncomingMessage(this, ea);
        }

        private void OnPagerStatus(ImStatusChanged e)
        {
            Debug.WriteLine("pager " + e.Status);
        }

        private void OnTyping(IncomingTypingRecieved e)
        {
            var ea = new TypingEventArgs(e.From, e.To, e.Contact, e.CallId, e.IsTyping);
            TypingAlert(this, ea);
        }

        private void OnIncomingSubscribe(IncomingSubscribeRecieved e)
        {
            Debug.WriteLine("Incoming SUBSCRIBE");
        }

        public void RegisterBuddy(IBuddy buddy)
        {
            Debug.WriteLine("SipUa.RegiterBuddy entering. Thread id = " + Thread.CurrentThread.ManagedThreadId);
            var b = buddy.As<IBuddyInternal>();
            if (b != null)
                lock (_instLock)
                {
                    Debug.WriteLine("SipUa.RegiterBuddy Locked");
                    try
                    {
                        _pendingBuddy = b;
                        int id = -1;
                        Debug.WriteLine("SipUa.RegiterBuddy About to call buddy_add");
                        id = _imApi.AddBuddyAndGetId(b.Config);
                        Debug.WriteLine("SipUa.RegiterBuddy added buddy");
                        b.SetId(id);
                        _buddies.Add(b.Id, b);
                        Debug.WriteLine("SipUa.RegiterBuddy About to call buddy_subscribe_pres");
                        _imApi.SubscribeBuddyPresence(id);
                        Debug.WriteLine("SipUa.RegiterBuddy subscribed presence");
                    }
                    finally
                    {
                        _pendingBuddy = null;
                    }
                }
            Debug.WriteLine("SipUa.RegiterBuddy unlocked");
        }

        public void UnregisterBuddy(IBuddy buddy)
        {
            var b = buddy.As<IBuddyInternal>();
            lock (_instLock)
            {
                if (buddy.Id != -1)
                {
                    _imApi.DeleteBuddy(buddy.Id);
                    _buddies.Remove(buddy.Id);
                    b.InternalDispose();
                }
            }
        }

        public IBuddy GetBuddyById(int id)
        {
            lock (_instLock)
            {
                if (_buddies.ContainsKey(id))
                    return _buddies[id];
                if (id == -1 && _pendingBuddy != null)
                    return _pendingBuddy;
                return null;
            }
        }

        public void SendTyping(IAccount account, string to, bool isTyping)
        {
            _imApi.SendIsTyping(account.Id, to);
        }

        public void SendMessage(IAccount account, string body, string to)
        {
            _imApi.SendIm(account.Id, to, "plain/text", body);
        }

        public void SendTypingInDialog(ICall dialog, bool isTyping)
        {
            Helper.GuardNotNull(dialog);
            Helper.GuardPositiveInt(dialog.Id);
            _callApi.SendTypingInd(dialog.Id, isTyping);
        }

        public void SendMessageInDialog(ICall dialog, string body)
        {
            Helper.GuardNotNull(dialog);
            Helper.GuardPositiveInt(dialog.Id);
            _callApi.SendIm(dialog.Id, "plain/text", body);
        }

        //protected override void CleanUp()
        //{
        //    //lock (_lock)
        //    {
        //        try
        //        {
        //            if (SingletonHolder<ICallManager>.Instance != null)
        //                SingletonHolder<ICallManager>.Instance.HangupAll();
        //        }
        //        catch (InvalidOperationException)
        //        {
        //        }

        //        foreach (pjsip4net.Buddy.Buddy buddy in Buddies)
        //            UnregisterBuddy(buddy);

        //        if (SingletonHolder<IAccountManagerInternal>.Instance != null)
        //            SingletonHolder<IAccountManagerInternal>.Instance.UnRegisterAllAccounts();

        //        _rtpTransport.Id = NativeConstants.PJSUA_INVALID_ID;
        //        GC.SuppressFinalize(_rtpTransport);
        //        _rtpTransport = null;

        //        if (_sipTransport != null)
        //            _sipTransport.Dispose();

        //        _basicApi.pjsua_destroy();
        //        //pjsua is super smart and will clean other parts I forgot to close

        //    }
        //}

        #endregion
    }
}