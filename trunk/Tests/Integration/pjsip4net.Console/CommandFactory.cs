using System;
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
            _parser.RegisterArgumentsForCommand<NullArguments>("unregister");
        }

        #region Implementation of ICommandFactory

        public ICommand Create(string cmd)
        {
            var output = _parser.Parse(cmd.Split(new []{' '}, StringSplitOptions.RemoveEmptyEntries));
            switch (output.CommandName)
            {
                case "register":
                    return new RegisterAccountCommand(_userAgent, output.ParsedArguments.As<RegisterAccountArguments>());
                case "unregister":
                    return new UnregisterAccountCommand(_userAgent);
                default:
                    return new QuitCommand(_userAgent);
            }
        }

        #endregion
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

    public class UnregisterAccountCommand : ICommand
    {
        private readonly ISipUserAgent _agent;

        public UnregisterAccountCommand(ISipUserAgent agent)
        {
            _agent = agent;
        }

        public void Execute()
        {
            System.Console.WriteLine("Enter account id to unregister.");
            _agent.AccountManager.Accounts.Each(x => System.Console.WriteLine(x.Id + " " + x.AccountId));
            var id = int.Parse(System.Console.ReadLine());
            _agent.AccountManager.GetAccountById(id).Unregister();
        }
    }
}