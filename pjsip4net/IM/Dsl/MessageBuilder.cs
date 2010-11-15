using pjsip4net.Accounts;
using pjsip4net.Calls;
using pjsip4net.Utils;

namespace pjsip4net.IM.Dsl
{
    public class MessageBuilder
    {
        private readonly SipUriBuilder _builder = new SipUriBuilder();
        private string _body;
        private Account _from;

        public MessageBuilder SetExtension(string extension)
        {
            _builder.AppendExtension(extension);
            return this;
        }

        public MessageBuilder SetDomain(string domain)
        {
            _builder.AppendDomain(domain);
            return this;
        }

        public MessageBuilder SetPort(string port)
        {
            _builder.AppendPort(port);
            return this;
        }

        public MessageBuilder SetBody(string body)
        {
            _body = body;
            return this;
        }

        public MessageBuilder SetAccount(Account account)
        {
            Helper.GuardNotNull(account);
            Helper.GuardNotNull(account.Transport);
            TransportType ttype = account.Transport.TransportType;
            _builder.AppendTransportSuffix(ttype);
            _from = account;
            return this;
        }

        public void SendMessage()
        {
            //pj_str_t to = new pj_str_t(_builder.ToString());
            //pj_str_t mime = new pj_str_t("plain/text");
            //pj_str_t content = new pj_str_t(_body);
            SipUserAgent.Instance.SendMessage(_from, _body, _builder.ToString());
            //Helper.GuardError(SipUserAgent.ApiFactory.GetImApi().pjsua_im_send(_from.Id, ref to, ref mime, ref content, null, IntPtr.Zero));
        }

        public void SendTyping(bool isTyping)
        {
            //pj_str_t to = new pj_str_t(_builder.ToString());
            //Helper.GuardError(SipUserAgent.ApiFactory.GetImApi().pjsua_im_typing(_from.Id, ref to, Convert.ToInt32(isTyping), null));
            SipUserAgent.Instance.SendTyping(_from, _builder.ToString(), isTyping);
        }
    }

    public class ToMessageBuilderExpression
    {
        private readonly MessageBuilder _builder;

        public ToMessageBuilderExpression(MessageBuilder builder)
        {
            Helper.GuardNotNull(builder);
            _builder = builder;
        }

        public AtMessageBuilderExpression To(string extension)
        {
            return new AtMessageBuilderExpression(_builder.SetExtension(extension));
        }
    }

    public class AtMessageBuilderExpression
    {
        private readonly MessageBuilder _builder;

        public AtMessageBuilderExpression(MessageBuilder builder)
        {
            Helper.GuardNotNull(builder);
            _builder = builder;
        }

        public ThroughMessageBuilderExpression At(string domain)
        {
            return new ThroughMessageBuilderExpression(_builder.SetDomain(domain));
        }
    }

    public class ThroughMessageBuilderExpression
    {
        private readonly MessageBuilder _builder;

        public ThroughMessageBuilderExpression(MessageBuilder builder)
        {
            Helper.GuardNotNull(builder);
            _builder = builder;
        }

        public FromMessageBuilderExpression Through(string port)
        {
            return new FromMessageBuilderExpression(_builder.SetPort(port));
        }
    }

    public class FromMessageBuilderExpression
    {
        private readonly MessageBuilder _builder;

        public FromMessageBuilderExpression(MessageBuilder builder)
        {
            Helper.GuardNotNull(builder);
            _builder = builder;
        }

        public FinalMessageBuilderExpression From(Account account)
        {
            return new FinalMessageBuilderExpression(_builder.SetAccount(account));
        }
    }

    public class FinalMessageBuilderExpression
    {
        private readonly MessageBuilder _builder;

        public FinalMessageBuilderExpression(MessageBuilder builder)
        {
            Helper.GuardNotNull(builder);
            _builder = builder;
        }

        public void Typing(bool isTyping)
        {
            _builder.SendTyping(isTyping);
        }

        public void Go(string message)
        {
            _builder.SetBody(message).SendMessage();
        }
    }

    public class InDialogMessageBuilder
    {
        private string _body;
        private Call _call;

        public InDialogMessageBuilder SetCall(Call call)
        {
            Helper.GuardNotNull(call);
            Helper.GuardPositiveInt(call.Id);
            _call = call;
            return this;
        }

        public InDialogMessageBuilder SetBody(string body)
        {
            _body = body;
            return this;
        }

        public void SendMessage()
        {
            //pj_str_t mime = new pj_str_t("plain/text");
            //pj_str_t content = new pj_str_t(_body);
            //Helper.GuardError(SipUserAgent.ApiFactory.GetCallApi().pjsua_call_send_im(_call.Id, ref mime, ref content, null, IntPtr.Zero));
            SipUserAgent.Instance.SendMessageInDialog(_call, _body);
        }

        public void SendTyping(bool isTyping)
        {
            //Helper.GuardError(SipUserAgent.ApiFactory.GetCallApi().pjsua_call_send_typing_ind(_call.Id, Convert.ToInt32(isTyping), null));
            SipUserAgent.Instance.SendTypingInDialog(_call, isTyping);
        }
    }

    public class OfMessageBuilderExpression
    {
        private readonly InDialogMessageBuilder _builder;

        public OfMessageBuilderExpression(InDialogMessageBuilder builder)
        {
            Helper.GuardNotNull(builder);
            _builder = builder;
        }

        public FinalInDialogMessageBuilderExpression Of(Call call)
        {
            return new FinalInDialogMessageBuilderExpression(_builder.SetCall(call));
        }
    }

    public class FinalInDialogMessageBuilderExpression
    {
        private readonly InDialogMessageBuilder _builder;

        public FinalInDialogMessageBuilderExpression(InDialogMessageBuilder builder)
        {
            Helper.GuardNotNull(builder);
            _builder = builder;
        }

        public void Typing(bool isTyping)
        {
            _builder.SendTyping(isTyping);
        }

        public void Go(string message)
        {
            _builder.SetBody(message).SendMessage();
        }
    }
}