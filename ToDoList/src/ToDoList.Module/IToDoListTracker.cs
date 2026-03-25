using System.Collections.Generic;
using System.Linq;
using TestApp.ToDoList.Entity;

namespace TestApp.ToDoList.Module
{
  /// <summary>
  /// Tracking to-do list items.
  /// </summary>
  public interface IToDoListTracker
  {

    #region Items
    /// <summary>
    /// Adds a new to-do item.
    /// </summary>
    /// <param name="title"></param>
    /// <returns></returns>
    ToDoItem AddItem(string title);
    /// <summary>
    /// Removes a to-do item.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    ToDoItem RemoveItem(int id);
    /// <summary>
    /// Gets a to-do item by its Id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    ToDoItem GetItem(int id);
    /// <summary>
    /// Gets all to-do items.
    /// </summary>
    /// <returns></returns>
    IEnumerable<ToDoItem> GetAllItems();


    /// <summary>
    /// Gets all to-do items and allows querrys.
    /// </summary>
    /// <returns></returns>
    IQueryable<ToDoItem> GetAllItemsOdata();

    /// <summary>
    /// Edits a to-do item.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="updatedTask"></param>
    /// <returns></returns>
    ToDoItem EditItem(int id, ToDoItem updatedTask);

    /// <summary>
    /// Adds a tag to the existing item, if the task does not exist yet it will be created
    /// </summary>
    /// <param name="itemId"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    ToDoItem AddTagToItem(int itemId, string tag);

    /// <summary>
    /// Removes a tag from the existing item
    /// </summary>
    /// <param name="itemId"></param>
    /// <param name="tagId"></param>
    /// <returns></returns>
    ToDoItem RemoveTagFromItem(int itemId, int tagId);

    #endregion

    #region Tags
    /// <summary>
    /// Gets all to-do tags.
    /// </summary>
    /// <returns></returns>
    IEnumerable<ToDoTag> GetAllTags();

    /// <summary>
    /// Adds a new to-do tag.
    /// </summary>
    /// <param name="title"></param>
    /// <returns></returns>
    ToDoTag AddTag(string title);
    /// <summary>
    /// Removes a to-do tag.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    ToDoTag RemoveTag(int id);
    /// <summary>
    /// Gets a to-do tag by its Id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    ToDoTag GetTag(int id);

    /// <summary>
    /// Edits a to-do tag.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="updatedTag"></param>
    /// <returns></returns>
    ToDoTag EditTag(int id, ToDoTag updatedTag);
    #endregion
  }
}