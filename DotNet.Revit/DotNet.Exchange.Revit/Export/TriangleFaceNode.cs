using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNet.Exchange.Revit.Export
{
    public sealed class TriangleFaceNode
    {
        internal TriangleFaceNode(int v1, int v2, int v3)
        {
            this.V1 = (int)v1;
            this.V2 = (int)v2;
            this.V3 = (int)v3;
        }

        public int V1 { get; private set; }
        public int V2 { get; private set; }
        public int V3 { get; private set; }
    }
}
