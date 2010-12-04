using pjsip4net.Accounts;
using pjsip4net.Accounts.Dsl;
using pjsip4net.Calls;
using pjsip4net.Calls.Dsl;
using pjsip4net.Core;
using pjsip4net.Core.Interfaces;
using pjsip4net.Core.Utils;
using pjsip4net.IM;
using pjsip4net.IM.Dsl;
using pjsip4net.Interfaces;
using pjsip4net.Media;
using pjsip4net.Transport;

namespace pjsip4net.Configuration
{
    public class DefaultComponentConfigurator : IConfigureComponents
    {
        #region Implementation of IConfigureComponents

        public void Configure(IContainer container)
        {
            Helper.GuardNotNull(container);
            container.RegisterAsSingleton<IObjectFactory, DefaultObjectFactory>();
            container.RegisterAsSingleton<ILocalRegistry, DefaultLocalRegistry>();
            container.RegisterAsSingleton(container.Get<ILocalRegistry>() as IConfigurationContext);

            //.RegisterAsSingleton<IApiFactory, Pjsip_1_4_ApiFactory>()

            container.RegisterAsSingleton<IImManager, DefaultImManager>();
            container.Register<IMessageBuilder, DefaultMessageBuilder>();

            container.RegisterAsSingleton<ISipUserAgent, DefaultSipUserAgent>();
            container.RegisterAsSingleton(container.Get<ISipUserAgent>() as ISipUserAgentInternal);
        }

        #endregion
    }
}