using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using pjsip.Interop;
using pjsip4net.Accounts.Dsl;
using pjsip4net.Transport;
using pjsip4net.Utils;

namespace pjsip4net.Accounts
{
    public class Account : Initializable, IIdentifiable<Account>
    {
        private readonly bool _isLocal;
        private readonly object _lock = new object();
        private readonly PjstrArrayWrapper _proxies;
        private readonly RegistrationSession _session;
        internal pjsua_acc_config _config = new pjsua_acc_config();
        private uint _lockCount;
        private VoIPTransport _transport;
        internal bool Default;

        #region Properties

        public string AccountId
        {
            get
            {
                GuardDisposed();
                return _config.id;
            }
            set
            {
                GuardDisposed();
                //bool modify = false;
                //try
                //{
                GuardNotInitializing();
                //}
                //catch (InvalidOperationException)
                //{
                //    modify = true;
                //}

                _config.id = new pj_str_t(value);
                //if (modify)
                //    Modify();
            }
        }

        public NetworkCredential Credential
        {
            get
            {
                GuardDisposed();
                return _config.cred_count > 0 ? _config.cred_info[0].ToNetworkCredential() : null;
            }
            set
            {
                GuardDisposed();
                //bool modify = false;
                //try
                //{
                GuardNotInitializing();
                //}
                //catch (InvalidOperationException)
                //{
                //    modify = true;
                //}
                if (value != null)
                {
                    _config.cred_info[0] = value.ToPjsipCredentialsInfo();
                    _config.cred_count = 1;
                }
                else _config.cred_count = 0;
                //if (modify)
                //    Modify();
            }
        }

        public SrtpRequirement UseSrtp
        {
            get
            {
                GuardDisposed();
                return (SrtpRequirement) _config.use_srtp;
            }
            set
            {
                GuardDisposed();
                GuardNotInitializing();
                _config.use_srtp = (pjmedia_srtp_use) value;
            }
        }

        public int SecureSignaling
        {
            get
            {
                GuardDisposed();
                return _config.srtp_secure_signaling;
            }
            set
            {
                GuardDisposed();
                GuardNotInitializing();
                _config.srtp_secure_signaling = value;
            }
        }

        public bool IsLocal
        {
            get
            {
                GuardDisposed();
                return _isLocal;
            }
        }

        public int Priority
        {
            get
            {
                GuardDisposed();
                return _config.priority;
            }
            set
            {
                GuardDisposed();
                //bool modify = false;
                //try
                //{
                GuardNotInitializing();
                //}
                //catch (InvalidOperationException)
                //{
                //    modify = true;
                //}

                _config.priority = value;
                //if (modify)
                //    Modify();
            }
        }

        public string RegistrarUri
        {
            get
            {
                GuardDisposed();
                return _config.reg_uri;
            }
            set
            {
                GuardDisposed();
                //bool modify = false;
                //try
                //{
                GuardNotInitializing();
                //}
                //catch (InvalidOperationException)
                //{
                //    modify = true;
                //}

                _config.reg_uri = new pj_str_t(value);

                string s = value.Remove(0, 4);
                if (s.Contains(":"))
                {
                    string[] splits = s.Split(new[] {':'});
                    RegistrarDomain = splits[0];
                    RegistrarPort = splits[1].Contains(";") ? splits[1].Split(new[] {';'})[0] : splits[1];
                }
                else
                {
                    RegistrarDomain = s;
                    RegistrarPort = "5060";
                }

                //if (modify)
                //    Modify();
            }
        }

        public string RegistrarDomain { get; internal set; }

        public string RegistrarPort { get; internal set; }

        public virtual VoIPTransport Transport
        {
            get { return _transport; }
            internal set
            {
                GuardDisposed();
                //GuardNotInitializing();
                _transport = value;
                //_config.transport_id = _transport.Id;
            }
        }

        public bool PublishPresence
        {
            get
            {
                GuardDisposed();
                return _config.publish_enabled == 1;
            }
            set
            {
                GuardDisposed();
                //bool modify = false;
                //try
                //{
                GuardNotInitializing();
                //}
                //catch (InvalidOperationException)
                //{
                //    modify = true;
                //}

                _config.publish_enabled = Convert.ToInt32(value);
                //if (modify)
                //    Modify();
            }
        }

        public ICollection<string> Proxies
        {
            get
            {
                GuardDisposed();
                return _proxies;
            }
        }

        public uint RegistrationTimeout
        {
            get
            {
                GuardDisposed();
                return _config.reg_timeout;
            }
            set
            {
                GuardDisposed();
                //bool modify = false;
                //try
                //{
                GuardNotInitializing();
                //}
                //catch (InvalidOperationException)
                //{
                //    modify = true;
                //}

                _config.reg_timeout = value;
                //if (modify)
                //    Modify();
            }
        }

        public uint KeepAliveInterval
        {
            get
            {
                GuardDisposed();
                return _config.ka_interval;
            }
            set
            {
                GuardDisposed();
                //bool modify = false;
                //try
                //{
                GuardNotInitializing();
                //}
                //catch (InvalidOperationException)
                //{
                //    modify = true;
                //}

                _config.ka_interval = value;
                //if (modify)
                //    Modify();
            }
        }

        public bool? IsOnline
        {
            get
            {
                GuardDisposed();
                var info = GetAccountInfo();
                return !Equals(info, default(pjsua_acc_info)) ? (bool?) (info.online_status == 1) : null;
            }
            //set
            //{
            //    GuardDisposed();
            //    if (IsRegistered)
            //    {
            //        //Helper.GuardError(SipUserAgent.ApiFactory.GetAccountApi().pjsua_acc_set_online_status(Id,
            //        //                                                                                      Convert.
            //        //                                                                                          ToInt32(
            //        //                                                                                          value.
            //        //                                                                                              Value)));
            //        pjrpid_element rpid = new pjrpid_element
            //                                  {
            //                                      activity = pjrpid_activity.PJRPID_ACTIVITY_UNKNOWN,
            //                                      note = new pj_str_t(value.Value ? "online" : "offline")
            //                                  };
            //        Helper.GuardError(SipUserAgent.ApiFactory.GetAccountApi()
            //            .pjsua_acc_set_online_status2(Id, Convert.ToInt32(value.Value), ref rpid));
            //        //if (!value.Value)
            //        //{
            //        //    pj_str_t stateStr = new pj_str_t();
            //        //    pj_str_t reason = new pj_str_t();
            //        //    Helper.GuardError(SipUserAgent.ApiFactory.GetImApi().pjsua_pres_notify(Id, IntPtr.Zero,
            //        //                                                                           pjsip_evsub_state.
            //        //                                                                               PJSIP_EVSUB_STATE_TERMINATED,
            //        //                                                                           ref stateStr, ref reason,
            //        //                                                                           0, null));
            //        //}
            //    }
            //}
        }

        public string OnlineStatusText
        {
            get
            {
                GuardDisposed();
                var info = GetAccountInfo();
                return !Equals(info, default(pjsua_acc_info)) ? info.online_status_text : "";
            }
        }

        public bool IsRegistered
        {
            get
            {
                GuardDisposed();
                return _session.IsRegistered;
            }
        }

        public bool? IsDefault
        {
            get
            {
                GuardDisposed();
                var info = GetAccountInfo();
                return !Equals(info, default(pjsua_acc_info)) ? (bool?) (info.is_default == 1) : null;
            }
        }

        public string Uri
        {
            get
            {
                GuardDisposed();
                var info = GetAccountInfo();
                return !Equals(info, default(pjsua_acc_info)) ? info.acc_uri : "";
            }
        }

        public bool? HasRegistration
        {
            get
            {
                GuardDisposed();
                var info = GetAccountInfo();
                return !Equals(info, default(pjsua_acc_info)) ? (bool?) (info.has_registration == 1) : null;
            }
        }

        public int? Expires
        {
            get
            {
                GuardDisposed();
                var info = GetAccountInfo();
                return !Equals(info, default(pjsua_acc_info)) ? (int?) info.expires : null;
            }
        }

        public int StatusCode
        {
            get
            {
                GuardDisposed();
                var info = GetAccountInfo();
                return !Equals(info, default(pjsua_acc_info)) ? (int) info.status : 0;
            }
        }

        public string StatusText
        {
            get
            {
                GuardDisposed();
                var info = GetAccountInfo();
                return !Equals(info, default(pjsua_acc_info)) ? info.status_text : "";
            }
        }

        internal bool IsInUse
        {
            get
            {
                lock (_lock)
                    return _lockCount != 0;
            }
        }

        public int Id { get; internal set; }

        #endregion

        #region Methods

        public Account(bool local)
            : this(local, false)
        {
        }

        public Account(bool local, bool @default)
        {
            Default = @default;
            _isLocal = local;
            _proxies = new PjstrArrayWrapper(_config.proxy);
            Id = -1;
            _session = new RegistrationSession(this);
            _session.StateChanged += delegate { OnRegistrationStateChanged(); };
        }

        public static WithAccountBuilderExpression Register()
        {
            return new WithAccountBuilderExpression(new AccountBuilder());
        }

        public override void BeginInit()
        {
            base.BeginInit();
            SipUserAgent.ApiFactory.GetAccountApi().pjsua_acc_config_default(_config);
            _proxies.BeginInit();
        }

        public override void EndInit()
        {
            base.EndInit();
            _config.proxy_cnt = (uint) _proxies.Count;
            Helper.GuardPositiveInt(Priority);
            if (!_isLocal)
            {
                Helper.GuardNotNullStr(AccountId);
                Helper.GuardNotNullStr(RegistrarUri);
                Helper.GuardError(SipUserAgent.ApiFactory.GetBasicApi().pjsua_verify_sip_url(AccountId));
                Helper.GuardError(SipUserAgent.ApiFactory.GetBasicApi().pjsua_verify_sip_url(RegistrarUri));
                if (_config.cred_count != 0)
                    Helper.GuardNotNull(Credential);
            }
            _proxies.EndInit();
        }

        public void PublishOnline(string note)
        {
            if (!PublishPresence)
                return;

            var rpid = new pjrpid_element
                           {
                               activity = pjrpid_activity.PJRPID_ACTIVITY_UNKNOWN,
                               note = new pj_str_t(note)
                           };
            Helper.GuardError(SipUserAgent.ApiFactory.GetAccountApi()
                                  .pjsua_acc_set_online_status2(Id, Convert.ToInt32(1), ref rpid));
        }

        public void RenewRegistration()
        {
            GuardDisposed();
            if (!_session.IsRegistered && Id != NativeConstants.PJSUA_INVALID_ID && !IsLocal)
            {
                Helper.GuardError(SipUserAgent.ApiFactory.GetAccountApi().pjsua_acc_set_registration(Id,
                                                                                                     Convert.ToInt32(
                                                                                                         true)));
                _session.HandleStateChanged();
            }
        }

        //public void SetRemoteRegisteration()
        //{
        //    GuardNotInitialized();
        //    if (!_isLocal && !IsRegistered)
        //    {
        //        Helper.GuardError(PJSUA_DLL.Accounts.pjsua_acc_set_registration(Id, true));
        //        _session.ChangeState(new RegisteringAccountState(_session));//there won't be any callback triggers here
        //    }
        //}

        //public void Unregister()
        //{
        //    GuardNotInitialized();
        //    if (!_isLocal && HasRegistration.HasValue && HasRegistration.Value && IsRegistered)
        //        //should be a callback trigger after to switch states
        //        Helper.GuardError(PJSUA_DLL.Accounts.pjsua_acc_set_registration(Id, false));
        //}

        internal void HandleStateChanged()
        {
            _session.HandleStateChanged();
        }

        protected void OnRegistrationStateChanged()
        {
            SingletonHolder<IAccountManagerInternal>.Instance.RaiseStateChanged(this);
        }

        internal pjsua_acc_info GetAccountInfo()
        {
            lock (_lock)
            {
                var info = new pjsua_acc_info();
                if (Id != NativeConstants.PJSUA_INVALID_ID)
                    try
                    {
                        Helper.GuardError(SipUserAgent.ApiFactory.GetAccountApi().pjsua_acc_get_info(Id, ref info));
                    }
                    catch (PjsipErrorException)
                    {
                        Helper.GuardError(SipUserAgent.ApiFactory.GetAccountApi().pjsua_acc_get_info(Id, ref info));
                    }
                return info;
            }
        }

        //protected virtual void Modify()
        //{
        //    Helper.GuardError(SipUserAgent.ApiFactory.GetAccountApi().pjsua_acc_modify(Id, _config));
        //}

        internal AccountStateChangedEventArgs GetEventArgs()
        {
            var info = GetAccountInfo();
            return new AccountStateChangedEventArgs
                       {
                           Id = Id,
                           Uri = _config.id,
                           StatusText = _isDisposed ? "" : info.status_text,
                           StatusCode = _isDisposed ? -1 : (int) info.status
                       };
        }

        private void AddRef()
        {
            lock (_lock)
                _lockCount++;
        }

        private void Release()
        {
            lock (_lock)
                _lockCount--;
        }

        internal virtual IDisposable Lock()
        {
            return new AccountLock(this);
        }

        protected override void CleanUp()
        {
            Debug.WriteLine("Account " + Id + " diposed");
            _session.Dispose();
        }

        #endregion

        #region Implementation of IEquatable<IIdentifiable<Account>>

        public bool Equals(IIdentifiable<Account> other)
        {
            return EqualsTemplate.Equals(this, other);
        }

        bool IIdentifiable<Account>.DataEquals(Account other)
        {
            return AccountId.Equals(other.AccountId);
        }

        #endregion

        #region Nested type: AccountLock

        private class AccountLock : IDisposable
        {
            private Account _owner;

            public AccountLock(Account owner)
            {
                Helper.GuardNotNull(owner);
                _owner = owner;
                _owner.AddRef();
            }

            #region IDisposable Members

            public void Dispose()
            {
                _owner.Release();
                _owner = null;
            }

            #endregion
        }

        #endregion
    }
}