using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNet.Exchange.Revit
{
    public class ExportContext : IExportContext
    {
        public HashSet<Tuple<XYZ, XYZ, XYZ>> elemIds = new HashSet<Tuple<XYZ, XYZ, XYZ>>();

        public bool IsCanceled()
        {
            return false;
        }

        public void OnDaylightPortal(DaylightPortalNode node)
        {

        }

        public RenderNodeAction OnElementBegin(ElementId elementId)
        {
            return RenderNodeAction.Proceed;
        }

        public void OnElementEnd(ElementId elementId)
        {

        }

        public RenderNodeAction OnFaceBegin(FaceNode node)
        {
            return RenderNodeAction.Proceed;
        }

        public void OnFaceEnd(FaceNode node)
        {

        }

        public RenderNodeAction OnInstanceBegin(InstanceNode node)
        {
            return RenderNodeAction.Proceed;
        }

        public void OnInstanceEnd(InstanceNode node)
        {

        }

        public void OnLight(LightNode node)
        {

        }

        public RenderNodeAction OnLinkBegin(LinkNode node)
        {
            return RenderNodeAction.Proceed;
        }

        public void OnLinkEnd(LinkNode node)
        {

        }

        public void OnMaterial(MaterialNode node)
        {

        }

        public void OnPolymesh(PolymeshTopology node)
        {
            foreach (var item2 in node.GetFacets())
            {
                var p1 = node.GetPoint(item2.V1);
                var p2 = node.GetPoint(item2.V2);
                var p3 = node.GetPoint(item2.V3);

                elemIds.Add(new Tuple<XYZ, XYZ, XYZ>(p1, p2, p3));
            }
        }

        public void OnRPC(RPCNode node)
        {

        }

        public RenderNodeAction OnViewBegin(ViewNode node)
        {
            return RenderNodeAction.Proceed;
        }

        public void OnViewEnd(ElementId elementId)
        {

        }

        public bool Start()
        {
            return true;
        }

        public void Finish()
        {

        }
    }
}
