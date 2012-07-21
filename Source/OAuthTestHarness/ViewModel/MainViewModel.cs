using System;
using System.Diagnostics;
using System.Windows.Input;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using Naif.Core.Authentication;
using RestSharp;
using OAuthTestHarness.Model;

namespace OAuthTestHarness.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm/getstarted
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {

        private readonly IAuthProvider authProvider;
        private bool loading;
        private Profile profile;

        public MainViewModel(IAuthProvider authProvider)
        {
            Debug.Assert(authProvider != null);
            this.authProvider = authProvider;

            GeniClient = ServiceLocator.GeniClient;

            if (IsInDesignMode)
            {
                profile = new Profile();
            }

            RefreshCommand = new RelayCommand(Refresh);
            LogoutCommand = new RelayCommand(Logout);
        }

        public ICommand RefreshCommand { get; private set; }
        public ICommand LogoutCommand { get; private set; }

        public string ApplicationTitle
        {
            get
            {
                return ViewModelLocator.AppName;
            }
        }

        public GeniClient GeniClient { get; set; }

        public string PageName
        {
            get
            {
                return "profile";
            }
        }

        public Profile Profile
        {
            get
            {
                if(profile == null)
                    authProvider.GetAccessCode(s => LoadProfile());

                return profile;
            }
            set
            {
                if (profile != value)
                {
                    profile = value;
                    RaisePropertyChanged("Profile");
                }
            }
        }

        private void Logout()
        {
            authProvider.Logout();      
            Profile = null;

        }

        private void Refresh()
        {
            Profile = null;
        }

        private void LoadProfile()
        {
            if (profile == null && !loading)
            {
                Debug.WriteLine("loading profile");
                loading = true;

                GeniClient.MakeApiRequest<Profile>(Method.GET, ParameterType.UrlSegment, "profile", ProfileLoaded);
            }
        }

        private void ProfileLoaded(IRestResponse<Profile> response)
        {
            loading = false;   
            Profile = response.Data;
        }
    }
}