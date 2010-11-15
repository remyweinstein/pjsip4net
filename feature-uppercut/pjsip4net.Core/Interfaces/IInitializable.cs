using System;

namespace pjsip4net.Core.Interfaces
{
    public interface IInitializable
    {
        bool IsInitialized { get; }
        IDisposable InitializationScope();
    }
}