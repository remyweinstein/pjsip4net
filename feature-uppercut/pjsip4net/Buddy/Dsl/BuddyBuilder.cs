using pjsip4net.Core;
using pjsip4net.Core.Utils;
using pjsip4net.Interfaces;

namespace pjsip4net.Buddy.Dsl
{
    public class BuddyBuilder
    {
        private string _name;
        private string _port;
        private string _domain;
        private TransportType _transport;
        private bool _subscribe;
        private IImManager _userAgent;
        private IObjectFactory _objectFactory;

        public BuddyBuilder(IImManager userAgent, IObjectFactory objectFactory)
        {
            Helper.GuardNotNull(userAgent);
            Helper.GuardNotNull(objectFactory);
            _userAgent = userAgent;
            _objectFactory = objectFactory;
        }

        public BuddyBuilder WithName(string name)
        {
            Helper.GuardNotNullStr(name);
            _name = name;
            return this;
        }

        public BuddyBuilder Through(string port)
        {
            Helper.GuardNotNullStr(port);
            Helper.GuardPositiveInt(int.Parse(port));
            _port = port;
            return this;
        }

        public BuddyBuilder At(string domain)
        {
            Helper.GuardNotNullStr(domain);
            _domain = domain;
            return this;
        }

        public BuddyBuilder Via(TransportType transport)
        {
            _transport = transport;
            return this;
        }

        public BuddyBuilder Subscribing()
        {
            _subscribe = true;
            return this;
        }

        public Buddy Register()
        {
            Helper.GuardNotNullStr(_name);
            //Helper.GuardNotNullStr(_domain);
            var buddy = CreateBuddy();
            var uriBuilder = new SipUriBuilder().AppendExtension(_name).AppendDomain(_domain)
                .AppendPort(string.IsNullOrEmpty(_port) ? "5060" : _port).AppendTransportSuffix(
                _transport);
            buddy.Uri = uriBuilder.ToString();
            buddy.Subscribe = _subscribe;

            InternalRegister(buddy);
            return buddy;
        }

        protected virtual Buddy CreateBuddy()//todo refactor to objectfactory
        {
            return _objectFactory.Create<Buddy>();
        }

        private void InternalRegister(Buddy buddy)
        {
            _userAgent.RegisterBuddy(buddy);
        }
    }

    //public class WithNameBuilderStep
    //{
    //    private BuddyBuilder _builder;

    //    public WithNameBuilderStep(BuddyBuilder builder)
    //    {
    //        Helper.GuardNotNull(builder);
    //        _builder = builder;
    //    }

    //    public AtBuilderStep WithName(string name)
    //    {
    //        return new AtBuilderStep(_builder.SetName(name));
    //    }
    //}

    //public class AtBuilderStep
    //{
    //    private readonly BuddyBuilder _builder;

    //    public AtBuilderStep(BuddyBuilder builder)
    //    {
    //        Helper.GuardNotNull(builder);
    //        _builder = builder;
    //    }

    //    public ThroughBuilderStep At(string domain)
    //    {
    //        return new ThroughBuilderStep(_builder.SetDomain(domain));
    //    }
    //}

    //public class ThroughBuilderStep
    //{
    //    private readonly BuddyBuilder _builder;

    //    public ThroughBuilderStep(BuddyBuilder builder)
    //    {
    //        Helper.GuardNotNull(builder);
    //        _builder = builder;
    //    }

    //    public ViaBuilderStep Through(string port)
    //    {
    //        return new ViaBuilderStep(_builder.SetPort(port));
    //    }
    //}

    //public class ViaBuilderStep
    //{
    //    private readonly BuddyBuilder _builder;

    //    public ViaBuilderStep(BuddyBuilder builder)
    //    {
    //        Helper.GuardNotNull(builder);
    //        _builder = builder;
    //    }

    //    public GoBuilderStep Via(TransportType transportType)
    //    {
    //        return new GoBuilderStep();
    //    }
    //}
}