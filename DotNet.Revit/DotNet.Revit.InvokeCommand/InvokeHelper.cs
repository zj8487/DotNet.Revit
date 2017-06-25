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


        /// <summary>
        /// 执行组合按键。
        /// </summary>
        /// <param name="key1">The key1.</param>
        /// <param name="key2">The key2.</param>
        public static void PressyKey(System.Windows.Forms.Keys key1, System.Windows.Forms.Keys key2)
        {
            keybd_event((byte)key1, 0, 0, 0);
            keybd_event((byte)key2, 0, 0, 0);
            keybd_event((byte)key1, 0, 2, 0);
            keybd_event((byte)key2, 0, 2, 0);
        }

        [DllImport("user32")]
        private static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);
    }
}
