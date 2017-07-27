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

            var refer = uiDoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element);

            var elem = doc.GetElement(refer);


            var export = new ExportElment();
            var exportFactory = new ExportFactory(doc, export);
            exportFactory.ExportLevel = 3;
            exportFactory.Export(elem);

            // 绘制测试，因绘制线速度较慢，所以当需要绘制测试时，请测试少量模型

            doc.Invoke(m =>
            {
                foreach (var polygonMesh in export.PolygonMeshNodes)
                {
                    foreach (var triangleFaces in polygonMesh.TriangleFaces)
                    {
                        var p1 = polygonMesh.Points[triangleFaces.V1];
                        var p2 = polygonMesh.Points[triangleFaces.V2];
                        var p3 = polygonMesh.Points[triangleFaces.V3];

                        CreateModelLine(doc, p1, p2);
                        CreateModelLine(doc, p2, p3);
                        CreateModelLine(doc, p3, p1);

                    }
                }
            });
            return Result.Succeeded;
        }

        private void CreateModelLine(Document doc, XYZ p1, XYZ p2)
        {
            using (var line = Line.CreateBound(p1, p2))
            {
                using (var skPlane = SketchPlane.Create(doc, this.ToPlane(p1, p2)))
                {
                    if (doc.IsFamilyDocument)
                    {
                        doc.FamilyCreate.NewModelCurve(line, skPlane);
                    }
                    else
                    {
                        doc.Create.NewModelCurve(line, skPlane);
                    }
                }
            }
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

    public static class DocumentExtension
    {
        /// <summary>
        /// 使用委托启动事务.事务内自动进行事务启动，提交、回滚等处理。
        /// </summary>
        /// <param name="doc">The document.</param>
        /// <param name="action">The action.</param>
        /// <param name="name">The name.</param>
        public static void Invoke(this Document doc, Action<Transaction> action, string name = "default")
        {
            using (var tr = new Transaction(doc, name))
            {
                tr.Start();

                action(tr);

                var status = tr.GetStatus();
                switch (status)
                {
                    case TransactionStatus.Started:
                        tr.Commit();
                        return;
                    case TransactionStatus.Committed:
                    case TransactionStatus.RolledBack:
                        break;
                    case TransactionStatus.Error:
                        tr.RollBack();
                        return;
                    default:
                        return;
                }
            }
        }
    }
}

