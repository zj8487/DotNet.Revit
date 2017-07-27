using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNet.Exchange.Revit.Export
{
    /// <summary>
    /// 自定义测试导出类.
    /// </summary>
    /// <seealso cref="DotNet.Exchange.Revit.Export.IExportElement" />
    public class ExportElment : IExportElement
    {
        public HashSet<PolygonMeshNode> elemIds = new HashSet<PolygonMeshNode>();

        bool IExportElement.OnElementStart(Autodesk.Revit.DB.Element elem)
        {
          
            return true;
        }

        bool IExportElement.OnGeometryObjectStart(GeometryObjectNode solid)
        {
            return true;
        }

        void IExportElement.OnMaterial(Autodesk.Revit.DB.ElementId materialId)
        {
            
        }

        void IExportElement.OnPolygonMesh(PolygonMeshNode polygonMesh)
        {
            elemIds.Add(polygonMesh);
        }

        void IExportElement.OnGeometryObjectEnd(GeometryObjectNode solid)
        {
            
        }

        void IExportElement.OnElementEnd(Autodesk.Revit.DB.Element elem)
        {
           
        }
    }
}
