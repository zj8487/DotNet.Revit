using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using System.Diagnostics;
using Autodesk.Windows;

namespace DotNet.Revit.InvokeCommand
{
    [Transaction(TransactionMode.Manual)]
    public class CmdInvoke : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //// 调用Look up Snoop Db

            //InvokeHelper.Invoke("CustomCtrl_%CustomCtrl_%CustomCtrl_%Add-Ins%Revit Lookup%Options%Snoop Db..");

            // 调用右键菜单 - 区域放大
            // 右键菜单的 命令Id 可在Revit日志内进行查询..

            InvokeHelper.Invoke("ID_ZOOM_IN");

            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    public class CmdInvokeTest : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // 当命令控件点击执行后触发事件...

            ComponentManager.ItemExecuted += ComponentManager_ItemExecuted;

            return Result.Succeeded;
        }

        void ComponentManager_ItemExecuted(object sender, Autodesk.Internal.Windows.RibbonItemExecutedEventArgs e)
        {
            if (e.Item == null)
                return;

            // 获取命令Id 
            var id = UIFramework.ControlHelper.GetCommandId(e.Item);

            Debug.WriteLine(string.Format("Text: {0}   ID: {1}", e.Item.Text, e.Item.Id));
        }
    }
}
