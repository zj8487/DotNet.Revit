using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNet.Exchange.Revit.Export
{
    public class PolygonMeshNode
    {
        public PolygonMeshNode()
        {
            this.Points = new List<XYZ>();
            this.TriangleFaces = new List<TriangleFaceNode>();
            this.Normals = new List<XYZ>();
            this.UVs = new List<UV>();
        }

        public List<XYZ> Points { get; private set; }
        public List<TriangleFaceNode> TriangleFaces { get; private set; }
        public List<XYZ> Normals { get; private set; }
        public List<UV> UVs { get; private set; }

        internal void AddPoint(XYZ point)
        {
            var flag = this.Points.FirstOrDefault(m => m.IsAlmostEqualTo(point, 1e-6));
            if (flag == null)
            {
                this.Points.Add(point);
            }
        }

        internal void AddUV(UV uv)
        {
            var flag = this.UVs.FirstOrDefault(m => m.IsAlmostEqualTo(uv, 1e-6));
            if (flag == null)
            {
                this.UVs.Add(uv);
            }
        }

        internal void AddNormal(XYZ normal)
        {
            var flag = this.Normals.FirstOrDefault(m => m.IsAlmostEqualTo(normal, 1e-6));
            if (flag == null)
            {
                this.Normals.Add(normal);
            }
        }
    }
}
