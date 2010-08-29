namespace pjsip4net.Utils
{
    public abstract class AbstractState
    {
        internal abstract void StateChanged();
    }

    public abstract class AbstractState<T> : AbstractState where T : StateMachine
    {
        protected T _owner;

        protected AbstractState(T owner)
        {
            Helper.GuardNotNull(owner);
            _owner = owner;
        }
    }
}