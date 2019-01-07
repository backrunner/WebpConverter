using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using MahApps.Metro.Controls;
using System.IO;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using IniParser;
using IniParser.Model;
using ImageProcessor.Imaging.Formats;
using ImageProcessor.Plugins.WebP.Imaging.Formats;
using ImageProcessor;

namespace WebpConverter
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        #region == 声明 ==
        //path
        private string filePath;
        private string appPath = AppDomain.CurrentDomain.BaseDirectory;
        private string configPath;

        //ini
        private FileIniDataParser iniParser = new FileIniDataParser();
        private IniData config = null;

        //dialog
        private MetroDialogSettings generalDialogSetting = new MetroDialogSettings()
        {
            AffirmativeButtonText = "确定",
            DialogTitleFontSize = 17,
            DialogMessageFontSize = 14
        };

        //flag
        private bool isToWebP = false;

        #endregion

        #region == 窗体初始化 ==
        public MainWindow()
        {
            InitializeComponent();
            configPath = appPath + @"config.ini";
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (File.Exists(configPath))
            {
                try
                {
                    config = iniParser.ReadFile(configPath);                    
                }
                catch
                {
                    //do nothing
                }

                if (config != null)
                {
                    //根据设置项更新UI
                    bool isAutoConvert = false;
                    bool.TryParse(config["Config"]["AutoConvert"], out isAutoConvert);
                    cb_autoConvert.IsChecked = isAutoConvert;

                    int formatIndex = 0;
                    int.TryParse(config["Config"]["OutputSelect"],out formatIndex);
                    cb_outputFormat.SelectedIndex = formatIndex;
                }
            }
            else
            {
                config = new IniData();
            }
        }
        #endregion

        #region == 格式转换 ==

        private async void Btn_convert_Click(object sender, RoutedEventArgs e)
        {
            if (filePath == null || filePath.Length <= 0)
            {
                await this.ShowMessageAsync("错误", "请选择需要转换的图片。", MessageDialogStyle.Affirmative, generalDialogSetting);
            } else
            {
                if (isToWebP)
                {
                    ConvertToWebP(filePath);
                } else
                {
                    switch (cb_outputFormat.SelectedIndex)
                    {
                        case 0:
                            ConvertToPNG(filePath);
                            break;
                        case 1:
                            ConvertToJPGHQ(filePath);
                            break;
                        case 2:
                            ConvertToJPGNQ(filePath);
                            break;
                    }
                }
            }
        }

        private void ConvertToWebP(string path)
        {
            ISupportedImageFormat webpFormat = new WebPFormat { Quality = 100 };
            ConvertImageToWebP(path, webpFormat);
        }

        private void ConvertToJPGHQ(string path)
        {
            ISupportedImageFormat jpgFormat = new JpegFormat { Quality = 100 };
            ConvertWebPToImage(path, jpgFormat, ".jpg");
        }        

        private void ConvertToJPGNQ(string path)
        {
            ISupportedImageFormat jpgFormat = new JpegFormat { Quality = 80 };
            ConvertWebPToImage(path, jpgFormat, ".jpg");
        }

        private void ConvertToPNG(string path)
        {
            ISupportedImageFormat pngFormat = new PngFormat { Quality = 100 };
            ConvertWebPToImage(path, pngFormat, ".png");
        }

        private async void ConvertImageToWebP(string path, ISupportedImageFormat format)
        {
            byte[] b_image;
            try
            {
                b_image = File.ReadAllBytes(path);
            }
            catch (Exception e)
            {
                await this.ShowMessageAsync("错误", "读入图片时发生错误: " + e.Message, MessageDialogStyle.Affirmative, generalDialogSetting);
                return;
            }
            using (MemoryStream inStream = new MemoryStream(b_image))
            {
                using (ImageFactory imageFactory = new ImageFactory(preserveExifData: true))
                {
                    string savePath = Path.GetDirectoryName(path) + @"/" + Path.GetFileNameWithoutExtension(path) + ".webp";
                    if (File.Exists(savePath))
                    {
                        var existDialogRes = await this.ShowMessageAsync("文件已存在", "将要保存的文件: " + savePath + " 已存在，是否覆盖？", MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings()
                        {
                            AffirmativeButtonText = "覆盖",
                            NegativeButtonText = "取消",
                            DialogTitleFontSize = 16
                        });
                        if (!(existDialogRes == MessageDialogResult.Affirmative))
                        {
                            return;
                        }
                    }
                    try
                    {
                        imageFactory.Load(inStream).Format(format).Save(savePath);
                    }
                    catch (Exception e)
                    {
                        await this.ShowMessageAsync("错误", "转换时发生错误: " + e.Message, MessageDialogStyle.Affirmative, generalDialogSetting);
                        return;
                    }
                    await this.ShowMessageAsync("转换完成", "图片已转换完成。", MessageDialogStyle.Affirmative, generalDialogSetting);
                }
            }
        }


        private async void ConvertWebPToImage(string path, ISupportedImageFormat format, string formatString)
        {
            byte[] b_image;
            try
            {
                b_image = File.ReadAllBytes(path);
            }
            catch (Exception e)
            {
                await this.ShowMessageAsync("错误", "读入图片时发生错误: " + e.Message, MessageDialogStyle.Affirmative, generalDialogSetting);
                return;
            }
            using (MemoryStream inStream = new MemoryStream(b_image))
            {
                using (ImageFactory imageFactory = new ImageFactory(preserveExifData: true))
                {
                    string savePath = Path.GetDirectoryName(path) + @"/" + Path.GetFileNameWithoutExtension(path) + formatString;
                    if (File.Exists(savePath))
                    {
                        var existDialogRes = await this.ShowMessageAsync("文件已存在", "将要保存的文件: " + savePath + " 已存在，是否覆盖？", MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings()
                        {
                            AffirmativeButtonText = "覆盖",
                            NegativeButtonText = "取消",
                            DialogTitleFontSize = 16
                        });
                        if (!(existDialogRes == MessageDialogResult.Affirmative))
                        {
                            return;
                        }
                    }
                    try
                    {
                        WebPFormat webpFormat = new WebPFormat();                        
                        imageFactory.Load(webpFormat.Load(inStream)).Format(format).Save(savePath);
                    }
                    catch (Exception e)
                    {
                        await this.ShowMessageAsync("错误", "转换时发生错误: " + e.Message, MessageDialogStyle.Affirmative, generalDialogSetting);
                        return;
                    }
                    await this.ShowMessageAsync("转换完成", "图片已转换完成。", MessageDialogStyle.Affirmative, generalDialogSetting);
                }
            }
        }

        #endregion

        #region == 文件拖放与选择 ==

        private void Rect_dragArea_Drop(object sender, DragEventArgs e)
        {
            string path = ((Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
            PutFile(path);
        }

        private void Lbl_dragArea_Drop(object sender, DragEventArgs e)
        {
            Rect_dragArea_Drop(sender, e);
        }

        private void Btn_selectFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog
            {
                Title = "请选择需要转换的图片文件",
                Filter = "图片文件|*.jpg|图片文件|*.png|图片文件|*.jpeg|图片文件|*.webp|所有文件|*.*"
            };
            if (fileDialog.ShowDialog() == true)
            {
                string path = fileDialog.FileName;
                PutFile(path);                
            }
        }

        //导入文件
        private async void PutFile(string path) {
            string extension = Path.GetExtension(path).ToLower();
            if (extension.Equals(".jpg") || extension.Equals(".jpeg") || extension.Equals(".png"))
            {
                filePath = path;
                tb_fileName.Text = filePath;
                isToWebP = true;
                lbl_outputFormat.Visibility = Visibility.Hidden;
                cb_outputFormat.Visibility = Visibility.Hidden;
            }
            else if (extension.Equals(".webp"))
            {
                filePath = path;
                tb_fileName.Text = filePath;
                isToWebP = false;
                lbl_outputFormat.Visibility = Visibility.Visible;
                cb_outputFormat.Visibility = Visibility.Visible;
            }
            else
            {
                await this.ShowMessageAsync("类型错误", "工具只支持 jpg，png，webp 格式的文件。", MessageDialogStyle.Affirmative, generalDialogSetting);
            }
        }

        #endregion

        #region == 设置 ==

        private void Cb_autoConvert_Checked(object sender, RoutedEventArgs e)
        {
            if (config == null)
            {
                config = new IniData();
            }
            config["Config"]["AutoConvert"] = cb_autoConvert.IsChecked.ToString();
            SaveConfig();
        }        

        private void Cb_outputFormat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (config == null)
            {
                config = new IniData();
            }
            config["Config"]["AutoConvert"] = cb_outputFormat.SelectedIndex.ToString();
            SaveConfig();            
        }

        private void SaveConfig()
        {
            try
            {
                iniParser.WriteFile(configPath, config);
            }
            catch
            {
                //do nothing
            }
        }

        #endregion
    }
}
