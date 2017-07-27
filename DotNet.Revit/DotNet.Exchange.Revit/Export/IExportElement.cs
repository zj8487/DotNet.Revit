using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNet.Exchange.Revit.Export
{
    /// <summary>
    /// IExportElement
    /// </summary>
    public interface IExportElement
    {
        /// <summary>
        /// 元素开始进行导出时执行.
        /// 如果返回结果为true，则开始进行此元素导出，否则放弃此元素导出.
        /// </summary>
        bool OnElementStart(Element elem);

        /// <summary>
        /// 当元素内的GeometryObject进行导出时执行.
        /// 如果返回结果为true，则开始进行此元素导出，否则放弃此元素导出.
        /// </summary>
        bool OnGeometryObjectStart(GeometryObjectNode geometryObject);

        /// <summary>
        /// 开始检索多边形的材质.
        /// </summary>
        void OnMaterial(ElementId materialId);

        /// <summary>
        /// 开始检索Solid内的多边形拓扑节点.
        /// </summary>
        void OnPolygonMesh(PolygonMeshNode polygonMesh);

        /// <summary>
        /// 当元素内的GeometryObject导出结束时执行.
        /// </summary>
        void OnGeometryObjectEnd(GeometryObjectNode geometryObject);

        /// <summary>
        /// 当元素导出结束时开始执行.
        /// </summary>
        void OnElementEnd(Element elem);
    }
}
