using System.Collections.Generic;
using System.Linq;
using System.Xml;
using GridStepAlternative.Infrastructure;
using GridStepAlternative.Model;
using PolylinesComparer.Model;

namespace GridStepAlternative.DataService
{
    public class EntityService : IEntityService
    {
        private readonly string _source;

        public EntityService(string source)
        {
            _source = source;
        }

        public List<Entity> GetEntitys()
        {
            // TODO: Реализуйте получение списка сущностей - графов

             var xDoc = new XmlDocument();
             xDoc.Load($@"{_source}\Entitys.xml");
             var xRoot = xDoc.DocumentElement;
 
             if (xRoot == null)
                 return null;
 
             return (from XmlNode xnode in xRoot
                 select new Entity
                 {
                     Id = int.Parse(xnode.ChildNodes[0].InnerText),
                     Name = xnode.ChildNodes[1].InnerText,
                     Center = new Coordinate(
                         double.Parse(xnode.ChildNodes[2].ChildNodes[0].InnerText),
                         double.Parse(xnode.ChildNodes[2].ChildNodes[1].InnerText)
                     )
                 }).ToList();
        }
    }
}