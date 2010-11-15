using System;
using pjsip4net.Core.Data;

namespace pjsip4net.Calls
{
    public class CallTransferEventArgs : EventArgs
    {
        public string Destination { get; internal set; }
        public SipStatusCode StatusCode { get; set; }
        public Call Call { get; internal set; }
    }
}