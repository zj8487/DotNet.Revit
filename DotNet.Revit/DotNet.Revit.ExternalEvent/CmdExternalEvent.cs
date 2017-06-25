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
        private ExternalEventHelper m_ExternalEventHelper;

        public static CmdExternalEvent Instance
        {
            get { return m_Instance; }
        }

        public ExternalEventHelper ExternalEventHelper
        {
            get { return m_ExternalEventHelper; }
        }


        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            m_Instance = this;
            m_ExternalEventHelper = new ExternalEventHelper(commandData.Application);
    

            var main = new MainWinodow();
            var mainHelper = new WindowInteropHelper(main);
            mainHelper.Owner = ComponentManager.ApplicationWindow;
            main.Show();

            return Result.Succeeded;
        }

    }

}
