using pjsip4net.IM.Dsl;

namespace pjsip4net.IM
{
    public static class InstantMessage
    {
        public static ToMessageBuilderExpression Send()
        {
            return new ToMessageBuilderExpression(new MessageBuilder());
        }

        public static OfMessageBuilderExpression SendInDialog()
        {
            return new OfMessageBuilderExpression(new InDialogMessageBuilder());
        }
    }
}