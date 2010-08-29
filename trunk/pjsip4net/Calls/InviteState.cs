using System.Diagnostics;
using pjsip.Interop;
using pjsip4net.Utils;

namespace pjsip4net.Calls
{
    /// <summary>
    /// Before INVITE is sent or received
    /// </summary>
    internal class NullInviteState : AbstractState<InviteSession>
    {
        public NullInviteState(InviteSession owner)
            : base(owner)
        {
            Debug.WriteLine("Call " + _owner.Call.Id + " NullInviteState");
            _owner.InviteState = CallInviteState.None;
        }

        #region Overrides of AbstractState

        internal override void StateChanged()
        {
            pjsua_call_info info = _owner.Call.GetCallInfo();
            if (info.state == pjsip_inv_state.PJSIP_INV_STATE_DISCONNECTED)
                _owner.ChangeState(new DisconnectedInviteState(_owner));
            else
                _owner.ChangeState(new CallingInviteState(_owner)); //after INVITE is sent
        }

        #endregion
    }

    /// <summary>
    /// After INVITE is sent
    /// </summary>
    internal class CallingInviteState : AbstractState<InviteSession>
    {
        public CallingInviteState(InviteSession owner)
            : base(owner)
        {
            Debug.WriteLine("Call " + _owner.Call.Id + " CallingInviteState");
            if (!_owner.Call.IsIncoming)
                SingletonHolder<ICallManagerInternal>.Instance.RaiseRingEvent(_owner.Call, true);
            _owner.IsRinging = true;
            _owner.InviteState = CallInviteState.Calling;
        }

        #region Overrides of AbstractState

        internal override void StateChanged()
        {
            pjsua_call_info info = _owner.Call.GetCallInfo();
            if (info.state == pjsip_inv_state.PJSIP_INV_STATE_CONFIRMED)
                _owner.ChangeState(new ConfirmedInviteState(_owner));
            else if (info.state == pjsip_inv_state.PJSIP_INV_STATE_CONNECTING)
                _owner.ChangeState(new ConnectingInviteState(_owner));
            else if (info.state == pjsip_inv_state.PJSIP_INV_STATE_DISCONNECTED)
                _owner.ChangeState(new DisconnectedInviteState(_owner));
        }

        #endregion
    }

    /// <summary>
    /// After INVITE is received
    /// </summary>
    internal class IncomingInviteState : AbstractState<InviteSession>
    {
        public IncomingInviteState(InviteSession owner)
            : base(owner)
        {
            Debug.WriteLine("Call " + _owner.Call.Id + " IncomingInviteState");
            //SingletonHolder<ICallManagerImpl>.Instance.RaiseRingEvent(_owner.Call, true);
            _owner.IsRinging = true;
            _owner.InviteState = CallInviteState.Incoming;
        }

        #region Overrides of AbstractState

        internal override void StateChanged()
        {
            pjsua_call_info info = _owner.Call.GetCallInfo();
            if (info.state == pjsip_inv_state.PJSIP_INV_STATE_CONFIRMED)
                _owner.ChangeState(new ConfirmedInviteState(_owner));
            else if (info.state == pjsip_inv_state.PJSIP_INV_STATE_CONNECTING)
                _owner.ChangeState(new ConnectingInviteState(_owner));
            else if (info.state == pjsip_inv_state.PJSIP_INV_STATE_EARLY)
                _owner.ChangeState(new EarlyInviteState(_owner));
            else if (info.state == pjsip_inv_state.PJSIP_INV_STATE_DISCONNECTED)
                _owner.ChangeState(new DisconnectedInviteState(_owner));
        }

        #endregion
    }

    /// <summary>
    /// After response with To tag
    /// </summary>
    internal class EarlyInviteState : AbstractState<InviteSession>
    {
        public EarlyInviteState(InviteSession owner)
            : base(owner)
        {
            Debug.WriteLine("Call " + _owner.Call.Id + " EarlyInviteState");
            _owner.InviteState = CallInviteState.Early;
        }

        #region Overrides of AbstractState

        internal override void StateChanged()
        {
            pjsua_call_info info = _owner.Call.GetCallInfo();
            if (info.state == pjsip_inv_state.PJSIP_INV_STATE_CONFIRMED)
                _owner.ChangeState(new ConfirmedInviteState(_owner));
            else if (info.state == pjsip_inv_state.PJSIP_INV_STATE_CONNECTING)
                _owner.ChangeState(new ConnectingInviteState(_owner));
            else if (info.state == pjsip_inv_state.PJSIP_INV_STATE_DISCONNECTED)
                _owner.ChangeState(new DisconnectedInviteState(_owner));
        }

        #endregion
    }

    /// <summary>
    /// After 2xx is sent/received
    /// </summary>
    internal class ConnectingInviteState : AbstractState<InviteSession>
    {
        public ConnectingInviteState(InviteSession owner)
            : base(owner)
        {
            Debug.WriteLine("Call " + _owner.Call.Id + " ConnectingInviteState");
            //if (_owner.IsRinging)
            //{
            //    SingletonHolder<ICallManagerImpl>.Instance.RaiseRingEvent(_owner.Call, false);
            //    _owner.IsRinging = false;
            //}
            _owner.InviteState = CallInviteState.Connecting;
        }

        #region Overrides of AbstractState

        internal override void StateChanged()
        {
            pjsua_call_info info = _owner.Call.GetCallInfo();
            if (info.state == pjsip_inv_state.PJSIP_INV_STATE_CONFIRMED)
                _owner.ChangeState(new ConfirmedInviteState(_owner));
            else if (info.state == pjsip_inv_state.PJSIP_INV_STATE_DISCONNECTED)
                _owner.ChangeState(new DisconnectedInviteState(_owner));
        }

        #endregion
    }

    /// <summary>
    /// After ACK is sent/received
    /// </summary>
    internal class ConfirmedInviteState : AbstractState<InviteSession>
    {
        public ConfirmedInviteState(InviteSession owner)
            : base(owner)
        {
            Debug.WriteLine("Call " + _owner.Call.Id + " ConfirmedInviteState");
            if (_owner.IsRinging)
            {
                SingletonHolder<ICallManagerInternal>.Instance.RaiseRingEvent(_owner.Call, false);
                _owner.IsRinging = false;
            }
            _owner.IsConfirmed = true;
            _owner.InviteState = CallInviteState.Confirmed;
        }

        #region Overrides of AbstractState

        internal override void StateChanged()
        {
            pjsua_call_info info = _owner.Call.GetCallInfo();
            if (info.state == pjsip_inv_state.PJSIP_INV_STATE_DISCONNECTED)
                _owner.ChangeState(new DisconnectedInviteState(_owner));
        }

        #endregion
    }

    /// <summary>
    /// Session is terminated
    /// </summary>
    internal class DisconnectedInviteState : AbstractState<InviteSession>
    {
        public DisconnectedInviteState(InviteSession owner)
            : base(owner)
        {
            Debug.WriteLine("Call " + _owner.Call.Id + " DisconnectedInviteState");
            _owner.IsConfirmed = false;
            _owner.IsDisconnected = true;
            _owner.InviteState = CallInviteState.Disconnected;
            if (_owner.IsRinging)
            {
                SingletonHolder<ICallManagerInternal>.Instance.RaiseRingEvent(_owner.Call, false);
                _owner.IsRinging = false;
            }
            if (!_owner.Call.HasMedia)
                SingletonHolder<ICallManagerInternal>.Instance.TerminateCall(_owner.Call);
        }

        #region Overrides of AbstractState

        internal override void StateChanged()
        {
        }

        #endregion
    }
}