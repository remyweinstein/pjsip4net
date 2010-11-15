﻿using NUnit.Framework;
using pjsip4net.Utils;

namespace pjsip4net.Tests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestFixture]
    public class MRUCacheTests
    {
        public MRUCacheTests()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        [Test]
        public void TryGetValue_AddValueWrapper_ReturnsWhatItWasProvided()
        {
            MruCache<ValueWrapper<int>, object> mruCache = new MruCache<ValueWrapper<int>, object>(12);

            object added = new object();
            ValueWrapper<int> key = new ValueWrapper<int>(0);

            mruCache.Add(key, added);

            object res;
            Assert.IsTrue(mruCache.TryGetValue(new ValueWrapper<int>(0), out res));
            Assert.AreEqual(added, res);
        }

        [Test]
        public void ValueWrapperEquals_ComapredToTheSame_ReturnsTrue()
        {
            ValueWrapper<int> vw1 = new ValueWrapper<int>(0);

            Assert.AreEqual(new ValueWrapper<int>(0), vw1);
        }

        [Test]
        public void ValueWrapperEquals_ComapredToTheOther_ReturnsFalse()
        {
            ValueWrapper<int> vw1 = new ValueWrapper<int>(0);

            Assert.IsFalse(vw1.Equals(new ValueWrapper<int>(1)));
        }

        [Test]
        public void TryGetValue_AddValueWrapperGetByAnother_ReturnsNull()
        {
            MruCache<ValueWrapper<int>, object> mruCache = new MruCache<ValueWrapper<int>, object>(12);

            object added = new object();
            ValueWrapper<int> key = new ValueWrapper<int>(0);

            mruCache.Add(key, added);

            object res;
            Assert.IsFalse(mruCache.TryGetValue(new ValueWrapper<int>(1), out res));
            Assert.IsNull(res);
        }

        [Test]
        public void TryGetValue_Add2ValueWrappers_ReturnsWhatItWasProvided()
        {
            MruCache<ValueWrapper<int>, object> mruCache = new MruCache<ValueWrapper<int>, object>(12);

            object added = new object();
            object added1 = new object();
            ValueWrapper<int> key = new ValueWrapper<int>(0);
            ValueWrapper<int> key1 = new ValueWrapper<int>(1);

            mruCache.Add(key, added);
            mruCache.Add(key1, added1);

            object res;
            Assert.IsTrue(mruCache.TryGetValue(new ValueWrapper<int>(1), out res));
            Assert.AreEqual(res, added1);
        }
    }
}