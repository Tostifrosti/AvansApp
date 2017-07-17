using System;
using System.Text;
using Windows.Security.Credentials;

namespace AvansApp.Models
{
    public class OAuth_Client
    {
        public string Consumer_Id { get; set; }
        public string Consumer_Token { get; set; }
        public string Consumer_Token_Secret { get; set; }

        public string OAuth_Token { get; set; }
        public string OAuth_Token_Secret { get; set; }
        public string OAuth_Verifier { get; set; }


        private string _oauth_access_token;
        public string OAuth_Access_Token { get { return _oauth_access_token; } set { _oauth_access_token = value; if (value != null) { AddToVault("oauth_access_token", "access_token", value); } } }
        private string _oauth_access_token_secret;
        public string OAuth_Access_Token_Secret { get { return _oauth_access_token_secret; } set { _oauth_access_token_secret = value; if (value != null) { AddToVault("oauth_access_token_secret", "access_token_secret", value); } } }
        public string OAuth_Authentification_Url { get; set; }


        public string Callback { get; set; }
        public string Callback_Confirmed { get; set; }
        public string Timestamp { get; set; }
        public string Nonce { get; set; }
        public string Signature_Method { get; set; }
        public string Signature { get; set; }
        public string Version { get; set; }

        private PasswordVault _vault;
        private Random _random;
        private static readonly DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);

        public OAuth_Client()
        {
            _vault = new PasswordVault();
            _random = new Random();
            this.Initialize();
        }

        public void Initialize()
        {
            Consumer_Id = "236";
            Consumer_Token = "<< your token here >>";
            Consumer_Token_Secret = "<< your token secret here >>";

            _oauth_access_token = (CheckTokenExists("oauth_access_token")) ? GetTokenFromVault("oauth_access_token") : "";
            _oauth_access_token_secret = (CheckTokenExists("oauth_access_token_secret")) ? GetTokenFromVault("oauth_access_token_secret") : "";

            // Login
            OAuth_Token = "";
            OAuth_Token_Secret = "";
            OAuth_Verifier = "";
            OAuth_Authentification_Url = "";

            Callback = "oob"; // presume "desktop" consumer
            Callback_Confirmed = "";
            Timestamp = GenerateTimeStamp();
            Nonce = GenerateNonce();
            Signature_Method = "HMAC-SHA1";
            Version = "1.0";
            
        }

        public bool IsLoggedIn()
        {
            return (CheckTokenExists("oauth_access_token") && CheckTokenExists("oauth_access_token_secret"));
        }

        public void NewRequest()
        {
            Timestamp = GenerateTimeStamp();
            Nonce = GenerateNonce();
        }
        private string GenerateTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - _epoch;
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }
        private string GenerateNonce()
        {
            var sb = new StringBuilder();
            for (int i = 0; i < 8; i++)
            {
                int g = _random.Next(3);
                switch (g)
                {
                    case 0:
                        // lowercase alpha
                        sb.Append((char)(_random.Next(26) + 97), 1);
                        break;
                    default:
                        // numeric digits
                        sb.Append((char)(_random.Next(10) + 48), 1);
                        break;
                }
            }
            return sb.ToString();
        }
        

        public string GetTokenFromVault(string name)
        {
            string result = null;
            try
            {
                var list = _vault.RetrieveAll();
                foreach (var item in list)
                {
                    if (item.Resource == name)
                    {
                        item.RetrievePassword();
                        if (!string.IsNullOrEmpty(item.Password))
                        {
                            result = item.Password;
                            break;
                        }
                    }
                }
            }
            catch (Exception) { result = null; }
            return result;
        }

        public bool CheckTokenExists(string name)
        {
            bool result = false;
            try
            {
                var list = _vault.RetrieveAll();
                foreach (var item in list)
                {
                    if (item.Resource == name)
                    {
                        item.RetrievePassword();
                        if (!string.IsNullOrEmpty(item.Password))
                        {
                            result = true;
                            break;
                        }
                        else
                        {
                            // Who put a empty key in the vault?!
                            _vault.Remove(item); // Remove the key
                        }
                    }
                }
            }
            catch (Exception) { result = false; }

            return result;
        }

        public void EmptyVault()
        {
            try
            {
                var list = _vault.RetrieveAll();

                foreach (var l in list)
                {
                    _vault.Remove(l);
                }
            }
            catch (Exception) { }

            Initialize();
        }
        public void AddToVault(string name, string username, string password)
        {
            try
            {
                var credential = new PasswordCredential(name, username, password);
                _vault.Add(credential);
            }
            catch (Exception) { }
        }
        public void RemoveFromVault(string name)
        {
            try
            {
                var list = _vault.RetrieveAll();
                foreach (var item in list)
                {
                    if (item.Resource == name)
                    {
                        _vault.Remove(item);
                        break;
                    }
                }
            }
            catch (Exception) { }
        }
    }
}
