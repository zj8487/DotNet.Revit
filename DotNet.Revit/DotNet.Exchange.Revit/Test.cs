using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Utility;
using System.Diagnostics;
using Autodesk.Revit.ApplicationServices;
using DotNet.Exchange.Revit.Export;
using BIM.RevitAPI.Core.Extensions;
using BIM.RevitAPI.Core.Extensions.Elements;

namespace DotNet.Exchange.Revit
{
    /// <summary>
    /// 自定义导出测试代码。
    /// 备注：自定义导出未经过长时间测试，在此开源是提供一个思路和方法，如果有遇到问题，可联系本人进行讨论.
    /// </summary>
    /// <seealso cref="Autodesk.Revit.UI.IExternalCommand" />
    [Transaction(TransactionMode.Manual)]
    public class Test : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiDoc = commandData.Application.ActiveUIDocument;

            var doc = uiDoc.Document;

            var elem = new ExportElment();
            var export = new ExportFactory(uiDoc.Document, elem);
            export.ExportLevel = 3;
            export.Export();

            doc.Invoke(m =>
            {
                foreach (var item in elem.elemIds)
                {
                    foreach (var item2 in item.TriangleFaces)
                    {
                        var p1 = item.Points[item2.V1];
                        var p2 = item.Points[item2.V2];
                        var p3 = item.Points[item2.V3];

                        doc.Create.NewModelCurve(Line.CreateBound(p1, p2), SketchPlane.Create(uiDoc.Document, this.ToPlane(p1, p2)));
                        doc.Create.NewModelCurve(Line.CreateBound(p2, p3), SketchPlane.Create(uiDoc.Document, this.ToPlane(p2, p3)));
                        doc.Create.NewModelCurve(Line.CreateBound(p3, p1), SketchPlane.Create(uiDoc.Document, this.ToPlane(p3, p1)));
                    }
                }
            });
            return Result.Succeeded;
        }

        private Autodesk.Revit.DB.Plane ToPlane(XYZ point, XYZ other)
        {
            var v = other - point;
            var angle = v.AngleTo(XYZ.BasisX);
            var norm = v.CrossProduct(XYZ.BasisX).Normalize();

            if (Math.Abs(angle - 0) < 1e-4)
            {
                angle = v.AngleTo(XYZ.BasisY);
                norm = v.CrossProduct(XYZ.BasisY).Normalize();
            }

            if (Math.Abs(angle - 0) < 1e-4)
            {
                angle = v.AngleTo(XYZ.BasisZ);
                norm = v.CrossProduct(XYZ.BasisZ).Normalize();
            }
            return new Autodesk.Revit.DB.Plane(norm, point);
        }
    }
}

