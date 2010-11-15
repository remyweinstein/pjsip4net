using pjsip4net.Accounts;
using pjsip4net.Calls;

namespace pjsip4net.Interfaces
{
    public interface ICallBuilder
    {
        ICallBuilder To(string extension);
        ICallBuilder At(string domain);
        ICallBuilder Through(string port);
        ICallBuilder From(Account account);
        Call Call();
    }
}