namespace Rs.Exp.BingDesktopWallpaper
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Net.Http;
    using Newtonsoft.Json.Linq;

    internal class BingWallpaperGrabber
    {
        // n = number of images
        // mkt = location (en-US)
        const string BING_IMAGE_ARCHIVE_URL = "http://www.bing.com/HPImageArchive.aspx?format=js&idx=0&n=1";

        /// <summary>
        /// This will download the latest Bing wallpaper for your region to your "My Pictures" folder.
        /// </summary>
        /// <param name="isoRegionCode"></param>
        /// <param name="imageResolution"></param>
        /// <returns></returns>
        public string GrabImage(string isoRegionCode = null, ImageResolution imageResolution = ImageResolution.W1920H1080)
        {
            string retValue;

            isoRegionCode = GetAndValidateRegionCode(isoRegionCode);

            using (HttpClient httpClient = new HttpClient())
            {
                // get latest bing image url
                string imageUrl = GetLatestBingImageUrl(httpClient, isoRegionCode);

                // adjust image resolution
                imageUrl = SetImageResolutionInUrl(imageUrl, imageResolution);

                // download image and store locally
                retValue = SaveLatestBingImageLocally(httpClient, imageUrl);
            }

            return retValue;
        }

        private string SetImageResolutionInUrl(string imageUrl, ImageResolution imageResolution)
        {
            if (imageResolution == ImageResolution.W1920H1080) // DEFAULT
            {
                return imageUrl;
            }

            if (imageResolution == ImageResolution.W1920H1200)
            {
                imageUrl = imageUrl.Replace("_1920x1080", "_1920x1200");
            }

            return imageUrl;
        }

        private string GetAndValidateRegionCode(string isoRegionCode)
        {
            if (!string.IsNullOrEmpty(isoRegionCode))
            {
                try
                {
                    CultureInfo cultureInfo = new CultureInfo(isoRegionCode);
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

        private string GetLatestBingImageUrl(HttpClient httpClient, string isoRegionCode)
        {
            string requestUri = string.Concat(BING_IMAGE_ARCHIVE_URL, "&mkt=", isoRegionCode);

            // get current image url
            string json = httpClient.GetStringAsync(requestUri).Result;
            JObject jObject = JObject.Parse(json);
            string imageUrl = jObject.SelectToken("images.[0].url").ToString();

            return string.Concat("http://www.bing.com", imageUrl);
        }

        private string SaveLatestBingImageLocally(HttpClient client, string imageUrl)
        {
            byte[] image = client.GetByteArrayAsync(imageUrl).Result;

            string imageFileName = ExtractImageFilenameFromUrl(imageUrl);
            string localFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "Bing Wallpapers", imageFileName);

            File.WriteAllBytes(localFilePath, image);

            return localFilePath;
        }

        private static string ExtractImageFilenameFromUrl(string imageUrl)
        {
            Uri uri = new Uri(imageUrl);
            return uri.Segments[uri.Segments.Length-1];
        }
    }
}
