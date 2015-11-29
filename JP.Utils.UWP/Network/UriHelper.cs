using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Windows.Foundation;

namespace JP.Utils.Network
{
    public static class UriHelper
    {
        public static Uri AddOrUpdateQuery(this Uri uri, string parameterName, string parameterValue)
        {
            List<KeyValuePair<string, string>> nameValueCollection = new List<KeyValuePair<string, string>>();

            string query = uri.Query;
            if (query == "?")
            {
                query = string.Empty;
            }
            if (query.Length > 0)
            {
                WwwFormUrlDecoder decoder = new WwwFormUrlDecoder(query);
                foreach (var nameValue in decoder)
                {
                    nameValueCollection.Add(new KeyValuePair<string, string>(nameValue.Name, nameValue.Value));
                }
            }

            parameterName = WebUtility.UrlEncode(parameterName);
            parameterValue = WebUtility.UrlEncode(parameterValue);

            bool exist = false;
            for (int i = 0; i < nameValueCollection.Count; i++)
            {
                if (nameValueCollection[i].Key == parameterName)
                {
                    nameValueCollection[i] = new KeyValuePair<string, string>(parameterName, parameterValue);
                    exist = true;
                    break;
                }
            }
            if (exist == false)
            {
                nameValueCollection.Add(new KeyValuePair<string, string>(parameterName, parameterValue));
            }

            query = string.Join("&", nameValueCollection.Select(temp => temp.Key + "=" + temp.Value));

            return new UriBuilder(uri)
            {
                Query = query
            }.Uri;
        }

        public static Uri AddOrUpdateQuery(this Uri uri, IEnumerable<KeyValuePair<string, string>> parameter)
        {
            foreach (var nameValue in parameter)
            {
                uri = uri.AddOrUpdateQuery(nameValue.Key, nameValue.Value);
            }
            return uri;
        }

        public static string GetQueryParameter(this Uri uri, string parameterName)
        {
            string query = uri.Query;
            if (query.Length <= 1)
            {
                return null;
            }
            WwwFormUrlDecoder decoder = new WwwFormUrlDecoder(query);
            return decoder.GetFirstValueByName(parameterName);
        }
    }
}