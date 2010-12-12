using System;
using System.Collections.ObjectModel;
using pjsip4net.Core.Interfaces;
using pjsip4net.IM;

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
        ReadOnlyCollection<IBuddy> Buddies { get; }
        event EventHandler<BuddyStateChangedEventArgs> BuddyStateChanged;
        event EventHandler<PagerEventArgs> IncomingMessage;
        event EventHandler<TypingEventArgs> TypingAlert;
        event EventHandler<NatEventArgs> NatDetected;

        void RegisterBuddy(IBuddy buddy);
        void UnregisterBuddy(IBuddy buddy);
        IBuddy GetBuddyById(int id);
        void SendMessage(IAccount account, string body, string to);
        void SendMessageInDialog(ICall dialog, string body);
        void SendTyping(IAccount account, string to, bool isTyping);
        void SendTypingInDialog(ICall dialog, bool isTyping);
    }
}