using CloudCore.Domain.Entities;

namespace CloudCore.Services.Interfaces.IRepositories
{
    /// <summary>
    /// Repository for managing file and folder items in the database
    /// </summary>
    public interface IItemRepository
    {
        #region Retrieval - Single Item

        /// <summary>
        /// Asynchronously retrieves a single item by its ID, ensuring it belongs to the specified user.
        /// </summary>
        /// <param name="itemtId">The ID of the item to retrieve.</param>
        /// <param name="userId">The ID of the user who owns the item.</param>
        /// <param name="itemType">The Type of the item to retrieve.</param>
        /// <returns>A Task that resolves to the Item object if found; otherwise, null.</returns>
        /// <param name="cancellationToken"></param>
        Task<Item?> GetItemAsync(int itemtId, int userId, string? itemType, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves an item by its name within a specific parent folder for a user
        /// </summary>
        /// <param name="userId">The ID of the user who owns the item</param>
        /// <param name="name">The name of the item to find</param>
        /// <param name="parentId">The ID of the parent folder (null for root)</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The Item object if found, otherwise null</returns>
        /// <param name="teamspaceId">Optional teamspace ID to filter by</param>
        Task<Item?> GetItemByNameAsync(int userId, string name, int? parentId, CancellationToken cancellationToken, int? teamspaceId = null);

        /// <summary>
        /// Retrieves a deleted item by its ID for the specified user.
        /// Returns null if not found or not marked as deleted.
        /// </summary>
        /// <param name="userId">The user who owns the item.</param>
        /// <param name="itemId">The ID of the item to retrieve.</param>
        /// <param name="cancellationToken">Token to cancel the operation if client disconnects</param>
        /// <returns>The deleted Item, or null if not found.</returns>
        Task<Item?> GetDeletedItemAsync(int userId, int itemId, CancellationToken cancellationToken);

        #endregion

        #region Retrieval - Multiple Items

        /// <summary>
        /// Retrieves all child items (files and folders) under a specified parent folder.
        /// </summary>
        /// <param name="userId">The user ID to filter items by</param>
        /// <param name="parentId">The ID of the parent folder to search under</param>
        /// <param name="cancellationToken">Token to cancel the operation if client disconnects</param>
        /// <param name="maxDepth">The maximal depth to search by</param>
        /// <returns>An async enumerable of all child items found recursively under the parent folder</returns>
        IAsyncEnumerable<Item> GetAllChildItemsAsync(int userId, int parentId, CancellationToken cancellationToken, int maxDepth = 10000);

        /// <summary>
        /// Retrieves immediate children of a folder without recursion
        /// </summary>
        /// <param name="userId">The user ID to filter items by</param>
        /// <param name="parentId">The ID of the parent folder (null for root)</param>
        /// <param name="cancellationToken">Token to cancel the operation if client disconnects</param>
        /// <param name="itemType">Optional filter by item type ("file" or "folder")</param>
        /// <returns>An async enumerable of direct child items</returns>
        /// <param name="includeDeleted">If true, includes soft-deleted items</param>
        IAsyncEnumerable<Item> GetDirectChildrenAsync(int userId, int? parentId, CancellationToken cancellationToken, string? itemType = null, bool includeDeleted = false);

        /// <summary>
        /// Asynchronously retrieves a paginated IEnumerable of items for a specific user, with options for filtering and sorting.
        /// </summary>
        /// <param name="userId">The ID of the user whose items are being requested.</param>
        /// <param name="parentId">The ID of the parent folder to filter by. Use null to retrieve items from the root.</param>
        /// <param name="page">The page number for pagination (starting from 1).</param>
        /// <param name="pageSize">The number of items to include on each page.</param>
        /// <param name="cancellationToken">Token to cancel the operation if client disconnects</param>
        /// <param name="sortBy">The field to sort the items by (e.g., "name", "createdAt"). Defaults to "name".</param>
        /// <param name="sortDir">The sort direction ("asc" for ascending, "desc" for descending). Defaults to "asc".</param>
        /// <param name="IsTrashFolder">A flag indicating whether to fetch items from the trash (where IsDeleted is true).</param>
        /// <param name="searchQuery">An optional search query to filter items by name.</param>
        /// <returns>A Task that resolves to a PaginatedResponse containing the list of items and pagination metadata.</returns>
        /// <param name="teamspaceId">An optional teamspace to filter items by it.</param>
        Task<(IEnumerable<Item> Items, int TotalCount)> GetItemsAsync(int userId, int? parentId, int page, int pageSize, CancellationToken cancellationToken, string? sortBy = "name", string? sortDir = "asc", bool IsTrashFolder = false, string? searchQuery = null, int? teamspaceId = null);

        /// <summary>
        /// Asynchronously retrieves multiple items by their IDs, ensuring they belong to the specified user.
        /// </summary>
        /// <param name="userId">The ID of the user who owns the items.</param>
        /// <param name="itemsIds">The IDs of the items to retrieve.</param>
        /// <param name="cancellationToken">Token to cancel the operation if client disconnects</param>
        /// <returns>An async enumerable of items matching the provided IDs.</returns>
        IAsyncEnumerable<Item> GetItemsByIdsForUserAsync(int userId, List<int> itemsIds, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously retrieves multiple deleted items by their IDs.
        /// </summary>
        /// <param name="itemsIds">The IDs of the items to retrieve.</param>
        /// <param name="cancellationToken">Token to cancel the operation if client disconnects</param>
        /// <returns>An enumerable of deleted items matching the provided IDs.</returns>
        Task<IEnumerable<Item>> GetDeletedItemsByIdsAsync(List<int> itemsIds, CancellationToken cancellationToken);

        #endregion

        #region Retrieval - Path Information

        /// <summary>
        /// Asynchronously constructs the full, relative path of a folder by traversing its parent hierarchy.
        /// </summary>
        /// <param name="folder">The folder item for which to build the path.</param>
        /// <param name="cancellationToken">Token to cancel the operation if client disconnects</param>
        /// <returns>A Task that resolves to the relative folder path as a string (e.g., "ParentFolder/SubFolder").</returns>
        /// <remarks>Does not include user-specific path prefix</remarks>
        Task<string> GetFolderPathAsync(Item folder, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously generates the breadcrumb path string for the specified folder item.
        /// The method recursively retrieves the folder's parent hierarchy to build the full path.
        /// </summary>
        /// <param name="folder">The folder item for which to build the breadcrumb path.</param>
        /// <param name="cancellationToken">Token to cancel the operation if client disconnects</param>
        /// <returns>The breadcrumb path as a string, constructed from the root folder to the specified folder.</returns>
        Task<string> GetBreadcrumbPathAsync(Item folder, CancellationToken cancellationToken);

        #endregion

        #region Existence Checks

        /// <summary>
        /// Asynchronously checks if an active (not deleted) item exists for a user.
        /// </summary>
        /// <param name="itemId">The ID of the item to check.</param>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="cancellationToken">Token to cancel the operation if client disconnects</param>
        /// <param name="itemType">Optional. A specific type to filter by (e.g., "file" or "folder").</param>
        /// <returns>A Task that resolves to true if the item exists, is active, and matches the criteria; otherwise, false.</returns>
        Task<bool> ItemExistsAsync(int itemId, int userId, CancellationToken cancellationToken, string? itemType = null);

        /// <summary>
        /// Asynchronously counts how many of the provided item IDs correspond to existing, active items owned by the user.
        /// </summary>
        /// <param name="itemIds">A list of item IDs to count.</param>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="cancellationToken">Token to cancel the operation if client disconnects</param>
        /// <returns>A Task that resolves to an integer representing the count of valid items.</returns>
        Task<int> CountExistingItemsAsync(List<int> itemIds, int userId, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously checks if an active item with a specific name and type already exists within a given parent folder.
        /// </summary>
        /// <param name="name">The name to check for uniqueness.</param>
        /// <param name="itemType">The type of the item ("file" or "folder").</param>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="parentId">The ID of the parent folder. Null for the root.</param>
        /// <param name="cancellationToken">Token to cancel the operation if client disconnects</param>
        /// <param name="excludeItemId">Optional. An item ID to exclude from the search, used during rename operations.</param>
        /// <returns>A Task that resolves to true if a duplicate item exists; otherwise, false.</returns>
        /// <param name="includeDeleted">If true, soft-deleted items are considered duplicates.</param>
        Task<bool> DoesItemExistByNameAsync(string name, string itemType, int userId, int? parentId, CancellationToken cancellationToken, int? excludeItemId = null, bool includeDeleted = false);

        /// <summary>
        /// Checks if a folder is a subfolder (child at any level) of another folder
        /// </summary>
        /// <param name="userId">The ID of the user who owns both folders</param>
        /// <param name="parentFolderId">The potential parent folder ID</param>
        /// <param name="childFolderId">The potential child folder ID to check</param>
        /// <param name="cancellationToken">Token to cancel the operation if client disconnects</param>
        /// <returns>True if childFolderId is a subfolder of parentFolderId at any depth, otherwise false</returns>
        /// <remarks>Used to prevent circular folder structures (e.g., moving a folder into its own subfolder)</remarks>
        Task<bool> IsFolderSubFolderAsync(int userId, int parentFolderId, int childFolderId, CancellationToken cancellationToken);

        #endregion

        #region Calculations

        /// <summary>
        /// Recursively calculates the total size and file count of a folder and all its subfolders
        /// </summary>
        /// <param name="userId">User ID to filter items by ownership</param>
        /// <param name="folderId">Folder ID to calculate size for (null for root level)</param>
        /// <param name="cancellationToken">Token to cancel the operation if client disconnects</param>
        /// <returns>Tuple containing total size in bytes and total file count</returns>
        /// <remarks>
        /// Creates a new database context for each call to ensure thread safety
        /// Recursively processes all subfolders to calculate complete hierarchy size
        /// Only counts files marked as "file" type and not soft-deleted
        /// Returns zero values if no items found in the specified folder
        /// </remarks>
        Task<(long totalSize, int fileCount)> CalculateArchiveSizeAsync(int userId, int? folderId, CancellationToken cancellationToken);

        #endregion

        #region Create/Update Operations

        /// <summary>
        /// Adds a single item in the database in a dedicated transaction, rolling back if any error occurs.
        /// </summary>
        /// <param name="item">The item to insert.</param>
        /// <param name="cancellationToken">Token to cancel the operation if client disconnects</param>
        Task AddItemInTranscationAsync(Item item, CancellationToken cancellationToken);

        /// <summary>
        /// Atomically updates multiple items in the database, committing all changes in a single transaction.
        /// </summary>
        /// <param name="items">The collection of items to be updated.</param>
        /// <param name="cancellationToken">Token to cancel the operation if client disconnects</param>
        /// <param name="batchSize">The number of items to process in each batch within the transaction.</param>
        Task UpdateItemsInTransactionAsync(IAsyncEnumerable<Item> items, CancellationToken cancellationToken, int batchSize = 500);

        #endregion

        #region Delete Operations

        /// <summary>
        /// Permanently deletes the given item from the database (hard delete), using a transaction.
        /// </summary>
        /// <param name="item">The item record to permanently remove.</param>
        /// <param name="cancellationToken">Token to cancel the operation if client disconnects</param>
        Task DeleteItemPermanentlyAsync(Item item, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves the IDs of all items in the trash that have passed the retention period.
        /// </summary>
        /// <param name="thresholdDate">The date before which items are considered expired.</param>
        /// <param name="cancellationToken">Token to cancel the operation if client disconnects</param>
        /// <returns>A list of expired item IDs.</returns>
        Task<List<int>> GetExpiredItemIdsAsync(DateTime thresholdDate, CancellationToken cancellationToken);

        /// <summary>
        /// Permanently deletes a list of items from the database by their IDs in a single transaction.
        /// </summary>
        /// <param name="itemIds">The list of item IDs to be deleted.</param>
        /// <param name="cancellationToken">Token to cancel the operation if client disconnects</param>
        /// <returns>The number of rows affected.</returns>
        Task<int> DeleteItemsByIdsAsync(List<int> itemIds, CancellationToken cancellationToken);

        #endregion
    }
}
