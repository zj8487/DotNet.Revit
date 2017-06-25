using Autodesk.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UIFramework;
using UIFrameworkServices;

namespace DotNet.Revit.InvokeCommand
{
    public static class InvokeHelper
    {
        /// <summary>
        /// 指定一个命令Id，调用命令.
        /// </summary>
        /// <param name="id">命令控件的Id值</param>
        /// <returns></returns>
        public static bool Invoke(string id)
        {
            var item = ComponentManager.Ribbon.FindItem(id, false, true);
            if (item == null || !(item is RibbonCommandItem))
                return false;

            if (ExternalCommandHelper.CanExecute(id))
            {
                ExternalCommandHelper.executeExternalCommand(id);
                return true;
            }
            else
            {
                var cmdId = UIFramework.ControlHelper.GetCommandId(item);

                if (!CommandHandlerService.canExecute(cmdId))
                {
                    var ex = (item as RibbonCommandItem).CommandHandler;
                    if (ex == null || !ex.CanExecute(null))
                        return false;

                    ex.Execute(null);
                    return true;
                }

                var type = typeof(BarControlId).Assembly.GetType("UIFramework.CommandUtility");
                type.InvokeMember("Execute",
                    BindingFlags.Static | BindingFlags.Public | BindingFlags.InvokeMethod,
                    Type.DefaultBinder, null,
                    new object[] { item });

                return true;
            }

        }

    }
}
