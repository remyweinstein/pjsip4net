using pjsip4net.Core.Interfaces;

namespace pjsip4net.Interfaces
{
    public interface ICall : IInitializable
    { }

    internal interface ICallInternal : ICall, IResource
    { }
}