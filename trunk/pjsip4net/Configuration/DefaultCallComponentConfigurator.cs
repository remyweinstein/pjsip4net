using System;
using pjsip4net.Calls;
using pjsip4net.Calls.Dsl;
using pjsip4net.Core.Interfaces;
using pjsip4net.Core.Utils;
using pjsip4net.Interfaces;

namespace pjsip4net.Configuration
{
    public class DefaultCallComponentConfigurator : IConfigureComponents
    {
        public void Configure(IContainer container)
        {
            Helper.GuardNotNull(container);
            container.RegisterAsSingleton<ICallManager, DefaultCallManager>();
            container.RegisterAsSingleton(container.Get<ICallManager>() as ICallManagerInternal);
            container.Register<ICallBuilder, DefaultCallBuilder>();
        }
    }
}