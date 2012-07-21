using System;
using System.Collections.Generic;
using System.Windows;

using Microsoft.Phone.Controls;

using GalaSoft.MvvmLight;

using Naif.Core.Authentication;
using OAuthTestHarness.Model;

namespace OAuthTestHarness.ViewModel
{

    public class AuthenticationViewModel : ViewModelBase, IAuthProvider
    {
        #region Private Members

        private Uri authUri = new Uri("about:blank");
        private string code;
        private bool isAuthenticating;
        private readonly Queue<Action<string>> queuedRequests = new Queue<Action<string>>();
        private readonly object sync = new object();

        #endregion

        #region Constructors

        public AuthenticationViewModel(AuthResult auth)
        {
            GeniClient = ServiceLocator.GeniClient;

            if (!IsInDesignMode)
            {
                GeniClient.Authenticated += new EventHandler(ClientAuthenticated);
                GeniClient.AuthenticationFailed += new EventHandler(ClientAuthenticationFailed);

                GeniClient.AuthResult = auth;
            }
        }

        #endregion

        public string ApplicationTitle
        {
            get { return ViewModelLocator.AppName; }
        }

        public string ApprovalHost
        {
            get
            {
                return "localhost";
            }
        }

        public Uri AuthUri
        {
            get
            {
                return authUri;
            }
            set
            {
                if (authUri != value)
                {
                    authUri = value;
                    RaisePropertyChanged("AuthUri");
                }
            }
        }

        public string Code
        {
            get
            {
                return code;
            }
            set
            {
                code = value;
                GeniClient.ExchangeCodeForToken(Code);
            }
        }

        public GeniClient GeniClient { get; set; }

        public bool IsAuthenticated
        {
            get
            {
                lock (sync)
                {
                    return GeniClient.IsAuthenticated;
                }
            }
        }

        public string PageName
        {
            get
            {
                return "login";
            }
        }

        public DateTime TokenExpiresAt
        {
            get
            {
                lock (sync)
                {
                    if (IsAuthenticated)
                    {
                        return GeniClient.AuthResult.ExpiresAt;
                    }
                }
                return DateTime.MinValue;
            }
        }

        public void GetAccessCode(Action<string> callback)
        {
            lock (sync)
            {
                if (isAuthenticating)
                {
                    queuedRequests.Enqueue(callback);
                }
                else if (IsAuthenticated)
                {
                    if (!GeniClient.AuthResult.IsExpired)
                    {
                        callback(GeniClient.AuthResult.AccessToken);
                    }
                    else
                    {
                        isAuthenticating = true;
                        queuedRequests.Enqueue(callback);
                        GeniClient.RefreshAccessToken();
                    }
                }
                else
                {
                    isAuthenticating = true;
                    queuedRequests.Enqueue(callback);

                    ((PhoneApplicationFrame)App.Current.RootVisual).Navigate(new Uri("/AuthenticationPage.xaml", UriKind.Relative));
                    AuthUri = GeniClient.AuthUri;
                }
            }
        }

        public void Logout()
        {
            lock (sync)
            {
                GeniClient.Revoke();
            }
            RaisePropertyChanged("IsAuthenticated");
        }

        private void ClientAuthenticated(object sender, EventArgs e)
        {
            lock (sync)
            {
                isAuthenticating = false;

                while (queuedRequests.Count > 0)
                    queuedRequests.Dequeue()(GeniClient.AuthResult.AccessToken);

                ViewModelLocator.SaveSetting("auth", GeniClient.AuthResult);
            }

            RaisePropertyChanged("IsAuthenticated");
        }

        private void ClientAuthenticationFailed(object sender, EventArgs e)
        {
            lock (sync)
            {
                isAuthenticating = false;
                GeniClient.Revoke();
                RaisePropertyChanged("IsAuthenticated");

                AuthUri = new Uri("about:blank");
                AuthUri = GeniClient.AuthUri;
                MessageBox.Show("Please try again", "Login failed", MessageBoxButton.OK);
            }
        }
    }
}