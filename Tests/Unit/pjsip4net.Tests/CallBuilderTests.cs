﻿using System;
using NUnit.Framework;
using pjsip4net.Accounts;
using pjsip4net.Calls;
using pjsip4net.Calls.Dsl;
using pjsip4net.Transport;
using Rhino.Mocks;

namespace pjsip4net.Tests
{
    /// <summary>
    /// Summary description for CallBuilderTests
    /// </summary>
    [TestFixture]
    public class CallBuilderTests
    {
        public CallBuilderTests()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public class CallBuilderFake : CallBuilder
        {
            protected override Call InternalCall()
            {
                return new Call(_account);
            }
        }

        [Ignore]
        public void Call_WithCorrectData_CreatesCall()
        {
            MockRepository mocks = new MockRepository();
            Account accStub = mocks.DynamicMock<Account>(false);
            Expect.Call(accStub.Transport).Return(VoIPTransport.CreateTLSTransport());
            Expect.Call(accStub.Lock()).Return(mocks.Stub<IDisposable>());
            mocks.ReplayAll();

            accStub.Transport = VoIPTransport.CreateTLSTransport();

            Call call = new ToCallBuilderExpression(new CallBuilderFake()).To("1000").At("74.208.167.44")
                .Through("5081").From(accStub).Go();

            accStub.VerifyAllExpectations();
            Assert.AreEqual(accStub, call.Account);
        }
    }
}