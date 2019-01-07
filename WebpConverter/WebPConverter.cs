using ImageProcessor;
using ImageProcessor.Imaging.Formats;
using ImageProcessor.Plugins.WebP.Imaging.Formats;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WebpConverter
{
    public static class WebPConverter
    {
        public static void ConvertToWebP(string path)
        {
            ISupportedImageFormat webpFormat = new WebPFormat { Quality = 100 };
            ConvertImageToWebP(path, webpFormat);
        }

        public static void ConvertToJPGHQ(string path)
        {
            ISupportedImageFormat jpgFormat = new JpegFormat { Quality = 100 };
            ConvertWebPToImage(path, jpgFormat, ".jpg");
        }

        public static void ConvertToJPGNQ(string path)
        {
            ISupportedImageFormat jpgFormat = new JpegFormat { Quality = 80 };
            ConvertWebPToImage(path, jpgFormat, ".jpg");
        }

        public static void ConvertToPNG(string path)
        {
            ISupportedImageFormat pngFormat = new PngFormat { Quality = 100 };
            ConvertWebPToImage(path, pngFormat, ".png");
        }

        private static void ConvertImageToWebP(string path, ISupportedImageFormat format)
        {
            byte[] b_image;
            try
            {
                b_image = File.ReadAllBytes(path);
            }
            catch (Exception e)
            {
                MessageBox.Show("读入图片时出现错误:\n" + e.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            using (MemoryStream inStream = new MemoryStream(b_image))
            {
                using (ImageFactory imageFactory = new ImageFactory(preserveExifData: true))
                {
                    string savePath = Path.GetDirectoryName(path) + @"/" + Path.GetFileNameWithoutExtension(path) + ".webp";
                    try
                    {
                        imageFactory.Load(inStream).Format(format).Save(savePath);
                    }
                    catch(Exception e)
                    {
                        MessageBox.Show("转换时出现错误:\n"+e.Message,"错误",MessageBoxButton.OK,MessageBoxImage.Error);
                        return;
                    }

                }
            }
        }

        private static void ConvertWebPToImage(string path, ISupportedImageFormat format,string formatString)
        {
            byte[] b_image;
            try
            {
                b_image = File.ReadAllBytes(path);
            }
            catch (Exception e)
            {
                MessageBox.Show("读入图片时出现错误:\n" + e.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            using (MemoryStream inStream = new MemoryStream(b_image))
            {
                using (ImageFactory imageFactory = new ImageFactory(preserveExifData: true))
                {
                    string savePath = Path.GetDirectoryName(path) + @"/" + Path.GetFileNameWithoutExtension(path) + formatString;
                    try
                    {
                        WebPFormat webpFormat = new WebPFormat();
                        imageFactory.Load(webpFormat.Load(inStream)).Format(format).Save(savePath);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("转换时出现错误:\n" + e.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                }
            }
        }
    }
}
