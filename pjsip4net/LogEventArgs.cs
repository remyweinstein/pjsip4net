using System;
using pjsip4net.Utils;

namespace pjsip4net
{
    public class LogEventArgs : EventArgs
    {
        public LogEventArgs(int level, string data)
        {
            Helper.GuardPositiveInt(level);
            Helper.GuardNotNullStr(data);
            Level = level;
            Data = data;
        }

        public int Level { get; private set; }
        public string Data { get; private set; }
    }
}