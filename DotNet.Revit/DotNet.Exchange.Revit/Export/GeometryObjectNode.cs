using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNet.Exchange.Revit.Export
{
    public class GeometryObjectNode
    {
        public GeometryObject GeometryObject { get; private set; }

        public GeometryObjectNode(GeometryObject geometryObject)
        {
            this.GeometryObject = geometryObject;
        }
    }
}
