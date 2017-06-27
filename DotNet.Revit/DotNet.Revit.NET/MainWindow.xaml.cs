using Autodesk.Revit;
using Autodesk.Revit.DB;
using Autodesk.RevitAddIns;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
using System.Windows.Shapes;

namespace DotNet.Revit.NET
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly static string RevitFolder = string.Empty;
        private Autodesk.Revit.ApplicationServices.Application m_App;

        static MainWindow()
        {
            RevitFolder = MainWindow.GetRevitInstallationPath(RevitVersion.Revit2016);

            MainWindow.AddEnvironmentPaths(new string[] { RevitFolder });
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        public MainWindow()
        {
            InitializeComponent();

            if (Directory.Exists(RevitFolder))
            {
                var product = Product.GetInstalledProduct();
                var clientId = new ClientApplicationId(Guid.NewGuid(), "DotNet", "BIMAPI");

                // I am authorized by Autodesk to use this UI-less functionality. 必须是此字符串。为啥？ Autodesk 规定的.
                product.Init(clientId, "I am authorized by Autodesk to use this UI-less functionality.");
                m_App = product.Application;
            }
        }

        /// <summary>
        /// 当解析程序集时触发，也就是当程序集加入到当前域开始解析时触发.
        /// </summary>
        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var assemblyName = new AssemblyName(args.Name);
            var file = string.Format("{0}.dll", System.IO.Path.Combine(RevitFolder, assemblyName.Name));

            if (File.Exists(file))
                return Assembly.LoadFrom(file);

            return null;
        }

        /// <summary>
        /// 动态加入PATH环境变量.
        /// </summary>
        /// <param name="paths">The paths.</param>
        private static void AddEnvironmentPaths(params string[] paths)
        {
            var path = new[] { Environment.GetEnvironmentVariable("PATH") ?? string.Empty };
            var newPath = string.Join(System.IO.Path.PathSeparator.ToString(), path.Concat(paths));
            Environment.SetEnvironmentVariable("PATH", newPath);
        }

        /// <summary>
        /// 指定Revit版本获取Revit安装路径.
        /// </summary>
        private static string GetRevitInstallationPath(RevitVersion version)
        {
            var product = Autodesk.RevitAddIns.RevitProductUtility.GetAllInstalledRevitProducts().FirstOrDefault(m => m.Version == version);
            if (product == null)
                return string.Empty;
            return product.InstallLocation;
        }

        /// <summary>
        /// 读取文件测试.
        /// </summary>
        private void readFile_Click(object sender, RoutedEventArgs e)
        {
            if (m_App == null)
            {
                MessageBox.Show("Revit NET 初始化失败，请检查电脑是否已安装Revit 相应版本！！");
                return;
            }

            var openFile = new OpenFileDialog();
            openFile.Filter = "Revit Project|*.rvt|Revit Family|*.rfa";

            if (openFile.ShowDialog() == true)
            {
                var file = openFile.FileName;

                if (!File.Exists(file))
                    return;

                var doc = m_App.OpenDocumentFile(file);
                if (doc == null)
                    return;

                var elems = new FilteredElementCollector(doc).OfClass(typeof(FamilyInstance));
                MessageBox.Show(string.Format("Docment ： {0}, Element： {1} 个", doc.PathName, elems.Count()));
            }
        }
    }
}
