using System;

using RestSharp;

namespace OAuthTestHarness.ViewModel
{
    public interface IAuthProvider
    {
        void GetAccessCode(Action<string> callback);
        void Logout();

        DateTime TokenExpiresAt { get; }
    }
}
