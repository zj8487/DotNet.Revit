using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;

namespace DotNet.Revit.ExternalEvent
{
    [Transaction(TransactionMode.Manual)]
    public class CmdExternalEvent : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {




            return Result.Succeeded;
        }
    }

    public class ExternalEventHandler : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {
           
        }

        public string GetName()
        {
            return "Name";
        }
    }
}
