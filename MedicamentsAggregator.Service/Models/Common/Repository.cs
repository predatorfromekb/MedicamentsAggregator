using System;
using System.Linq;
using MedicamentsAggregator.Service.DataLayer;

namespace MedicamentsAggregator.Service.Models.Common
{
    public class Repository
    {
        private readonly MedicamentsAggregatorContextFactory _medicamentsAggregatorContextFactory;

        public Repository(MedicamentsAggregatorContextFactory medicamentsAggregatorContextFactory)
        {
            _medicamentsAggregatorContextFactory = medicamentsAggregatorContextFactory;
        }

        public EntityContainer<T> AddOrUpdate<T>(T[] entities, Action<T,T> update) where T : class, IFixedIdEntity
        {
            using (var context = _medicamentsAggregatorContextFactory.CreateContext())
            {
                var entitiesFromDatabase = GetEntitiesFromDatabase(entities, context);
                
                var entitiesToInsert = GetEntitiesToInsert(entities, entitiesFromDatabase);
                context.AddRange(entitiesToInsert);
                
                var entitiesToUpdateFromDb = GetEntitiesToUpdate(entities, entitiesFromDatabase, update);
                context.UpdateRange(entitiesToUpdateFromDb);
                
                context.SaveChanges();
                return new EntityContainer<T>
                {
                    InsertedEntities = entitiesToInsert,
                    UpdatedEntities = entitiesToUpdateFromDb,
                };
            }
        }
        
        private T[] GetEntitiesFromDatabase<T>(T[] entities, MedicamentsAggregatorContext context) where T : class, IFixedIdEntity
        {
            var ids = entities.Select(e => e.Id).ToArray();
                
            return context.Set<T>()
                .Where(e => ids.Contains(e.Id))
                .ToArray();
        }

        private T[] GetEntitiesToInsert<T>(T[] entities, T[] entitiesFromDatabase) where T : class, IFixedIdEntity
        {
            var now = DateTime.Now;
            
            return entities
                .Except<T>(entitiesFromDatabase, EntityEqualityComparer.Instance)
                .Select(entity => {
                    entity.UpdatedDate = now;
                    return entity;
                })
                .ToArray();
        }
        
        private T[] GetEntitiesToUpdate<T>(T[] entities, T[] entitiesFromDatabase, Action<T,T> update) where T : class, IFixedIdEntity
        {
            var now = DateTime.Now;
            var dayBefore = DateTime.Now.AddDays(-1);
            
            var obsoleteEntitiesFromDatabase = entitiesFromDatabase
                .Where(e => e.UpdatedDate <= dayBefore)
                .ToArray();

            var entitiesToUpdate = entities
                .Intersect<T>(obsoleteEntitiesFromDatabase, EntityEqualityComparer.Instance)
                .ToDictionary(e => e.Id, e => e);
            
            return obsoleteEntitiesFromDatabase
                .Intersect<T>(entities, EntityEqualityComparer.Instance)
                .Select(entity => {
                    update(entity, entitiesToUpdate[entity.Id]);
                    entity.UpdatedDate = now;
                    return entity;
                })
                .ToArray();
        }
    }
}