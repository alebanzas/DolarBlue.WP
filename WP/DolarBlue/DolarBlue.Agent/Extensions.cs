using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DolarBlueAgent
{
    public static class UriExtensions
    {
        private static Uri ToApiCallUri(this string source, string param, bool alwaysRefresh = false)
        {
            string AppName = "DOLARBLUEWP";
            string AppVersion = "2.2.4.8";
            if (!string.IsNullOrWhiteSpace(param))
            {
                param = "&" + param;
            }
            else
            {
                param = string.Empty;
            }
            var refresh = string.Empty;
            if (alwaysRefresh)
            {
                refresh = string.Format("&__t={0}.{1}", DateTime.UtcNow.Hour, DateTime.UtcNow.Minute);
            }
            
            var apiCallUri = new Uri(string.Format("http://api.alebanzas.com.ar{0}/?appId={1}&versionId={2}&openCount={3}&installationId={4}{5}{6}",
                source,
                AppName,
                AppVersion,
                0,
                new Guid(),
                refresh,
                param));

            Debug.WriteLine(apiCallUri);

            return apiCallUri;
        }

        public static Uri ToApiCallUri(this string source, Dictionary<string, object> param, bool alwaysRefresh = false)
        {
            var pp = string.Join("&", param.Select(x => x.Key + "=" + x.Value));

            return ToApiCallUri(source, pp, alwaysRefresh);
        }

        public static Uri ToApiCallUri(this string source, bool alwaysRefresh = false)
        {
            return ToApiCallUri(source, string.Empty, alwaysRefresh);
        }
    }
}
