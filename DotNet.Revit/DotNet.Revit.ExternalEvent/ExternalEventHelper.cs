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
        private ExternalEventHandlerCommon externalEventHandlerCommon;
        private Autodesk.Revit.UI.ExternalEvent externalEvent;

        /// <summary>
        /// 外部事件刚刚开始并且准备执行时触发.
        /// </summary>
        public event EventHandler<ExternalEventArg> Started;

        /// <summary>
        /// 外部事件结束时触发.
        /// </summary>
        public event EventHandler<ExternalEventArg> End;

        public ExternalEventHelper(UIApplication uiApp)
        {
            this.externalEventHandlerCommon = new ExternalEventHandlerCommon();
            this.externalEvent = Autodesk.Revit.UI.ExternalEvent.Create(this.externalEventHandlerCommon);

            this.externalEventHandlerCommon.Started += externalEventCommon_Started;
            this.externalEventHandlerCommon.End += externalEventCommon_End;
        }

        public ExternalEventHelper(UIControlledApplication uiControlApp)
        {
            this.externalEventHandlerCommon = new ExternalEventHandlerCommon();
            this.externalEvent = Autodesk.Revit.UI.ExternalEvent.Create(this.externalEventHandlerCommon);

            this.externalEventHandlerCommon.Started += externalEventCommon_Started;
            this.externalEventHandlerCommon.End += externalEventCommon_End;
        }


        public void Invoke(Action<UIApplication> action, string name = "")
        {
            this.externalEventHandlerCommon.Name = string.IsNullOrWhiteSpace(name) ? Guid.NewGuid().ToString() : name;
            this.externalEventHandlerCommon.Action = action;
            this.externalEvent.Raise();
        }

        private void externalEventCommon_End(object sender, ExternalEventArg e)
        {
            if (this.End != null)
                this.End(this, e);
        }

        private void externalEventCommon_Started(object sender, ExternalEventArg e)
        {
            if (this.Started != null)
                this.Started(this, e);
        }

        class ExternalEventHandlerCommon : IExternalEventHandler
        {
            internal Action<UIApplication> Action { get; set; }
            internal string Name { get; set; }

            public event EventHandler<ExternalEventArg> Started;
            public event EventHandler<ExternalEventArg> End;

            public void Execute(UIApplication app)
            {
                if (this.Action == null)
                    return;
                try
                {
                    if (this.Started != null)
                        this.Started(this, new ExternalEventArg(app, this.Name));

                    this.Action(app);
                }
                finally
                {
                    this.End(this, new ExternalEventArg(app, this.Name));
                }
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
