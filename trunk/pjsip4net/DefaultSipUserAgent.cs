using System;
using pjsip4net.Core;
using pjsip4net.Core.Interfaces.ApiProviders;
using pjsip4net.Core.Utils;
using pjsip4net.Interfaces;

namespace pjsip4net
{
    public class DefaultSipUserAgent : Resource, ISipUserAgentInternal
    {
        private IBasicApiProvider _basicApi;

        public DefaultSipUserAgent(IBasicApiProvider basicApi)
        {
            Helper.GuardNotNull(basicApi);
            _basicApi = basicApi;
        }

        #region Implementation of IDisposable

        public void Dispose()
        {
            InternalDispose();
        }

        #endregion

        #region Implementation of ISipUserAgent

        public IImManager ImManager { get; private set; }
        public ICallManager CallManager { get; private set; }
        public IAccountManager AccountManager { get; private set; }
        public IMediaManager MediaManager { get; private set; }

        public void DumpInfo(bool detail)
        {
            _basicApi.Dump(detail);
        }

        public void Destroy()
        {
            InternalDispose();
        }

        public void SetManagers(IImManager imManager, ICallManager callManager, 
            IAccountManager accountManager, IMediaManager mediaManager)
        {
            Helper.GuardNotNull(imManager);
            Helper.GuardNotNull(callManager);
            Helper.GuardNotNull(accountManager);
            Helper.GuardNotNull(mediaManager);
            ImManager = imManager;
            CallManager = callManager;
            AccountManager = accountManager;
            MediaManager = mediaManager;
        }

        #endregion

        protected override void CleanUp()
        {
            base.CleanUp();
            ImManager = null;
            CallManager = null;
            AccountManager = null;
            MediaManager = null;

            _basicApi.Destroy();
        }
    }
}