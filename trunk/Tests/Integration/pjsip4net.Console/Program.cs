using System;
using pjsip.Interop;
using pjsip4net.Calls;
using pjsip4net.Configuration;
using pjsip4net.Core;
using pjsip4net.Core.Configuration;
using pjsip4net.Core.Data;

namespace pjsip4net.Console
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var ua = Configure.Pjsip4Net().FromConfig().WithVersion_1_4().Build().Start();
            ua.Log += (s, e) => System.Console.WriteLine(e.Data);
            ua.ImManager.IncomingMessage += ImManager_IncomingMessage;
            ua.CallManager.CallRedirected += CallManager_CallRedirected;
            var factory = new CommandFactory(ua);
            factory.Create("?").Execute();

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

        static void ImManager_IncomingMessage(object sender, PagerEventArgs e)
        {
            System.Console.WriteLine("Message from " + e.From + ", text: " + e.Body);
        }

        static void CallManager_CallRedirected(object sender, CallRedirectedEventArgs e)
        {
            e.Option = RedirectOption.Accept;
        }
    }
}