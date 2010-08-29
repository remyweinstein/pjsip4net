namespace pjsip4net
{
    public enum SrtpRequirement
    {
        /// <summary>
        /// PJMEDIA_SRTP_DISABLED 	
        /// When this flag is specified, SRTP will be disabled, and the transport will reject RTP/SAVP offer. 
        /// </summary>
        Disabled,
        /// <summary>
        /// PJMEDIA_SRTP_OPTIONAL 	
        /// When this flag is specified, SRTP will be advertised as optional and incoming SRTP offer will be accepted. 
        /// </summary>
        Optional,
        /// <summary>
        /// PJMEDIA_SRTP_MANDATORY 	
        /// When this flag is specified, the transport will require that RTP/SAVP media shall be used. 
        /// </summary>
        Mandatory
    }
}