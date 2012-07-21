using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naif.Core.Authentication
{
    public class GeniClient : OAuthClient
    {
        public GeniClient()
            : base()
        {
            BaseUri = "https://www.geni.com";
            RedirectUri = "http://localhost";
            TokenEndPoint = "/platform/oauth/request_token";
            AuthEndPoint = "/platform/oauth/authorize";
            ApiEndPoint = "/api";

            Initialize();
        }

    }
}
