using NUnit.Framework;
using pjsip4net.Configuration;
using pjsip4net.Core.Configuration;
using pjsip4net.Core.Container;
using pjsip4net.Core.Interfaces;
using Rhino.Mocks;

namespace pjsip4net.Tests
{
    [TestFixture]
    public class given_a_configure
    {
        private Configure _sut;
        private IContainer _container;

        [SetUp]
        public void Setup()
        {
            _sut = Configure.Pjsip4Net();
            _container = MockRepository.GenerateMock<IContainer>();
        }

        [TearDown]
        public void Teardown()
        {
            _sut = null;
            _container = null;
        }

        [Test]
        public void when_ctor_called_then_it_should_create_a_simple_container()
        {
            Assert.That(_sut.Container, Is.InstanceOf(typeof(SimpleContainer)));
        }

        [Test]
        public void when_build_called_then_it_should_register_container_in_container()
        {
            _container.Expect(x => x.RegisterAsSingleton(Arg<IContainer>.Is.Equal(_sut.Container)));
            _sut.Build();
            _container.VerifyAllExpectations();
        }
    }
}