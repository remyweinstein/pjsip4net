using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace pjsip4net.Core.Utils
{
    public static class Extensions
    {
        public static IEnumerable<T> Each<T>(this IEnumerable<T> collection, Action<T> act)
        {
            foreach (var item in collection)
                act(item);
            return collection;
        }

        public static ConstructorInfo SelectEligibleConstructor(this Type type)
        {
            return (from c in type.GetConstructors()
                    orderby c.GetParameters().Length descending
                    select c).FirstOrDefault();
        }
    }
}