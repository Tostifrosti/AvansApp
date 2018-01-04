using System;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Collections.Generic;
using Windows.Foundation.Metadata;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.Security.Authentication.Web;

using AvansApp.Models;
using AvansApp.Helpers;
using AvansApp.Services;

namespace AvansApp.ViewModels.Pages
{
    public class LoginPageViewModel : ViewModelBase
    {
        public NavigationService NavigationService
        {
            get
            {
                return Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<NavigationService>();
            }
        }
        private bool IsLoggedIn { get; set; }
        public ICommand OnLoginButtonClickCommand { get; private set; }
        public bool _isLoginButtonVisible;
        public bool IsLoginButtonVisible { get { return _isLoginButtonVisible; } private set { Set(ref _isLoginButtonVisible, value); } }
        public LoginPageViewModel()
        {
            IsLoggedIn = false;
            IsLoginButtonVisible = true;
            OnLoginButtonClickCommand = new RelayCommand<ItemClickEventArgs>(OnLoginButtonClick);
            ShowStatusBar(Color.FromArgb(255, 198, 0, 42), Colors.White); // Change color of Statusbar
        }

        public void Initialize()
        {
            IsLoginButtonVisible = true;
        }
        
        private string GetAccessToken(string webAuthResponsedata)
        {
            string responseData = webAuthResponsedata.Substring(webAuthResponsedata.IndexOf("oauth_token"));
            string request_token = null;
            string oauth_verifier = null;
            string[] keyValPairs = responseData.Split('&');

            for (int i = 0; i < keyValPairs.Length; i++)
            {
                string[] splits = keyValPairs[i].Split('=');
                switch (splits[0])
                {
                    case "oauth_token":
                        request_token = splits[1];
                        break;
                    case "oauth_verifier":
                        oauth_verifier = splits[1];
                        break;
                }
            }
            return oauth_verifier;
        }
        private async Task<bool> Request()
        {
            OAuth_Response request_response = await OAuth.GetInstance().AcquireRequestToken("https://publicapi.avans.nl/oauth/request_token", Models.Enums.HttpMethod.GET,
                    new List<string>() { "consumer_key", "consumer_secret", "timestamp", "nonce", "signature_method", "signature", "callback", "version" });
            return (request_response != null);
        }

        private async void Login()
        {
            OAuth.GetInstance().Client.EmptyVault();
            bool result = await Request();

            if (result)
            {
                // Login
                try
                {
                    string start_url = OAuth.GetInstance().Client.OAuth_Authentification_Url + "?oauth_token=" + OAuth.GetInstance().Client.OAuth_Token;
                    string end_url = "https://publicapi.avans.nl/oauth/" + OAuth.GetInstance().Client.Callback;

                    WebAuthenticationResult webAuthenticationResult = await WebAuthenticationBroker.AuthenticateAsync(
                                    WebAuthenticationOptions.None,
                                    new Uri(start_url),
                                    new Uri(end_url)); // SSO (Single Sign On)

                    switch (webAuthenticationResult.ResponseStatus)
                    {
                        case WebAuthenticationStatus.Success:
                            // Successful authentication. 
                            string token_verifier = GetAccessToken(webAuthenticationResult.ResponseData);

                            OAuth.GetInstance().Client.OAuth_Verifier = token_verifier;

                            bool acc_result = await AccessToken();

                            if (acc_result)
                                Resume();
                            else
                                throw new Exception("No Access Token.");

                            break;
                        case WebAuthenticationStatus.ErrorHttp:
                            // HTTP error. 
                            //System.Diagnostics.Debug.WriteLine("Authentication Http Error: " + webAuthenticationResult.ResponseErrorDetail.ToString());

                            // Dialog
                            var auth_http_error = new MessageDialog("ErrorMsgAuthDialog".GetLocalized(), "ErrorMsgAuthDialogHeader".GetLocalized());
                            await auth_http_error.ShowAsync();

                            IsLoginButtonVisible = true;

                            break;
                        case WebAuthenticationStatus.UserCancel:
                            // User Canceled the Authentication
                            IsLoginButtonVisible = true;
                            break;
                        default:
                            // Other error.
                            //Debug.WriteLine("Authentication Error: " + webAuthenticationResult.ResponseData.ToString());
                            //throw new Exception("Authentication Error!");
                            break;
                    }

                }
                catch (Exception)
                {
                    // Authentication failed. Handle parameter, SSL/TLS, and Network Unavailable errors here.
                    //Debug.WriteLine("Webauthentication Error: " + Error.Message);

                    // Dialog
                    var auth_error = new MessageDialog("ErrorMsgDialog".GetLocalized(), "ErrorMsgDialogHeader".GetLocalized());
                    await auth_error.ShowAsync();

                    IsLoginButtonVisible = true;
                }
            }
            else
            {
                // Dialog
                var auth_error = new MessageDialog("ErrorMsgDialog".GetLocalized(), "ErrorMsgDialogHeader".GetLocalized());
                await auth_error.ShowAsync();

                IsLoginButtonVisible = true;
            }
        }

        private async Task<bool> AccessToken()
        {
            OAuth_Response access_token_response = await OAuth.GetInstance().AcquireRequestAccessToken("https://publicapi.avans.nl/oauth/access_token", Models.Enums.HttpMethod.GET,
                new List<string>() { "consumer_key", "consumer_secret", "timestamp", "nonce", "signature_method", "signature", "callback", "version", "token", "verifier" }); // Acquire the access token (when user already have signed in.)

            return (access_token_response != null);
        }

        private void Resume()
        {
            IsLoginButtonVisible = false;

            NavigationService.NavigateToPage(typeof(SchedulePageViewModel).FullName, new Views.MainPage());
        }
        private void OnLoginButtonClick(ItemClickEventArgs e)
        {
            IsLoginButtonVisible = false;

            // Login
            Login();
        }
        private async void ShowStatusBar(Color background, Color foreground)
        {
            // Turn on SystemTray for mobile
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                // Added Reference: Windows Desktop Extensions for the UWP
                // Added Reference: Windows Mobile Extensions for the UWP
                var statusbar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
                await statusbar.ShowAsync();
                statusbar.BackgroundOpacity = 1;
                statusbar.BackgroundColor = background;
                statusbar.ForegroundColor = foreground;
            }
        }


    }
}
