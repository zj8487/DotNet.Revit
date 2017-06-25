using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UIFramework;
using UIFrameworkServices;

namespace DotNet.Revit.ShortKey
{
    public static class ShortKeyHelper
    {
        /// <summary>
        /// 基于Revit封装的RibbonButton设置其快捷键.
        /// </summary>
        /// <param name="btn">RibbonButton.</param>
        /// <param name="key">快捷键字符串.</param>
        /// <returns></returns>
        public static bool SetShortCut(this Autodesk.Revit.UI.RibbonButton btn, string key)
        {
            if (btn == null)
                return false;

            var item = btn.GetType().InvokeMember("getRibbonItem",
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod,
                Type.DefaultBinder, btn, null);

            if (item == null)
                return false;

            return (item as Autodesk.Windows.RibbonItem).SetShortCut(key);
        }

        /// <summary>
        /// 基于通用库封装的RibbonCommandItem设置其快捷键.
        /// </summary>
        /// <param name="commandItem">RibbonCommandItem.</param>
        /// <param name="key">快捷键字符串.</param>
        /// <returns></returns>
        public static bool SetShortCut(this Autodesk.Windows.RibbonItem commandItem, string key)
        {
            if (commandItem == null || string.IsNullOrEmpty(key))
                return false;

            var parentTab = default(Autodesk.Windows.RibbonTab);
            var parentPanel = default(Autodesk.Windows.RibbonPanel);

            Autodesk.Windows.ComponentManager.Ribbon.FindItem(commandItem.Id, false, out parentPanel, out parentTab, true);

            if (parentTab == null || parentPanel == null)
                return false;

            var path = string.Format("{0}>{1}", parentTab.Id, parentPanel.Source.Id);
            var cmdId = ControlHelper.GetCommandId(commandItem);

            if (string.IsNullOrEmpty(cmdId))
            {
                cmdId = Guid.NewGuid().ToString();
                ControlHelper.SetCommandId(commandItem, cmdId);
            }

            var shortcutItem = new ShortcutItem(commandItem.Text, cmdId, key, path);
            shortcutItem.ShortcutType = StType.RevitAPI;
            KeyboardShortcutService.applyShortcutChanges(new Dictionary<string, ShortcutItem>()
                { 
                   {
                     cmdId,shortcutItem
                   }
                });
            return true;
        }
    }
}
