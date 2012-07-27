using System;
using System.Net;
using System.Diagnostics;

using RestSharp;

namespace Naif.Core.Authentication
{
    public abstract class OAuthClient
    {
        #region Private Members

        private RestClient client;

        #endregion

        #region Public Events

        public event EventHandler Authenticated;
        public event EventHandler AuthenticationFailed;

        #endregion

        #region Constructors

        public OAuthClient()
        {
        }

        #endregion

        #region Public Properties

        public string ApiEndPoint { get; set; }

        public string ApiKey { get; set; }

        public string ApiSecret { get; set; }

        public string ApprovalEndPoint { get; set; }

        public string AuthEndPoint { get; set; }

        public AuthResult AuthResult { get; set; }

        public Uri AuthUri
        {
            get
            {
                UriBuilder builder = new UriBuilder(BaseUri);
                builder.Path = AuthEndPoint;
                builder.Query = string.Format("response_type=code&redirect_uri={0}&scope={1}&client_id={2}", RedirectUri, Scope, ApiKey);
                if (!String.IsNullOrEmpty(ExtraAuthParams))
                {
                    builder.Query += ExtraAuthParams;
                }
                return builder.Uri;
            }
        }

        public string BaseUri { get; set; }

        public string ExtraAuthParams { get; set; }

        public bool IsAuthenticated
        {
            get { return AuthResult != null && !string.IsNullOrEmpty(AuthResult.AccessToken); }
        }

        public String RedirectUri { get; set; }

        public string Scope { get; set; }

        public string TokenEndPoint { get; set; }

        #endregion

        #region Private Methods

        private void GetAccessTokenCallback(IRestResponse<AuthResult> response)
        {
            if (response == null || response.StatusCode != HttpStatusCode.OK ||
                response.Data == null || string.IsNullOrEmpty(response.Data.AccessToken))
            {
                OnAuthenticationFailed();
            }
            else
            {
                Debug.Assert(response.Data != null);
                AuthResult = response.Data;
                OnAuthenticated();
            }
        }

        private void RefreshAccessTokenCallback(IRestResponse<AuthResult> response)
        {
            if (response == null || response.StatusCode != HttpStatusCode.OK ||
                response.Data == null || string.IsNullOrEmpty(response.Data.AccessToken))
            {
                OnAuthenticationFailed();
            }
            else
            {
                Debug.Assert(response.Data != null);
                Debug.Assert(AuthResult != null);

                AuthResult r = response.Data;
                r.RefreshToken = AuthResult.RefreshToken;
                AuthResult = r;
                OnAuthenticated();
            }
        }

        #endregion

        #region Protected Methods

        protected void Initialize()
        {
            client = new RestClient(BaseUri);
        }

        protected virtual void OnAuthenticated()
        {
            var e = Authenticated;
            if (e != null)
            {
                e(this, EventArgs.Empty);
            }
        }

        protected virtual void OnAuthenticationFailed()
        {
            var e = AuthenticationFailed;
            if (e != null)
            {
                e(this, EventArgs.Empty);
            }
        }

        #endregion

        #region Public Methods

        public void ExchangeCodeForToken(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                OnAuthenticationFailed();
            }
            else
            {
                var request = new RestRequest(TokenEndPoint, Method.POST);
                request.AddParameter("code", code);
                request.AddParameter("client_id", ApiKey);
                request.AddParameter("client_secret", ApiSecret);
                request.AddParameter("redirect_uri", RedirectUri);
                request.AddParameter("grant_type", "authorization_code");

                client.ExecuteAsync<AuthResult>(request, GetAccessTokenCallback);
            }
        }

        public void MakeApiRequest<T>(Method requestMethod, ParameterType authTokenParameterType, string apiCall, Action<IRestResponse<T>> callback) where T : class, new()
        {
            RestClient client = new RestClient(BaseUri);

            if (authTokenParameterType == ParameterType.HttpHeader)
            {
                client.Authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(AuthResult.AccessToken);
            }
            else if (authTokenParameterType == ParameterType.UrlSegment)
            {
                client.Authenticator = new OAuth2UriQueryParameterAuthenticator(AuthResult.AccessToken);
            }

            var request = new RestRequest(ApiEndPoint + "/" + apiCall, requestMethod);
            client.ExecuteAsync<T>(request, callback);
        }

        public void RefreshAccessToken()
        {
            Debug.Assert(IsAuthenticated);

            var authorize = new RestRequest(TokenEndPoint, Method.POST);
            authorize.AddParameter("refresh_token", AuthResult.RefreshToken);
            authorize.AddParameter("client_id", ApiKey);
            authorize.AddParameter("client_secret", ApiSecret);
            authorize.AddParameter("grant_type", "refresh_token");

            client.ExecuteAsync<AuthResult>(authorize, RefreshAccessTokenCallback);
        }

        public void Revoke()
        {
            AuthResult = null;
        }

        #endregion
    }
}
