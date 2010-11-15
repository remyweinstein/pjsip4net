using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using pjsip.Interop;
using pjsip4net.Utils;

namespace pjsip4net.Media
{
    internal class MediaManager : IMediaManagerInternal
    {
        //#region Singleton

        private static readonly object _lock = new object();
        //private static MediaManager _instance;

        public MediaManager()
        {
            ConferenceBridge = new ConferenceBridge();
        }

        #region Private Data

        private MediaConfig _config;
        private SoundDevice _curCapture;
        private SoundDevice _curPlayback;
        private bool _muted;

        #endregion

        #region Properties

        private ReadOnlyCollection<CodecInfo> _codecs;

        private ReadOnlyCollection<SoundDevice> _sndDevs;
        public WavPlayer Ringer { get; set; }
        public WavPlayer CallbackRinger { get; set; }
        public WavPlayer BusyRinger { get; set; }
        public ConferenceBridge ConferenceBridge { get; private set; }

        public ReadOnlyCollection<CodecInfo> Codecs
        {
            get
            {
                if (_codecs == null)
                    lock (_lock)
                        if (_codecs == null)
                        {
                            var _infos = new pjsua_codec_info[32];
                            uint count = 32;
                            Helper.GuardError(SipUserAgent.ApiFactory.GetMediaApi().pjsua_enum_codecs(_infos, ref count));
                            var infos = new List<CodecInfo>();
                            for (int i = 0; i < count; i++)
                                infos.Add(new CodecInfo(_infos[i]));

                            _codecs = new ReadOnlyCollection<CodecInfo>(infos);
                        }
                return _codecs;
            }
        }

        public ReadOnlyCollection<SoundDevice> SoundDevices
        {
            get
            {
                if (_sndDevs == null)
                    lock (_lock)
                        if (_sndDevs == null)
                        {
                            var _infos = new pjmedia_snd_dev_info[32];
                            uint count = 32;
                            Helper.GuardError(SipUserAgent.ApiFactory.GetMediaApi().pjsua_enum_snd_devs(_infos,
                                                                                                        ref count));
                            var infos = new List<SoundDevice>();
                            for (int i = 0; i < count; i++)
                                infos.Add(new SoundDevice(_infos[i]) {Id = i});

                            _sndDevs = new ReadOnlyCollection<SoundDevice>(infos);
                        }
                return _sndDevs;
            }
        }

        public IEnumerable<SoundDevice> PlaybackDevices
        {
            get { return SoundDevices.Where(s => s.IsPlayback).Distinct(new ByNameEqualityComparer()); }
        }

        public IEnumerable<SoundDevice> CaptureDevices
        {
            get { return SoundDevices.Where(s => s.IsCapture).Distinct(new ByNameEqualityComparer()); }
        }

        public SoundDevice CurrentPlaybackDevice
        {
            get { return _curPlayback; }
        }

        public SoundDevice CurrentCaptureDevice
        {
            get { return _curCapture; }
        }

        //private int _outDev = 0;
        //public SoundDevice PlaybackDevice
        //{
        //    get
        //    {
        //        int outDev = -1, captDev = -1;
        //        Helper.GuardError(PJSUA_DLL.Media.pjsua_get_snd_dev(ref captDev, ref outDev));
        //        _outDev = outDev;
        //        _captDev = captDev;
        //        return _captDev != NativeConstants.PJSUA_INVALID_ID ? SoundDevices[_outDev] : null;
        //    }
        //    set
        //    {
        //        Helper.GuardNotNull(value);
        //        Helper.GuardInRange(1u, 100u, value.OutputCount);
        //        Helper.GuardPositiveInt(SoundDevices.IndexOf(value));
        //        _outDev = SoundDevices.IndexOf(value);
        //        Helper.GuardError(PJSUA_DLL.Media.pjsua_set_snd_dev(_captDev, _outDev));
        //    }
        //}

        //private int _captDev = 0;
        //public SoundDevice CaptureDevice
        //{
        //    get
        //    {
        //        int outDev = -1, captDev = -1;
        //        Helper.GuardError(PJSUA_DLL.Media.pjsua_get_snd_dev(ref captDev, ref outDev));
        //        _outDev = outDev;
        //        _captDev = captDev;
        //        return _captDev != NativeConstants.PJSUA_INVALID_ID ? SoundDevices[_captDev] : null;
        //    }
        //    set
        //    {
        //        Helper.GuardNotNull(value);
        //        Helper.GuardInRange(1u, 100u, value.InputCount);
        //        Helper.GuardPositiveInt(SoundDevices.IndexOf(value));
        //        _captDev = SoundDevices.IndexOf(value);
        //        Helper.GuardError(PJSUA_DLL.Media.pjsua_set_snd_dev(_captDev, _outDev));
        //    }
        //}

        #endregion

        #region Methods

        public void ToggleMute()
        {
            lock (_lock)
            {
                if (_muted)
                    SetDevices();
                else
                    Helper.GuardError(SipUserAgent.ApiFactory.GetMediaApi().pjsua_set_null_snd_dev());

                _muted = !_muted;
            }
        }

        public void SetConfiguration(MediaConfig config)
        {
            _config = config;
            SetDevices();
        }

        public void SetSoundDevices(SoundDevice playback, SoundDevice capture)
        {
            SetSoundDevices(playback == null ? -1 : playback.Id, capture == null ? -1 : capture.Id);
        }

        public void SetSoundDevices(int playback, int capture)
        {
            _config.CaptureDeviceId = capture;
            _config.PlaybackDeviceId = playback;

            SetDevices();
        }

        internal void CreateRingers()
        {
            //Ringer = new WavPlayer(@"Sounds\old-phone-ring6.wav", true);
            //CallbackRinger = new WavPlayer(@"Sounds\ring.wav", true);
            //BusyRinger = new WavPlayer(@"Sounds\busy.wav", true);
        }

        private void SetDevices()
        {
            //if (_curCapture == null && _curPlayback == null)
            {
                _curCapture = _config.CaptureDeviceId != NativeConstants.PJSUA_INVALID_ID
                                  ? SoundDevices.Where(s => s.Id == _config.CaptureDeviceId).Take(1).SingleOrDefault()
                                  : null;
                _curPlayback = _config.PlaybackDeviceId != NativeConstants.PJSUA_INVALID_ID
                                   ? SoundDevices.Where(s => s.Id == _config.PlaybackDeviceId).Take(1).SingleOrDefault()
                                   : null;
            }

            if (_curCapture != null && _curPlayback != null)
                Helper.GuardError(SipUserAgent.ApiFactory.GetMediaApi().pjsua_set_snd_dev(_curCapture.Id,
                                                                                          _curPlayback.Id));
            else
                Helper.GuardError(SipUserAgent.ApiFactory.GetMediaApi().pjsua_set_null_snd_dev());
        }

        #endregion

        #region Nested type: ByNameEqualityComparer

        private class ByNameEqualityComparer : IEqualityComparer<SoundDevice>, IEqualityComparer
        {
            #region Implementation of IEqualityComparer<SoundDevice>

            public bool Equals(SoundDevice x, SoundDevice y)
            {
                return x.Name.Equals(y.Name);
            }

            public int GetHashCode(SoundDevice obj)
            {
                return obj.Name.GetHashCode();
            }

            #endregion

            #region Implementation of IEqualityComparer

            public bool Equals(object x, object y)
            {
                return Equals((SoundDevice) x, (SoundDevice) y);
            }

            public int GetHashCode(object obj)
            {
                return GetHashCode((SoundDevice) obj);
            }

            #endregion
        }

        #endregion

        //internal static MediaManager Instance
        //{
        //    get
        //    {
        //        if (_instance == null)
        //            lock(_lock)
        //                if (_instance == null)
        //                    _instance = new MediaManager();
        //        return _instance;
        //    }
        //}

        //#endregion
    }
}