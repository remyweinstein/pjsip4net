using System;

namespace pjsip4net.Utils
{
    public class NfaState<Q> : AbstractState<StateMachine>
    {
        public NfaState(StateMachine owner)
            : base(owner)
        {
            StateType = StateType.NormalState;
        }

        public Q Id { get; set; }

        public StateType StateType { get; set; }
        public Action Act { get; set; }

        public virtual void NextToken<S>(S token)
        {
        }

        public virtual void ClearState()
        {
        }

        #region Overrides of AbstractState

        internal override void StateChanged()
        {
            //if (Act != null) Act();
        }

        #endregion
    }

    public enum StateType
    {
        StartState,
        FinalState,
        NormalState,
        SinkState
    }
}