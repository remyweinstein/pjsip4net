using System;

namespace pjsip4net.Utils
{
    public class StateMachine
    {
        protected AbstractState _state;

        public event EventHandler StateChanged = delegate { };

        protected virtual void OnStateChanged()
        {
            StateChanged(this, EventArgs.Empty);
        }

        public void HandleStateChanged()
        {
            if (_state != null)
                _state.StateChanged();
        }

        internal void ChangeState(AbstractState newState)
        {
            Helper.GuardNotNull(newState);
            _state = newState;
            OnStateChanged();
        }
    }
}