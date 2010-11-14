using System;

namespace pjsip4net.Interfaces
{
    public interface ISipUserAgent : IDisposable
    {
        IImManager ImManager { get; }
        ICallManager CallManager { get; }
        IAccountManager AccountManager { get; }
        IMediaManager MediaManager { get; }

        void DumpInfo(bool detail);
        void Destroy();
    }

    internal interface ISipUserAgentInternal : ISipUserAgent
    {
        void SetManagers(IImManager imManager, ICallManager callManager, IAccountManager accountManager,
                         IMediaManager mediaManager);
    }
}