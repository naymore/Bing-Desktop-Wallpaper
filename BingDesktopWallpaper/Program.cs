namespace Rs.Exp.BingDesktopWallpaper
{
    public class Program
    {
        public static int Main(string[] args)
        {
            BingWallpaperGrabber bingWallpaperGrabber = new BingWallpaperGrabber();
            string imagePath = bingWallpaperGrabber.GrabImage(isoRegionCode: null, imageResolution: ImageResolution.W1920H1200);

            DesktopWallpaper desktopWallpaper = new DesktopWallpaper();
            int returnValue = desktopWallpaper.SetImage(imagePath);

            return returnValue;
        }
    }
}