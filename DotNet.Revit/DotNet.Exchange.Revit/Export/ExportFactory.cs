using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNet.Exchange.Revit.Export
{
    public class ExportFactory
    {
        #region fields
        private uint m_ExportLevel;
        private Document m_Document;
        private IExportElement m_ExportHandle;
        #endregion

        #region properties
        /// <summary>
        /// 导出等级.
        /// 1 - 10；默认等级为5.
        /// </summary>
        public uint ExportLevel
        {
            get
            {
                return m_ExportLevel;
            }
            set
            {
                m_ExportLevel = value;
            }
        }
        #endregion

        #region ctors
        public ExportFactory(Document doc, IExportElement exportHandle)
        {
            if (exportHandle == null || doc == null)
                throw new NullReferenceException("exportHandle or doc parameter is null reference !!");

            m_ExportLevel = 5;
            m_Document = doc;
            m_ExportHandle = exportHandle;
        }
        #endregion

        #region methods
        /// <summary>
        /// 导出指定的元素
        /// </summary>
        /// <param name="elemArray">The elem array.</param>
        public void Export(params Element[] elemArray)
        {
            for (int i = 0; i < elemArray.Length; i++)
            {
                this.ElementExport(elemArray[i]);
            }
        }

        /// <summary>
        /// 导出当前文档的所有元素.
        /// </summary>
        public void Export()
        {
            var view3d = new FilteredElementCollector(m_Document)
                .OfClass(typeof(View3D)).FirstOrDefault(m => (m as View3D).Origin != null);

            if (view3d == null)
                throw new NullReferenceException();

            var elems = new FilteredElementCollector(m_Document, view3d.Id).WhereElementIsNotElementType();

            var eum = elems.GetElementIterator();
            while (eum.MoveNext())
            {
                this.ElementExport(eum.Current);
            }
        }

        /// <summary>
        /// 异步导出指定元素.
        /// </summary>
        public void AsynchExport(params Element[] elemArray)
        {
            Task.Run(() => this.Export(elemArray));
        }

        /// <summary>
        /// 异步导出当前文档所有元素. 
        /// </summary>
        public void AsynchExport()
        {
            Task.Run(() => this.Export());
        }

        #endregion

        #region privates

        /// <summary>
        /// Elements the export.
        /// </summary>
        private bool ElementExport(Element elem)
        {
            if (!m_ExportHandle.OnElementStart(elem))
            {
                return false;
            }

            var transform = default(Transform);
            if (elem is Instance)
            {
                transform = (elem as Instance).GetTransform();
            }


            var objects = this.GetGeometryObject(elem);
            if (objects.Count == 0)
            {
                return false;
            }

            while (objects.Count > 0)
            {
                var obj = objects.Pop();

                var node = new GeometryObjectNode(obj);
                if (!m_ExportHandle.OnGeometryObjectStart(node))
                {
                    return false;
                }

                if (obj.GetType().Equals(typeof(Solid)))
                {
                    var solid = obj as Solid;

                    var faces = solid.Faces;

                    var eum = faces.GetEnumerator();
                    while (eum.MoveNext())
                    {
                        var face = (Face)eum.Current;

                        // not handle....
                        m_ExportHandle.OnMaterial(face.MaterialElementId);

                        var polygonMeshNode = this.FaceConvert(face, transform);

                        m_ExportHandle.OnPolygonMesh(polygonMeshNode);
                    }

                }
                m_ExportHandle.OnGeometryObjectEnd(node);
            }

            m_ExportHandle.OnElementEnd(elem);
            return true;
        }


        /// <summary>
        /// Faces the convert.
        /// </summary>
        private PolygonMeshNode FaceConvert(Face face, Transform transform)
        {
            var result = new PolygonMeshNode();

            var mesh = face.Triangulate(m_ExportLevel * 0.1);
            var temps = new Dictionary<int, XYZ>();

            // resolve triangle face..
            for (int i = 0; i < mesh.NumTriangles; i++)
            {
                var triangle = mesh.get_Triangle(i);

                var pt1 = triangle.get_Vertex(0);
                var pt2 = triangle.get_Vertex(1);
                var pt3 = triangle.get_Vertex(2);

                var pi1 = (int)triangle.get_Index(0);
                var pi2 = (int)triangle.get_Index(1);
                var pi3 = (int)triangle.get_Index(2);

                if (!temps.ContainsKey(pi1))
                {
                    temps.Add(pi1, pt1);
                }

                if (!temps.ContainsKey(pi2))
                {
                    temps.Add(pi2, pt2);
                }

                if (!temps.ContainsKey(pi3))
                {
                    temps.Add(pi3, pt3);
                }

                result.TriangleFaces.Add(new TriangleFaceNode(pi1, pi2, pi3));
            }

            var eum = temps.OrderBy(m => m.Key).GetEnumerator();
            while (eum.MoveNext())
            {
                result.Points.Add(eum.Current.Value);
            }


            // resolve point and uv..
            foreach (EdgeArray edgeArray in face.EdgeLoops)
            {
                foreach (Edge item in edgeArray)
                {
                    result.AddUV(item.EvaluateOnFace(0, face));
                    result.AddUV(item.EvaluateOnFace(1, face));
                }
            }

            // resolve normal..

            if (face.GetType().Equals(typeof(PlanarFace)))
            {
                var planarFace = face as PlanarFace;
                result.AddNormal(planarFace.FaceNormal);
            }
            else
            {
                foreach (var item in result.TriangleFaces)
                {
                    var normal = this.GetNormal(result.Points[item.V1], result.Points[item.V2], result.Points[item.V3]);
                    result.AddNormal(normal);
                }
            }

            if (transform != null)
            {
                for (int i = 0; i < result.Points.Count; i++)
                {
                    result.Points[i] = transform.OfPoint(result.Points[i]);
                }

                for (int i = 0; i < result.Normals.Count; i++)
                {
                    result.Normals[i] = transform.OfVector(result.Normals[i]);
                }
            }
            return result;
        }

        public Autodesk.Revit.DB.Plane ToPlane(XYZ point, XYZ other)
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

        /// <summary>
        /// Gets the normal.
        /// </summary>
        private XYZ GetNormal(XYZ p1, XYZ p2, XYZ p3)
        {
            var v1 = p2 - p1;
            var v2 = p3 - p1;
            return v1.CrossProduct(v2).Normalize();
        }

        /// <summary>
        /// Gets the geometry object.
        /// </summary>
        private Stack<GeometryObject> GetGeometryObject(Element elem)
        {
            var geometryElement = elem.get_Geometry(new Options() { DetailLevel = ViewDetailLevel.Fine, ComputeReferences = true });
            var objects = new List<GeometryObject>();
            this.RecursionObject(geometryElement, ref objects);

            return new Stack<GeometryObject>(objects);
        }

        /// <summary>
        /// Recursions the object.
        /// </summary>
        private void RecursionObject(GeometryElement geometryElement, ref List<GeometryObject> geometryObjects)
        {
            if (geometryElement == null)
                return;

            var eum = geometryElement.GetEnumerator();
            while (eum.MoveNext())
            {
                var current = eum.Current;
                var type = current.GetType();

                if (type.Equals(typeof(GeometryInstance)))
                {
                    this.RecursionObject((current as GeometryInstance).SymbolGeometry, ref geometryObjects);
                }
                else if (type.Equals(typeof(GeometryElement)))
                {
                    this.RecursionObject(current as GeometryElement, ref geometryObjects);
                }
                else
                {
                    if (type.Equals(typeof(Solid)))
                    {
                        var solid = current as Solid;
                        if (solid.Edges.Size == 0 || solid.Faces.Size == 0)
                        {
                            continue;
                        }
                    }
                    geometryObjects.Add(current);
                }
            }
        }

        #endregion
    }
}
