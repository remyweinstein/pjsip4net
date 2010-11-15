using pjsip4net.Core.Data;
using pjsip4net.Core.Interfaces;

namespace pjsip4net.Media
{
    public class MediaComponentConfigurator : IConfigureComponents
    {
        #region Implementation of IConfigureComponents

        public void Configure(IContainer container)
        {
            container.Register<CodecInfo, CodecInfo>()
                .Register<WavPlayer, WavPlayer>();
                //.Register<Wav>();
        }

        #endregion
    }
}