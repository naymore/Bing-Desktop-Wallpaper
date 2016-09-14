using System.Collections.Generic;
using System.Net;
using NLog;

namespace Rs.Exp.BingDesktopWallpaper
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Net.Http;
    using Newtonsoft.Json.Linq;

    internal class BingWallpaperGrabber : IDisposable
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly HttpClient _httpClient;
        private readonly HttpClientHandler _handler;

        const string BING_IMAGE_ARCHIVE_URL = "http://www.bing.com/HPImageArchive.aspx?format=js&idx=0&n=1";

        internal BingWallpaperGrabber()
        {
            _handler = new HttpClientHandler { AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip };
            _httpClient = new HttpClient(_handler);
        }

        internal string GrabImage(string isoRegionCode, List<string> imageResolutions)
        {
            isoRegionCode = GetAndValidateRegionCode(isoRegionCode);
            _logger.Debug("Region Code (validated) is {0}", isoRegionCode);

            string baseImageUrl = GetDefaultSizedImageUrl(isoRegionCode);
            _logger.Debug("Default (sized) image url is {0}", baseImageUrl);

            string bestImageUrl = GetBestSizedImageUrl(baseImageUrl, imageResolutions);
            _logger.Info("Best (sized) image url is {0}", bestImageUrl);

            if (string.IsNullOrEmpty(bestImageUrl))
            {
                _logger.Info("No image url found for desired resolutions.");
                return null;
            }

            string retValue = SaveLatestBingImageLocally(bestImageUrl);
            _logger.Info("Image saved at {0}", retValue);

            return retValue;
        }

        private string GetBestSizedImageUrl(string baseImageUrl, List<string> imageResolutions)
        {
            foreach (string resolution in imageResolutions)
            {
                string testUrl = baseImageUrl.Replace("_1920x1080", "_" + resolution);
                bool success = CheckIfImageExists(testUrl);

                _logger.Debug("Image resolution {0}: {1}", resolution, success ? "OK" : "NOT FOUND");

                if (success)
                    return testUrl;
            }

            return null;
        }

        private string GetDefaultSizedImageUrl(string isoRegionCode)
        {
            string requestUri = string.Concat(BING_IMAGE_ARCHIVE_URL, "&mkt=", isoRegionCode);

            // get current image url
            string json = _httpClient.GetStringAsync(requestUri).Result;
            JObject jObject = JObject.Parse(json);
            string imageUrl = jObject.SelectToken("images.[0].url").ToString();

            return string.Concat("http://www.bing.com", imageUrl);
        }

        private bool CheckIfImageExists(string imageUrl)
        {
            HttpRequestMessage httpRequestMsg = new HttpRequestMessage(HttpMethod.Head, imageUrl);
            HttpResponseMessage response = _httpClient.SendAsync(httpRequestMsg).Result;

            return response.IsSuccessStatusCode;
        }

        private string GetAndValidateRegionCode(string isoRegionCode)
        {
            if (!string.IsNullOrEmpty(isoRegionCode))
            {
                try
                {
                    new CultureInfo(isoRegionCode);
                }
                catch (CultureNotFoundException)
                {
                    isoRegionCode = null;
                }
            }

            if (string.IsNullOrEmpty(isoRegionCode))
            {
                isoRegionCode = CultureInfo.CurrentUICulture.IetfLanguageTag;
            }

            return isoRegionCode;
        }
        private string SaveLatestBingImageLocally(string imageUrl)
        {
            byte[] image = _httpClient.GetByteArrayAsync(imageUrl).Result;

            string imageFileName = ExtractImageFilenameFromUrl(imageUrl);
            string localFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "Bing Wallpapers", imageFileName);

            File.WriteAllBytes(localFilePath, image);

            return localFilePath;
        }

        private static string ExtractImageFilenameFromUrl(string imageUrl)
        {
            Uri uri = new Uri(imageUrl);
            return uri.Segments[uri.Segments.Length - 1];
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
            _handler?.Dispose();
        }
    }
}
