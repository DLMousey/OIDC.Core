using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Routing;

namespace OAuthServer.Utility.Attributes
{
    public class HttpLinkAttribute : HttpMethodAttribute
    {
        private static readonly IEnumerable<string> _supportedMethods = (IEnumerable<string>) new string[1]
        {
            "LINK"
        };

        public HttpLinkAttribute()
            : base(HttpLinkAttribute._supportedMethods)
        {
        }

        public HttpLinkAttribute(string template)
            : base(HttpLinkAttribute._supportedMethods, template)
        {
            if (template == null)
                throw new ArgumentNullException(nameof (template));
        }
    }
}