using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FaceDetection.Implementation
{
    public class Authentication
    {
        private string subscriptionKey;
        private string tokenFetchUri;
        private IMemoryCache _cache;

        public Authentication(string tokenFetchUri, string subscriptionKey, IMemoryCache cache)
        {
            _cache = cache;


            if (string.IsNullOrWhiteSpace(tokenFetchUri))
            {
                throw new ArgumentNullException(nameof(tokenFetchUri));
            }
            if (string.IsNullOrWhiteSpace(subscriptionKey))
            {
                throw new ArgumentNullException(nameof(subscriptionKey));
            }
            this.tokenFetchUri = tokenFetchUri;
            this.subscriptionKey = subscriptionKey;
        }

        public async Task<string> FetchTokenAsync()
        {
            string cached_token;
            bool found = _cache.TryGetValue("token", out cached_token);
            if (!found)
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", this.subscriptionKey);
                    UriBuilder uriBuilder = new UriBuilder(this.tokenFetchUri);

                    HttpResponseMessage result = await client.PostAsync(uriBuilder.Uri.AbsoluteUri, null).ConfigureAwait(false);
                    var token = await result.Content.ReadAsStringAsync().ConfigureAwait(false);
                    _cache.Set("token", token, new MemoryCacheEntryOptions().SetAbsoluteExpiration(relative: TimeSpan.FromMinutes(8)));
                    return token;
                }
            }
            else
            {
                return cached_token;
            }
        }
    }
}
