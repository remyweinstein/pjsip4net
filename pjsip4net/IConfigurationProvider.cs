using pjsip4net.Media;

namespace pjsip4net
{
    public interface IConfigurationProvider
    {
        void Configure(UaConfig config, MediaConfig mediaConfig, LoggingConfig loggingConfig);
        void Store(UaConfig config, MediaConfig mediaConfig, LoggingConfig loggingConfig);
    }
}