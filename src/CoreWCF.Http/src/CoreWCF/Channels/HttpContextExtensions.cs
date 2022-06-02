// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace CoreWCF.Channels
{
    internal static class HttpContextExtensions
    {
        private const string ContentTypeHeaderName = "Content-Type";
        private const string AuthenticateResultName = "HttpContext.AuthenticateResult";

        internal static WebHeaderCollection ToWebHeaderCollection(this HttpRequest httpRequest)
        {
            var webHeaders = new WebHeaderCollection();
            foreach (System.Collections.Generic.KeyValuePair<string, Microsoft.Extensions.Primitives.StringValues> header in httpRequest.Headers)
            {
                if (header.Key.StartsWith(":")) // HTTP/2 pseudo header, skip as they appear on other properties of HttpRequest
                    continue;

                webHeaders[header.Key] = header.Value;
            }

            webHeaders[ContentTypeHeaderName] = httpRequest.ContentType;

            return webHeaders;
        }

        internal static void SetAuthenticateResult(this HttpContext httpContext, AuthenticateResult authenticateResult)
        {
            if (authenticateResult == null)
            {
                throw new ArgumentException($"Parameter {nameof(authenticateResult)} for method {nameof(SetAuthenticateResult)} cannot be null.");
            }
            httpContext.Items[AuthenticateResultName] = authenticateResult;
        }

        internal static AuthenticateResult GetAuthenticateResult(this HttpContext httpContext)
        {
            if (httpContext.Items.TryGetValue(AuthenticateResultName, out var authenticateResult))
            {
                return authenticateResult as AuthenticateResult;
            }

            return null;
        }
    }
}
