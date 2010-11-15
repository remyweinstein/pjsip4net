using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace pjsip4net.Media
{
    public interface IMediaManager
    {
        ConferenceBridge ConferenceBridge { get; }
        ReadOnlyCollection<CodecInfo> Codecs { get; }
        ReadOnlyCollection<SoundDevice> SoundDevices { get; }
        IEnumerable<SoundDevice> PlaybackDevices { get; }
        IEnumerable<SoundDevice> CaptureDevices { get; }
        SoundDevice CurrentPlaybackDevice { get; }
        SoundDevice CurrentCaptureDevice { get; }
        void ToggleMute();
        void SetSoundDevices(SoundDevice playback, SoundDevice capture);
        void SetSoundDevices(int playback, int capture);
    }

    internal interface IMediaManagerInternal : IMediaManager
    {
        void SetConfiguration(MediaConfig mediaConfig);
    }
}