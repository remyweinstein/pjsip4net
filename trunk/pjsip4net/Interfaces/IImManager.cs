using System;
using System.Collections.ObjectModel;
using pjsip4net.Accounts;
using pjsip4net.Buddy;
using pjsip4net.Calls;
using pjsip4net.Core.Interfaces;

namespace pjsip4net.Interfaces
{
    public interface IImManager : IDisposable, IInitializable
    {
        //IApiFactory ApiFactory { get; set; }
        //IConfigurationProvider ConfigurationProvider { get; set; }
        //IAccountManager AccountManager { get; }
        //ICallManager CallManager { get; }
        //IMediaManager MediaManager { get; }
        //UaConfig Config { get; }
        //LoggingConfig LoggingConfig { get; }
        //MediaConfig MediaConfig { get; }
        ReadOnlyCollection<Buddy.Buddy> Buddies { get; }
        event EventHandler<LogEventArgs> Log;
        event EventHandler<BuddyStateChangedEventArgs> BuddyStateChanged;
        event EventHandler<PagerEventArgs> IncomingMessage;
        event EventHandler<TypingEventArgs> TypingAlert;
        event EventHandler<NatEventArgs> NatDetected;

        void RegisterBuddy(Buddy.Buddy buddy);
        void UnregisterBuddy(Buddy.Buddy buddy);
        Buddy.Buddy GetBuddyById(int id);
        void SendMessage(Account account, string body, string to);
        void SendMessageInDialog(Call dialog, string body);
        void SendTyping(Account account, string to, bool isTyping);
        void SendTypingInDialog(Call dialog, bool isTyping);
    }
}