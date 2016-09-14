using System.Collections.Generic;

namespace Rs.Exp.BingDesktopWallpaper
{
    internal enum ImageResolution
    {
        W1920H1080,
        W1920H1200
    }

    internal enum ImageAspectRatio
    {
        AR4T3,
        AT16T9,
        AT16T10,
        DUALMONITOR
    }

    internal class ImageAspectRatioLists
    {
        public static List<string> AR4T3 = new List<string> { "2048x1536", "1920x1440", "1680x1260", "1600x1200", "1440x1080", "1280x960", "1024x768" };

        public static List<string> AR16T9 = new List<string> { "15360x8640", "7680x4320", "5120x2880", "4096x2304", "3840x2160", "2560x1440", "1920x1080", "1600x900", "1366x768", "1280x720" };

        public static List<string> AR16T10 = new List<string> { "4096x2560", "3840x2400", "2560x1600", "1920x1200", "1680×1050", "1440x900", "1280x800" };

        public static List<string> DUALMONITOR = new List<string> { "3840x1200", "3840x1080" };

        public static List<string> CUSTOM = new List<string> { "4096x2560", "3840x2400", "15360x8640", "7680x4320", "5120x2880", "4096x2304", "2560x1600", "3840x1200", "3840x2160", "2560x1440", "3840x1080", "1920x1200", "1920x1080" };
    }
}