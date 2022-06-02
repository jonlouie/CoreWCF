// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Security.Principal;
using CoreWCF.Channels;
using CoreWCF.IdentityModel.Tokens;

namespace CoreWCF.Security
{
    internal static class SecurityUtils
    {
        public static void ValidateAnonymityConstraint(WindowsIdentity identity, bool allowUnauthenticatedCallers)
        {
            if (!allowUnauthenticatedCallers && identity.User.IsWellKnown(WellKnownSidType.AnonymousSid))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(
                    new SecurityTokenValidationException(SR.AnonymousLogonsAreNotAllowed));
            }
        }

        public static bool ShouldUseAuthentication(Binding binding)
        {
            if (binding is WSHttpBinding wsHttpBinding
                && wsHttpBinding.Security.Mode != SecurityMode.None
                && wsHttpBinding.Security.Transport.ClientCredentialType != HttpClientCredentialType.None)
            {
                return true;
            }
            return false;
        }

        //public static bool IsAuthenticationRequired(IHttpTransportFactorySettings httpTransportFactorySettings)
        //{
        //    if (httpTransportFactorySettings is HttpTransportSettings httpTransportSettings)
        //    {
        //        // Is impersonation required?
        //        if ((httpTransportSettings.SecurityMode == SecurityMode.Transport || httpTransportSettings.SecurityMode == SecurityMode.TransportWithMessageCredential)
        //            && httpTransportSettings.ClientCredentialType == HttpClientCredentialType.Windows)
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}
    }
}
