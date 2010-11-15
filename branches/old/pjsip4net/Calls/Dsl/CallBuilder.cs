using pjsip4net.Accounts;
using pjsip4net.Transport;
using pjsip4net.Utils;

namespace pjsip4net.Calls.Dsl
{
    public class CallBuilder
    {
        private readonly SipUriBuilder _sb = new SipUriBuilder();
        protected Account _account;

        internal CallBuilder()
        {
        }

        public CallBuilder SetExtension(string extension)
        {
            _sb.AppendExtension(extension);
            return this;
        }

        public CallBuilder SetDomain(string domain)
        {
            _sb.AppendDomain(domain);
            return this;
        }

        public CallBuilder SetPort(string port)
        {
            _sb.AppendPort(port);
            return this;
        }

        public CallBuilder SetAccount(Account account)
        {
            _account = account;
            return this;
        }

        public Call Call()
        {
            _account = _account ?? SingletonHolder<IAccountManager>.Instance.DefaultAccount;
            VoIPTransport transport = _account.Transport;
            if (transport is TcpTransport)
                _sb.AppendTransportSuffix(TransportType.Tcp);
            else if (transport is TlsTransport)
                _sb.AppendTransportSuffix(TransportType.Tls);

            return InternalCall();
        }

        protected virtual Call InternalCall()
        {
            return SingletonHolder<ICallManager>.Instance.MakeCall(_account, _sb.ToString());
        }
    }

    public class ToCallBuilderExpression
    {
        private readonly CallBuilder _builder;

        public ToCallBuilderExpression(CallBuilder builder)
        {
            Helper.GuardNotNull(builder);
            _builder = builder;
        }

        public AtCallBuilderExpression To(string extension)
        {
            return new AtCallBuilderExpression(_builder.SetExtension(extension));
        }
    }

    public class AtCallBuilderExpression
    {
        private readonly CallBuilder _builder;

        public AtCallBuilderExpression(CallBuilder builder)
        {
            Helper.GuardNotNull(builder);
            _builder = builder;
        }

        public ThroughCallBuilderExpression At(string domain)
        {
            return new ThroughCallBuilderExpression(_builder.SetDomain(domain));
        }
    }

    public class ThroughCallBuilderExpression
    {
        private readonly CallBuilder _builder;

        public ThroughCallBuilderExpression(CallBuilder builder)
        {
            Helper.GuardNotNull(builder);
            _builder = builder;
        }

        public FromCallBuilderExpression Through(string port)
        {
            return new FromCallBuilderExpression(_builder.SetPort(port));
        }
    }

    public class FromCallBuilderExpression
    {
        private readonly CallBuilder _builder;

        public FromCallBuilderExpression(CallBuilder builder)
        {
            Helper.GuardNotNull(builder);
            _builder = builder;
        }

        public GoCallBuilderExpression From(Account account)
        {
            return new GoCallBuilderExpression(_builder.SetAccount(account));
        }
    }

    public class GoCallBuilderExpression
    {
        private readonly CallBuilder _builder;

        public GoCallBuilderExpression(CallBuilder builder)
        {
            Helper.GuardNotNull(builder);
            _builder = builder;
        }

        public Call Go()
        {
            return _builder.Call();
        }
    }
}