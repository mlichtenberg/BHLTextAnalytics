using System.Net;
using System.Text;

namespace BHLTextAnalytics
{
    public static class BHLApi3
    {
        private static string _getItemMetadataEndpoint = "https://www.biodiversitylibrary.org/api3?op=GetItemMetadata&id={0}&pages={1}&ocr={2}&parts={3}&format={4}&apikey={5}";
        private static string _nameSearchEndpoint = "https://www.biodiversitylibrary.org/api3?op=NameSearch&name={0}&format={1}&apikey={2}";

        public static string GetItemMetadata(int id, bool includePages, bool includeOcr, bool includeParts,
            ResponseFormat format, string apiKey)
        {
            string pages = includePages ? "t" : "f";
            string ocr = includeOcr ? "t" : "f";
            string parts = includeParts ? "t" : "f";
            string fmt = (format == ResponseFormat.Json ? "json" : "xml");
            string url = string.Format(_getItemMetadataEndpoint, id.ToString(), pages, ocr, parts, fmt, apiKey);
            return InvokeMethod(url);
        }

        public static string NameSearch(string name, ResponseFormat format, string apiKey)
        {
            string fmt = (format == ResponseFormat.Json ? "json" : "xml");
            string url = string.Format(_nameSearchEndpoint, name, fmt, apiKey);
            return InvokeMethod(url);
        }

        private static string InvokeMethod(string url)
        {
            WebClient wc = new WebClient();
            wc.Encoding = Encoding.UTF8;
            string apiResponse = wc.DownloadString(url);
            return apiResponse;
        }

        public enum ResponseFormat
        {
            Xml,
            Json
        }
    }
}
