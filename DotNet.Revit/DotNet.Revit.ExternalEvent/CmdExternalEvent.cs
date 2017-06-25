using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using DotNet.Revit.ExternalEvent;
using System.Windows.Interop;
using Autodesk.Windows;

namespace DotNet.Revit.ExternalEvent
{
    [Transaction(TransactionMode.Manual)]
    public class CmdExternalEvent : IExternalCommand
    {
        private static CmdExternalEvent m_Instance;
        private Autodesk.Revit.UI.ExternalEvent m_ExternalEvent;

        public static CmdExternalEvent Instance
        {
            get { return CmdExternalEvent.m_Instance; }
        }

        public Autodesk.Revit.UI.ExternalEvent ExternalEvent
        {
            get { return m_ExternalEvent; }
        }

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            m_ExternalEvent = Autodesk.Revit.UI.ExternalEvent.Create(new ExternalEventHandler());

            var main = new MainWinodow();
            var mainHelper=new WindowInteropHelper(main);
            mainHelper.Owner = ComponentManager.ApplicationWindow;
            main.Show();


            return Result.Succeeded;
        }
    }

    public class ExternalEventHandler : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {
            var uiDoc = app.ActiveUIDocument;
            var doc = uiDoc.Document;

            var ids = uiDoc.Selection.GetElementIds();

            if (ids.Count > 0)
            {
                doc.Invoke(m => 
                {
                    doc.Delete(ids);

                });
            }
        }

        public string GetName()
        {
            return "Name";
        }
    }

}
