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
using System.Windows.Shapes;
using DotNet.Revit.ExternalEvent;
using Autodesk.Revit.UI.Selection;

namespace DotNet.Revit.ExternalEvent
{
    /// <summary>
    /// Interaction logic for MainWinodow.xaml
    /// </summary>
    public partial class MainWinodow : Window
    {
        public MainWinodow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var helper = CmdExternalEvent.Instance.ExternalEventHelper;
    
            helper.Invoke(m =>
            {
                var uiDoc = m.ActiveUIDocument;
                var doc = uiDoc.Document;

                var ids = uiDoc.Selection.GetElementIds();

                if (ids.Count > 0)
                {
                    doc.Invoke(x =>
                    {
                        doc.Delete(ids);

                    });
                }
            });

            helper.Invoke(m =>
            {
                var uiDoc = m.ActiveUIDocument;
                var doc = uiDoc.Document;
                try
                {
                    var refer = uiDoc.Selection.PickObject(ObjectType.Element);
                    doc.Invoke(x =>
                    {
                        doc.Delete(refer.ElementId);
                    });
                }
                catch (Exception)
                {

                }
            });
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }
    }
}
