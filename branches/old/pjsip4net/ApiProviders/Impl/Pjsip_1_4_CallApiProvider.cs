using System;
using pjsip.Interop;

namespace pjsip4net.ApiProviders.Impl
{
    public class Pjsip_1_4_CallApiProvider : ICallApiProvider
    {
        #region ICallApiProvider Members

        public uint pjsua_call_get_max_count()
        {
            return PJSUA_DLL.Calls.pjsua_call_get_max_count();
        }

        public uint pjsua_call_get_count()
        {
            return PJSUA_DLL.Calls.pjsua_call_get_count();
        }

        public int pjsua_enum_calls(int[] ids, ref uint count)
        {
            return PJSUA_DLL.Calls.pjsua_enum_calls(ids, ref count);
        }

        public int pjsua_call_make_call(int acc_id, ref pj_str_t dst_uri, uint options, IntPtr user_data,
                                        pjsua_msg_data msg_data, ref int p_call_id)
        {
            return PJSUA_DLL.Calls.pjsua_call_make_call(acc_id, ref dst_uri, options, user_data, msg_data, ref p_call_id);
        }

        public bool pjsua_call_is_active(int call_id)
        {
            return PJSUA_DLL.Calls.pjsua_call_is_active(call_id);
        }

        public bool pjsua_call_has_media(int call_id)
        {
            return PJSUA_DLL.Calls.pjsua_call_has_media(call_id);
        }

        public int pjsua_call_get_conf_port(int call_id)
        {
            return PJSUA_DLL.Calls.pjsua_call_get_conf_port(call_id);
        }

        public int pjsua_call_get_info(int call_id, ref pjsua_call_info info)
        {
            return PJSUA_DLL.Calls.pjsua_call_get_info(call_id, ref info);
        }

        public int pjsua_call_set_user_data(int call_id, IntPtr user_data)
        {
            return PJSUA_DLL.Calls.pjsua_call_set_user_data(call_id, user_data);
        }

        public IntPtr pjsua_call_get_user_data(int call_id)
        {
            return PJSUA_DLL.Calls.pjsua_call_get_user_data(call_id);
        }

        public int pjsua_call_get_rem_nat_type(int call_id, ref pj_stun_nat_type p_type)
        {
            return PJSUA_DLL.Calls.pjsua_call_get_rem_nat_type(call_id, ref p_type);
        }

        public int pjsua_call_answer(int call_id, uint code, ref pj_str_t reason, pjsua_msg_data msg_data)
        {
            return PJSUA_DLL.Calls.pjsua_call_answer(call_id, code, ref reason, msg_data);
        }

        public int pjsua_call_hangup(int call_id, uint code, ref pj_str_t reason, pjsua_msg_data msg_data)
        {
            return PJSUA_DLL.Calls.pjsua_call_hangup(call_id, code, ref reason, msg_data);
        }

        public int pjsua_call_process_redirect(int call_id, pjsip_redirect_op cmd)
        {
            return PJSUA_DLL.Calls.pjsua_call_process_redirect(call_id, cmd);
        }

        public int pjsua_call_set_hold(int call_id, pjsua_msg_data msg_data)
        {
            return PJSUA_DLL.Calls.pjsua_call_set_hold(call_id, msg_data);
        }

        public int pjsua_call_reinvite(int call_id, int unhold, pjsua_msg_data msg_data)
        {
            return PJSUA_DLL.Calls.pjsua_call_reinvite(call_id, unhold, msg_data);
        }

        public int pjsua_call_update(int call_id, uint options, pjsua_msg_data msg_data)
        {
            return PJSUA_DLL.Calls.pjsua_call_update(call_id, options, msg_data);
        }

        public int pjsua_call_xfer(int call_id, ref pj_str_t dest, pjsua_msg_data msg_data)
        {
            return PJSUA_DLL.Calls.pjsua_call_xfer(call_id, ref dest, msg_data);
        }

        public int pjsua_call_xfer_replaces(int call_id, int dest_call_id, uint options, pjsua_msg_data msg_data)
        {
            return PJSUA_DLL.Calls.pjsua_call_xfer_replaces(call_id, dest_call_id, options, msg_data);
        }

        public int pjsua_call_dial_dtmf(int call_id, ref pj_str_t digits)
        {
            return PJSUA_DLL.Calls.pjsua_call_dial_dtmf(call_id, ref digits);
        }

        public int pjsua_call_send_im(int call_id, ref pj_str_t mime_type, ref pj_str_t content, pjsua_msg_data msg_data,
                                      IntPtr user_data)
        {
            return PJSUA_DLL.Calls.pjsua_call_send_im(call_id, ref mime_type, ref content, msg_data, user_data);
        }

        public int pjsua_call_send_typing_ind(int call_id, int is_typing, pjsua_msg_data msg_data)
        {
            return PJSUA_DLL.Calls.pjsua_call_send_typing_ind(call_id, is_typing, msg_data);
        }

        public int pjsua_call_send_request(int call_id, ref pj_str_t method, pjsua_msg_data msg_data)
        {
            return PJSUA_DLL.Calls.pjsua_call_send_request(call_id, ref method, msg_data);
        }

        public void pjsua_call_hangup_all()
        {
            PJSUA_DLL.Calls.pjsua_call_hangup_all();
        }

        public int pjsua_call_dump(int call_id, int with_media, IntPtr buffer, uint maxlen, string indent)
        {
            return PJSUA_DLL.Calls.pjsua_call_dump(call_id, with_media, buffer, maxlen, indent);
        }

        #endregion
    }
}