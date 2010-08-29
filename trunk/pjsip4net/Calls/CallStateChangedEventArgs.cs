using System;

namespace pjsip4net.Calls
{
    public class CallStateChangedEventArgs : EventArgs
    {
        public int Id { get; internal set; }
        public string DestinationUri { get; internal set; }
        public TimeSpan Duration { get; internal set; }
        public CallInviteState InviteState { get; internal set; }
        public CallMediaState MediaState { get; internal set; }
    }
}