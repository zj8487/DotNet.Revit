using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using RS = DotNet.Revit.Ribbon.Properties.Resources;

namespace DotNet.Revit.Ribbon
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);

        public MainWindow()
        {
            InitializeComponent();

            // 设置大图标
            this.ID_DoNet.LargeImage = this.ConvertTo(RS.Github32);
            this.ID_Revit.LargeImage = this.ConvertTo(RS.Github32);
            this.ID_GitHub.LargeImage = this.ConvertTo(RS.Github32);
            this.ID_GitHubChild.LargeImage = this.ConvertTo(RS.Github32);

            // 绑定命令
            this.ID_DoNet.CommandHandler = new CmdCommand();
            this.ID_Revit.CommandHandler = new CmdCommand();
            this.ID_GitHub.CommandHandler = new CmdCommand();
            this.ID_GitHubChild.CommandHandler = new CmdCommand();
        }

        private BitmapSource ConvertTo(Icon icon)
        {
            var hIcon = icon.Handle;
            try
            {
                var source = Imaging.CreateBitmapSourceFromHIcon(
                    hIcon,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
                return source;
            }
            finally
            {
                DeleteObject(hIcon);
            }
        }
    }

    public class CmdCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            var item = parameter as Autodesk.Windows.RibbonItem;
            MessageBox.Show(string.Format("This is {0} command test !!", item.Text));
        }
    }
}
