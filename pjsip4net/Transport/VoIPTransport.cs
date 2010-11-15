using System;
using pjsip.Interop;
using pjsip4net.Utils;

namespace pjsip4net.Transport
{
    public abstract class VoIPTransport : Initializable, IIdentifiable<VoIPTransport>
    {
        #region Protected Data

        internal pjsua_transport_config _config = new pjsua_transport_config();
        internal pjsua_transport_info _info;
        internal pjsip_transport_type_e _transportType;

        #endregion

        #region Properties

        public TransportType TransportType
        {
            get
            {
                switch (_transportType)
                {
                    case pjsip_transport_type_e.PJSIP_TRANSPORT_UDP:
                        return TransportType.Udp;
                    case pjsip_transport_type_e.PJSIP_TRANSPORT_TCP:
                        return TransportType.Tcp;
                    case pjsip_transport_type_e.PJSIP_TRANSPORT_TLS:
                        return TransportType.Tls;
                    default:
                        return 0;
                }
            }
        }

        public uint Port
        {
            get
            {
                GuardDisposed();
                return _config.port;
            }
            set
            {
                GuardDisposed();
                GuardNotInitializing();
                _config.port = value;
            }
        }

        public string PublicAddress
        {
            get
            {
                GuardDisposed();
                return _config.public_addr;
            }
            set
            {
                GuardDisposed();
                GuardNotInitializing();
                _config.public_addr = new pj_str_t(value);
            }
        }

        public string BoundAddress
        {
            get
            {
                GuardDisposed();
                return _config.bound_addr;
            }
            set
            {
                GuardDisposed();
                GuardNotInitializing();
                _config.bound_addr = new pj_str_t(value);
            }
        }

        public string TransportName
        {
            get
            {
                GuardDisposed();
                if (!Equals(_info, default(pjsua_transport_info)))
                    return _info.type_name;
                return "";
            }
        }

        public string TransportDescription
        {
            get
            {
                GuardDisposed();
                if (!Equals(_info, default(pjsua_transport_info)))
                    return _info.info;
                return "";
            }
        }

        public bool? IsReliable
        {
            get
            {
                GuardDisposed();
                if (!Equals(_info, default(pjsua_transport_info)))
                    return ((pjsip_transport_flags_e) _info.flag & pjsip_transport_flags_e.PJSIP_TRANSPORT_RELIABLE) !=
                           0;
                return null;
            }
        }

        public bool? IsSecure
        {
            get
            {
                GuardDisposed();
                if (!Equals(_info, default(pjsua_transport_info)))
                    return ((pjsip_transport_flags_e) _info.flag & pjsip_transport_flags_e.PJSIP_TRANSPORT_SECURE) != 0;
                return null;
            }
        }

        public int Id { get; internal set; }

        //public string LocalName
        //{
        //    get
        //    {
        //        GuardDisposed();
        //        if (!Equals(_info, default(pjsua_transport_info)))
        //            return _info.local_name.host + ":" + _info.local_name.port;
        //        return "";
        //    }
        //}

        #endregion

        #region Fabric Methods

        public static UdpTransport CreateUDPTransport()
        {
            return new UdpTransport();
        }

        public static TcpTransport CreateTCPTransport()
        {
            return new TcpTransport();
        }

        public static TlsTransport CreateTLSTransport()
        {
            return new TlsTransport();
        }

        #endregion

        #region Interfaces implementations

        protected override void CleanUp()
        {
            if (Id != NativeConstants.PJSUA_INVALID_ID)
                Helper.GuardError(SipUserAgent.ApiFactory.GetTransportApi().pjsua_transport_close(Id,
                                                                                                  Convert.ToInt32(false)));
        }

        public override void BeginInit()
        {
            base.BeginInit();
            SipUserAgent.ApiFactory.GetTransportApi().pjsua_transport_config_default(_config);
        }

        public override void EndInit()
        {
            base.EndInit();
            Helper.GuardInRange(1u, 65535u, Port);
        }

        #endregion

        #region Methods

        protected VoIPTransport()
        {
            Id = NativeConstants.PJSUA_INVALID_ID;
        }

        #endregion

        #region Implementation of IEquatable<IIdentifiable<VoIPTransport>>

        public bool Equals(IIdentifiable<VoIPTransport> other)
        {
            return EqualsTemplate.Equals(this, other);
        }

        public virtual bool DataEquals(VoIPTransport other)
        {
            return Port.Equals(other.Port);
        }

        #endregion
    }

    public class UdpTransport : VoIPTransport
    {
        internal UdpTransport()
        {
            _transportType = pjsip_transport_type_e.PJSIP_TRANSPORT_UDP;
            _config.port = 5060;
        }
    }

    public class TcpTransport : VoIPTransport
    {
        internal TcpTransport()
        {
            _transportType = pjsip_transport_type_e.PJSIP_TRANSPORT_TCP;
            _config.port = 5060;
        }
    }

    public class TlsTransport : VoIPTransport
    {
        internal TlsTransport()
        {
            _transportType = pjsip_transport_type_e.PJSIP_TRANSPORT_TLS;
            _config.port = 5061;
        }

        public String CAListFile
        {
            get { return _config.tls_setting.ca_list_file; }
            set { _config.tls_setting.ca_list_file = new pj_str_t(value); }
        }

        public String CertificateFile
        {
            get { return _config.tls_setting.cert_file; }
            set { _config.tls_setting.cert_file = new pj_str_t(value); }
        }

        public String PrivateKeyFile
        {
            get { return _config.tls_setting.privkey_file; }
            set { _config.tls_setting.privkey_file = new pj_str_t(value); }
        }

        public bool VerifyServer
        {
            get { return Convert.ToBoolean(_config.tls_setting.verify_server); }
            set { _config.tls_setting.verify_server = Convert.ToInt32(value); }
        }

        public bool VerifyClient
        {
            get { return Convert.ToBoolean(_config.tls_setting.verify_client); }
            set { _config.tls_setting.verify_client = Convert.ToInt32(value); }
        }

        public bool RequireClientCertificate
        {
            get { return Convert.ToBoolean(_config.tls_setting.require_client_cert); }
            set { _config.tls_setting.require_client_cert = Convert.ToInt32(value); }
        }
    }
}