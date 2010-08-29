using System;
using System.Configuration;
using pjsip4net.Utils;

namespace pjsip4net.Configuration
{
    internal class SipUriValidator : ConfigurationValidatorBase
    {
        public override void Validate(object value)
        {
            try
            {
                Helper.GuardError(SipUserAgent.ApiFactory.GetBasicApi().pjsua_verify_sip_url(value.ToString()));
            }
            catch (PjsipErrorException ex)
            {
                throw new ConfigurationErrorsException("Invalid sip uri", ex);
            }
        }

        public override bool CanValidate(Type type)
        {
            return type.Equals(typeof (string));
        }
    }

    internal class SipUriValidatorAttribute : ConfigurationValidatorAttribute
    {
        public override ConfigurationValidatorBase ValidatorInstance
        {
            get { return new SipUriValidator(); }
        }
    }
}