using System;
using System.Collections.ObjectModel;
using pjsip4net.Accounts;
using pjsip4net.Core.Data.Events;
using pjsip4net.Core.Interfaces;
using pjsip4net.Core.Interfaces.ApiProviders;

namespace pjsip4net.Interfaces
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

    internal interface IAccountManagerInternal : IAccountManager, IInitializable
    {
        IAccountApiProvider Provider { get; }

        void RaiseStateChanged(Account account);
        void OnRegistrationState(RegistrationStateChanged e);
        void UnRegisterAllAccounts();
    }
}