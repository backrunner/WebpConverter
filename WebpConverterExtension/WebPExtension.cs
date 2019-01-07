using SharpShell.Attributes;
using SharpShell.SharpContextMenu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using WebpConverter;

namespace WebpConverterExtension
{
    [ComVisible(true)]
    [COMServerAssociation(AssociationType.AllFiles)]
    public class WebPExtension : SharpContextMenu
    {
        protected override bool CanShowMenu()
        {
            foreach (var filePath in SelectedItemPaths)
            {
                if (Path.GetExtension(filePath).ToLower() == ".webp")
                {
                    return true;
                }
            }
            return false;
        }

        protected override ContextMenuStrip CreateMenu()
        {
            var menu = new ContextMenuStrip();
            var item_jpg = new ToolStripMenuItem
            {
                Text = "将 WebP 转换为 JPG-高质量"
            };
            var item_png = new ToolStripMenuItem
            {
                Text = "将 WebP 转换为 PNG"
            };
            var separator_top = new ToolStripSeparator();
            var separator_bottom = new ToolStripSeparator();

            item_jpg.Click += (sender, args) => ConvertToJPG();
            item_png.Click += (sender, args) => ConvertToPNG();

            //add items to menu
            menu.Items.Add(separator_top);
            menu.Items.Add(item_png);
            menu.Items.Add(item_jpg);            
            menu.Items.Add(separator_bottom);

            return menu;
        }

        private void ConvertToJPG()
        {
            foreach (var filePath in SelectedItemPaths)
            {
                if (Path.GetExtension(filePath).ToLower() == ".webp")
                {
                    WebPConverter.ConvertToJPGHQ(filePath);
                }
            }
        }
        
        private void ConvertToPNG()
        {
            foreach (var filePath in SelectedItemPaths)
            {
                if (Path.GetExtension(filePath).ToLower() == ".webp")
                {
                    WebPConverter.ConvertToPNG(filePath);
                }
            }
        }
    }

    [ComVisible(true)]
    [COMServerAssociation(AssociationType.AllFiles)]
    public class WebPExtension_Reverse : SharpContextMenu
    {
        protected override bool CanShowMenu()
        {
            foreach (var filePath in SelectedItemPaths)
            {
                string extension = Path.GetExtension(filePath).ToLower();
                if (extension.Equals(".jpg") || extension.Equals(".jpeg") || extension.Equals(".png"))
                {
                    return true;
                }
            }
            return false;
        }

        protected override ContextMenuStrip CreateMenu()
        {
            var menu = new ContextMenuStrip();
            var item = new ToolStripMenuItem
            {
                Text = "将图片转换为 WebP"
            };

            var separator_top = new ToolStripSeparator();
            var separator_bottom = new ToolStripSeparator();

            item.Click += (sender, args) => ConvertToWebP();

            //add items to menu
            menu.Items.Add(separator_top);
            menu.Items.Add(item);
            menu.Items.Add(separator_bottom);

            return menu;
        }

        private void ConvertToWebP()
        {
            foreach (var filePath in SelectedItemPaths)
            {
                string extension = Path.GetExtension(filePath).ToLower();
                if (extension.Equals(".jpg") || extension.Equals(".jpeg") || extension.Equals(".png"))
                {
                    WebPConverter.ConvertToWebP(filePath);
                }
            }
        }
    }
}
