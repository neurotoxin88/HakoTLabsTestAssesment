using System.Collections.Generic;
using System.Linq;
using TestApp.ToDoList.Entity;

namespace TestApp.ToDoList.Repository
{
  /// <summary>
  /// Repository interface for managing to-do items.
  /// </summary>
  public interface IToDoItemsRepository
  {
    /// <summary>
    /// Gets all to-do items from DB.
    /// </summary>
    /// <returns></returns>
    ICollection<ToDoItem> GetAllItems();

    IQueryable<ToDoItem> GetAllItemsOdata();

    /// <summary>
    /// Gets single to-do item by Id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    ToDoItem GetItemById(int id);
    /// <summary>
    /// Creates a new to-do item.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    ToDoItem Create(ToDoItem item);
    /// <summary>
    /// Updates an existing to-do item.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    ToDoItem Update(ToDoItem item);
    /// <summary>
    /// Delete an existing to-do item.
    /// </summary>
    /// <param name="id"></param>
    void Delete(int id);

    /// <summary>
    /// Adds a tag to the item
    /// </summary>
    /// <param name="tagId"></param>
    /// <param name="itemId"></param>
    /// <exception cref="KeyNotFoundException"></exception>
    ToDoItem AddTagToItem(int itemId, int tagId);

    /// <summary>
    /// Removes a tag from the item
    /// </summary>
    /// <param name="tagId"></param>
    /// <param name="itemId"></param>
    /// <exception cref="KeyNotFoundException"></exception>
    ToDoItem RemoveTagFromItem(int itemId, int tagId);
  }
}
