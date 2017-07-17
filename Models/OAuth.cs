using AvansApp.Models.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.Storage.Streams;
using Windows.Web.Http;

namespace AvansApp.Models
{
    public class OAuth
    {
        private List<string> _includes_params { get; set; }
        private static string unreservedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";
        
        public OAuth_Client Client { get; set; }
        private static OAuth Instance { get; set; }
      
        public static OAuth GetInstance() {
            if (Instance == null)
                Instance = new OAuth();
            return Instance;
        }
        
        private OAuth()
        {
            Client = new OAuth_Client();
        }
        
       
        public async Task<OAuth_Response> AcquireRequestToken(string uri, Enums.HttpMethod method = Enums.HttpMethod.GET, List<string> includes = null)
        {
            Client.NewRequest(); // Always refresh with every request.

            _includes_params = (includes != null) ? includes : new List<string>();
            
            var authzHeader = GetAuthorizationHeader(uri, GetHttpMethod(method));
            
            // prepare the token request
            var request = new HttpClient();
            request.DefaultRequestHeaders.Add("Authorization", authzHeader);
            
            HttpResponseMessage httpResponse = new HttpResponseMessage();
            
            OAuth_Response r = null;

            try
            {
                //Send the GET request
                httpResponse = await request.GetAsync(new Uri(uri));
                httpResponse.EnsureSuccessStatusCode();
                string httpResponseBody = await httpResponse.Content.ReadAsStringAsync();

                using (var reader = new System.IO.StringReader(httpResponseBody))
                {
                    r = new OAuth_Response(reader.ReadToEnd());
                    
                    // Sometimes the request_token URL gives us an access token,
                    // with no user interaction required. Eg, when prior approval
                    // has already been granted.
                    try
                    {
                        if (r["oauth_token"] != null)
                            Client.OAuth_Token = r["oauth_token"];
                        if (r["oauth_token_secret"] != null)
                            Client.OAuth_Token_Secret = r["oauth_token_secret"];
                        if (r["authentification_url"] != null)
                            Client.OAuth_Authentification_Url = r["authentification_url"];
                        if (r["oauth_callback_confirmed"] != null)
                            Client.Callback_Confirmed = r["oauth_callback_confirmed"];
                        if (r["oauth_verifier"] != null)
                            Client.OAuth_Verifier = r["oauth_verifier"];
                    }
                    catch(Exception e) { Debug.WriteLine(e.Message); r = null; }
                    request.Dispose();
                }
            }
            catch (Exception ex)
            {
                request.Dispose();
                Debug.WriteLine(ex.Message);
                r = null;
            }

            return r;
        }
        public async Task<OAuth_Response> AcquireRequestAccessToken(string uri, Enums.HttpMethod method = Enums.HttpMethod.GET, List<string> includes = null)
        {
            Client.NewRequest(); // Always refresh with every request.

            _includes_params = (includes != null) ? includes : new List<string>();

            var authzHeader = GetAuthorizationHeader(uri, GetHttpMethod(method));

            // prepare the token request
            var request = new HttpClient();
            request.DefaultRequestHeaders.Add("Authorization", authzHeader);

            HttpResponseMessage httpResponse = new HttpResponseMessage();

            OAuth_Response r = null;

            try
            {
                //Send the GET request
                httpResponse = await request.GetAsync(new Uri(uri));
                httpResponse.EnsureSuccessStatusCode();
                string httpResponseBody = await httpResponse.Content.ReadAsStringAsync();

                using (var reader = new System.IO.StringReader(httpResponseBody))
                {
                    r = new OAuth_Response(reader.ReadToEnd());

                    // Sometimes the request_token URL gives us an access token,
                    // with no user interaction required. Eg, when prior approval
                    // has already been granted.
                    try
                    {
                        if (r["oauth_token"] != null)
                            Client.OAuth_Access_Token = r["oauth_token"];
                        if (r["oauth_token_secret"] != null)
                            Client.OAuth_Access_Token_Secret = r["oauth_token_secret"];
                        if (r["oauth_callback_confirmed"] != null)
                            Client.Callback_Confirmed = r["oauth_callback_confirmed"];
                        if (r["oauth_verifier"] != null)
                            Client.OAuth_Verifier = r["oauth_verifier"];
                    }
                    catch (Exception e) { Debug.WriteLine(e.Message); r = null; }
                    request.Dispose();
                }
            }
            catch (Exception ex)
            {
                request.Dispose();
                Debug.WriteLine(ex.Message);
                r = null;
            }

            return r;
        }

        public async Task<T> RequestJSON<T>(string base_url, List<string> parameters, Enums.HttpMethod method = Enums.HttpMethod.GET)
        {
            Client.NewRequest();

            _includes_params = new List<string> { "consumer_key", "signature", "signature_method", "nonce", "timestamp", "token", "version" };

            string uri = base_url;
            uri += "?format=json";

            foreach (string item in parameters)
                uri += "&" + item;
            
            var authHeader = GetAuthorizationHeader(uri, GetHttpMethod(method));
            var request = new HttpClient();

            request.DefaultRequestHeaders.Add("Authorization", authHeader);
            HttpResponseMessage httpResponse = new HttpResponseMessage();

            T result = default(T);

            try
            {
                //Send the request
                httpResponse = await request.GetAsync(new Uri(uri));
                httpResponse.EnsureSuccessStatusCode();

                Task<IBuffer> asyncBuffer = httpResponse.Content.ReadAsBufferAsync().AsTask();
                asyncBuffer.Wait();
                byte[] resultByteArray = asyncBuffer.Result.ToArray();
                string httpResponseBody = Encoding.UTF8.GetString(resultByteArray, 0, resultByteArray.Length);

                // Deserialize json to Object
                if (httpResponseBody != string.Empty)
                {
                    T r = JsonConvert.DeserializeObject<T>(httpResponseBody);
                    result = r;
                }
                request.Dispose();
            }
            catch (Exception ex)
            {
                request.Dispose();
                result = default(T);
                Debug.WriteLine(ex.Message);
            }

            return result;
        }
        public async Task<XmlDocument> RequestXML(string base_url, List<string> parameters, Enums.HttpMethod method = Enums.HttpMethod.GET)
        {
            Client.NewRequest();

            _includes_params = new List<string> { "consumer_key", "signature", "signature_method", "nonce", "timestamp", "token", "version" };

            string uri = base_url;
            uri += "?format=xml";

            foreach (string item in parameters)
                uri += "&" + item;

            var authHeader = GetAuthorizationHeader(uri, GetHttpMethod(method));
            var request = new HttpClient();

            request.DefaultRequestHeaders.Add("Authorization", authHeader);
            HttpResponseMessage httpResponse = new HttpResponseMessage();

            XmlDocument result = null;

            try
            {
                //Send the request
                httpResponse = await request.GetAsync(new Uri(uri));
                httpResponse.EnsureSuccessStatusCode();

                Task<IBuffer> asyncBuffer = httpResponse.Content.ReadAsBufferAsync().AsTask();
                asyncBuffer.Wait();
                byte[] resultByteArray = asyncBuffer.Result.ToArray();
                string httpResponseBody = Encoding.UTF8.GetString(resultByteArray, 0, resultByteArray.Length);

                // Load XML
                if (httpResponseBody != string.Empty)
                {
                    result = new XmlDocument();
                    result.LoadXml(httpResponseBody);
                }
                request.Dispose();
            }
            catch (Exception ex)
            {
                request.Dispose();
                result = null;
                Debug.WriteLine(ex.Message);
            }

            return result;
        }
        public async Task<string> RequestRaw(string base_url, List<string> parameters, Enums.HttpMethod method = Enums.HttpMethod.GET)
        {
            Client.NewRequest(); // Always refresh with every request.

            _includes_params = new List<string>() { "consumer_key", "signature", "signature_method", "nonce", "timestamp", "token", "version" }; //"consumer_secret", "callback",

            string uri = base_url;
            uri += "?format=json";

            foreach (string item in parameters)
                uri += "&" + item;

            var authzHeader = GetAuthorizationHeader(uri, GetHttpMethod(method));

            // prepare the token request
            var request = new HttpClient();
            request.DefaultRequestHeaders.Add("Authorization", authzHeader);

            HttpResponseMessage httpResponse = new HttpResponseMessage();

            string result = string.Empty;

            try
            {
                //Send the request
                httpResponse = await request.GetAsync(new Uri(uri));
                httpResponse.EnsureSuccessStatusCode();
                //string httpResponseBody = await httpResponse.Content.ReadAsStringAsync();
                Task<IBuffer> asyncBuffer = httpResponse.Content.ReadAsBufferAsync().AsTask();
                asyncBuffer.Wait();
                byte[] resultByteArray = asyncBuffer.Result.ToArray();
                string httpResponseBody = Encoding.UTF8.GetString(resultByteArray, 0, resultByteArray.Length);

                result = httpResponseBody;
                request.Dispose();
            }
            catch (Exception ex)
            {
                request.Dispose();
                result = string.Empty;
                Debug.WriteLine(ex.Message);
            }

            return result;
        }

        private void Sign(string uri, string method)
        {
            var signatureBase = GetSignatureBase(uri, method);
            var hash = GetHash();

            byte[] dataBuffer = Encoding.ASCII.GetBytes(signatureBase);
            byte[] hashBytes = hash.ComputeHash(dataBuffer);
            var sig = Convert.ToBase64String(hashBytes);

            Client.Signature = sig;
        }
        private string GetSignatureBase(string url, string method)
        {
            // normalize the URI
            Uri uri = new Uri(url);
            string normUrl = string.Format("{0}://{1}", uri.Scheme, uri.Host);
            if (!((uri.Scheme == "http" && uri.Port == 80) ||
                  (uri.Scheme == "https" && uri.Port == 443)) )
                normUrl += ":" + uri.Port;

            normUrl += uri.AbsolutePath;

            // the sigbase starts with the method and the encoded URI
            var sb = new StringBuilder();
            sb.Append(method)
                .Append('&')
                .Append(UrlEncode(normUrl))
                .Append('&');

            

            // The parameters follow. This must include all oauth params
            // plus any query params on the uri.  Also, each uri may
            // have a distinct set of query params.

            // first, get the query params
            Dictionary<string, string> query_params = ExtractQueryParameters(uri.Query);

            // add to that list all non-empty oauth params
            Dictionary<string, string> par = this.GetHeaders(_includes_params);
      
            foreach (var pr in par)
            {
                // Exclude all oauth params that are secret or
                // signatures; any secrets must not be shared,
                // and any existing signature will be invalid.
                if (!String.IsNullOrEmpty(pr.Key) &&
                    !pr.Key.EndsWith("_secret") && 
                    !pr.Key.EndsWith("signature") &&
                    _includes_params.Contains(pr.Key))
                {
                    query_params.Add("oauth_" + pr.Key, (pr.Key == "callback") ? UrlEncode(pr.Value) : pr.Value);
                }
            }

            // concat+format the sorted list of all those params
            var sb1 = new StringBuilder();
            foreach (KeyValuePair<string, string> item in query_params.OrderBy(x => x.Key))
            {
                // even "empty" params need to be encoded this way.
                sb1.AppendFormat("{0}={1}&", item.Key, item.Value);
            }

            // append the UrlEncoded version of that string to the sigbase
            sb.Append(UrlEncode(sb1.ToString().TrimEnd('&')));
            var result = sb.ToString();

            return result;
        }
        private string GetAuthorizationHeader(string uri, string method)
        {
            return GetAuthorizationHeader(uri, method, null);
        }
        private string GetAuthorizationHeader(string uri, string method, string realm)
        {
            if(string.IsNullOrEmpty(Client.Consumer_Token))
                throw new ArgumentNullException("consumer_key");

            if (string.IsNullOrEmpty(Client.Signature_Method))
                throw new ArgumentNullException("signature_method");

            Sign(uri, method);

            var new_params = this.GetHeaders(_includes_params);
            var erp = EncodeRequestParameters(new_params);
             

            //var erp = EncodeRequestParameters(_params);
            return (string.IsNullOrEmpty(realm)) ? "OAuth " + erp : string.Format("OAuth realm=\"{0}\", ", realm) + erp;
        }

        private Dictionary<string, string> GetHeaders(List<string> include)
        {
            var p = new Dictionary<string, string>();
            // Exclude all oauth params that are secret or
            // signatures; any secrets must not be shared,
            // and any existing signature will be invalid.

            if (include.Contains("consumer_id") && !String.IsNullOrEmpty(Client.Consumer_Id))
                p.Add("consumer_id", Client.Consumer_Id);
            if (include.Contains("consumer_key") && !String.IsNullOrEmpty(Client.Consumer_Token))
                p.Add("consumer_key", Client.Consumer_Token);
            if (_includes_params.Contains("consumer_secret") && !String.IsNullOrEmpty(Client.Consumer_Token_Secret))
                p.Add("consumer_secret", Client.Consumer_Token_Secret);
            if(String.IsNullOrEmpty(Client.OAuth_Access_Token))
            {
                if (include.Contains("token") && !String.IsNullOrEmpty(Client.OAuth_Token))
                    p.Add("token", Client.OAuth_Token);
                if (include.Contains("token_secret") && !String.IsNullOrEmpty(Client.OAuth_Token_Secret))
                    p.Add("token_secret", Client.OAuth_Token_Secret);
            } else {
                if (include.Contains("token") && !String.IsNullOrEmpty(Client.OAuth_Access_Token))
                    p.Add("token", Client.OAuth_Access_Token);
                if (include.Contains("token_secret") && !String.IsNullOrEmpty(Client.OAuth_Access_Token_Secret))
                    p.Add("token_secret", Client.OAuth_Access_Token_Secret);
            }
            
            if (include.Contains("verifier") && !String.IsNullOrEmpty(Client.OAuth_Verifier))
                p.Add("verifier", Client.OAuth_Verifier);
            
            if (_includes_params.Contains("signature") && !String.IsNullOrEmpty(Client.Signature))
                p.Add("signature", Client.Signature);
            if (include.Contains("signature_method") && !String.IsNullOrEmpty(Client.Signature_Method))
                p.Add("signature_method", Client.Signature_Method);
            if (include.Contains("version") && !String.IsNullOrEmpty(Client.Version))
                p.Add("version", Client.Version);
            if (include.Contains("timestamp") && !String.IsNullOrEmpty(Client.Timestamp))
                p.Add("timestamp", Client.Timestamp);
            if (include.Contains("nonce") && !String.IsNullOrEmpty(Client.Nonce))
                p.Add("nonce", Client.Nonce);
            if (include.Contains("callback") && !String.IsNullOrEmpty(Client.Callback))
                p.Add("callback", Client.Callback);
            if (include.Contains("callback_confirmed") && !String.IsNullOrEmpty(Client.Callback_Confirmed))
                p.Add("callback_confirmed", Client.Callback_Confirmed);

            return p;
        }

        private HashAlgorithm GetHash()
        {
            if (Client.Signature_Method != "HMAC-SHA1")
                throw new NotImplementedException("The signature_method is not implemented!");
            var token_s = (!String.IsNullOrEmpty(Client.OAuth_Access_Token_Secret) ? Client.OAuth_Access_Token_Secret : Client.OAuth_Token_Secret); //this["token_secret"];
            if (!_includes_params.Contains("token"))
                token_s = "";
            //var keystring = string.Format("{0}&{1}", UrlEncode(this["consumer_secret"]), UrlEncode(token_s)); // consumer_secret & token_secret
            var keystring = string.Format("{0}&{1}", UrlEncode(Client.Consumer_Token_Secret), UrlEncode(token_s));
            return new HMACSHA1 { Key = Encoding.ASCII.GetBytes(keystring) };
        }
        public static string UrlEncode(string value)
        {
            var result = new StringBuilder();
            foreach (char symbol in value)
            {
                if (unreservedChars.IndexOf(symbol) != -1)
                    result.Append(symbol);
                else
                {
                    foreach (byte b in Encoding.UTF8.GetBytes(symbol.ToString()))
                    {
                        result.Append('%' + string.Format("{0:X2}", b));
                    }
                }
            }
            return result.ToString();
        }

        private static string EncodeRequestParameters(ICollection<KeyValuePair<string, string>> p)
        {
            var sb = new StringBuilder();
            foreach (KeyValuePair<string, string> item in p.OrderBy(x => x.Key))
            {
                if (!string.IsNullOrEmpty(item.Value) &&
                    !item.Key.EndsWith("secret"))
                    sb.AppendFormat("oauth_{0}=\"{1}\", ",
                                    item.Key,
                                    UrlEncode(item.Value));
            }

            return sb.ToString().TrimEnd(' ').TrimEnd(',');
        }
        private Dictionary<string, string> ExtractQueryParameters(string queryString)
        {
            if (queryString.StartsWith("?"))
                queryString = queryString.Remove(0, 1);

            var result = new Dictionary<string, string>();

            if (string.IsNullOrEmpty(queryString))
                return result;

            foreach (string s in queryString.Split('&'))
            {
                if (!string.IsNullOrEmpty(s) && !s.StartsWith("oauth_"))
                {
                    if (s.IndexOf('=') > -1)
                    {
                        string[] temp = s.Split('=');
                        result.Add(temp[0], temp[1]);
                    }
                    else
                        result.Add(s, string.Empty);
                }
            }

            return result;
        }

        private string GetOAuthFormat(OAuthFormat format)
        {
            switch (format)
            {
                case OAuthFormat.JSON:
                    return "format=json";
                case OAuthFormat.XML:
                    return "format=xml";
                default:
                    return "format=json";
            }
        }
        private string GetHttpMethod(Enums.HttpMethod method)
        {
            switch (method)
            {
                case Enums.HttpMethod.GET:
                    return "GET";
                case Enums.HttpMethod.POST:
                    return "POST";
                case Enums.HttpMethod.PUT:
                    return "PUT";
                case Enums.HttpMethod.PATCH:
                    return "PATCH";
                case Enums.HttpMethod.DELETE:
                    return "DELETE";
                default:
                    return "GET";
            }
        }
    }
}
