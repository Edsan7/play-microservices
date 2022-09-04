using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Driver;
using Play.Common.Service.Entities;

namespace Play.Common.Service.Repositories
{

    public class MongoRepository<T> : IRepository<T> where T : IEntity
    {
        private readonly IMongoCollection<T> dbCollection;
        private readonly FilterDefinitionBuilder<T> filteBuilder = Builders<T>.Filter;

        public MongoRepository(IMongoDatabase database, string collectionName)
        {
            dbCollection = database.GetCollection<T>(collectionName);
        }

        public async Task<IReadOnlyCollection<T>> GetAllAsync()
        {
            return await dbCollection.Find(filteBuilder.Empty).ToListAsync();
        }

        public async Task<IReadOnlyCollection<T>> GetAllAsync(Expression<Func<T, bool>> expression)
        {
            return await dbCollection.Find(expression).ToListAsync();
        }

        public async Task<T> GetAsync(Guid id)
        {
            return await dbCollection.Find(filteBuilder.Eq(x => x.Id, id)).FirstOrDefaultAsync();
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>> expression)
        {
            return await dbCollection.Find(expression).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(T entity)
        {
            if (entity is null)
                throw new ArgumentNullException(nameof(entity));

            await dbCollection.InsertOneAsync(entity);
        }

        public async Task UpdateAsync(T entity)
        {
            if (entity is null)
                throw new ArgumentNullException(nameof(entity));

            await dbCollection.ReplaceOneAsync(filteBuilder.Eq(x => x.Id, entity.Id), entity);
        }


        public async Task RemoveAsync(Guid id)
        {
            await dbCollection.DeleteOneAsync(filteBuilder.Eq(x => x.Id, id));
        }

    }
}