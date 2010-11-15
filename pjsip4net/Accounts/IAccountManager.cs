using System;
using System.Collections.ObjectModel;

namespace pjsip4net.Accounts
{
    public interface IAccountManager
    {
        ReadOnlyCollection<Account> Accounts { get; }
        Account DefaultAccount { get; set; }
        event EventHandler<AccountStateChangedEventArgs> AccountStateChanged;
        void RegisterAccount(Account account, bool @default);
        void UnregisterAccount(Account account);
        Account GetAccountById(int id);
    }

    internal interface IAccountManagerInternal : IAccountManager
    {
        void RaiseStateChanged(Account account);
        void OnRegistrationState(int acc_id);
        void UnRegisterAllAccounts();
    }
}