using pjsip.Interop;

namespace pjsip4net.ApiProviders.Impl
{
    public class Pjsip_1_4_BasicApiProvider : IBasicApiProvider
    {
        #region IBasicApiProvider Members

        public void pjsua_config_default(pjsua_config cfg)
        {
            PJSUA_DLL.Basic.pjsua_config_default(cfg);
        }

        public int pjsua_init(pjsua_config ua_cfg, pjsua_logging_config log_cfg, pjsua_media_config media_cfg)
        {
            return PJSUA_DLL.Basic.pjsua_init(ua_cfg, log_cfg, media_cfg);
        }

        public void pjsua_logging_config_default(pjsua_logging_config cfg)
        {
            PJSUA_DLL.Basic.pjsua_logging_config_default(cfg);
        }

        //public void pjsua_msg_data_init(pjsua_msg_data msg_data)
        //{
        //    PJSUA_DLL.Basic.pjsua_msg_data_init(msg_data);
        //}

        public int pjsua_create()
        {
            return PJSUA_DLL.Basic.pjsua_create();
        }

        public int pjsua_start()
        {
            return PJSUA_DLL.Basic.pjsua_start();
        }

        public int pjsua_destroy()
        {
            return PJSUA_DLL.Basic.pjsua_destroy();
        }

        public int pjsua_handle_events(uint msec_timeout)
        {
            return PJSUA_DLL.Basic.pjsua_handle_events(msec_timeout);
        }

        public int pjsua_reconfigure_logging(pjsua_logging_config c)
        {
            return PJSUA_DLL.Basic.pjsua_reconfigure_logging(c);
        }

        public int pjsua_detect_nat_type()
        {
            return PJSUA_DLL.Basic.pjsua_detect_nat_type();
        }

        public int pjsua_get_nat_type(ref pj_stun_nat_type type)
        {
            return PJSUA_DLL.Basic.pjsua_get_nat_type(ref type);
        }

        public int pjsua_verify_sip_url(string url)
        {
            return PJSUA_DLL.Basic.pjsua_verify_sip_url(url);
        }

        public void pjsua_perror(string sender, string title, int status)
        {
            PJSUA_DLL.Basic.pjsua_perror(sender, title, status);
        }

        public void pjsua_dump(bool detail)
        {
            PJSUA_DLL.Basic.pjsua_dump(detail);
        }

        #endregion
    }
}