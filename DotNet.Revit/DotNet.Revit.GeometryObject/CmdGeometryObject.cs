using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI.Selection;
using DotNet.Revit;

namespace DotNet.Revit.GeometryObject
{
    
    [Transaction(TransactionMode.Manual)]
    public class CmdGeometryObject : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiDoc = commandData.Application.ActiveUIDocument;
            var doc = uiDoc.Document;

            try
            {
                // 选取一个门
                var refer = uiDoc.Selection.PickObject(ObjectType.Element,
                    new SelectionFilterCommon(m => m.Category.Id.IntegerValue == (int)BuiltInCategory.OST_Doors));

                var elem = doc.GetElement(refer);
                var geomObjects = elem.GetGeometryObjects(new Options() { IncludeNonVisibleObjects = true });

                // 获取门的所有有效Solid

                var solids = geomObjects.OfType<Solid>();
            }
            catch
            {
                return Result.Failed;
            }
            return Result.Succeeded;
        }
    }

    /// <summary>
    /// 通用元素交互选择过滤器.
    /// </summary>
    /// <seealso cref="Autodesk.Revit.UI.Selection.ISelectionFilter"/>
    public class SelectionFilterCommon : ISelectionFilter
    {
        private Func<Element, bool> m_Func;
        private Func<Reference, XYZ, bool> m_Func2;

        public SelectionFilterCommon(Func<Element, bool> match, Func<Reference, XYZ, bool> func2)
        {
            m_Func = match;
            m_Func2 = func2;
        }

        public SelectionFilterCommon(Func<Element, bool> match)
            : this(match, null)
        {

        }

        public bool AllowElement(Element elem)
        {
            return m_Func == null ? true : m_Func(elem);
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return m_Func2 == null ? true : m_Func2(reference, position);
        }
    }
}
