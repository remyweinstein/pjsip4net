using System;
using System.Collections.ObjectModel;
using pjsip4net.Accounts;
using pjsip4net.Calls;
using pjsip4net.Core.Data.Events;
using pjsip4net.Core.Interfaces;
using pjsip4net.Core.Interfaces.ApiProviders;
using pjsip4net.Core.Utils;

namespace pjsip4net.Interfaces
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

    internal interface ICallManagerInternal : ICallManager, IInitializable
    {
        IBasicApiProvider BasicApiProvider { get; }
        ICallApiProvider CallApiProvider { get; }
        IMediaApiProvider MediaApiProvider { get; }

        void RaiseCallStateChanged(Call call);
        void RaiseRingEvent(Call call, bool ringOn);
        void TerminateCall(Call call);
        void OnCallState(CallStateChanged e);
        void OnIncomingCall(IncomingCallRecieved e);
        void OnCallMediaState(CallMediaStateChanged e);
        void OnStreamDestroyed(StreamDestroyed e);
        void OnDtmfDigit(DtmfRecieved e);
        void OnCallTransfer(CallTransferRequested e);
        void OnCallTransferStatus(CallTransferStatusChanged e);
    }
}