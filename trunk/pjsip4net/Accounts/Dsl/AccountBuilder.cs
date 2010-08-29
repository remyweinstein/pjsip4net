using System;
using System.Net;
using pjsip4net.Transport;
using pjsip4net.Utils;

namespace pjsip4net.Accounts.Dsl
{
    public class AccountBuilder
    {
        protected bool _default;
        protected string _login;
        protected string _password;
        protected string _port;
        protected bool _publish;
        protected string _registrarDomain;
        protected uint _regTimeout;
        protected VoIPTransport _transport;

        internal AccountBuilder()
        {
        }

        public AccountBuilder SetLogin(string login)
        {
            _login = login;
            return this;
        }

        public AccountBuilder SetPassword(string password)
        {
            _password = password;
            return this;
        }

        public AccountBuilder SetDomain(string domain)
        {
            _registrarDomain = domain;
            return this;
        }

        public AccountBuilder SetPort(string port)
        {
            _port = port;
            return this;
        }

        public AccountBuilder SetTransport(VoIPTransport transport)
        {
            _transport = transport;
            return this;
        }

        public AccountBuilder SetDefault(bool isDefault)
        {
            _default = isDefault;
            return this;
        }

        public AccountBuilder SetPublish(bool publishPresence)
        {
            _publish = publishPresence;
            return this;
        }

        public AccountBuilder SetRegistrationTimeout(uint registrationTimeout)
        {
            _regTimeout = registrationTimeout;
            return this;
        }

        public Account Register()
        {
            if (string.IsNullOrEmpty(_registrarDomain))
                throw new ArgumentNullException("domain");

            Account account = CreateAccount();
            using (account.InitializationScope())
            {
                account.Credential = new NetworkCredential(_login, _password, "*");

                var sb = new SipUriBuilder();
                sb.AppendExtension(_login).AppendDomain(_registrarDomain);
                if (!string.IsNullOrEmpty(_port))
                    sb.AppendPort(_port);
                _transport = _transport ?? SipUserAgent.Instance.SipTransport;
                if (_transport is TcpTransport)
                    sb.AppendTransportSuffix(TransportType.Tcp);
                else if (_transport is TlsTransport)
                    sb.AppendTransportSuffix(TransportType.Tls);
                account.AccountId = sb.ToString();

                sb.Clear();

                sb.AppendDomain(_registrarDomain);
                if (!string.IsNullOrEmpty(_port))
                    sb.AppendPort(_port);
                if (_transport is TcpTransport)
                    sb.AppendTransportSuffix(TransportType.Tcp);
                else if (_transport is TlsTransport)
                    sb.AppendTransportSuffix(TransportType.Tls);

                account.RegistrarUri = sb.ToString();
                account.Transport = _transport;
                account.PublishPresence = _publish;
                if (_regTimeout != default(int))
                    account.RegistrationTimeout = _regTimeout;
            }

            InternalRegister(account);
            return account;
        }

        protected virtual Account CreateAccount()
        {
            return new Account(false);
        }

        protected virtual void InternalRegister(Account account)
        {
            SingletonHolder<IAccountManagerInternal>.Instance.RegisterAccount(account, _default);
        }
    }

    public class WithAccountBuilderExpression
    {
        private readonly AccountBuilder _builder;

        internal WithAccountBuilderExpression(AccountBuilder builder)
        {
            Helper.GuardNotNull(builder);
            _builder = builder;
        }

        public AtAccountBuilderExpression With(string login, string password)
        {
            return
                new AtAccountBuilderExpression(_builder.SetLogin(login).SetPassword(password));
        }

        public AtAccountBuilderExpression With(string login, string password, uint registrationTimeout)
        {
            return
                new AtAccountBuilderExpression(
                    _builder.SetLogin(login).SetPassword(password).SetRegistrationTimeout(registrationTimeout));
        }
    }

    public class AtAccountBuilderExpression
    {
        private readonly AccountBuilder _builder;

        internal AtAccountBuilderExpression(AccountBuilder builder)
        {
            Helper.GuardNotNull(builder);
            _builder = builder;
        }

        public ThroughAccountBuilderExpression At(string domain)
        {
            return new ThroughAccountBuilderExpression(_builder.SetDomain(domain));
        }
    }

    public class ThroughAccountBuilderExpression
    {
        private readonly AccountBuilder _builder;

        internal ThroughAccountBuilderExpression(AccountBuilder builder)
        {
            Helper.GuardNotNull(builder);
            _builder = builder;
        }

        public OverAccountBuilderExpression Through(string port)
        {
            return new OverAccountBuilderExpression(_builder.SetPort(port));
        }
    }

    public class OverAccountBuilderExpression
    {
        private readonly AccountBuilder _builder;

        internal OverAccountBuilderExpression(AccountBuilder builder)
        {
            Helper.GuardNotNull(builder);
            _builder = builder;
        }

        public GoAccountBuilderExpression Over(VoIPTransport transport)
        {
            return new GoAccountBuilderExpression(_builder.SetTransport(transport));
        }
    }

    public class GoAccountBuilderExpression
    {
        private readonly AccountBuilder _builder;

        internal GoAccountBuilderExpression(AccountBuilder builder)
        {
            Helper.GuardNotNull(builder);
            _builder = builder;
        }

        public Account Go(bool isDefault, bool publishPresence)
        {
            return _builder.SetDefault(isDefault).SetPublish(publishPresence).Register();
        }
    }
}