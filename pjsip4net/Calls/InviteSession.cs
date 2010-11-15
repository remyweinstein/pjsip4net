using System;
using pjsip4net.Utils;

namespace pjsip4net.Calls
{
    internal class InviteSession : StateMachine, IDisposable
    {
        private WeakReference _call;

        public InviteSession(Call owner)
        {
            Helper.GuardNotNull(owner);
            _call = new WeakReference(owner);
            _state = owner.IsIncoming ? (AbstractState) new IncomingInviteState(this) : new NullInviteState(this);
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

        public bool IsConfirmed { get; set; }
        public bool IsDisconnected { get; set; }
        public bool IsRinging { get; set; }
        public CallInviteState InviteState { get; set; }

        #region IDisposable Members

        public void Dispose()
        {
            _call = null;
            _state = null;
        }

        #endregion
    }
}