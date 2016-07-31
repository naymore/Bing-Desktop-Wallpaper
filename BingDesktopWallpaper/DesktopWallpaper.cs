using System;
using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace Rs.Exp.BingDesktopWallpaper
{
    internal class DesktopWallpaper
    {
        const int SET_DESKTOP_BACKGROUND = 20;
        const int UPDATE_INI_FILE = 1;
        const int SEND_WINDOWS_INI_CHANGE = 2;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);
        
        /// <summary>
        /// This will set your Windows Desktop Wallpaper.
        /// </summary>
        /// <param name="imagePath"></param>
        /// <param name="orientation"></param>
        /// <returns></returns>
        public int SetImage(string imagePath, PictureOrientation orientation = PictureOrientation.Fill)
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true))
            {
                if (key == null) throw new Exception("Registry key not found!");

                switch (orientation)
                {
                    case PictureOrientation.Tile:
                        key.SetValue(@"PicturePosition", "0");
                        key.SetValue(@"TileWallpaper", "1");
                        break;
                    case PictureOrientation.Center:
                        key.SetValue(@"PicturePosition", "0");
                        key.SetValue(@"TileWallpaper", "0");
                        break;
                    case PictureOrientation.Stretch:
                        key.SetValue(@"PicturePosition", "2");
                        key.SetValue(@"TileWallpaper", "0");
                        break;
                    case PictureOrientation.Fit:
                        key.SetValue(@"PicturePosition", "6");
                        key.SetValue(@"TileWallpaper", "0");
                        break;
                    case PictureOrientation.Fill:
                        key.SetValue(@"PicturePosition", "10");
                        key.SetValue(@"TileWallpaper", "0");
                        break;
                }

                key.Close();
            }

            return SystemParametersInfo(SET_DESKTOP_BACKGROUND, 0, imagePath, UPDATE_INI_FILE | SEND_WINDOWS_INI_CHANGE);
        }
    }
}