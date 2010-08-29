using System;
using pjsip.Interop;
using pjsip4net.Utils;

namespace pjsip4net.Buddy
{
    public class Buddy : Initializable, IIdentifiable<Buddy>
    {
        private readonly object _lock = new object();
        internal pjsua_buddy_config _config = new pjsua_buddy_config();

        #region Properties

        public string Uri
        {
            get { return _config.uri; }
            set
            {
                GuardNotInitializing();
                _config.uri = new pj_str_t(value);
            }
        }

        public bool Subscribe
        {
            get { return Convert.ToBoolean(_config.subscribe); }
            set
            {
                GuardNotInitializing();
                _config.subscribe = Convert.ToInt32(value);
            }
        }

        public string Contact
        {
            get { return GetInfo().contact; }
        }

        public BuddyStatus Status
        {
            get { return (BuddyStatus) GetInfo().status; }
        }

        public string StatusText
        {
            get { return GetInfo().status_text; }
        }

        public bool MonitoringPresence
        {
            get { return Convert.ToBoolean(GetInfo().monitor_pres); }
        }

        public BuddyActivity Activity
        {
            get { return (BuddyActivity) GetInfo().rpid.activity; }
        }

        public String ActivityNote
        {
            get { return GetInfo().rpid.note; }
        }

        public int Id { get; internal set; }

        #endregion

        #region Methods

        public Buddy()
        {
            SipUserAgent.ApiFactory.GetImApi().pjsua_buddy_config_default(_config);
            Id = -1;
        }

        public override void EndInit()
        {
            base.EndInit();
            Helper.GuardError(SipUserAgent.ApiFactory.GetBasicApi().pjsua_verify_sip_url(_config.uri));
        }

        public void UpdatePresenceState()
        {
            Helper.GuardError(SipUserAgent.ApiFactory.GetImApi().pjsua_buddy_update_pres(Id));
        }

        protected override void CleanUp()
        {
            base.CleanUp();
            Id = NativeConstants.PJSUA_INVALID_ID;
        }

        internal pjsua_buddy_info GetInfo()
        {
            GuardDisposed();
            //lock (_lock)
            {
                var info = new pjsua_buddy_info();
                if (Id != NativeConstants.PJSUA_INVALID_ID)
                    try
                    {
                        Helper.GuardError(SipUserAgent.ApiFactory.GetImApi().pjsua_buddy_get_info(Id, ref info));
                    }
                    catch (PjsipErrorException)
                    {
                        Helper.GuardError(SipUserAgent.ApiFactory.GetImApi().pjsua_buddy_get_info(Id, ref info));
                    }
                return info;
            }
        }

        internal BuddyStateChangedEventArgs GetEventArgs()
        {
            var info = GetInfo();
            return new BuddyStateChangedEventArgs
                       {
                           Id = Id,
                           Uri = info.uri,
                           StatusText = _isDisposed ? "" : info.status_text,
                           Status = _isDisposed ? BuddyStatus.Unknown : (BuddyStatus) info.status,
                           Note = _isDisposed ? "unknown" : info.rpid.note
                       };
        }

        #endregion

        #region Implementation of IEquatable<IIdentifiable<Buddy>>

        public bool Equals(IIdentifiable<Buddy> other)
        {
            return EqualsTemplate.Equals(this, other);
        }

        bool IIdentifiable<Buddy>.DataEquals(Buddy other)
        {
            return Uri.Equals(other.Uri);
        }

        #endregion
    }
}