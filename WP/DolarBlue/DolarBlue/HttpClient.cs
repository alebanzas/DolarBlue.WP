using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace DolarBlue
{
    public class HttpClient
    {
        private const string AppKey = "78f00a47badb4b8eafadf5e7114d917a";
        private const string AppSecret = "7a691d5e604f46628601eb6b8c4fca3a";

        public HttpWebRequest Get(Uri requestUrl)
        {
            return SendRequestGetResponse(requestUrl);
        }

        private HttpWebRequest SendRequestGetResponse(Uri requestUrl)
        {
            var request = (HttpWebRequest)WebRequest.Create(requestUrl);

            request.Method = "GET";
            //request.ContentType = "application/json";
            //http://www.w3.org/Protocols/rfc2616/rfc2616-sec3.html#sec3.3.1
            request.Headers["X-ABS-Date"] = DateTime.UtcNow.ToUniversalTime().ToString("r", CultureInfo.InvariantCulture);

            var signature = GetSignature(AppSecret, request);
            request.Headers[HttpRequestHeader.Authorization] = string.Format("{0} {1}", "ABS-H", string.Format("{0}:{1}", AppKey, signature));

            return request;
        }

        private string GetSignature(string secret, HttpWebRequest request)
        {
            string canonicalizedMessage = string.Join("\n", GetCanonicalParts(request));
            if (string.IsNullOrEmpty(secret) || string.IsNullOrEmpty(canonicalizedMessage))
            {
                return "";
            }
            byte[] secretBytes = Encoding.UTF8.GetBytes(secret);
            byte[] valueBytes = Encoding.UTF8.GetBytes(canonicalizedMessage);
            string signature;

            using (var hmac = new HMACSHA256(secretBytes))
            {
                byte[] hash = hmac.ComputeHash(valueBytes);
                signature = Convert.ToBase64String(hash);
            }
            return signature;
        }

        private string[] GetCanonicalParts(HttpWebRequest request)
        {
            var result = new List<string>
            {
                request.Method
            };

            var httpRequestHeaders = request.Headers;
            result.Add(httpRequestHeaders[HttpRequestHeader.ContentMd5]);
            result.Add(httpRequestHeaders[HttpRequestHeader.ContentType] ?? string.Empty);
            result.Add(httpRequestHeaders[HttpRequestHeader.Date] ?? string.Empty);

            result.AddRange(from key in httpRequestHeaders.AllKeys where key.StartsWith("X-ABS", StringComparison.InvariantCultureIgnoreCase) select string.Format("{0}:{1}", key.ToLowerInvariant().Trim(), httpRequestHeaders[key]));

            result.Add(request.RequestUri.LocalPath + request.RequestUri.Query);

            return result.ToArray();
        }

        //public string ContentMd5HashForHeader(string input)
        //{
        //    // http://www.w3.org/Protocols/rfc2616/rfc2616-sec14.html#sec14.15
        //    MD5 md5 = MD5.Create();
        //    byte[] inputBytes = Encoding.UTF8.GetBytes(input);
        //    byte[] hashBytes = md5.ComputeHash(inputBytes);
        //
        //    return Convert.ToBase64String(hashBytes);
        //}
    }
}
