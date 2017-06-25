using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNet.Revit.ExternalEvent
{
    public class ExternalEventHelper
    {
        private ExternalEventCommon m_ExternalEventCommon;

        public ExternalEventHelper(UIApplication uiApp)
        {
            m_ExternalEventCommon = new ExternalEventCommon();
        }

        public ExternalEventHelper(UIControlledApplication uiControlApp)
        {
            m_ExternalEventCommon = new ExternalEventCommon();
        }


        class ExternalEventCommon : IExternalEventHandler
        {
            private Action<UIApplication> Action { get; set; }
            private string Name { get; set; }

            public void Execute(UIApplication app)
            {
                if (this.Action == null)
                    return;


                this.Action(app);
            }

            public string GetName()
            {
                return this.Name;
            }
        }
    }

    /// <summary>
    /// 外部事件参数.
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class ExternalEventArg : EventArgs
    {
        public ExternalEventArg(UIApplication app, string name)
        {
            this.App = app;
            this.Name = name;
        }

        public UIApplication App { get; private set; }

        public string Name { get; private set; }
    }
}
