using System;
using pjsip.Interop;

namespace pjsip4net.ApiProviders.Impl
{
    public class Pjsip_1_4_IMApiProvider : IIMApiProvider
    {
        #region IIMApiProvider Members

        public void pjsua_buddy_config_default(pjsua_buddy_config cfg)
        {
            PJSUA_DLL.IM.pjsua_buddy_config_default(cfg);
        }

        public uint pjsua_get_buddy_count()
        {
            return PJSUA_DLL.IM.pjsua_get_buddy_count();
        }

        public bool pjsua_buddy_is_valid(int buddy_id)
        {
            return PJSUA_DLL.IM.pjsua_buddy_is_valid(buddy_id);
        }

        public int pjsua_enum_buddies(int[] ids, ref uint count)
        {
            return PJSUA_DLL.IM.pjsua_enum_buddies(ids, ref count);
        }

        public int pjsua_buddy_find(ref pj_str_t uri)
        {
            return PJSUA_DLL.IM.pjsua_buddy_find(ref uri);
        }

        public int pjsua_buddy_get_info(int buddy_id, ref pjsua_buddy_info info)
        {
            return PJSUA_DLL.IM.pjsua_buddy_get_info(buddy_id, ref info);
        }

        public int pjsua_buddy_set_user_data(int buddy_id, IntPtr user_data)
        {
            return PJSUA_DLL.IM.pjsua_buddy_set_user_data(buddy_id, user_data);
        }

        public IntPtr pjsua_buddy_get_user_data(int buddy_id)
        {
            return PJSUA_DLL.IM.pjsua_buddy_get_user_data(buddy_id);
        }

        public int pjsua_buddy_add(pjsua_buddy_config buddy_cfg, ref int p_buddy_id)
        {
            return PJSUA_DLL.IM.pjsua_buddy_add(buddy_cfg, ref p_buddy_id);
        }

        public int pjsua_buddy_del(int buddy_id)
        {
            return PJSUA_DLL.IM.pjsua_buddy_del(buddy_id);
        }

        public int pjsua_buddy_subscribe_pres(int buddy_id, int subscribe)
        {
            return PJSUA_DLL.IM.pjsua_buddy_subscribe_pres(buddy_id, subscribe);
        }

        public int pjsua_buddy_update_pres(int buddy_id)
        {
            return PJSUA_DLL.IM.pjsua_buddy_update_pres(buddy_id);
        }

        public int pjsua_pres_notify(int acc_id, IntPtr srv_pres, pjsip_evsub_state state, ref pj_str_t state_str,
                                     ref pj_str_t reason, int with_body, pjsua_msg_data msg_data)
        {
            return PJSUA_DLL.IM.pjsua_pres_notify(acc_id, srv_pres, state, ref state_str, ref reason, with_body,
                                                  msg_data);
        }

        public void pjsua_pres_dump(int verbose)
        {
            PJSUA_DLL.IM.pjsua_pres_dump(verbose);
        }

        public int pjsua_im_send(int acc_id, ref pj_str_t to, ref pj_str_t mime_type, ref pj_str_t content,
                                 pjsua_msg_data msg_data, IntPtr user_data)
        {
            return PJSUA_DLL.IM.pjsua_im_send(acc_id, ref to, ref mime_type, ref content, msg_data, user_data);
        }

        public int pjsua_im_typing(int acc_id, ref pj_str_t to, int is_typing, pjsua_msg_data msg_data)
        {
            return PJSUA_DLL.IM.pjsua_im_typing(acc_id, ref to, is_typing, msg_data);
        }

        #endregion
    }
}