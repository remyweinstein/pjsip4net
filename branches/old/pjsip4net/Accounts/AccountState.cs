using System.Diagnostics;
using pjsip.Interop;
using pjsip4net.Utils;

namespace pjsip4net.Accounts
{
    /// <summary>
    /// Initial state
    /// </summary>
    internal class InitializingAccountState : AbstractState<RegistrationSession>
    {
        public InitializingAccountState(RegistrationSession owner)
            : base(owner)
        {
            Debug.WriteLine("Account " + _owner.Account.Id + " InitializingAccountState");
            _owner.IsRegistered = false;
        }

        #region Overrides of AccountState

        internal override void StateChanged()
        {
            pjsua_acc_info info = _owner.Account.GetAccountInfo();
            if (_owner.Account.IsLocal)
                _owner.ChangeState(new RegisteredAccountState(_owner));
            else if (info.status == pjsip_status_code.PJSIP_SC_TRYING)
                _owner.ChangeState(new RegisteringAccountState(_owner));
            else
                _owner.ChangeState(new UnknownStatusState(_owner, info.status, info.status_text));
        }

        #endregion
    }

    /// <summary>
    /// After remote registration session started
    /// </summary>
    internal class RegisteringAccountState : AbstractState<RegistrationSession>
    {
        public RegisteringAccountState(RegistrationSession owner)
            : base(owner)
        {
            Debug.WriteLine("Account " + _owner.Account.Id + " RegisteringAccountState");
            _owner.IsRegistered = false;
        }

        #region Overrides of AccountState

        internal override void StateChanged()
        {
            pjsua_acc_info info = _owner.Account.GetAccountInfo();
            if (info.status == pjsip_status_code.PJSIP_SC_REQUEST_TIMEOUT)
                _owner.ChangeState(new TimedOutAccountRegistrationState(_owner));
            else if (info.status == pjsip_status_code.PJSIP_SC_OK)
                _owner.ChangeState(new RegisteredAccountState(_owner));
            else
                _owner.ChangeState(new UnknownStatusState(_owner, info.status, info.status_text));
        }

        #endregion
    }

    /// <summary>
    /// Either after PJSIP_SC_OK = 200 recieved or local account been added
    /// </summary>
    internal class RegisteredAccountState : AbstractState<RegistrationSession>
    {
        public RegisteredAccountState(RegistrationSession owner)
            : base(owner)
        {
            Debug.WriteLine("Account " + _owner.Account.Id + " RegisteredAccountState");
            _owner.IsRegistered = true;
            //if (_owner.Account.PublishPresence)
            //    _owner.Account.IsOnline = true;
        }

        #region Overrides of AccountState

        internal override void StateChanged()
        {
            if (_owner.Account.Id == NativeConstants.PJSUA_INVALID_ID && _owner.Account.IsLocal)
                _owner.ChangeState(new InitializingAccountState(_owner));
            else
            {
                pjsua_acc_info info = _owner.Account.GetAccountInfo();
                if (info.status == (pjsip_status_code) 1 || info.status == (pjsip_status_code) 200) //OK
                    return;
                //_owner.Account.IsOnline = false;
                if (info.status == pjsip_status_code.PJSIP_SC_REQUEST_TIMEOUT)
                    _owner.ChangeState(new TimedOutAccountRegistrationState(_owner));
                else if (info.status == pjsip_status_code.PJSIP_SC_TRYING)
                    _owner.ChangeState(new RegisteringAccountState(_owner));
                else
                    _owner.ChangeState(new UnknownStatusState(_owner, info.status, info.status_text));
            }
        }

        #endregion
    }

    /// <summary>
    /// After PJSIP_SC_REQUEST_TIMEOUT = 408 recieved 
    /// </summary>
    internal class TimedOutAccountRegistrationState : AbstractState<RegistrationSession>
    {
        public TimedOutAccountRegistrationState(RegistrationSession owner)
            : base(owner)
        {
            Debug.WriteLine("Account " + _owner.Account.Id + " TimedOutAccountRegistrationState");
            _owner.IsRegistered = false;
            //_owner.Account.Dispose();//account can be re-registered - no need to dispose and delete
        }

        #region Overrides of AccountState

        internal override void StateChanged()
        {
            pjsua_acc_info info = _owner.Account.GetAccountInfo();
            if (info.status == pjsip_status_code.PJSIP_SC_OK)
                _owner.ChangeState(new RegisteredAccountState(_owner));
            else if (info.status == pjsip_status_code.PJSIP_SC_TRYING)
                _owner.ChangeState(new RegisteringAccountState(_owner));
            else
                _owner.ChangeState(new UnknownStatusState(_owner, info.status, info.status_text));
        }

        #endregion
    }

    /// <summary>
    /// After unknown status recieved
    /// </summary>
    internal class UnknownStatusState : AbstractState<RegistrationSession>
    {
        public UnknownStatusState(RegistrationSession owner, pjsip_status_code code, string statusText)
            : base(owner)
        {
            _owner.IsRegistered = false;
            StatusCode = code;
            StatusText = statusText;
            Debug.WriteLine("Account " + _owner.Account.Id + " UnknownStatusState");
            Debug.WriteLine(StatusText);
        }

        public pjsip_status_code StatusCode { get; private set; }
        public string StatusText { get; private set; }

        internal override void StateChanged()
        {
            pjsua_acc_info info = _owner.Account.GetAccountInfo();
            if (info.status == pjsip_status_code.PJSIP_SC_OK)
                _owner.ChangeState(new RegisteredAccountState(_owner));
            else if (info.status == pjsip_status_code.PJSIP_SC_REQUEST_TIMEOUT)
                _owner.ChangeState(new TimedOutAccountRegistrationState(_owner));
            else if (info.status == pjsip_status_code.PJSIP_SC_TRYING)
                _owner.ChangeState(new RegisteringAccountState(_owner));
            else
                _owner.ChangeState(new UnknownStatusState(_owner, info.status, info.status_text));
        }
    }
}