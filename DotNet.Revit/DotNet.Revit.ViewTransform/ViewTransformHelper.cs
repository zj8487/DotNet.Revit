using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNet.Revit.ViewTransform
{
    public static class ViewTransformHelper
    {
        /// <summary>
        /// 屏幕点转为空间三维点.
        /// </summary>
        /// <param name="view">指定转换到的视图.</param>
        /// <param name="point">指定屏幕上的点.</param>
        /// <returns></returns>
        public static XYZ ScreenToWorld(this View view, System.Drawing.Point point)
        {
            if (view == null)
                throw new NullReferenceException();

            var uiDoc = new UIDocument(view.Document);
            var uiView = uiDoc.GetOpenUIViews().FirstOrDefault(m => m.ViewId == view.Id);

            if (uiView == null)
                throw new NullReferenceException();

            var rect = uiView.GetWindowRectangle();
            var corners = uiView.GetZoomCorners();

            var mousePoint = new XYZ(point.X, point.Y, 0);

            var screenLeftlower = new XYZ(rect.Left, rect.Bottom, 0);
            var screenRightupper = new XYZ(rect.Right, rect.Top, 0);

            // 换算比例
            var scale = corners[0].DistanceTo(corners[1])
                / screenLeftlower.DistanceTo(screenRightupper);

            var xdis = (point.X - screenLeftlower.X) * scale;
            var ydis = (screenLeftlower.Y - point.Y) * scale;

            var vp = uiDoc.ActiveView.UpDirection;
            var vr = uiDoc.ActiveView.RightDirection;

            var distance = mousePoint.DistanceTo(screenLeftlower) * scale;
            var result = corners[0] + vr * xdis + vp * ydis;

            return result;
        }
    }
}
