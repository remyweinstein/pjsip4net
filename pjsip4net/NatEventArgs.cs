using System;
using pjsip.Interop;

namespace pjsip4net
{
    public enum NatType
    {
        Unknown,
        ErrorUnknown,
        Open,
        Blocked,
        SymmetricUdp,
        FullCone,
        Symmetric,
        Restricted,
        PortRestricted
    }

    public class NatEventArgs : EventArgs
    {
        internal NatEventArgs(pj_stun_nat_detect_result result)
        {
            StatusText = result.status_text;
            NatType = (NatType) result.nat_type;
            NatTypeName = result.nat_type_name;
        }

        public string StatusText { get; set; }
        public NatType NatType { get; set; }
        public string NatTypeName { get; set; }
    }
}