using System;

namespace pjsip4net.Utils
{
    public class EventArgs<T> : EventArgs
    {
        public EventArgs()
        {
        }

        public EventArgs(T data)
        {
            Helper.GuardNotNull(data);
            Data = data;
        }

        public T Data { get; set; }
    }
}