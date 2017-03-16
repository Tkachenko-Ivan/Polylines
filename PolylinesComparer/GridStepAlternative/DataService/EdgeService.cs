using System.Collections.Generic;
using System.Linq;
using System.Xml;
using GridStepAlternative.Infrastructure;
using PolylinesComparer.Model;

namespace GridStepAlternative.DataService
{
    public class EdgeService : IEdgeService
    {
        private readonly string _source;

        public EdgeService(string source)
        {
            _source = source;
        }

        public List<Coordinate> GetCoord(int nodeStart, int nodeEnd, int chainId, int entityId)
        {
            // TODO: Реализуйте получение координат ребра между двумя последовательно идущими вершинами одной цепочки

             var xDoc = new XmlDocument();
             xDoc.Load($@"{_source}\Chains_{entityId}.xml");
             var xRoot = xDoc.DocumentElement;

             if (xRoot == null)
                 return null;

             foreach (XmlNode xnode in xRoot)
             {
                 if (int.Parse(xnode.ChildNodes[0].InnerText) != chainId)
                     continue;

                 foreach (XmlNode node in xnode.ChildNodes[2].ChildNodes)
                 {
                     if (int.Parse(node.ChildNodes[0].InnerText) != nodeStart ||
                         int.Parse(node.ChildNodes[1].InnerText) != nodeEnd)
                         continue;

                     return (from XmlNode coord in node.ChildNodes[2].ChildNodes
                         select
                         new Coordinate(
                             double.Parse(coord.ChildNodes[0].InnerText),
                             double.Parse(coord.ChildNodes[1].InnerText)
                         )
                     ).ToList();
                 }
                 break;
             }

             return null;
        }
    }
}