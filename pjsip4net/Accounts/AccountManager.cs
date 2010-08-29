using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using pjsip.Interop;
using pjsip4net.Utils;

namespace pjsip4net.Accounts
{
    internal class AccountManager : IAccountManagerInternal
    {
        //#region Singleton

        private static readonly object _lock = new object();
        //private static AccountManager _instance;

        //internal static AccountManager Instance
        //{
        //    get
        //    {
        //        if (_instance == null)
        //            lock (_lock)
        //                if (_instance == null)
        //                    _instance = new AccountManager();
        //        return _instance;
        //    }
        //}

        //#endregion

        private readonly SortedDictionary<int, Account> _accounts;
        //private SynchronizationContext _syncContext;
        private Queue<Account> _deleting;

        #region Properties

        public event EventHandler<AccountStateChangedEventArgs> AccountStateChanged = delegate { };

        public ReadOnlyCollection<Account> Accounts
        {
            get
            {
                lock (_lock)
                    return new ReadOnlyCollection<Account>(_accounts.Values.Where(t => !t.IsLocal).ToList());
            }
        }

        public Account DefaultAccount
        {
            get
            {
                try
                {
                    return _accounts[SipUserAgent.ApiFactory.GetAccountApi().pjsua_acc_get_default()];
                }
                catch (Exception)
                {
                    return null;
                }
            }
            set
            {
                Helper.GuardNotNull(value);
                Helper.GuardPositiveInt(value.Id);
                if (SipUserAgent.ApiFactory.GetAccountApi().pjsua_acc_is_valid(value.Id))
                    Helper.GuardError(SipUserAgent.ApiFactory.GetAccountApi().pjsua_acc_set_default(value.Id));
            }
        }

        #endregion

        #region Methods

        public void OnRegistrationState(int acc_id)
        {
            Account account = null;
            lock (_lock)
                if (_accounts.ContainsKey(acc_id) && _accounts[acc_id] != null)
                    account = _accounts[acc_id];
            if (account != null) account.HandleStateChanged();
        }

        public void RaiseStateChanged(Account account)
        {
            //try
            //{
            //    if (_syncContext != null)
            //        _syncContext.Post(delegate { AccountStateChanged(this, account.GetEventArgs()); }, null);
            //    else
            //        AccountStateChanged(this, account.GetEventArgs());
            //}
            //catch (InvalidOperationException)
            //{
            AccountStateChanged(this, account.GetEventArgs());
            //}
        }

        public void RegisterAccount(Account account, bool @default)
        {
            Helper.GuardNotNull(account);

            lock (_lock)
            {
                account.Transport = account.Transport ?? SipUserAgent.Instance.SipTransport;
                int id = -1;
                if (account.IsLocal)
                    Helper.GuardError(
                        SipUserAgent.ApiFactory.GetAccountApi().pjsua_acc_add_local(
                            SipUserAgent.Instance.SipTransport.Id,
                            Convert.ToInt32(@default), ref id));
                else
                    Helper.GuardError(SipUserAgent.ApiFactory.GetAccountApi().pjsua_acc_add(account._config,
                                                                                            Convert.ToInt32(@default),
                                                                                            ref id));

                account.Id = id;
                _accounts.Add(account.Id, account);
                account.HandleStateChanged();
            }
        }

        public void UnregisterAccount(Account account)
        {
            if (account.IsInUse)
                throw new InvalidOperationException("Can't delete account as long as it's being used by other parties");

            lock (_lock)
                if (_accounts.ContainsKey(account.Id))
                {
                    //if (account.IsRegistered)
                    //    Helper.GuardError(PJSUA_DLL.Accounts.pjsua_acc_set_registration(account.Id, false));

                    //_deleting.Enqueue(account);//TODO raise events for accounts being deleted
                    Helper.GuardError(SipUserAgent.ApiFactory.GetAccountApi().pjsua_acc_del(account.Id));
                    _accounts.Remove(account.Id);
                    account.Id = NativeConstants.PJSUA_INVALID_ID;
                    if (account.IsLocal)
                        account.HandleStateChanged();
                    account.InternalDispose();
                }
        }

        public Account GetAccountById(int id)
        {
            lock (_lock)
                if (_accounts.ContainsKey(id))
                    return _accounts[id];
            return null;
        }

        public void UnRegisterAllAccounts()
        {
            foreach (Account account in _accounts.Values.ToList())
                UnregisterAccount(account);
            Thread.Sleep(1000);
        }

        #endregion

        public AccountManager()
        {
            _accounts = new SortedDictionary<int, Account>();
            //_syncContext = SynchronizationContext.Current;
            _deleting = new Queue<Account>();
        }
    }
}