using System.Collections.Generic;
using pjsip4net.Core;
using pjsip4net.Core.Data;
using pjsip4net.Core.Utils;

namespace pjsip4net.Interfaces
{
    internal interface ILocalRegistry
    {
        IVoIPTransport SipTransport { get; set; }
        IVoIPTransport RtpTransport { get; set; }
        UaConfig Config { get; set; }
        MediaConfig MediaConfig { get; set; }
        LoggingConfig LoggingConfig { get; set; }

        Tuple<TransportType, TransportConfig> TransportConfig { get; set; }
        IEnumerable<AccountConfig> AccountConfigs { get; set; }
    }
}