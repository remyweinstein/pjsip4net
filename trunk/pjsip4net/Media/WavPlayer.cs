using System;
using System.IO;
using pjsip.Interop;
using pjsip4net.Utils;

namespace pjsip4net.Media
{
    public class WavPlayer : Resource, IIdentifiable<WavPlayer>, IDisposable
    {
        #region Private Data

        private MediaSlotInfo _mediaInfo;

        #endregion

        #region Properties

        public string File { get; private set; }

        public MediaSlotInfo ConferenceSlot
        {
            get
            {
                //GuardDisposed();
                if (Id != NativeConstants.PJSUA_INVALID_ID)
                    if (_mediaInfo == null)
                        _mediaInfo =
                            new MediaSlotInfo(SipUserAgent.ApiFactory.GetMediaApi().pjsua_player_get_conf_port(Id));
                return _mediaInfo;
            }
        }

        public int Id { get; private set; }

        #endregion

        #region Methods

        public WavPlayer(string file, bool loop)
        {
            Id = NativeConstants.PJSUA_INVALID_ID;
            Helper.GuardNotNullStr(file);

            int id = NativeConstants.PJSUA_INVALID_ID;
            var filename = new pj_str_t(Path.GetFullPath(file));
            File = file;

            Helper.GuardError(SipUserAgent.ApiFactory.GetMediaApi().pjsua_player_create(ref filename, loop ? 1u : 0u,
                                                                                        ref id));
            Id = id;
            //SipUserAgent.Instance.MediaManager.ConferenceBridge.ConnectToSoundDevice(ConferenceSlot.Id);
        }

        public void SetPosition(uint position)
        {
            GuardDisposed();
            Helper.GuardError(SipUserAgent.ApiFactory.GetMediaApi().pjsua_player_set_pos(Id, position));
        }

        protected override void CleanUp()
        {
            try
            {
                SipUserAgent.Instance.MediaManager.ConferenceBridge.DisconnectFromSoundDevice(ConferenceSlot.Id);
                Helper.GuardError(SipUserAgent.ApiFactory.GetMediaApi().pjsua_player_destroy(Id));
            }
            finally
            {
                _mediaInfo = null;
                Id = NativeConstants.PJSUA_INVALID_ID;
            }
        }

        #endregion

        #region Implementation of IDisposable

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion

        #region Implementation of IEquatable<IIdentifiable<WavPlayer>>

        public bool Equals(IIdentifiable<WavPlayer> other)
        {
            return EqualsTemplate.Equals(this, other);
        }

        bool IIdentifiable<WavPlayer>.DataEquals(WavPlayer other)
        {
            return true;
        }

        #endregion
    }
}