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

namespace WebpConverter
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            configPath = appPath + @"config.ini";
        }

        #region == 声明 ==
        //path
        private string filePath;
        private string appPath = AppDomain.CurrentDomain.BaseDirectory;
        private string configPath;

        //ini
        private FileIniDataParser iniParser = new FileIniDataParser();
        private IniData config = null;

        #endregion

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
                    cb_autoConvert.IsChecked = bool.Parse(config["Config"]["AutoConvert"]);
                    cb_outputFormat.SelectedIndex = int.Parse(config["Config"]["OutputSelect"]);
                }
            }
            else
            {
                config = new IniData();
            }
        }

        private void Btn_convert_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ConvertToWebP()
        {

        }

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
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Title = "请选择需要转换的图片文件";
            fileDialog.Filter = "图片文件|*.jpg|图片文件|*.png|图片文件|*.jpeg|图片文件|*.webp|所有文件|*.*";
            if (fileDialog.ShowDialog() == true)
            {
                string path = fileDialog.FileName;
                PutFile(path);                
            }
        }

        //导入文件
        private async void PutFile(string path) {
            string extension = Path.GetExtension(path);
            if (extension.Equals(".jpg") || extension.Equals(".jpeg") || extension.Equals(".png"))
            {
                filePath = path;
                tb_fileName.Text = filePath;
                lbl_outputFormat.Visibility = Visibility.Hidden;
                cb_outputFormat.Visibility = Visibility.Hidden;
            }
            else if (extension.Equals(".webp"))
            {
                filePath = path;
                tb_fileName.Text = filePath;
                lbl_outputFormat.Visibility = Visibility.Visible;
                cb_outputFormat.Visibility = Visibility.Visible;
            }
            else
            {
                await this.ShowMessageAsync("类型错误", "工具只支持jpg，png，webp格式的文件。");
            }
        }

        #endregion

        #region == 设置 ==

        private void Cb_autoConvert_Checked(object sender, RoutedEventArgs e)
        {
            config["Config"]["AutoConvert"] = cb_autoConvert.IsChecked.ToString();
            SaveConfig();
        }        

        private void Cb_outputFormat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
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
