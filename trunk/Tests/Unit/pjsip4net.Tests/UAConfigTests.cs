using System;
using NUnit.Framework;
using pjsip4net.Transport;

namespace pjsip4net.Tests
{
    /// <summary>
    /// Summary description for UAConfigTests
    /// </summary>
    [TestFixture]
    public class UAConfigTests
    {
        private static SipUserAgent _ua;

        public UAConfigTests()
        {
        }


        [Ignore]
        [ExpectedException(typeof(InvalidOperationException))]
        public void RegisterTpt_TwoEqualTpts_ThrowsException()
        {
            UaConfig config = new UaConfig();

            config.RegisterTransport(VoIPTransport.CreateTCPTransport());
            config.RegisterTransport(VoIPTransport.CreateTCPTransport());
        }
    }
}