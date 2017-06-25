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
        public static bool Invoke(string cmdId)
        {
            if (ExternalCommandHelper.CanExecute(cmdId))
            {
                ExternalCommandHelper.executeExternalCommand(cmdId);
                return true;
            }
            else if (CommandHandlerService.canExecute(cmdId))
            {
                CommandHandlerService.invokeCommandHandler(cmdId);
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
