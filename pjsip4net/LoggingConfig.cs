using System;
using pjsip.Interop;
using pjsip4net.Utils;

namespace pjsip4net
{
    public class LoggingConfig : Initializable
    {
        internal pjsua_logging_config _logconfig = new pjsua_logging_config();

        internal LoggingConfig()
        {
            SipUserAgent.ApiFactory.GetBasicApi().pjsua_logging_config_default(_logconfig);
        }

        public bool LogMessages
        {
            get { return _logconfig.msg_logging == 1; }
            set
            {
                GuardNotInitializing();
                _logconfig.msg_logging = Convert.ToInt32(value);
            }
        }

        public uint LogLevel
        {
            get { return _logconfig.level; }
            set
            {
                GuardNotInitializing();
                _logconfig.level = value;
                _logconfig.console_level = value;
            }
        }

        public bool TraceAndDebug { get; set; }

        public override void EndInit()
        {
            base.EndInit();
            Helper.GuardInRange(0u, 5u, _logconfig.level);
        }
    }
}