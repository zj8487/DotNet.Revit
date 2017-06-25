using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Windows;
using System.Reflection;

namespace DotNet.Revit.ShortKey
{
    /// <summary>
    /// 动态设置命令控件的快捷键
    /// </summary>
    /// <seealso cref="Autodesk.Revit.UI.IExternalCommand" />
    [Transaction(TransactionMode.Manual)]
    public class CmdShortKey : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, Autodesk.Revit.DB.ElementSet elements)
        {
            // 基于Revit的命令控件动态设置快捷键测试....

            var panel = commandData.Application.CreateRibbonPanel("ShortKeyTest");
            var data = new PushButtonData("Buttonname2", "Button2", typeof(CmdTest).Assembly.Location, typeof(CmdTest).FullName);
            var btn = panel.AddItem(data) as Autodesk.Revit.UI.RibbonButton;

            btn.SetShortCut("BIMAPI");


            // 修改 门 按钮的快捷键

            var item = Autodesk.Windows.ComponentManager.Ribbon.FindItem("ID_OBJECTS_DOOR", false);

            if (item == null)
                return Result.Succeeded;

            item.SetShortCut("DOOR");
            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    public class CmdTest : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            System.Windows.MessageBox.Show("Shortcut  is valid!!");
            return Result.Succeeded;
        }
    }
}
