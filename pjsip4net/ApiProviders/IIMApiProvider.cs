using System;
using pjsip.Interop;

namespace pjsip4net.ApiProviders
{
    public interface IIMApiProvider
    {
        void pjsua_buddy_config_default(pjsua_buddy_config cfg);
        uint pjsua_get_buddy_count();
        bool pjsua_buddy_is_valid(int buddy_id);
        int pjsua_enum_buddies(int[] ids, ref uint count);
        int pjsua_buddy_find(ref pj_str_t uri);
        int pjsua_buddy_get_info(int buddy_id, ref pjsua_buddy_info info);
        int pjsua_buddy_set_user_data(int buddy_id, IntPtr user_data);
        IntPtr pjsua_buddy_get_user_data(int buddy_id);
        int pjsua_buddy_add(pjsua_buddy_config buddy_cfg, ref int p_buddy_id);
        int pjsua_buddy_del(int buddy_id);
        int pjsua_buddy_subscribe_pres(int buddy_id, int subscribe);
        int pjsua_buddy_update_pres(int buddy_id);

        int pjsua_pres_notify(int acc_id, IntPtr srv_pres, pjsip_evsub_state state,
                              ref pj_str_t state_str, ref pj_str_t reason, int with_body, pjsua_msg_data msg_data);

        void pjsua_pres_dump(int verbose);

        int pjsua_im_send(int acc_id, ref pj_str_t to, ref pj_str_t mime_type, ref pj_str_t content,
                          pjsua_msg_data msg_data, IntPtr user_data);

        int pjsua_im_typing(int acc_id, ref pj_str_t to, int is_typing,
                            pjsua_msg_data msg_data);
    }
}