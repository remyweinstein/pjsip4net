using DoxWox.SIPUserAgent.Utils;
using NUnit.Framework;
using pjsip4net.ApiProviders;
using Rhino.Mocks;

namespace pjsip4net.Tests
{
    /// <summary>
    /// Summary description for SipUriParserTests
    /// </summary>
    [TestFixture]
    public class SipUriParserTests
    {
        public SipUriParserTests()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        [Test]
        public void Parse_ValidSipUriWithDomainAsIP_ValidPropertiesFilled()
        {
            SipUserAgent.ApiFactory = MockRepository.GenerateStub<IApiFactory>();
            var basicApi = MockRepository.GenerateStub<IBasicApiProvider>();
            SipUserAgent.ApiFactory.Stub(f => f.GetBasicApi()).Return(basicApi);
            basicApi.Stub(p => p.pjsua_verify_sip_url(Arg<string>.Is.Anything)).Return(0);

            var sut = new SipUriParser("<sip:12.25.12.1>");

            Assert.AreEqual("12.25.12.1", sut.Domain);
            Assert.AreEqual("", sut.Extension);
            Assert.AreEqual("5060", sut.Port);
            Assert.AreEqual("", sut.Password);
        }
        
        [Test]
        public void Parse_ValidSipUriWithDomainAsDns_ValidPropertiesFilled()
        {
            SipUserAgent.ApiFactory = MockRepository.GenerateStub<IApiFactory>();
            var basicApi = MockRepository.GenerateStub<IBasicApiProvider>();
            SipUserAgent.ApiFactory.Stub(f => f.GetBasicApi()).Return(basicApi);
            basicApi.Stub(p => p.pjsua_verify_sip_url(Arg<string>.Is.Anything)).Return(0);

            var sut = new SipUriParser("sip:doxwox.com");

            Assert.AreEqual("doxwox.com", sut.Domain);
            Assert.AreEqual("", sut.Extension);
            Assert.AreEqual("5060", sut.Port);
            Assert.AreEqual("", sut.Password);
        }
        
        [Test]
        public void Parse_ValidSipUriWithAlfabetExtensionAndDomainAsDns_ValidPropertiesFilled()
        {
            SipUserAgent.ApiFactory = MockRepository.GenerateStub<IApiFactory>();
            var basicApi = MockRepository.GenerateStub<IBasicApiProvider>();
            SipUserAgent.ApiFactory.Stub(f => f.GetBasicApi()).Return(basicApi);
            basicApi.Stub(p => p.pjsua_verify_sip_url(Arg<string>.Is.Anything)).Return(0);

            var sut = new SipUriParser("sip:test@doxwox.com");

            Assert.AreEqual("doxwox.com", sut.Domain);
            Assert.AreEqual("test", sut.Extension);
            Assert.AreEqual("5060", sut.Port);
            Assert.AreEqual("", sut.Password);
        }
        
        [Test]
        public void Parse_ValidSipUriWithAlfabetExtensionAndDomainAsDnsAndPort_ValidPropertiesFilled()
        {
            SipUserAgent.ApiFactory = MockRepository.GenerateStub<IApiFactory>();
            var basicApi = MockRepository.GenerateStub<IBasicApiProvider>();
            SipUserAgent.ApiFactory.Stub(f => f.GetBasicApi()).Return(basicApi);
            basicApi.Stub(p => p.pjsua_verify_sip_url(Arg<string>.Is.Anything)).Return(0);

            var sut = new SipUriParser("sip:test@doxwox.com:5080");

            Assert.AreEqual("doxwox.com", sut.Domain);
            Assert.AreEqual("test", sut.Extension);
            Assert.AreEqual("5080", sut.Port);
            Assert.AreEqual("", sut.Password);
        }
        
        [Test]
        public void Parse_ValidSipUriWithAlfabetExtensionAndPwdAndDomainAsDnsAndPort_ValidPropertiesFilled()
        {
            SipUserAgent.ApiFactory = MockRepository.GenerateStub<IApiFactory>();
            var basicApi = MockRepository.GenerateStub<IBasicApiProvider>();
            SipUserAgent.ApiFactory.Stub(f => f.GetBasicApi()).Return(basicApi);
            basicApi.Stub(p => p.pjsua_verify_sip_url(Arg<string>.Is.Anything)).Return(0);

            var sut = new SipUriParser("sip:test:test@doxwox.com:5080");

            Assert.AreEqual("doxwox.com", sut.Domain);
            Assert.AreEqual("test", sut.Extension);
            Assert.AreEqual("5080", sut.Port);
            Assert.AreEqual("test", sut.Password);
        }
        
        [Test]
        public void Parse_ValidSipUriWithAlfabetExtensionAndPwdAndDomainAsDnsAndPortWithTransportHeader_ValidPropertiesFilled()
        {
            SipUserAgent.ApiFactory = MockRepository.GenerateStub<IApiFactory>();
            var basicApi = MockRepository.GenerateStub<IBasicApiProvider>();
            SipUserAgent.ApiFactory.Stub(f => f.GetBasicApi()).Return(basicApi);
            basicApi.Stub(p => p.pjsua_verify_sip_url(Arg<string>.Is.Anything)).Return(0);

            var sut = new SipUriParser("sip:test:test@doxwox.com:5080;transport=tcp");

            Assert.AreEqual("doxwox.com", sut.Domain);
            Assert.AreEqual("test", sut.Extension);
            Assert.AreEqual("5080", sut.Port);
            Assert.AreEqual("test", sut.Password);
            Assert.AreEqual(1, sut.Headers.Count);
            Assert.AreEqual(TransportType.Tcp, sut.Transport);
        }
        
        [Test]
        public void Parse_ValidSipUriWithAlfabetDomainAsDnsWithTransportHeader_ValidPropertiesFilled()
        {
            SipUserAgent.ApiFactory = MockRepository.GenerateStub<IApiFactory>();
            var basicApi = MockRepository.GenerateStub<IBasicApiProvider>();
            SipUserAgent.ApiFactory.Stub(f => f.GetBasicApi()).Return(basicApi);
            basicApi.Stub(p => p.pjsua_verify_sip_url(Arg<string>.Is.Anything)).Return(0);

            var sut = new SipUriParser("sip:doxwox.com;transport=tcp");

            Assert.AreEqual("doxwox.com", sut.Domain);
            Assert.AreEqual("", sut.Extension);
            Assert.AreEqual("5060", sut.Port);
            Assert.AreEqual("", sut.Password);
            Assert.AreEqual(1, sut.Headers.Count);
            Assert.AreEqual(TransportType.Tcp, sut.Transport);
        }
        
        [Ignore]
        public void Parse_ValidSipUriWithAlfabetDomainAsDnsWithTransportHeaderAndSomaOtherHeader_ValidPropertiesFilled()
        {
            SipUserAgent.ApiFactory = MockRepository.GenerateStub<IApiFactory>();
            var basicApi = MockRepository.GenerateStub<IBasicApiProvider>();
            SipUserAgent.ApiFactory.Stub(f => f.GetBasicApi()).Return(basicApi);
            basicApi.Stub(p => p.pjsua_verify_sip_url(Arg<string>.Is.Anything)).Return(0);

            var sut = new SipUriParser("sip:doxwox.com;transport=tcp;otherheader");

            Assert.AreEqual("doxwox.com", sut.Domain);
            Assert.AreEqual("", sut.Extension);
            Assert.AreEqual("5060", sut.Port);
            Assert.AreEqual("", sut.Password);
            Assert.AreEqual(2, sut.Headers.Count);
            Assert.AreEqual(TransportType.Tcp, sut.Transport);
        }

    }
}