namespace pjsip4net.Console
{
    public class RegisterAccountArguments
    {
        public string Extension { get; set; }
        public string Password { get; set; }
        public string Domain { get; set; }
        public string Port { get; set; }
        public string Transport { get; set; }
    }

    public class CodecArguments
    {
        public string CodecId { get; set; }
        public string Frequency { get; set; }
        public string Channels { get; set; }
        public string Priority { get; set; }
    }

    public class DeviceArguments
    {
        public string PlaybackId { get; set; }
        public string CaptureId { get; set; }
    }

    public class IdArguments
    {
        public string Id { get; set; }
    }

    public class CallArguments
    {
        public string To { get; set; }
        public string At { get; set; }
        public string Through { get; set; }
        //public string Via { get; set; }
        public string From { get; set; }
    }

    public class NullArguments{}
}