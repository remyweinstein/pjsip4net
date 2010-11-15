using pjsip.Interop;

namespace pjsip4net.ApiProviders
{
    public interface IBasicApiProvider
    {
        void pjsua_config_default(pjsua_config cfg);
        int pjsua_init(pjsua_config ua_cfg, pjsua_logging_config log_cfg, pjsua_media_config media_cfg);
        void pjsua_logging_config_default(pjsua_logging_config cfg);
        //void pjsua_msg_data_init(pjsua_msg_data msg_data);
        int pjsua_create();
        int pjsua_start();
        int pjsua_destroy();
        int pjsua_handle_events(uint msec_timeout);
        int pjsua_reconfigure_logging(pjsua_logging_config c);
        int pjsua_detect_nat_type();
        int pjsua_get_nat_type(ref pj_stun_nat_type type);
        int pjsua_verify_sip_url(string url);
        void pjsua_perror(string sender, string title, int status);
        void pjsua_dump(bool detail);
    }
}