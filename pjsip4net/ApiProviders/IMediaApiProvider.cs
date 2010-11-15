using System;
using pjsip.Interop;

namespace pjsip4net.ApiProviders
{
    public interface IMediaApiProvider
    {
        void pjsua_media_config_default(pjsua_media_config cfg);
        uint pjsua_conf_get_max_ports();
        uint pjsua_conf_get_active_ports();
        int pjsua_enum_conf_ports(int[] id, ref uint count);
        int pjsua_conf_get_port_info(int port_id, ref pjsua_conf_port_info info);
        int pjsua_conf_connect(int source, int sink);
        int pjsua_conf_disconnect(int source, int sink);
        int pjsua_conf_adjust_tx_level(int slot, float level);
        int pjsua_conf_adjust_rx_level(int slot, float level);
        int pjsua_conf_get_signal_level(int slot, ref uint tx_level, ref uint rx_level);
        int pjsua_player_create(ref pj_str_t filename, uint options, ref int p_id);

        int pjsua_playlist_create(ref pj_str_t file_names, uint file_count, ref pj_str_t label,
                                  uint options, ref int p_id);

        int pjsua_player_get_conf_port(int id);
        int pjsua_player_get_port(int id, ref IntPtr p_port);
        int pjsua_player_set_pos(int id, uint samples);
        int pjsua_player_destroy(int id);

        int pjsua_recorder_create(ref pj_str_t filename, uint enc_type, IntPtr enc_param,
                                  int max_size, uint options, ref int p_id);

        int pjsua_recorder_get_conf_port(int id);
        int pjsua_recorder_get_port(int id, ref IntPtr p_port);
        int pjsua_recorder_destroy(int id);
        int pjsua_enum_snd_devs(pjmedia_snd_dev_info[] info, ref uint count);
        int pjsua_get_snd_dev(ref int capture_dev, ref int playback_dev);
        int pjsua_set_snd_dev(int capture_dev, int playback_dev);
        int pjsua_set_null_snd_dev();
        IntPtr pjsua_set_no_snd_dev();
        int pjsua_set_ec(uint tail_ms, uint options);
        int pjsua_get_ec_tail(ref uint p_tail_ms);
        int pjsua_enum_codecs(pjsua_codec_info[] id, ref uint count);
        int pjsua_codec_set_priority(ref pj_str_t codec_id, byte priority);
        int pjsua_codec_get_param(ref pj_str_t codec_id, ref pjmedia_codec_param param);
        int pjsua_codec_set_param(ref pj_str_t codec_id, ref pjmedia_codec_param param);
        int pjsua_media_transports_create(pjsua_transport_config cfg);
    }
}