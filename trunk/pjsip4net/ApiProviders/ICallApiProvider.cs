using System;
using pjsip.Interop;

namespace pjsip4net.ApiProviders
{
    public interface ICallApiProvider
    {
        uint pjsua_call_get_max_count();
        uint pjsua_call_get_count();
        int pjsua_enum_calls(int[] ids, ref uint count);

        int pjsua_call_make_call(int acc_id, ref pj_str_t dst_uri, uint options, IntPtr user_data,
                                 pjsua_msg_data msg_data, ref int p_call_id);

        bool pjsua_call_is_active(int call_id);
        bool pjsua_call_has_media(int call_id);
        int pjsua_call_get_conf_port(int call_id);
        int pjsua_call_get_info(int call_id, ref pjsua_call_info info);
        int pjsua_call_set_user_data(int call_id, IntPtr user_data);
        IntPtr pjsua_call_get_user_data(int call_id);
        int pjsua_call_get_rem_nat_type(int call_id, ref pj_stun_nat_type p_type);
        int pjsua_call_answer(int call_id, uint code, ref pj_str_t reason, pjsua_msg_data msg_data);
        int pjsua_call_hangup(int call_id, uint code, ref pj_str_t reason, pjsua_msg_data msg_data);
        int pjsua_call_process_redirect(int call_id, pjsip_redirect_op cmd);
        int pjsua_call_set_hold(int call_id, pjsua_msg_data msg_data);
        int pjsua_call_reinvite(int call_id, int unhold, pjsua_msg_data msg_data);
        int pjsua_call_update(int call_id, uint options, pjsua_msg_data msg_data);
        int pjsua_call_xfer(int call_id, ref pj_str_t dest, pjsua_msg_data msg_data);
        int pjsua_call_xfer_replaces(int call_id, int dest_call_id, uint options, pjsua_msg_data msg_data);
        int pjsua_call_dial_dtmf(int call_id, ref pj_str_t digits);

        int pjsua_call_send_im(int call_id, ref pj_str_t mime_type, ref pj_str_t content,
                               pjsua_msg_data msg_data, IntPtr user_data);

        int pjsua_call_send_typing_ind(int call_id, int is_typing,
                                       pjsua_msg_data msg_data);

        int pjsua_call_send_request(int call_id,
                                    ref pj_str_t method, pjsua_msg_data msg_data);

        void pjsua_call_hangup_all();
        int pjsua_call_dump(int call_id, int with_media, IntPtr buffer, uint maxlen, string indent);
    }
}