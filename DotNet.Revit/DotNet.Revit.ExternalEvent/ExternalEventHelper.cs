using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNet.Revit.ExternalEvent
{
    /// <summary>
    /// 外部事件的封装.
    /// </summary>
    public class ExternalEventHelper
    {
        #region fields
        private ExternalEventHandlerCommon externalEventHandlerCommon;
        private Autodesk.Revit.UI.ExternalEvent externalEvent;
        #endregion

        #region events
        /// <summary>
        /// 外部事件刚刚开始并且准备执行时触发.
        /// </summary>
        public event EventHandler<ExternalEventArg> Started;

        /// <summary>
        /// 外部事件结束时触发.
        /// </summary>
        public event EventHandler<ExternalEventArg> End;
        #endregion

        #region ctors
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
        #endregion

        #region methods
        public void Invoke(Action<UIApplication> action, string name = "")
        {
            var nf = string.IsNullOrWhiteSpace(name) ? Guid.NewGuid().ToString() : name;
            this.externalEventHandlerCommon.Actions.Enqueue(new KeyValuePair<string, Action<UIApplication>>(nf, action));
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
        #endregion

        #region nestedClasss
        class ExternalEventHandlerCommon : IExternalEventHandler
        {
            internal Queue<KeyValuePair<string, Action<UIApplication>>> Actions { get; set; }

            public event EventHandler<ExternalEventArg> Started;
            public event EventHandler<ExternalEventArg> End;

            internal ExternalEventHandlerCommon()
            {
                this.Actions = new Queue<KeyValuePair<string, Action<UIApplication>>>();
            }

            public void Execute(UIApplication app)
            {
                while (this.Actions.Count > 0)
                {
                    var first = this.Actions.Dequeue();

                    if (string.IsNullOrWhiteSpace(first.Key) || first.Value == null)
                        continue;

                    try
                    {
                        if (this.Started != null)
                            this.Started(this, new ExternalEventArg(app, first.Key));

                        first.Value(app);
                    }
                    finally
                    {
                        this.End(this, new ExternalEventArg(app, first.Key));
                    }
                }
            }

            public string GetName()
            {
                return "";
            }
        }
        #endregion
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
