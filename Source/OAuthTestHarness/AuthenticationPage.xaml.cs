using System.Collections.Generic;
using System.Windows;
using System.Windows.Navigation;

using Microsoft.Phone.Controls;

namespace OAuthTestHarness
{
    /// <summary>
    /// Description for AuthenticationPage.
    /// </summary>
    public partial class AuthenticationPage : PhoneApplicationPage
    {
        /// <summary>
        /// Initializes a new instance of the AuthenticationPage class.
        /// </summary>
        public AuthenticationPage()
        {
            InitializeComponent();
        }

        private void webBrowser1_Navigating(object sender, NavigatingEventArgs e)
        {
            if (e.Uri.Host.Equals("localhost"))
            {
                webBrowser1.Visibility = Visibility.Collapsed;
                e.Cancel = true;
                var parameters = ParseQueryString(e.Uri.Query);

                // setting this text will bind it back to the view model
                codeBlock.Text = parameters.ContainsKey("code") ? parameters["code"] : null;
            }
        }

        private void webBrowser1_Navigated(object sender, NavigationEventArgs e)
        {
            webBrowser1.Visibility = Visibility.Visible;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (IsAuthenticatedCheckbox.IsChecked.Value && NavigationService.CanGoBack)
                NavigationService.GoBack();            
        }

        private Dictionary<string, string> ParseQueryString(string queryString)
        {
            Dictionary<string, string> queryParameters = new Dictionary<string, string>();
            string[] querySegments = queryString.Split('&');
            foreach (string segment in querySegments)
            {
                string[] parts = segment.Split('=');
                if (parts.Length > 0)
                {
                    string key = parts[0].Trim(new char[] { '?', ' ' });
                    string val = parts[1].Trim();

                    queryParameters.Add(key, val);
                }
            }
            return queryParameters;
        }
    }
}