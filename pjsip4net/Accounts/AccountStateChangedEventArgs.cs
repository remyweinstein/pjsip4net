using System;

namespace pjsip4net.Accounts
{
    public class AccountStateChangedEventArgs : EventArgs
    {
        public int Id { get; internal set; }
        public string Uri { get; internal set; }
        public string StatusText { get; internal set; }
        public int StatusCode { get; internal set; }
    }
}