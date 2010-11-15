using System;
using pjsip.Interop;

namespace pjsip4net.ApiProviders
{
    public interface IAccountApiProvider
    {
        void pjsua_acc_config_default(pjsua_acc_config cfg);
        uint pjsua_acc_get_count();
        bool pjsua_acc_is_valid(int acc_id);
        int pjsua_acc_set_default(int acc_id);
        int pjsua_acc_get_default();
        int pjsua_acc_add(pjsua_acc_config acc_cfg, int is_default, ref int p_acc_id);
        int pjsua_acc_add_local(int tid, int is_default, ref int p_acc_id);
        int pjsua_acc_set_user_data(int acc_id, IntPtr user_data);
        IntPtr pjsua_acc_get_user_data(int acc_id);
        int pjsua_acc_del(int acc_id);
        int pjsua_acc_modify(int acc_id, pjsua_acc_config acc_cfg);
        int pjsua_acc_set_online_status(int acc_id, int is_online);
        int pjsua_acc_set_online_status2(int acc_id, int is_online, ref pjrpid_element pr);
        int pjsua_acc_set_registration(int acc_id, int renew);
        int pjsua_acc_get_info(int acc_id, ref pjsua_acc_info info);
        int pjsua_enum_accs(int[] ids, ref uint count);
        int pjsua_acc_enum_info(pjsua_acc_info[] info, ref uint count);
        int pjsua_acc_find_for_outgoing(ref pj_str_t url);
        int pjsua_acc_set_transport(int acc_id, int tp_id);
    }
}