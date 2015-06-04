using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Tracing;

namespace webapitmpl.Providers
{
    public class TraceWriter : ITraceWriter
    {
        public void Trace(HttpRequestMessage request, string category, TraceLevel level, Action<TraceRecord> traceAction)
        {
            if (level >= TraceLevel.Info)
            {
                TraceRecord rec = new TraceRecord(request, category, level);
                traceAction(rec);

                var items = new List<string>();
                items.Add(G(rec.Kind));

                if (!String.IsNullOrEmpty(rec.Operator))
                {
                    items.Add(String.Format("{0}::{1}", rec.Operator, rec.Operation));
                }

                if (rec.Message != null)
                {
                    items.Add(rec.Message);
                }

                Console.WriteLine(String.Join(" ", items));
            }
        }

        private string G(TraceKind kind)
        {
            switch (kind)
            {
                case TraceKind.Begin:
                    return "BEGIN";

                case TraceKind.End:
                    return "END  ";

                default:
                case TraceKind.Trace:
                    return String.Empty;
            }
        }
    }
}
