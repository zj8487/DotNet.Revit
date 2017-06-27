using Autodesk.Revit;
using Autodesk.Revit.DB;
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
        private readonly static string RevitFolder = @"D:\Program Files\Autodesk\Revit 2016";

        static MainWindow()
        {
            MainWindow.AddEnvironmentPaths(new string[] { RevitFolder });
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 当解析程序集时触发，也就是当程序集加入到当前域开始解析时触发.
        /// </summary>
        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            AssemblyName assemblyName = new AssemblyName(args.Name);
            return Assembly.LoadFrom(System.IO.Path.Combine(RevitFolder, string.Format("{0}.dll", assemblyName.Name)));
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

        private void readFile_Click(object sender, RoutedEventArgs e)
        {
            var openFile = new OpenFileDialog();
            openFile.Filter = "Revit Project|*.rvt|Revit Family|*.rfa";

            if (openFile.ShowDialog() == true)
            {
                var file = openFile.FileName;

                if (!File.Exists(file))
                    return;

                var product = Product.GetInstalledProduct();
                var clientId = new ClientApplicationId(Guid.NewGuid(), "DotNet", "BIMAPI");

                // I am authorized by Autodesk to use this UI-less functionality. 必须是此字符串。为啥？ Autodesk 规定的.
                product.Init(clientId, "I am authorized by Autodesk to use this UI-less functionality.");

                var app = product.Application;

                var doc = app.OpenDocumentFile(file);

                var elems = new FilteredElementCollector(doc, doc.ActiveView.Id).WhereElementIsNotElementType();

                MessageBox.Show(string.Format("Docment ： {0}, Element： {1} 个"));
            }
        }
    }
}
