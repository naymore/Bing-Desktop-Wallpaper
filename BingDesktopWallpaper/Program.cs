namespace Rs.Exp.BingDesktopWallpaper
{
    public class Program
    {
        public static int Main(string[] args)
        {
            BingWallpaperGrabber bingWallpaperGrabber = new BingWallpaperGrabber();

            string imagePath = bingWallpaperGrabber.GrabImage("en-US", ImageAspectRatioLists.CUSTOM);

            if (string.IsNullOrEmpty(imagePath))
            {
                return 0;
            }

            DesktopWallpaper desktopWallpaper = new DesktopWallpaper();
            int returnValue = desktopWallpaper.SetImage(imagePath);

            return returnValue;
        }
    }
}