using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace VLDonFeedStockApp.Models
{
    public static class WebRequests
    {
        public static async Task<string> GetRequest(string url, string _jwtToken)
        {
            HttpClient _tokenClientPrice = new HttpClient();
            _tokenClientPrice.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwtToken);
            var _responseTokenPrice = await _tokenClientPrice.GetStringAsync(url);
            return _responseTokenPrice;
        }
    }
}
