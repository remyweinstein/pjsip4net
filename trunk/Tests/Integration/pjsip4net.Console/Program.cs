using pjsip.Interop;
using pjsip4net.Configuration;
using pjsip4net.Core.Configuration;

namespace pjsip4net.Console
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var ua = Configure.Pjsip4Net().WithVersion_1_4().Build().Start();
        }
    }
}