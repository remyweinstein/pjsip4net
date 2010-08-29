using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using pjsip.Interop;
using pjsip4net.Accounts;
using pjsip4net.ApiProviders;
using pjsip4net.ApiProviders.Impl;
using pjsip4net.Buddy;
using pjsip4net.Calls;
using pjsip4net.Configuration;
using pjsip4net.Media;
using pjsip4net.Transport;
using pjsip4net.Utils;

namespace pjsip4net
{
    public class SipUserAgent : Initializable, IDisposable
    {
        #region Singleton

        private static readonly object _lock = new object();
        private static SipUserAgent _instance;

        static SipUserAgent()
        {
            ApiFactory = new Pjsip_1_4_ApiFactory();

            SingletonHolder<IAccountManagerInternal>.SetInstanceInjector(() => new AccountManager());
            SingletonHolder<ICallManagerInternal>.SetInstanceInjector(() => new CallManager());
            SingletonHolder<IMediaManagerInternal>.SetInstanceInjector(() => new MediaManager());

            SingletonHolder<IAccountManager>.SetInstanceInjector(() => SingletonHolder<IAccountManagerInternal>.Instance);
            SingletonHolder<ICallManager>.SetInstanceInjector(() => SingletonHolder<ICallManagerInternal>.Instance);
            SingletonHolder<IMediaManager>.SetInstanceInjector(() => SingletonHolder<IMediaManagerInternal>.Instance);
        }

        private SipUserAgent()
        {
            Helper.GuardError(ApiFactory.GetBasicApi().pjsua_create());

            //_syncContext = SynchronizationContext.Current;
            //_sipTransport = VoIPTransport.CreateUDPTransport();
            _rtpTransport = VoIPTransport.CreateUDPTransport();

            Config = new UaConfig();
            LoggingConfig = new LoggingConfig();
            MediaConfig = new MediaConfig();

            ConfigurationProvider = new CfgFileConfigurationProvider();
        }

        public static SipUserAgent Instance
        {
            get
            {
                if (_instance == null)
                    lock (_lock)
                        if (_instance == null)
                            _instance = new SipUserAgent();
                return _instance;
            }
        }

        #endregion

        #region Private Data

        private readonly SortedDictionary<int, pjsip4net.Buddy.Buddy> _buddies = new SortedDictionary<int, pjsip4net.Buddy.Buddy>();
        private readonly object _instLock = new object();
        private pjsip4net.Buddy.Buddy _pendingBuddy;
        //private SipUserAgentSettingsSection _section;
        private VoIPTransport _rtpTransport;
        private VoIPTransport _sipTransport;
        //private SynchronizationContext _syncContext;

        #endregion

        #region Properties

        public static IApiFactory ApiFactory { get; set; }

        public IConfigurationProvider ConfigurationProvider { get; set; }

        public IAccountManager AccountManager
        {
            get { return !IsInitialized ? null : SingletonHolder<IAccountManager>.Instance; }
        }

        public ICallManager CallManager
        {
            get { return !IsInitialized ? null : SingletonHolder<ICallManager>.Instance; }
        }

        public IMediaManager MediaManager
        {
            get { return !IsInitialized ? null : SingletonHolder<IMediaManager>.Instance; }
        }

        public ConferenceBridge Conference
        {
            get { return !IsInitialized ? null : MediaManager.ConferenceBridge; }
        }

        public VoIPTransport SipTransport
        {
            get { return _sipTransport; }
        }

        public UaConfig Config { get; private set; }

        public LoggingConfig LoggingConfig { get; private set; }

        public MediaConfig MediaConfig { get; private set; }

        public ReadOnlyCollection<pjsip4net.Buddy.Buddy> Buddies
        {
            get
            {
                lock (_instLock)
                {
                    return _buddies.Values.ToList().AsReadOnly();
                }
            }
        }

        public event EventHandler<LogEventArgs> Log = delegate { };

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

            Config.BeginInit();
            LoggingConfig.BeginInit();
            MediaConfig.BeginInit();

            //_sipTransport.BeginInit();
            _rtpTransport.BeginInit();
            _rtpTransport.Port = 4000;

            if (ConfigurationProvider != null)
                ConfigurationProvider.Configure(Config, MediaConfig, LoggingConfig);
        }

        public override void EndInit()
        {
            base.EndInit();

            Config.EndInit();
            LoggingConfig.EndInit();

            _sipTransport = Config.Transports.Count > 0 ? Config.Transports[0] : VoIPTransport.CreateUDPTransport();

            CallManager.MaxCalls = Config.MaxCalls;

            //account state callback
            Config._config.cb.on_reg_state = SingletonHolder<IAccountManagerInternal>.Instance.OnRegistrationState;
            //call state callbacks
            Config._config.cb.on_call_state = SingletonHolder<ICallManagerInternal>.Instance.OnCallState;
            Config._config.cb.on_call_media_state = SingletonHolder<ICallManagerInternal>.Instance.OnCallMediaState;
            Config._config.cb.on_incoming_call = SingletonHolder<ICallManagerInternal>.Instance.OnIncomingCall;
            Config._config.cb.on_stream_destroyed = SingletonHolder<ICallManagerInternal>.Instance.OnStreamDestroyed;
            Config._config.cb.on_dtmf_digit = SingletonHolder<ICallManagerInternal>.Instance.OnDtmfDigit;
            Config._config.cb.on_call_transfer_request = SingletonHolder<ICallManagerInternal>.Instance.OnCallTransfer;
            Config._config.cb.on_call_transfer_status = SingletonHolder<ICallManagerInternal>.Instance.OnCallTransferStatus;

            Config._config.cb.on_nat_detect = OnNatDetect;
            Config._config.cb.on_buddy_state = OnBuddyState;
            Config._config.cb.on_incoming_subscribe = OnIncomingSubscribe;
            Config._config.cb.on_pager = OnPager;
            Config._config.cb.on_pager_status = OnPagerStatus;
            Config._config.cb.on_typing = OnTyping;

            //logging callback
            LoggingConfig._logconfig.AnonymousMember1 = OnLog;

            if (!_sipTransport.IsInitialized)
                _sipTransport.EndInit();
            _rtpTransport.EndInit();

            //if (MediaConfig.TurnAuthentication == null)
            //    MediaConfig.TurnAuthentication = new StunCredential {IsPasswordHashed = false};
            MediaConfig.EndInit();

            Helper.GuardError(ApiFactory.GetBasicApi().pjsua_init(Config._config, LoggingConfig._logconfig,
                                                                  MediaConfig._config));
        }

        private void OnNatDetect(ref pj_stun_nat_detect_result res)
        {
            pj_stun_nat_detect_result res1 = res;
            //try
            //{
            //    if (_syncContext != null)
            //        _syncContext.Send(s => NatDetected(this, new NatEventArgs(res1)), null);
            //    else
            //        NatDetected(this, new NatEventArgs(res1));
            //}
            //catch (InvalidOperationException)
            //{
            NatDetected(this, new NatEventArgs(res1));
            //}
        }

        private void OnBuddyState(int buddy_id)
        {
            Debug.WriteLine("SipUa.OnBuddyState entering. Thread id = " + Thread.CurrentThread.ManagedThreadId);
            lock (_instLock)
            {
                Debug.WriteLine("SipUa.OnBuddyState locked");
                Debug.WriteLine("SipUa.OnBuddyState about to raise event");
                if (_buddies.ContainsKey(buddy_id) && _buddies[buddy_id] != null)
                    RaiseBuddyState(_buddies[buddy_id]);
                else if (_pendingBuddy != null) //this is a new buddy it still does not have an Id
                    RaiseBuddyState(_pendingBuddy);
                Debug.WriteLine("SipUa.OnBuddyState raised event");
            }
            Debug.WriteLine("SipUa.OnBuddyState unlocked");
        }

        private void RaiseBuddyState(pjsip4net.Buddy.Buddy buddy)
        {
            //try
            //{
            //    if (_syncContext != null)
            //        _syncContext.Post(s => BuddyStateChanged(this, buddy.GetEventArgs()), null);
            //    else
            //        BuddyStateChanged(this, buddy.GetEventArgs());
            //}
            //catch (InvalidOperationException)
            //{
            BuddyStateChanged(this, buddy.GetEventArgs());
            //}
        }

        private void OnPager(int call_id, ref pj_str_t from, ref pj_str_t to,
                             ref pj_str_t contact, ref pj_str_t mime_type, ref pj_str_t body)
        {
            var ea = new PagerEventArgs(from, to, contact, mime_type, call_id, body);
            //try
            //{
            //    if (_syncContext != null)
            //        _syncContext.Post(s => IncomingMessage(this, ea), null);
            //    else
            //        IncomingMessage(this, ea);
            //}
            //catch (InvalidOperationException)
            //{
            IncomingMessage(this, ea);
            //}
        }

        private void OnPagerStatus(int call_id, ref pj_str_t to, ref pj_str_t body,
                                   IntPtr user_data, pjsip_status_code status, ref pj_str_t reason)
        {
            //try
            //{
            //    if (_syncContext != null)
            //        _syncContext.Post(s => Console.WriteLine("pager " + status), null);
            //    else
            //        Console.WriteLine("pager " + status);
            //}
            //catch (InvalidOperationException)
            //{
            Debug.WriteLine("pager " + status);
            //}
        }

        private void OnTyping(int call_id, ref pj_str_t from, ref pj_str_t to,
                              ref pj_str_t contact, int is_typing)
        {
            var ea = new TypingEventArgs(from, to, contact, call_id, Convert.ToBoolean(is_typing));
            //try
            //{
            //    if (_syncContext != null)
            //        _syncContext.Post(s => TypingAlert(this, ea), null);
            //    else
            //        TypingAlert(this, ea);
            //}
            //catch (InvalidOperationException)
            //{
            TypingAlert(this, ea);
            //}
        }

        private void OnIncomingSubscribe(int acc_id, IntPtr srv_pres, int buddy_id,
                                         ref pj_str_t from, IntPtr rdata, ref pjsip_status_code code,
                                         ref pj_str_t reason, pjsua_msg_data msg_data)
        {
            Debug.WriteLine("Incoming SUBSCRIBE");
        }

        private void OnLog(int level, string data, int len)
        {
            if (LoggingConfig.TraceAndDebug)
            {
                Trace.Write(data);
                //Debug.WriteLine(data);
            }
            if (level <= 2)
                Helper.LastError = data;

            //try
            //{
            //    if (_syncContext != null)
            //        _syncContext.Post(delegate { Log(this, new LogEventArgs(level, data)); }, null);
            //    else 
            //        Log(this, new LogEventArgs(level, data));
            //}
            //catch (InvalidOperationException)
            //{
            Log(this, new LogEventArgs(level, data));
            //}
        }

        public void RegisterBuddy(pjsip4net.Buddy.Buddy buddy)
        {
            Debug.WriteLine("SipUa.RegiterBuddy entering. Thread id = " + Thread.CurrentThread.ManagedThreadId);
            lock (_instLock)
            {
                Debug.WriteLine("SipUa.RegiterBuddy Locked");
                try
                {
                    _pendingBuddy = buddy;
                    int id = NativeConstants.PJSUA_INVALID_ID;
                    Debug.WriteLine("SipUa.RegiterBuddy About to call buddy_add");
                    Helper.GuardError(ApiFactory.GetImApi().pjsua_buddy_add(buddy._config, ref id));
                    Debug.WriteLine("SipUa.RegiterBuddy added buddy");
                    buddy.Id = id;
                    _buddies.Add(buddy.Id, buddy);
                    Debug.WriteLine("SipUa.RegiterBuddy About to call buddy_subscribe_pres");
                    Helper.GuardError(ApiFactory.GetImApi().pjsua_buddy_subscribe_pres(id, 1));
                    Debug.WriteLine("SipUa.RegiterBuddy subscribed presence");
                }
                finally
                {
                    _pendingBuddy = null;
                }
            }
            Debug.WriteLine("SipUa.RegiterBuddy unlocked");
        }

        public void UnregisterBuddy(pjsip4net.Buddy.Buddy buddy)
        {
            lock (_instLock)
            {
                if (buddy.Id != NativeConstants.PJSUA_INVALID_ID)
                {
                    Helper.GuardError(ApiFactory.GetImApi().pjsua_buddy_del(buddy.Id));
                    _buddies.Remove(buddy.Id);
                    buddy.InternalDispose();
                }
            }
        }

        public pjsip4net.Buddy.Buddy GetBuddyById(int id)
        {
            lock (_instLock)
            {
                if (_buddies.ContainsKey(id))
                    return _buddies[id];
                if (id == NativeConstants.PJSUA_INVALID_ID && _pendingBuddy != null)
                    return _pendingBuddy;
                return null;
            }
        }

        public void SendTyping(Account account, string to, bool isTyping)
        {
            var toto = new pj_str_t(to);
            Helper.GuardError(
                ApiFactory.GetImApi().pjsua_im_typing(account.Id, ref toto, Convert.ToInt32(isTyping), null));
        }

        public void SendMessage(Account account, string body, string to)
        {
            var toto = new pj_str_t(to);
            var bodybody = new pj_str_t(body);
            var mime = new pj_str_t("plain/text");
            Helper.GuardError(
                ApiFactory.GetImApi().pjsua_im_send(account.Id, ref toto, ref mime, ref bodybody, null, IntPtr.Zero));
        }

        public void SendTypingInDialog(Call dialog, bool isTyping)
        {
            Helper.GuardNotNull(dialog);
            Helper.GuardPositiveInt(dialog.Id);
            Helper.GuardError(
                ApiFactory.GetCallApi().pjsua_call_send_typing_ind(dialog.Id, Convert.ToInt32(isTyping), null));
        }

        public void SendMessageInDialog(Call dialog, string body)
        {
            var bodybody = new pj_str_t(body);
            var mime = new pj_str_t("plain/text");
            Helper.GuardError(
                ApiFactory.GetCallApi().pjsua_call_send_im(dialog.Id, ref mime, ref bodybody, null, IntPtr.Zero));
        }

        public void Start()
        {
            GuardDisposed();
            GuardNotInitialized();

            //create transport for all accounts
            int id = 0;
            Helper.GuardError(ApiFactory.GetTransportApi().pjsua_transport_create(_sipTransport._transportType,
                                                                                  _sipTransport._config, ref id));
            _sipTransport.Id = id;

            //create media transport
            //try
            {
                Helper.GuardError(ApiFactory.GetMediaApi().pjsua_media_transports_create(_rtpTransport._config));
            }
            //catch (OutOfMemoryException)
            //{
            //}

            //create media manager and configure
            SingletonHolder<IMediaManagerInternal>.Instance.SetConfiguration(MediaConfig);

            Helper.GuardError(ApiFactory.GetBasicApi().pjsua_start());
            /*Helper.GuardError(*/
            //PJSUA_DLL.Basic.pjsua_detect_nat_type()/*)*/;

            //create ringers
            //MediaManager.CreateRingers();

            //always register local account first
            AccountManager.RegisterAccount(new Account(true) {Transport = _sipTransport}, true);
            //TODO: generalize transport injection

            //register configured accounts
            IList<Account> preconfiguredAccounts = Config.GetPreConfiguredAccounts();
            if (preconfiguredAccounts.Count > 0)
                for (int i = 0; i < preconfiguredAccounts.Count; i++)
                {
                    preconfiguredAccounts[i].Transport = _sipTransport;
                    AccountManager.RegisterAccount(preconfiguredAccounts[i],
                                                   preconfiguredAccounts[i].Default);
                }
        }

        public void StoreConfiguration()
        {
            using (Config.InitializationScope())
            {
                Config.SetPreConfiguredAccounts(AccountManager.Accounts);
                if (Config.Transports.Count == 0)
                    Config.RegisterTransport(_sipTransport);
            }

            if (ConfigurationProvider != null)
                ConfigurationProvider.Store(Config, MediaConfig, LoggingConfig);
        }

        public void Destroy()
        {
            GuardDisposed();
            GuardNotInitialized();
            Dispose(true);
        }

        protected override void CleanUp()
        {
            lock (_lock)
            {
                try
                {
                    if (SingletonHolder<ICallManager>.Instance != null)
                        SingletonHolder<ICallManager>.Instance.HangupAll();
                }
                catch (InvalidOperationException)
                {
                }

                foreach (pjsip4net.Buddy.Buddy buddy in Buddies)
                    UnregisterBuddy(buddy);

                if (SingletonHolder<IAccountManagerInternal>.Instance != null)
                    SingletonHolder<IAccountManagerInternal>.Instance.UnRegisterAllAccounts();

                _rtpTransport.Id = NativeConstants.PJSUA_INVALID_ID;
                GC.SuppressFinalize(_rtpTransport);
                _rtpTransport = null;

                if (_sipTransport != null)
                    _sipTransport.InternalDispose();

                ApiFactory.GetBasicApi().pjsua_destroy();
                //pjsua is super smart and will clean other parts I forgot to close

                _instance = null;
            }
        }

        #endregion
    }
}