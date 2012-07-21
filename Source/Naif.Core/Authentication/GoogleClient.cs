using System;
using System.Linq;

namespace Naif.Core.Authentication
{
    public class GoogleClient : OAuthClient
    {
        public GoogleClient() : base()
        {
            BaseUri = "https://accounts.google.com";
            RedirectUri = "http://localhost";
            ApprovalEndPoint = "/o/oauth2/approval";
            TokenEndPoint = "/o/oauth2/token";
            AuthEndPoint = "/o/oauth2/auth";

            Initialize();
        }
    }
}
