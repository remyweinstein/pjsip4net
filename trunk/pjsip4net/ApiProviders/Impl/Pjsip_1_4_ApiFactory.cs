namespace pjsip4net.ApiProviders.Impl
{
    public class Pjsip_1_4_ApiFactory : IApiFactory
    {
        private static readonly IAccountApiProvider _account;
        private static readonly IBasicApiProvider _basic;
        private static readonly ICallApiProvider _call;
        private static readonly IIMApiProvider _im;
        private static readonly IMediaApiProvider _media;
        private static readonly ITransportApiProvider _transport;

        static Pjsip_1_4_ApiFactory()
        {
            _basic = new Pjsip_1_4_BasicApiProvider();
            _transport = new Pjsip_1_4_TransportApiProvider();
            _account = new Pjsip_1_4_AccountApiProvider();
            _call = new Pjsip_1_4_CallApiProvider();
            _im = new Pjsip_1_4_IMApiProvider();
            _media = new Pjsip_1_4_MediaApiProvider();
        }

        #region IApiFactory Members

        public IBasicApiProvider GetBasicApi()
        {
            return _basic;
        }

        public ITransportApiProvider GetTransportApi()
        {
            return _transport;
        }

        public IAccountApiProvider GetAccountApi()
        {
            return _account;
        }

        public ICallApiProvider GetCallApi()
        {
            return _call;
        }

        public IIMApiProvider GetImApi()
        {
            return _im;
        }

        public IMediaApiProvider GetMediaApi()
        {
            return _media;
        }

        #endregion
    }
}