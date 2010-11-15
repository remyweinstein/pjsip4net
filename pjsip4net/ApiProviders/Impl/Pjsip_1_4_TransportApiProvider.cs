using pjsip.Interop;

namespace pjsip4net.ApiProviders.Impl
{
    public class Pjsip_1_4_TransportApiProvider : ITransportApiProvider
    {
        #region ITransportApiProvider Members

        public void pjsua_transport_config_default(pjsua_transport_config cfg)
        {
            PJSUA_DLL.Transport.pjsua_transport_config_default(cfg);
        }

        public int pjsua_transport_create(pjsip_transport_type_e type, pjsua_transport_config cfg, ref int p_id)
        {
            return PJSUA_DLL.Transport.pjsua_transport_create(type, cfg, ref p_id);
        }

        public int pjsua_enum_transports(int[] id, ref uint count)
        {
            return PJSUA_DLL.Transport.pjsua_enum_transports(id, ref count);
        }

        public int pjsua_transport_get_info(int id, pjsua_transport_info info)
        {
            return PJSUA_DLL.Transport.pjsua_transport_get_info(id, info);
        }

        public int pjsua_transport_set_enable(int id, int enabled)
        {
            return PJSUA_DLL.Transport.pjsua_transport_set_enable(id, enabled);
        }

        public int pjsua_transport_close(int id, int force)
        {
            return PJSUA_DLL.Transport.pjsua_transport_close(id, force);
        }

        #endregion
    }
}