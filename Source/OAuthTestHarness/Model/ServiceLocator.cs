using System;
using Naif.Core.Authentication;

namespace OAuthTestHarness.Model
{
    public static class ServiceLocator
    {
        static ServiceLocator()
        {
            GeniClient = new GeniClient()
                                {
                                    ApiKey = "STL4BDobrxDxMOS1kesXMZlVSPQ7V2Ohz5aGavVd",
                                    ApiSecret = "YreK8vWhKXWLqbpZHqoOtFhmpomNYTZg9rXajEnM",

                                    // this specifies which Google APIs your app intends to use and needs permission for
                                    Scope = "https://www.googleapis.com/auth/userinfo.email https://www.googleapis.com/auth/userinfo.profile"
                                };
        }

        public static GeniClient GeniClient { get; set; }
    }
}
