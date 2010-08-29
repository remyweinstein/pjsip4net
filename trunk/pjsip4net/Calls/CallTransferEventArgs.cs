using System;

namespace pjsip4net.Calls
{
    public class CallTransferEventArgs : EventArgs
    {
        public string Destination { get; internal set; }
        public int StatusCode { get; set; }
        public Call Call { get; internal set; }
    }
}