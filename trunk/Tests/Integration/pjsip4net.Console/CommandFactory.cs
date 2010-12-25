using System;
using System.Linq;
using Magnum.CommandLine;
using pjsip4net.Core.Utils;
using pjsip4net.Interfaces;

namespace pjsip4net.Console
{
    public class CommandFactory : ICommandFactory
    {
        private readonly ISipUserAgent _userAgent;
        private readonly CommandLineParser _parser = new CommandLineParser();

        public CommandFactory(ISipUserAgent userAgent)
        {
            _userAgent = userAgent;
            _parser.RegisterArgumentsForCommand<RegisterAccountArguments>("register");
            _parser.RegisterArgumentsForCommand<IdArguments>("unregister");
            _parser.RegisterArgumentsForCommand<NullArguments>("accounts");
            _parser.RegisterArgumentsForCommand<NullArguments>("codecs");
            _parser.RegisterArgumentsForCommand<CodecArguments>("setcodec");
            _parser.RegisterArgumentsForCommand<DeviceArguments>("setdevice");
            _parser.RegisterArgumentsForCommand<NullArguments>("devices");
            _parser.RegisterArgumentsForCommand<NullArguments>("calls");
            _parser.RegisterArgumentsForCommand<CallArguments>("makecall");
            _parser.RegisterArgumentsForCommand<IdArguments>("hangupcall");
            System.Console.WriteLine(_parser.WhatsRegistered());
        }

        #region Implementation of ICommandFactory

        public ICommand Create(string cmd)
        {
            var output = _parser.Parse(cmd.Split(new []{' '}, StringSplitOptions.RemoveEmptyEntries));
            switch (output.CommandName)
            {
                case "register":
                    return new RegisterAccountCommand(_userAgent, output.ParsedArguments.As<RegisterAccountArguments>());
                case "accounts":
                    return new ShowAccountsCommand(_userAgent);
                case "unregister":
                    return new UnregisterAccountCommand(_userAgent, output.ParsedArguments.As<IdArguments>());
                case "codecs":
                    return new ShowCodecsCommand(_userAgent);
                case "setcodec":
                    return new SetCodecCommand(_userAgent,output.ParsedArguments.As<CodecArguments>());
                case "devices":
                    return new ShowDevicesCommand(_userAgent);
                case "setdevice":
                    return new SetDeviceCommand(_userAgent, output.ParsedArguments.As<DeviceArguments>());
                case "calls":
                    return new ShowCallsCommand(_userAgent);
                case "makecall":
                    return new MakeCallCommand(_userAgent, output.ParsedArguments.As<CallArguments>());
                case "hangupcall":
                    return new HangupCallCommand(_userAgent, output.ParsedArguments.As<IdArguments>());
                default:
                    return new NoOpCommand();
            }
        }

        #endregion
    }

    public class NoOpCommand : ICommand
    {
        public void Execute()
        {
        }
    }

    public class QuitCommand : ICommand
    {
        private readonly ISipUserAgent _agent;

        public QuitCommand(ISipUserAgent agent)
        {
            _agent = agent;
        }

        #region Implementation of ICommand

        public void Execute()
        {
            _agent.Destroy();
        }

        #endregion
    }

    public class RegisterAccountCommand : ICommand
    {
        private readonly ISipUserAgent _agent;
        private readonly RegisterAccountArguments _arguments;

        public RegisterAccountCommand(ISipUserAgent agent, RegisterAccountArguments arguments)
        {
            _agent = agent;
            _arguments = arguments;
        }

        #region Implementation of ICommand

        public void Execute()
        {
            var builder =
                _agent.Container.Get<IAccountBuilder>().WithExtension(_arguments.Extension).WithPassword(
                    _arguments.Password);
            if (_arguments.Port != null)
                builder.Through(_arguments.Port);
            builder.At(_arguments.Domain).Register();
        }

        #endregion
    }

    public class ShowAccountsCommand : ICommand
    {
        private readonly ISipUserAgent _agent;

        public ShowAccountsCommand(ISipUserAgent agent)
        {
            _agent = agent;
        }

        public void Execute()
        {
            System.Console.WriteLine("Available accounts");
            _agent.AccountManager.Accounts.Each(x => System.Console.WriteLine(x.AccountId + " " +
                                                                              x.StatusText));
        }
    }

    public class UnregisterAccountCommand : ICommand
    {
        private readonly ISipUserAgent _agent;
        private readonly IdArguments _args;

        public UnregisterAccountCommand(ISipUserAgent agent, IdArguments args)
        {
            _agent = agent;
            _args = args;
        }

        public void Execute()
        {
            _agent.AccountManager.GetAccountById(int.Parse(_args.Id)).Unregister();
        }
    }

    public class SetCodecCommand : ICommand
    {
        private readonly ISipUserAgent _agent;
        private readonly CodecArguments _args;

        public SetCodecCommand(ISipUserAgent agent, CodecArguments args)
        {
            _agent = agent;
            _args = args;
        }

        public void Execute()
        {
            _agent.MediaManager.Codecs.Single(x =>
                                              x.CodecId.Equals(
                                                  string.Concat(_args.CodecId + "/" + _args.Frequency + "/" +
                                                                _args.Channels)))
                .Priority = byte.Parse(_args.Priority);
        }
    }

    public class ShowCodecsCommand : ICommand
    {
        private readonly ISipUserAgent _agent;

        public ShowCodecsCommand(ISipUserAgent agent)
        {
            _agent = agent;
        }

        public void Execute()
        {
            System.Console.WriteLine("Available codecs (name & priority)");
            _agent.MediaManager.Codecs.Each(x => System.Console.WriteLine(x.CodecId + " " + x.Priority));
        }
    }

    public class SetDeviceCommand : ICommand
    {
        private readonly ISipUserAgent _agent;
        private readonly DeviceArguments _args;

        public SetDeviceCommand(ISipUserAgent agent, DeviceArguments args)
        {
            _agent = agent;
            _args = args;
        }

        public void Execute()
        {
            _agent.MediaManager.SetSoundDevices(int.Parse(_args.PlaybackId), int.Parse(_args.CaptureId));
        }
    }

    public class ShowDevicesCommand : ICommand
    {
        private readonly ISipUserAgent _agent;

        public ShowDevicesCommand(ISipUserAgent agent)
        {
            _agent = agent;
        }

        public void Execute()
        {
            System.Console.Write("Current playback device - ");
            System.Console.WriteLine(_agent.MediaManager.CurrentPlaybackDevice.Id + " " + 
                _agent.MediaManager.CurrentPlaybackDevice.Name);
            System.Console.Write("Current capture device - ");
            System.Console.WriteLine(_agent.MediaManager.CurrentCaptureDevice.Id + " " + 
                _agent.MediaManager.CurrentCaptureDevice.Name);
            System.Console.WriteLine("---------------------------");
            System.Console.WriteLine("Available playback devices");
            _agent.MediaManager.PlaybackDevices.Each(x => System.Console.WriteLine(x.Id + " " + x.Name));
            System.Console.WriteLine("Available capture devices");
            _agent.MediaManager.CaptureDevices.Each(x => System.Console.WriteLine(x.Id + " " + x.Name));
        }
    }

    public class ShowCallsCommand : ICommand
    {
        private readonly ISipUserAgent _agent;

        public ShowCallsCommand(ISipUserAgent agent)
        {
            _agent = agent;
        }

        public void Execute()
        {
            System.Console.WriteLine("Active calls");
            _agent.CallManager.Calls.Each(x => System.Console.WriteLine(x.ToString(true)));
        }
    }

    public class MakeCallCommand : ICommand
    {
        private readonly ISipUserAgent _agent;
        private readonly CallArguments _args;

        public MakeCallCommand(ISipUserAgent agent, CallArguments args)
        {
            _agent = agent;
            _args = args;
        }

        public void Execute()
        {
            _agent.Container.Get<ICallBuilder>().To(_args.To).At(_args.At).Through(_args.Through)
                .From(_agent.AccountManager.GetAccountById(Convert.ToInt32(_args.From))).Call();
        }
    }

    public class HangupCallCommand : ICommand
    {
        private readonly ISipUserAgent _agent;
        private readonly IdArguments _args;

        public HangupCallCommand(ISipUserAgent agent, IdArguments args)
        {
            _agent = agent;
            _args = args;
        }

        public void Execute()
        {
            _agent.CallManager.GetCallById(Convert.ToInt32(_args.Id)).Hangup();
        }
    }

}