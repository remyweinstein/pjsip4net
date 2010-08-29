using System.Diagnostics;
using pjsip.Interop;
using pjsip4net.Media;
using pjsip4net.Utils;

namespace pjsip4net.Calls
{
    internal class NoneMediaState : AbstractState<MediaSession>
    {
        public NoneMediaState(MediaSession owner)
            : base(owner)
        {
            Debug.WriteLine("Call " + _owner.Call.Id + " NoneMediaState");
            _owner.IsActive = false;
            _owner.IsHeld = false;
            _owner.MediaState = CallMediaState.None;
        }

        #region Overrides of AbstractState

        internal override void StateChanged()
        {
            var info = _owner.Call.GetCallInfo();
            if (info.media_status == pjsua_call_media_status.PJSUA_CALL_MEDIA_ACTIVE)
            {
                if (SipUserAgent.Instance.Config.AutoConference)
                    _owner.ChangeState(new ConferenceMediaStateDecorator(_owner, new ActiveMediaState(_owner)));
                else _owner.ChangeState(new ActiveMediaState(_owner));
            }
            else if (info.media_status == pjsua_call_media_status.PJSUA_CALL_MEDIA_ERROR)
                _owner.ChangeState(new ErrorMediaState(_owner));
        }

        #endregion
    }

    internal class ActiveMediaState : AbstractState<MediaSession>
    {
        public ActiveMediaState(MediaSession owner)
            : base(owner)
        {
            _owner.IsHeld = false;
            Debug.WriteLine("Call " + _owner.Call.Id + " ActiveMediaState");
            //connect call's media to sound device
            if (!_owner.IsActive)
            {
                SingletonHolder<IMediaManager>.Instance.ConferenceBridge.ConnectToSoundDevice(
                    _owner.Call.ConferenceSlotId);
                _owner.IsActive = true;
                _owner.MediaState = CallMediaState.Active;
            }
        }

        #region Overrides of AbstractState

        internal override void StateChanged()
        {
            var info = _owner.Call.GetCallInfo();
            if (info.state == pjsip_inv_state.PJSIP_INV_STATE_DISCONNECTED)
            {
                _owner.ChangeState(new DisconnectedMediaState(_owner));
                return;
            }

            if (info.media_status == pjsua_call_media_status.PJSUA_CALL_MEDIA_ERROR)
                _owner.ChangeState(new ErrorMediaState(_owner));
            else if (info.media_status == pjsua_call_media_status.PJSUA_CALL_MEDIA_LOCAL_HOLD)
                _owner.ChangeState(new LocalHoldMediaState(_owner));
            else if (info.media_status == pjsua_call_media_status.PJSUA_CALL_MEDIA_REMOTE_HOLD)
                _owner.ChangeState(new RemoteHoldMediaState(_owner));
            else if (info.media_status == pjsua_call_media_status.PJSUA_CALL_MEDIA_ACTIVE)
                _owner.ChangeState(new ActiveMediaState(_owner)); //to remove decorator
        }

        #endregion
    }

    internal class DisconnectedMediaState : AbstractState<MediaSession>
    {
        public DisconnectedMediaState(MediaSession owner)
            : base(owner)
        {
            Debug.WriteLine("Call " + _owner.Call.Id + " DisconnectedMediaState");
            _owner.IsActive = false;
            _owner.IsHeld = false;
            _owner.MediaState = CallMediaState.Disconnected;
            SingletonHolder<ICallManagerInternal>.Instance.TerminateCall(_owner.Call);
        }

        internal override void StateChanged()
        {
        }
    }

    internal class RemoteHoldMediaState : AbstractState<MediaSession>
    {
        public RemoteHoldMediaState(MediaSession owner)
            : base(owner)
        {
            _owner.IsHeld = true;
            Debug.WriteLine("Call " + _owner.Call.Id + " RemoteHoldMediaState");
            _owner.MediaState = CallMediaState.RemoteHold;
            //connect media if not connected
            if (!_owner.IsActive)
            {
                _owner.IsActive = true;
                SingletonHolder<IMediaManager>.Instance.ConferenceBridge.ConnectToSoundDevice(
                    _owner.Call.ConferenceSlotId);
            }
        }

        #region Overrides of AbstractState

        internal override void StateChanged()
        {
            var info = _owner.Call.GetCallInfo();
            if (info.media_status == pjsua_call_media_status.PJSUA_CALL_MEDIA_ACTIVE)
            {
                if (SipUserAgent.Instance.Config.AutoConference)
                    _owner.ChangeState(new ConferenceMediaStateDecorator(_owner, new ActiveMediaState(_owner)));
                else _owner.ChangeState(new ActiveMediaState(_owner));
            }
            else if (info.media_status == pjsua_call_media_status.PJSUA_CALL_MEDIA_ERROR)
                _owner.ChangeState(new ErrorMediaState(_owner));
            else if (info.media_status == pjsua_call_media_status.PJSUA_CALL_MEDIA_NONE)
                _owner.ChangeState(new DisconnectedMediaState(_owner));
        }

        #endregion
    }

    internal class LocalHoldMediaState : AbstractState<MediaSession>
    {
        public LocalHoldMediaState(MediaSession owner)
            : base(owner)
        {
            _owner.IsHeld = true;
            Debug.WriteLine("Call " + _owner.Call.Id + " LocalHoldMediaState");
            _owner.MediaState = CallMediaState.LocalHold;
            //disconnect call's media from sound device if connected
            if (_owner.IsActive)
            {
                _owner.IsActive = false;
                SingletonHolder<IMediaManager>.Instance.ConferenceBridge.DisconnectFromSoundDevice(
                    _owner.Call.ConferenceSlotId);
            }
        }

        #region Overrides of AbstractState

        internal override void StateChanged()
        {
            var info = _owner.Call.GetCallInfo();
            if (info.media_status == pjsua_call_media_status.PJSUA_CALL_MEDIA_ACTIVE)
            {
                if (SipUserAgent.Instance.Config.AutoConference)
                    _owner.ChangeState(new ConferenceMediaStateDecorator(_owner, new ActiveMediaState(_owner)));
                else _owner.ChangeState(new ActiveMediaState(_owner));
            }
            else if (info.media_status == pjsua_call_media_status.PJSUA_CALL_MEDIA_ERROR)
                _owner.ChangeState(new ErrorMediaState(_owner));
            else if (info.media_status == pjsua_call_media_status.PJSUA_CALL_MEDIA_NONE)
                _owner.ChangeState(new DisconnectedMediaState(_owner));
        }

        #endregion
    }

    internal class ErrorMediaState : AbstractState<MediaSession>
    {
        public ErrorMediaState(MediaSession owner)
            : base(owner)
        {
            _owner.IsHeld = false;
            Debug.WriteLine("Call " + _owner.Call.Id + " ErrorMediaState");
            _owner.MediaState = CallMediaState.Error;
            //disconnect call's media from sound device if connected
            if (_owner.IsActive)
            {
                _owner.IsActive = false;
                SingletonHolder<IMediaManager>.Instance.ConferenceBridge.DisconnectFromSoundDevice(
                    _owner.Call.ConferenceSlotId);
            }
        }

        #region Overrides of AbstractState

        internal override void StateChanged()
        {
        }

        #endregion
    }

    internal class ConferenceMediaStateDecorator : AbstractState<MediaSession>
    {
        private readonly ActiveMediaState _inner;

        public ConferenceMediaStateDecorator(MediaSession owner, ActiveMediaState inner)
            : base(owner)
        {
            Helper.GuardNotNull(inner);
            _inner = inner;
            Debug.WriteLine("Call " + _owner.Call.Id + " ConferenceMediaStateDecorator");
            SingletonHolder<IMediaManager>.Instance.ConferenceBridge.ConnectCall(_owner.Call);
            _owner.IsInConference = true;
        }

        #region Overrides of AbstractState

        internal override void StateChanged()
        {
            if (_owner.IsInConference)
            {
                SingletonHolder<IMediaManager>.Instance.ConferenceBridge.DisconnectCall(_owner.Call);
                _owner.IsInConference = false;
            }
            _inner.StateChanged(); //let pj take care about disconnection
        }

        #endregion
    }
}