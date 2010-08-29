using System;
using pjsip4net.Utils;

namespace pjsip4net.Calls
{
    internal class MediaSession : StateMachine, IDisposable
    {
        private WeakReference _call;

        public MediaSession(Call call)
        {
            Helper.GuardNotNull(call);
            _call = new WeakReference(call);
            _state = new NoneMediaState(this);
        }

        public Call Call
        {
            get
            {
                if (_call.IsAlive)
                    return (Call) _call.Target;
                throw new ObjectDisposedException("call");
            }
        }

        public bool IsHeld { get; set; }
        public bool IsActive { get; set; }
        public bool IsInConference { get; set; }
        public CallMediaState MediaState { get; set; }

        #region IDisposable Members

        public void Dispose()
        {
            _call = null;
            _state = null;
        }

        #endregion

        public void ConnectToConference()
        {
            if (!IsActive)
                return;
            if (IsInConference)
                return;

            _state = new ConferenceMediaStateDecorator(this, _state as ActiveMediaState);
        }

        public void DisconnectFromConference()
        {
            if (!IsActive)
                return;
            if (!IsInConference)
                return;

            _state.StateChanged();
        }
    }
}