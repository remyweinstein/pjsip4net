using System;
using NUnit.Framework;
using pjsip.Interop;
using pjsip4net.Accounts;
using pjsip4net.ApiProviders;
using pjsip4net.Calls;
using pjsip4net.IM;
using pjsip4net.Transport;
using Rhino.Mocks;
using Is=Rhino.Mocks.Constraints.Is;

namespace pjsip4net.Tests
{
    /// <summary>
    /// Summary description for MessageBuilderTest
    /// </summary>
    [TestFixture]
    public class MessageBuilderTest
    {
        public MessageBuilderTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private MockRepository _mocks;

        [TestFixtureSetUp]
        public void MyTestInitialize() 
        {
            _mocks = new MockRepository();
            SipUserAgent.ApiFactory = _mocks.DynamicMock<IApiFactory>();
            var b = _mocks.Stub<IBasicApiProvider>();
            b.Stub(x => x.pjsua_config_default(Arg<pjsua_config>.Is.Anything));
            b.Stub(p => p.pjsua_verify_sip_url(null)).Constraints(Is.Anything()).Return(0); 
            SipUserAgent.ApiFactory.Stub(f => f.GetBasicApi()).Return(b);
            //SipUserAgent.ApiFactory.Stub(f => f.GetCallApi()).Return(MockRepository.GenerateStub<ICallApiProvider>());
            SipUserAgent.ApiFactory.Stub(f => f.GetMediaApi()).Return(MockRepository.GenerateStub<IMediaApiProvider>());
            SipUserAgent.ApiFactory.Stub(f => f.GetImApi()).Return(MockRepository.GenerateStub<IIMApiProvider>());
            SipUserAgent.ApiFactory.Stub(f => f.GetAccountApi()).Return(MockRepository.GenerateStub<IAccountApiProvider>());
            SipUserAgent.ApiFactory.Stub(f => f.GetTransportApi()).Return(MockRepository.GenerateStub<ITransportApiProvider>());
        }

        [TestFixtureTearDown]
        public void MyTestTeardown()
        {
            try
            {
                SipUserAgent.Instance.InternalDispose();
            }
            catch (InvalidOperationException)
            {
            }
        }

        [Test]
        public void MessageBuilder_CorrectArguments_CallsApi()
        {
            SipUserAgent.ApiFactory.Expect(f => f.GetImApi());

            Account a = new Account(false);
            a.Id = 0;
            a.Transport = VoIPTransport.CreateUDPTransport();
            InstantMessage.Send().To("1000").At("74.208.167.77").Through("5081").From(a).Typing(true);
            
            SipUserAgent.ApiFactory.VerifyAllExpectations();
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void MessageBuilder_EmptyDomain_ThrowsException()
        {
            InstantMessage.Send().To("").At("").Through("");
            Assert.Fail();// shouldn't get here
        }

        [Test]
        public void InDialogBuilder_CorrectArguments_CallsApi()
        {
            SipUserAgent.ApiFactory.Expect(f => f.GetCallApi()).Return(_mocks.Stub<ICallApiProvider>());
            //_mocks.ReplayAll();

            Call c = new Call(new Account(false), 0);
            using (c.InitializationScope()) { }

            InstantMessage.SendInDialog().Of(c).Go("sasa");
            
            SipUserAgent.ApiFactory.VerifyAllExpectations();
        }
    }
}