using System;
using System.Net;
using pjsip.Interop;

namespace pjsip4net.Media
{
    public class MediaConfig : Initializable
    {
        internal pjsua_media_config _config = new pjsua_media_config();

        public int CaptureDeviceId = NativeConstants.PJSUA_INVALID_ID,
                   PlaybackDeviceId = NativeConstants.PJSUA_INVALID_ID;

        internal MediaConfig()
        {
        }

        #region Properties

        public uint ClockRate
        {
            get { return _config.clock_rate; }
            set
            {
                GuardNotInitializing();
                _config.clock_rate = value;
            }
        }

        public uint SoundDeviceClockRate
        {
            get { return _config.snd_clock_rate; }
            set
            {
                GuardNotInitializing();
                _config.snd_clock_rate = value;
            }
        }

        public uint ChannelCount
        {
            get { return _config.channel_count; }
            set
            {
                GuardNotInitializing();
                _config.channel_count = value;
            }
        }

        public uint AudioFramePerTime
        {
            get { return _config.audio_frame_ptime; }
            set
            {
                GuardNotInitializing();
                _config.audio_frame_ptime = value;
            }
        }

        public uint MaxMediaPorts
        {
            get { return _config.max_media_ports; }
            set
            {
                GuardNotInitializing();
                _config.max_media_ports = value;
            }
        }

        public bool HasIOQueue
        {
            get { return _config.has_ioqueue == 1; }
            set
            {
                GuardNotInitializing();
                _config.has_ioqueue = Convert.ToInt32(value);
            }
        }

        public uint ThreadCount
        {
            get { return _config.thread_cnt; }
            set
            {
                GuardNotInitializing();
                _config.thread_cnt = value;
            }
        }

        public uint Quality
        {
            get { return _config.quality; }
            set
            {
                GuardNotInitializing();
                _config.quality = value;
            }
        }

        public uint DefaultCodecPTime
        {
            get { return _config.ptime; }
            set
            {
                GuardNotInitializing();
                _config.ptime = value;
            }
        }

        public bool VadEnabled
        {
            get { return !(_config.no_vad == 1); }
            set
            {
                GuardNotInitializing();
                _config.no_vad = Convert.ToInt32(!value);
            }
        }

        public uint ILBCMode
        {
            get { return _config.ilbc_mode; }
            set
            {
                GuardNotInitializing();
                _config.ilbc_mode = value;
            }
        }

        public uint EcOptions
        {
            get { return _config.ec_options; }
            set
            {
                GuardNotInitializing();
                _config.ec_options = value;
            }
        }

        public uint EcTailLength
        {
            get { return _config.ec_tail_len; }
            set
            {
                GuardNotInitializing();
                _config.ec_tail_len = value;
            }
        }

        public bool EnableICE
        {
            get { return _config.enable_ice == 1; }
            set
            {
                GuardNotInitializing();
                _config.enable_ice = Convert.ToInt32(value);
            }
        }

        //public bool ICENoHostCandidates
        //{
        //    get { return _config.ice_no_host_cands == 1; }
        //    set
        //    {
        //        GuardNotInitializing();
        //        _config.ice_no_host_cands = Convert.ToInt32(value);
        //    }
        //}

        public bool ICENoRtcp
        {
            get { return _config.ice_no_rtcp == 1; }
            set
            {
                GuardNotInitializing();
                _config.ice_no_rtcp = Convert.ToInt32(value);
            }
        }

        public bool EnableTurn
        {
            get { return _config.enable_turn == 1; }
            set
            {
                GuardNotInitializing();
                _config.enable_turn = Convert.ToInt32(value);
            }
        }

        public string TurnServer
        {
            get { return _config.turn_server; }
            set
            {
                GuardNotInitializing();
                _config.turn_server = new pj_str_t(value);
            }
        }

        public TransportType TurnConnectionType
        {
            get { return (TransportType) _config.turn_conn_type; }
            set
            {
                GuardNotInitializing();
                _config.turn_conn_type = (pj_turn_tp_type) value;
            }
        }

        public NetworkCredential TurnAuthentication
        {
            get { return _config.turn_auth_cred.ToNetworkCredential(); }
            set
            {
                GuardNotInitializing();
                _config.turn_auth_cred = value.ToStunAuthCredential();
            }
        }

        public TimeSpan SoundDeviceAutoCloseTime
        {
            get { return TimeSpan.FromSeconds(_config.snd_auto_close_time); }
            set
            {
                GuardNotInitializing();
                _config.snd_auto_close_time = (int) value.TotalSeconds;
            }
        }

        #endregion

        public override void BeginInit()
        {
            base.BeginInit();
            SipUserAgent.ApiFactory.GetMediaApi().pjsua_media_config_default(_config);
        }

        public override void EndInit()
        {
            base.EndInit();
            //validate properties here

            //_config.turn_auth_cred = new NetworkCredential("test", "1234", "test").ToStunAuthCredential();//todo: don't hardcode this is rediculous. move to config.
        }
    }
}