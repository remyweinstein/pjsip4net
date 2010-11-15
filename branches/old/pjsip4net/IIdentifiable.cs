using System;

namespace pjsip4net
{
    public interface IIdentifiable<T> : IEquatable<IIdentifiable<T>>
    {
        int Id { get; }
        bool DataEquals(T other);
    }
}