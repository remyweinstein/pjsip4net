using System;
using pjsip4net.Core;
using pjsip4net.Core.Data;
using pjsip4net.Core.Interfaces;
using pjsip4net.Core.Interfaces.ApiProviders;
using pjsip4net.Core.Utils;
using pjsip4net.Interfaces;

namespace pjsip4net.IM
{
    public class Buddy : Initializable, IBuddyInternal, IIdentifiable<Buddy>
    {
        private readonly object _lock = new object();
        private IIMApiProvider _imApi;
        internal BuddyConfig _config;

        #region Properties

        public string Uri
        {
            get { return _config.Uri; }
            set
            {
                GuardNotInitializing();
                _config.Uri = value;
            }
        }

        public bool Subscribe
        {
            get { return _config.Subscribe; }
            set
            {
                GuardNotInitializing();
                _config.Subscribe = value;
            }
        }

        public string Contact
        {
            get { return GetInfo().Contact; }
        }

        public BuddyStatus Status
        {
            get { return GetInfo().Status; }
        }

        public string StatusText
        {
            get { return GetInfo().StatusText; }
        }

        public bool MonitoringPresence
        {
            get { return GetInfo().MonitorPresence; }
        }

        public BuddyActivity Activity
        {
            get { return (BuddyActivity) GetInfo().Rpid.Activity; }
        }

        public string ActivityNote
        {
            get { return GetInfo().Rpid.Note; }
        }

        public int Id { get; internal set; }

        #endregion

        #region Methods

        public Buddy(IIMApiProvider imApi)
        {
            Helper.GuardNotNull(imApi);
            _imApi = imApi;
            _config = _imApi.GetDefaultConfig();
            Id = -1;
        }

        public override void EndInit()
        {
            base.EndInit();
            Helper.GuardIsTrue(new SipUriParser(Uri).IsValid);
        }

        public void UpdatePresenceState()
        {
            GuardDisposed();
            _imApi.UpdatePresence(Id);
        }

        protected override void CleanUp()
        {
            base.CleanUp();
            Id = -1;
        }

        public BuddyConfig Config
        {
            get { return _config; }
        }

        public BuddyInfo GetInfo()
        {
            GuardDisposed();
            //lock (_lock)
            {
                if (Id != -1)
                    try
                    {
                        return _imApi.GetInfo(Id);
                    }
                    catch (PjsipErrorException)
                    {
                        return _imApi.GetInfo(Id);
                    }
                return null;
            }
        }

        public BuddyStateChangedEventArgs GetEventArgs()
        {
            var info = GetInfo();
            return new BuddyStateChangedEventArgs
                       {
                           Id = Id,
                           Uri = info.Uri,
                           StatusText = _isDisposed ? "" : info.StatusText,
                           Status = _isDisposed ? BuddyStatus.Unknown : info.Status,
                           Note = _isDisposed ? "unknown" : info.Rpid.Note
                       };
        }

        public void SetId(int id)
        {
            Helper.GuardPositiveInt(id);
            Id = id;
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

        //public static BuddyBuilder Create()
        //{
        //    return new BuddyBuilder();
        //}
    }
}