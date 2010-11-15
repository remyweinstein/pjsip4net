using System;
using System.Collections.ObjectModel;
using pjsip.Interop;
using pjsip4net.Accounts;
using pjsip4net.Utils;

namespace pjsip4net.Calls
{
    public interface ICallManager
    {
        ReadOnlyCollection<Call> Calls { get; }
        uint MaxCalls { get; set; }
        event EventHandler<EventArgs<Call>> IncomingCall;
        event EventHandler<CallStateChangedEventArgs> CallStateChanged;
        event EventHandler<DtmfEventArgs> IncomingDtmfDigit;
        event EventHandler<RingEventArgs> Ring;
        event EventHandler<CallTransferEventArgs> CallTransfer;
        Call MakeCall(string destinationUri);
        Call MakeCall(Account account, string destinationUri);
        void HangupAll();
        Call GetCallById(int id);
    }

    internal interface ICallManagerInternal : ICallManager
    {
        void RaiseCallStateChanged(Call call);
        void RaiseRingEvent(Call call, bool ringOn);
        void TerminateCall(Call call);
        void OnCallState(int call_id, ref IntPtr e);
        void OnIncomingCall(int acc_id, int call_id, IntPtr rdata);
        void OnCallMediaState(int call_id);
        void OnStreamDestroyed(int call_id, IntPtr sess, uint stream_idx);
        void OnDtmfDigit(int call_id, int digit);
        void OnCallTransfer(int call_id, ref pj_str_t dst, ref pjsip_status_code code);
        void OnCallTransferStatus(int call_id, int st_code, ref pj_str_t st_text, int final, ref int p_cont);
    }
}