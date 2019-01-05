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
        }

        private string filePath;

        private async void Rect_dragArea_Drop(object sender, DragEventArgs e)
        {
            string path = ((Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
            string extension = Path.GetExtension(path);
            if (extension.Equals(".jpg") || extension.Equals(".jpeg") || extension.Equals(".png"))
            {
                filePath = path;
                tb_fileName.Text = filePath;
                lbl_outputFormat.Visibility = Visibility.Hidden;
                cb_outputFormat.Visibility = Visibility.Hidden;
            } else if (extension.Equals(".webp"))
            {
                filePath = path;
                tb_fileName.Text = filePath;
                lbl_outputFormat.Visibility = Visibility.Visible;
                cb_outputFormat.Visibility = Visibility.Visible;
            } else
            {
                await this.ShowMessageAsync("类型错误", "工具只支持jpg，png，webp格式的文件。");
            }
        }

        private void Lbl_dragArea_Drop(object sender, DragEventArgs e)
        {
            Rect_dragArea_Drop(sender, e);
        }

        private void Btn_selectFile_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
