using System.Collections.Generic;
using GridStepAlternative.Model;

namespace GridStepAlternative.Infrastructure
{
    /// <summary>
    /// Контракт для получения списка сущностей - графов
    /// </summary>
    /// <remarks>
    /// Сущностью может быть, например город, каждый со своим набором маршрутов 
    /// </remarks>
    public interface IEntityService
    {
        List<Entity> GetEntitys();
    }
}