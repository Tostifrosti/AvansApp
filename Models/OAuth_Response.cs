using System.Collections.Generic;

namespace AvansApp.Models
{
    public class OAuth_Response
    {
        public string AllText { get; set; }
        private Dictionary<string, string> _params;
        
        public string this[string ix]
        {
            get { return _params.ContainsKey(ix) ? _params[ix] : null; }
        }
        
        internal OAuth_Response(string alltext)
        {
            AllText = alltext;
            _params = new Dictionary<string, string>();
            var kvpairs = alltext.Split('&');
            foreach (var pair in kvpairs)
            {
                var kv = pair.Split('=');
                _params.Add(kv[0], kv[1]);
            }
            // expected keys:
            //   oauth_token, oauth_token_secret, user_id, screen_name, etc
        }
    }
}
