using System.Collections.ObjectModel;
using System.Linq;
using pjsip.Interop;
using pjsip4net.Utils;

namespace pjsip4net.Media
{
    public class MediaSlotInfo : IIdentifiable<MediaSlotInfo>
    {
        private pjsua_conf_port_info _info;

        #region Properties

        public string Name
        {
            get { return GetInfo().name; }
        }

        public uint ClockRate
        {
            get { return GetInfo().clock_rate; }
        }

        public uint ChannelCount
        {
            get { return GetInfo().clock_rate; }
        }

        public uint SamplesPerFrame
        {
            get { return GetInfo().samples_per_frame; }
        }

        public uint BitsPerSample
        {
            get { return GetInfo().bits_per_sample; }
        }

        public ReadOnlyCollection<MediaSlotInfo> Listeners
        {
            get
            {
                return new ReadOnlyCollection<MediaSlotInfo>(
                    GetInfo().listeners.Select(id => new MediaSlotInfo(id)).ToList());
            }
        }

        public int Id { get; private set; }

        #endregion

        public MediaSlotInfo(int id)
        {
            Helper.GuardPositiveInt(id);
            Id = id;
        }

        private pjsua_conf_port_info GetInfo()
        {
            try
            {
                Helper.GuardError(SipUserAgent.ApiFactory.GetMediaApi().pjsua_conf_get_port_info(Id, ref _info));
            }
            catch (PjsipErrorException)
            {
                Helper.GuardError(SipUserAgent.ApiFactory.GetMediaApi().pjsua_conf_get_port_info(Id, ref _info));
            }
            return _info;
        }

        #region Implementation of IEquatable<IIdentifiable<MediaSlotInfo>>

        public bool Equals(IIdentifiable<MediaSlotInfo> other)
        {
            return EqualsTemplate.Equals(this, other);
        }

        bool IIdentifiable<MediaSlotInfo>.DataEquals(MediaSlotInfo other)
        {
            return true;
        }

        #endregion

        //internal void Accept(ISlotVisitor visitor)
        //{
        //    visitor.VisitSlot(this);
        //}
    }
}