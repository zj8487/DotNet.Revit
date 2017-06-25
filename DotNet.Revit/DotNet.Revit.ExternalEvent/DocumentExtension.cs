using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNet.Revit.ExternalEvent
{
    public static class DocumentExtension
    {
        public static void Invoke(this Document doc, Action<Transaction> action, string name = "INVOKE")
        {
            using (var tr = new Transaction(doc, name))
            {
                tr.Start();
                action(tr);
               
                if (tr.GetStatus() == TransactionStatus.Started)
                    tr.Commit();
                else
                    tr.RollBack();
            }
        }
    }
}
