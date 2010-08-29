namespace pjsip4net.ApiProviders
{
    public interface IApiFactory
    {
        IBasicApiProvider GetBasicApi();
        ITransportApiProvider GetTransportApi();
        IAccountApiProvider GetAccountApi();
        ICallApiProvider GetCallApi();
        IIMApiProvider GetImApi();
        IMediaApiProvider GetMediaApi();
    }
}