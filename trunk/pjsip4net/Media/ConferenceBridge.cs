using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using pjsip4net.Calls;
using pjsip4net.Utils;

namespace pjsip4net.Media
{
    public class ConferenceBridge
    {
        #region Private Data

        private readonly List<Call> _calls = new List<Call>();
        private readonly object _lock = new object();

        #endregion

        #region Properties

        public bool IsConferenceActive
        {
            get { return _calls.Count > 1; }
        }

        public uint MaxPorts
        {
            get { return SipUserAgent.ApiFactory.GetMediaApi().pjsua_conf_get_max_ports(); }
        }

        public uint ActivePorts
        {
            get { return SipUserAgent.ApiFactory.GetMediaApi().pjsua_conf_get_active_ports(); }
        }

        public double? RxLevel
        {
            get { return _calls.Count > 0 ? _calls.Select(c => c.RxLevel).Average() : (double?) null; }
            set
            {
                foreach (Call call in _calls)
                    call.RxLevel = value.Value;
            }
        }

        public double? TxLevel
        {
            get { return _calls.Count > 0 ? _calls.Select(c => c.TxLevel).Average() : (double?) null; }
            set
            {
                foreach (Call call in _calls)
                    call.TxLevel = value.Value;
            }
        }

        #endregion

        #region Methods

        internal ConferenceBridge()
        {
        }

        public void InterConnect(int slotX, int slotY)
        {
            Helper.GuardPositiveInt(slotX);
            Helper.GuardPositiveInt(slotY);
            Helper.GuardError(SipUserAgent.ApiFactory.GetMediaApi().pjsua_conf_connect(slotX, slotY));
            Helper.GuardError(SipUserAgent.ApiFactory.GetMediaApi().pjsua_conf_connect(slotY, slotX));
        }

        public void Disconnect(int slotX, int slotY)
        {
            Helper.GuardPositiveInt(slotX);
            Helper.GuardPositiveInt(slotY);
            Helper.GuardError(SipUserAgent.ApiFactory.GetMediaApi().pjsua_conf_disconnect(slotX, slotY));
            Helper.GuardError(SipUserAgent.ApiFactory.GetMediaApi().pjsua_conf_disconnect(slotY, slotX));
        }

        public void ConnectToSoundDevice(int slotId)
        {
            InterConnect(slotId, 0);
        }

        public void DisconnectFromSoundDevice(int slotId)
        {
            Disconnect(slotId, 0);
        }

        internal void ConnectCall(Call call)
        {
            Helper.GuardNotNull(call);
            Helper.GuardPositiveInt(call.Id);
            //Helper.GuardNotNull(call.ConferenceSlotInfo);

            //lock (_lock)
            {
                foreach (Call c in _calls)
                    InterConnect(call.ConferenceSlotId, c.ConferenceSlotId);
                _calls.Add(call);

                Debug.WriteLine("Conference connected call id = " + call.Id);
            }
        }

        internal void DisconnectCall(Call call)
        {
            Helper.GuardNotNull(call);
            Helper.GuardPositiveInt(call.Id);

            //lock (_lock)
            {
                _calls.Remove(call);

                //if (call.IsActive && call.HasMedia)
                //foreach (var c in _calls)
                //    Disconnect(call.ConferenceSlotId, c.ConferenceSlotId);

                Debug.WriteLine("Conference disconnected call id = " + call.Id);
            }
        }

        #endregion
    }
}