using System;
using NUnit.Framework;
using pjsip4net.Core.Interfaces;
using pjsip4net.Interfaces;
using Rhino.Mocks;

namespace pjsip4net.Tests
{
    [TestFixture]
    public class given_a_default_component_configurator
    {
        private DefaultComponentConfigurator _sut;
        private IContainer _container;

        [SetUp]
        public void TestSetup()
        {
            _sut = new DefaultComponentConfigurator();
            _container = MockRepository.GenerateMock<IContainer>();
        }

        [TearDown]
        public void Teardown()
        {
            _sut = null;
            _container = null;
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void when_configure_is_called_with_null_container_should_throw_exception()
        {
            _sut.Configure(null);
            Assert.Fail("Should have thrown an exception");
        }
        
        [Test]
        public void when_configure_is_called__should_register_DefaultObjectFactory()
        {
            _container.Expect(x => x.RegisterAsSingleton<IObjectFactory, DefaultObjectFactory>()).Repeat.Once();
            _sut.Configure(_container);
            _container.VerifyAllExpectations();
        }
    }
}