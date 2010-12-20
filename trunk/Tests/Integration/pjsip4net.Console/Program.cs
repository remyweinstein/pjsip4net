using System;
using pjsip.Interop;
using pjsip4net.Configuration;
using pjsip4net.Core;
using pjsip4net.Core.Configuration;

namespace pjsip4net.Console
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var ua = Configure.Pjsip4Net().FromConfig().WithVersion_1_4().Build().Start();
            ua.Log += (s, e) => System.Console.WriteLine(e.Data);
            var factory = new CommandFactory(ua);

            while (true)
            {
                try
                {
                    var line = System.Console.ReadLine();
                    if (string.IsNullOrEmpty(line))
                        break;
                    var command = factory.Create(line);
                    command.Execute();
                }
                catch(PjsipErrorException ex)
                {
                    System.Console.WriteLine(ex.Message);
                }
                catch(SystemException ex)
                {
                    System.Console.WriteLine(ex.Message);
                }
            }
            ua.Destroy();
        }
    }
}