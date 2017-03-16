using System.Collections.Generic;
using System.Linq;
using System.Xml;
using GridStepAlternative.Infrastructure;

namespace GridStepAlternative.DataService
{
    public class NodeService : INodeService
    {
        private readonly string _source;

        public NodeService(string source)
        {
            _source = source;
        }

        public List<int> GetNodesByEntity(int entityId)
        {
            // TODO: Реализуйте получение вершин сущности

             var xDoc = new XmlDocument();
             xDoc.Load($@"{_source}\Entitys.xml");
             var xRoot = xDoc.DocumentElement;

             if (xRoot == null)
                 return null;

             return (from XmlNode xnode in xRoot
                 where int.Parse(xnode.ChildNodes[0].InnerText) == entityId
                 select (from XmlNode nodes in xnode.ChildNodes[3].ChildNodes
                     select int.Parse(nodes.ChildNodes[0].InnerText)).ToList()).FirstOrDefault();
        }

        public List<int> GetNodesByChain(int entityId, int chainId)
        {
            // TODO: Реализуйте получение вершин цепочки, сортированных в порядке следования

             var xDoc = new XmlDocument();
             xDoc.Load($@"C:\Users\Иван\Desktop\Data\Chains_{entityId}.xml");
             var xRoot = xDoc.DocumentElement;

             if (xRoot == null)
                 return null;

             var result = new List<int>();
             foreach (XmlNode xnode in xRoot)
             {
                 if (int.Parse(xnode.ChildNodes[0].InnerText) != chainId)
                     continue;

                 foreach (XmlNode node in xnode.ChildNodes[2].ChildNodes)
                 {
                     result.Add(int.Parse(node.ChildNodes[0].InnerText));
                     result.Add(int.Parse(node.ChildNodes[1].InnerText));
                 }
                 break;
             }
             return result.Distinct().ToList();
        }
    }
}