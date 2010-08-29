using NUnit.Framework;
using pjsip4net.Utils;

namespace pjsip4net.Tests
{
    /// <summary>
    /// Summary description for EqualsTemplateIdentifiableTests
    /// </summary>
    [TestFixture]
    public class EqualsTemplateIdentifiableTests
    {
        public EqualsTemplateIdentifiableTests()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        class MyClass : IIdentifiable<MyClass>
        {
            public int MyProperty { get; set; }

            #region Implementation of IEquatable<IIdentifiable<MyClass>>

            public bool Equals(IIdentifiable<MyClass> other)
            {
                return EqualsTemplate.Equals(this, other);
            }

            #endregion

            #region Implementation of IIdentifiable<MyClass>

            public int Id { get; set; }

            public bool DataEquals(MyClass other)
            {
                return MyProperty.Equals(other.MyProperty);
            }

            #endregion
        }

        [Test]
        public void Equals_EqualObjects_ReturnsTrue()
        {
            var sut1 = new MyClass();
            var sut2 = new MyClass();
            Assert.IsTrue(sut1.Equals(sut2));
        }
        
        [Test]
        public void Equals_DiffIds_ReturnsFalse()
        {
            var sut1 = new MyClass();
            var sut2 = new MyClass(){Id = 1};
            Assert.IsFalse(sut1.Equals(sut2));
        }
        
        [Test]
        public void Equals_DiffProps_ReturnsFalse()
        {
            var sut1 = new MyClass();
            var sut2 = new MyClass(){MyProperty = 1};
            Assert.IsFalse(sut1.Equals(sut2));
        }
    }
}