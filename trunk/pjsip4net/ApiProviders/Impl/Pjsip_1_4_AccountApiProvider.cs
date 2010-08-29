using System;
using pjsip.Interop;

namespace pjsip4net.ApiProviders.Impl
{
    public class Pjsip_1_4_AccountApiProvider : IAccountApiProvider
    {
        #region IAccountApiProvider Members

        public void pjsua_acc_config_default(pjsua_acc_config cfg)
        {
            PJSUA_DLL.Accounts.pjsua_acc_config_default(cfg);
        }

        public uint pjsua_acc_get_count()
        {
            return PJSUA_DLL.Accounts.pjsua_acc_get_count();
        }

        public bool pjsua_acc_is_valid(int acc_id)
        {
            return PJSUA_DLL.Accounts.pjsua_acc_is_valid(acc_id);
        }

        public int pjsua_acc_set_default(int acc_id)
        {
            return PJSUA_DLL.Accounts.pjsua_acc_set_default(acc_id);
        }

        public int pjsua_acc_get_default()
        {
            return PJSUA_DLL.Accounts.pjsua_acc_get_default();
        }

        public int pjsua_acc_add(pjsua_acc_config acc_cfg, int is_default, ref int p_acc_id)
        {
            return PJSUA_DLL.Accounts.pjsua_acc_add(acc_cfg, is_default, ref p_acc_id);
        }

        public int pjsua_acc_add_local(int tid, int is_default, ref int p_acc_id)
        {
            return PJSUA_DLL.Accounts.pjsua_acc_add_local(tid, is_default, ref p_acc_id);
        }

        public int pjsua_acc_set_user_data(int acc_id, IntPtr user_data)
        {
            return PJSUA_DLL.Accounts.pjsua_acc_set_user_data(acc_id, user_data);
        }

        public IntPtr pjsua_acc_get_user_data(int acc_id)
        {
            return PJSUA_DLL.Accounts.pjsua_acc_get_user_data(acc_id);
        }

        public int pjsua_acc_del(int acc_id)
        {
            return PJSUA_DLL.Accounts.pjsua_acc_del(acc_id);
        }

        public int pjsua_acc_modify(int acc_id, pjsua_acc_config acc_cfg)
        {
            return PJSUA_DLL.Accounts.pjsua_acc_modify(acc_id, acc_cfg);
        }

        public int pjsua_acc_set_online_status(int acc_id, int is_online)
        {
            return PJSUA_DLL.Accounts.pjsua_acc_set_online_status(acc_id, is_online);
        }

        public int pjsua_acc_set_online_status2(int acc_id, int is_online, ref pjrpid_element pr)
        {
            return PJSUA_DLL.Accounts.pjsua_acc_set_online_status2(acc_id, is_online, ref pr);
        }

        public int pjsua_acc_set_registration(int acc_id, int renew)
        {
            return PJSUA_DLL.Accounts.pjsua_acc_set_registration(acc_id, renew);
        }

        public int pjsua_acc_get_info(int acc_id, ref pjsua_acc_info info)
        {
            return PJSUA_DLL.Accounts.pjsua_acc_get_info(acc_id, ref info);
        }

        public int pjsua_enum_accs(int[] ids, ref uint count)
        {
            return PJSUA_DLL.Accounts.pjsua_enum_accs(ids, ref count);
        }

        public int pjsua_acc_enum_info(pjsua_acc_info[] info, ref uint count)
        {
            return PJSUA_DLL.Accounts.pjsua_acc_enum_info(info, ref count);
        }

        public int pjsua_acc_find_for_outgoing(ref pj_str_t url)
        {
            return PJSUA_DLL.Accounts.pjsua_acc_find_for_outgoing(ref url);
        }

        public int pjsua_acc_set_transport(int acc_id, int tp_id)
        {
            return PJSUA_DLL.Accounts.pjsua_acc_set_transport(acc_id, tp_id);
        }

        #endregion
    }
}