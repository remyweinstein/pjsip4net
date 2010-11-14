using System;
using pjsip4net.Core.Utils;

namespace pjsip4net.Calls
{
    public class RingEventArgs : EventArgs
    {
        public RingEventArgs(bool ringOn, Call call)
        {
            Helper.GuardNotNull(call);
            Helper.GuardPositiveInt(call.Id);
            RingOn = ringOn;
            IsRingback = call.IsIncoming;
            CallId = call.Id;
        }

        public bool RingOn { get; private set; }
        public bool IsRingback { get; private set; }
        public int CallId { get; private set; }
    }
}