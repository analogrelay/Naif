using System.IO.IsolatedStorage;

using GalaSoft.MvvmLight.Threading;

using Naif.Core.Authentication;

namespace OAuthTestHarness.ViewModel
{
    public class ViewModelLocator
    {
        private static AuthenticationViewModel authViewModel;
        private static MainViewModel mainViewModel;

        public const string AppName = "OAuth Test Harness";

        public ViewModelLocator()
        {
            Instance = this;

            DispatcherHelper.Initialize();
            CreateAuth();
            CreateMain();
        }

        #region Public Methods

        public AuthenticationViewModel Auth
        {
            get
            {
                return AuthStatic;
            }
        }

        public MainViewModel Main
        {
            get
            {
                return MainStatic;
            }
        }

        #endregion

        #region Public Static Methods

        public static AuthenticationViewModel AuthStatic
        {
            get
            {
                if (authViewModel == null)
                {
                    CreateAuth();
                }

                return authViewModel;
            }
        }

        public static void Cleanup()
        {
            ClearMain();
            ClearAuth();
        }

        public static void ClearAuth()
        {
            if (authViewModel != null)
                authViewModel.Cleanup();

            authViewModel = null;
        }

        public static void ClearMain()
        {
            if (mainViewModel != null)
                mainViewModel.Cleanup();

            mainViewModel = null;
        }

        public static void CreateAuth()
        {
            if (authViewModel == null)
            {
                AuthResult auth;
                IsolatedStorageSettings.ApplicationSettings.TryGetValue<AuthResult>("auth", out auth);
                authViewModel = new AuthenticationViewModel(auth);
            }
        }

        public static void CreateMain()
        {
            if (mainViewModel == null)
            {
                mainViewModel = new MainViewModel(AuthStatic);
            }
        }

        public static ViewModelLocator Instance { get; private set; }

        public static MainViewModel MainStatic
        {
            get
            {
                if (mainViewModel == null)
                {
                    CreateMain();
                }

                return mainViewModel;
            }
        }

        public static void SaveSetting(string name, object value)
        {
            DispatcherHelper.UIDispatcher.BeginInvoke(() =>
                                                {
                                                    IsolatedStorageSettings.ApplicationSettings[name] = value;
                                                    IsolatedStorageSettings.ApplicationSettings.Save();
                                                });
                                            }

        #endregion
    }
}