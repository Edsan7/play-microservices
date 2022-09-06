using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Play.Common.Service.Repositories;
using Play.Inventory.Service.Dtos;
using Play.Inventory.Service.Entities;

namespace Play.Inventory.Service.Controllers
{
    [ApiController]
    [Route("items")]
    public class ItemsController : ControllerBase
    {

        private readonly IRepository<InventoryItem> _itemsRepository;

        private readonly IRepository<CatalogItem> _catalogItemsRepository;



        public ItemsController(IRepository<InventoryItem> itemsRepository, IRepository<CatalogItem> catalogItemsRepository)
        {
            this._itemsRepository = itemsRepository;
            _catalogItemsRepository = catalogItemsRepository;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryItemDto>>> GetAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                return BadRequest();


            var inventoryItems = await _itemsRepository.GetAllAsync(x => x.UserId == userId);
            var itemIds = inventoryItems.Select(i => i.CatalogItemId);
            var catalogItems = await _catalogItemsRepository.GetAllAsync(x => itemIds.Contains(x.Id));

            var itemsDtos = inventoryItems.Select(x =>
            {
                var catalogItem = catalogItems.FirstOrDefault(y => y.Id == x.CatalogItemId);

                return x.AsDto(catalogItem.Name, catalogItem.Description);
            });

            return Ok(itemsDtos);
        }

        [HttpPost]
        public async Task<ActionResult> PostAsync(GrantItemsDto grantItemsDto)
        {
            var inventoryItem = await _itemsRepository.GetAsync(x => x.UserId == grantItemsDto.UserId && x.CatalogItemId == grantItemsDto.CatalogItemId);

            if (inventoryItem is null)
            {
                inventoryItem = new InventoryItem
                {
                    CatalogItemId = grantItemsDto.CatalogItemId,
                    UserId = grantItemsDto.UserId,
                    Quantity = grantItemsDto.Quantity,
                    AcquiredDate = DateTimeOffset.Now
                };

                await _itemsRepository.CreateAsync(inventoryItem);
            }
            else
            {
                inventoryItem.Quantity += grantItemsDto.Quantity;
                await _itemsRepository.UpdateAsync(inventoryItem);
            }

            return Ok();


        }

    }
}