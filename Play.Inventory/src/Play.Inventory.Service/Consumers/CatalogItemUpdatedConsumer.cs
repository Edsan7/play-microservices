using System.Threading.Tasks;
using MassTransit;
using Play.Common.Service.Repositories;
using Play.Inventory.Service.Entities;
using static Play.Catalog.Contracts.Contracts;

namespace Play.Inventory.Service.Consumers
{
    public class CatalogItemUpdatedConsumer : IConsumer<CatalogItemUpdated>
    {
        private readonly IRepository<CatalogItem> _repository;

        public CatalogItemUpdatedConsumer(IRepository<CatalogItem> repository)
        {
            _repository = repository;
        }

        public async Task Consume(ConsumeContext<CatalogItemUpdated> context)
        {
            var message = context.Message;

            var item = await _repository.GetAsync(message.ItemId);

            if (item is null)
            {
                item = new CatalogItem
                {
                    Id = message.ItemId,
                    Name = message.Name,
                    Description = message.Description,
                    CreatedDate = System.DateTimeOffset.Now
                };

                await _repository.CreateAsync(item);
                return;
            }

            item.Name = message.Name;
            item.Description = message.Description;

            await _repository.UpdateAsync(item);

        }
    }
}