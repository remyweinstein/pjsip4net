using System;
using pjsip.Interop;

namespace pjsip4net.ApiProviders.Impl
{
    public class Pjsip_1_4_MediaApiProvider : IMediaApiProvider
    {
        #region IMediaApiProvider Members

        public void pjsua_media_config_default(pjsua_media_config cfg)
        {
            PJSUA_DLL.Media.pjsua_media_config_default(cfg);
        }

        public uint pjsua_conf_get_max_ports()
        {
            return PJSUA_DLL.Media.pjsua_conf_get_max_ports();
        }

        public uint pjsua_conf_get_active_ports()
        {
            return PJSUA_DLL.Media.pjsua_conf_get_active_ports();
        }

        public int pjsua_enum_conf_ports(int[] id, ref uint count)
        {
            return PJSUA_DLL.Media.pjsua_enum_conf_ports(id, ref count);
        }

        public int pjsua_conf_get_port_info(int port_id, ref pjsua_conf_port_info info)
        {
            return PJSUA_DLL.Media.pjsua_conf_get_port_info(port_id, ref info);
        }

        public int pjsua_conf_connect(int source, int sink)
        {
            return PJSUA_DLL.Media.pjsua_conf_connect(source, sink);
        }

        public int pjsua_conf_disconnect(int source, int sink)
        {
            return PJSUA_DLL.Media.pjsua_conf_disconnect(source, sink);
        }

        public int pjsua_conf_adjust_tx_level(int slot, float level)
        {
            return PJSUA_DLL.Media.pjsua_conf_adjust_tx_level(slot, level);
        }

        public int pjsua_conf_adjust_rx_level(int slot, float level)
        {
            return PJSUA_DLL.Media.pjsua_conf_adjust_rx_level(slot, level);
        }

        public int pjsua_conf_get_signal_level(int slot, ref uint tx_level, ref uint rx_level)
        {
            return PJSUA_DLL.Media.pjsua_conf_get_signal_level(slot, ref tx_level, ref rx_level);
        }

        public int pjsua_player_create(ref pj_str_t filename, uint options, ref int p_id)
        {
            return PJSUA_DLL.Media.pjsua_player_create(ref filename, options, ref p_id);
        }

        public int pjsua_playlist_create(ref pj_str_t file_names, uint file_count, ref pj_str_t label, uint options,
                                         ref int p_id)
        {
            return PJSUA_DLL.Media.pjsua_playlist_create(ref file_names, file_count, ref label, options, ref p_id);
        }

        public int pjsua_player_get_conf_port(int id)
        {
            return PJSUA_DLL.Media.pjsua_player_get_conf_port(id);
        }

        public int pjsua_player_get_port(int id, ref IntPtr p_port)
        {
            return PJSUA_DLL.Media.pjsua_player_get_port(id, ref p_port);
        }

        public int pjsua_player_set_pos(int id, uint samples)
        {
            return PJSUA_DLL.Media.pjsua_player_set_pos(id, samples);
        }

        public int pjsua_player_destroy(int id)
        {
            return PJSUA_DLL.Media.pjsua_player_destroy(id);
        }

        public int pjsua_recorder_create(ref pj_str_t filename, uint enc_type, IntPtr enc_param, int max_size,
                                         uint options, ref int p_id)
        {
            return PJSUA_DLL.Media.pjsua_recorder_create(ref filename, enc_type, enc_param, max_size, options, ref p_id);
        }

        public int pjsua_recorder_get_conf_port(int id)
        {
            return PJSUA_DLL.Media.pjsua_recorder_get_conf_port(id);
        }

        public int pjsua_recorder_get_port(int id, ref IntPtr p_port)
        {
            return PJSUA_DLL.Media.pjsua_recorder_get_port(id, ref p_port);
        }

        public int pjsua_recorder_destroy(int id)
        {
            return PJSUA_DLL.Media.pjsua_recorder_destroy(id);
        }

        public int pjsua_enum_snd_devs(pjmedia_snd_dev_info[] info, ref uint count)
        {
            return PJSUA_DLL.Media.pjsua_enum_snd_devs(info, ref count);
        }

        public int pjsua_get_snd_dev(ref int capture_dev, ref int playback_dev)
        {
            return PJSUA_DLL.Media.pjsua_get_snd_dev(ref capture_dev, ref playback_dev);
        }

        public int pjsua_set_snd_dev(int capture_dev, int playback_dev)
        {
            return PJSUA_DLL.Media.pjsua_set_snd_dev(capture_dev, playback_dev);
        }

        public int pjsua_set_null_snd_dev()
        {
            return PJSUA_DLL.Media.pjsua_set_null_snd_dev();
        }

        public IntPtr pjsua_set_no_snd_dev()
        {
            return PJSUA_DLL.Media.pjsua_set_no_snd_dev();
        }

        public int pjsua_set_ec(uint tail_ms, uint options)
        {
            return PJSUA_DLL.Media.pjsua_set_ec(tail_ms, options);
        }

        public int pjsua_get_ec_tail(ref uint p_tail_ms)
        {
            return PJSUA_DLL.Media.pjsua_get_ec_tail(ref p_tail_ms);
        }

        public int pjsua_enum_codecs(pjsua_codec_info[] id, ref uint count)
        {
            return PJSUA_DLL.Media.pjsua_enum_codecs(id, ref count);
        }

        public int pjsua_codec_set_priority(ref pj_str_t codec_id, byte priority)
        {
            return PJSUA_DLL.Media.pjsua_codec_set_priority(ref codec_id, priority);
        }

        public int pjsua_codec_get_param(ref pj_str_t codec_id, ref pjmedia_codec_param param)
        {
            return PJSUA_DLL.Media.pjsua_codec_get_param(ref codec_id, ref param);
        }

        public int pjsua_codec_set_param(ref pj_str_t codec_id, ref pjmedia_codec_param param)
        {
            return PJSUA_DLL.Media.pjsua_codec_set_param(ref codec_id, ref param);
        }

        public int pjsua_media_transports_create(pjsua_transport_config cfg)
        {
            return PJSUA_DLL.Media.pjsua_media_transports_create(cfg);
        }

        #endregion
    }
}