using System.Collections.Generic;
using System.Linq;
using System.Xml;
using GridStepAlternative.Infrastructure;

namespace GridStepAlternative.DataService
{
    public class СhainService : IСhainService
    {
        private readonly string _source;

        public СhainService(string source)
        {
            _source = source;
        }

        public List<int> GetСhainByEntity(int entityId)
        {
            // TODO: Реализуйте получение списка цепочек

            var xDoc = new XmlDocument();
            xDoc.Load($@"{_source}\Chains_{entityId}.xml");
            var xRoot = xDoc.DocumentElement;

            if (xRoot == null)
                return null;

            return (from XmlNode xnode in xRoot
                select int.Parse(xnode.ChildNodes[0].InnerText)).ToList();
        }
    }
}