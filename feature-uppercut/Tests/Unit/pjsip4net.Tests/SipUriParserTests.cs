using NUnit.Framework;
using pjsip4net.Core;
using pjsip4net.Core.Interfaces.ApiProviders;
using pjsip4net.Core.Utils;
using pjsip4net.IM;
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
            var basicApi = MockRepository.GenerateStub<IBasicApiProvider>();

            var sut = new SipUriParser("<sip:12.25.12.1>");

            Assert.AreEqual("12.25.12.1", sut.Domain);
            Assert.AreEqual("", sut.Extension);
            Assert.AreEqual("5060", sut.Port);
            Assert.AreEqual("", sut.Password);
        }
        
        [Test]
        public void Parse_ValidSipUriWithDomainAsDns_ValidPropertiesFilled()
        {
            var sut = new SipUriParser("sip:doxwox.com");

            Assert.AreEqual("doxwox.com", sut.Domain);
            Assert.AreEqual("", sut.Extension);
            Assert.AreEqual("5060", sut.Port);
            Assert.AreEqual("", sut.Password);
        }
        
        [Test]
        public void Parse_ValidSipUriWithAlfabetExtensionAndDomainAsDns_ValidPropertiesFilled()
        {
            var sut = new SipUriParser("sip:test@doxwox.com");

            Assert.AreEqual("doxwox.com", sut.Domain);
            Assert.AreEqual("test", sut.Extension);
            Assert.AreEqual("5060", sut.Port);
            Assert.AreEqual("", sut.Password);
        }
        
        [Test]
        public void Parse_ValidSipUriWithAlfabetExtensionAndDomainAsDnsAndPort_ValidPropertiesFilled()
        {
            var sut = new SipUriParser("sip:test@doxwox.com:5080");

            Assert.AreEqual("doxwox.com", sut.Domain);
            Assert.AreEqual("test", sut.Extension);
            Assert.AreEqual("5080", sut.Port);
            Assert.AreEqual("", sut.Password);
        }
        
        [Test]
        public void Parse_ValidSipUriWithAlfabetExtensionAndPwdAndDomainAsDnsAndPort_ValidPropertiesFilled()
        {
            var sut = new SipUriParser("sip:test:test@doxwox.com:5080");

            Assert.AreEqual("doxwox.com", sut.Domain);
            Assert.AreEqual("test", sut.Extension);
            Assert.AreEqual("5080", sut.Port);
            Assert.AreEqual("test", sut.Password);
        }
        
        [Test]
        public void Parse_ValidSipUriWithAlfabetExtensionAndPwdAndDomainAsDnsAndPortWithTransportHeader_ValidPropertiesFilled()
        {
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