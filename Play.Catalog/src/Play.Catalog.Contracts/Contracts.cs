using System;

namespace Play.Catalog.Contracts
{
    public class Contracts
    {
        public record CatalogItemCreated(Guid ItemId, String Name, String Description);

        public record CatalogItemUpdated(Guid ItemId, String Name, String Description);

        public record CatalogItemDeleted(Guid ItemId);
        
    }
}
