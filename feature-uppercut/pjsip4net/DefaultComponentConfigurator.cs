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

namespace pjsip4net
{
    public class DefaultComponentConfigurator : IConfigureComponents
    {
        #region Implementation of IConfigureComponents

        public void Configure(IContainer container)
        {
            Helper.GuardNotNull(container);
            container
                .RegisterAsSingleton<IObjectFactory, DefaultObjectFactory>()
                .RegisterAsSingleton<ILocalRegistry, DefaultLocalRegistry>()
                .RegisterAsSingleton((IConfigurationContext) container.Get<ILocalRegistry>())

                //.RegisterAsSingleton<IApiFactory, Pjsip_1_4_ApiFactory>()

                .RegisterAsSingleton<IVoIPTransportFactory, DefaultVoIPTransportFactory>()
                .Register<IVoIPTransport, UdpTransport>(TransportType.Udp.ToString())
                .Register<IVoIPTransport, TcpTransport>(TransportType.Tcp.ToString())
                .Register<IVoIPTransport, TlsTransport>(TransportType.Tls.ToString())

                .RegisterAsSingleton<IAccountManager, DefaultAccountManager>()
                .RegisterAsSingleton((IAccountManagerInternal) container.Get<IAccountManager>())
                .Register<IAccountBuilder, AccountBuilder>()

                .RegisterAsSingleton<ICallManager, DefaultCallManager>()
                .RegisterAsSingleton((ICallManagerInternal) container.Get<ICallManager>())
                .Register<ICallBuilder, DefaultCallBuilder>()

                .RegisterAsSingleton<IConferenceBridge, DefaultConferenceBridge>()
                .RegisterAsSingleton<IMediaManager, DefaultMediaManager>()
                .RegisterAsSingleton((IMediaManagerInternal) container.Get<IMediaManager>())

                .RegisterAsSingleton<IImManager, DefaultImManager>()
                .Register<IMessageBuilder, DefaultMessageBuilder>()

                .RegisterAsSingleton<ISipUserAgent, DefaultSipUserAgent>()
                .RegisterAsSingleton((ISipUserAgentInternal) container.Get<ISipUserAgent>());
        }

        #endregion
    }
}