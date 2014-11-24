using System;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Web.Http.SelfHost;
using System.Web.Http.SelfHost.Channels;

namespace ConDep.Node
{
    public class NtlmSelfHostConfiguration : HttpSelfHostConfiguration
    {
        public NtlmSelfHostConfiguration(string baseAddress)
            : base(baseAddress)
        { }

        public NtlmSelfHostConfiguration(Uri baseAddress)
            : base(baseAddress)
        { }

        protected override BindingParameterCollection OnConfigureBinding(HttpBinding httpBinding)
        {
            httpBinding.Security.Mode = HttpBindingSecurityMode.Transport;
            //httpBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Ntlm;
            httpBinding.ConfigureTransportBindingElement =
                element => element.AuthenticationScheme = AuthenticationSchemes.Negotiate;
            return base.OnConfigureBinding(httpBinding);
        }
    }
}