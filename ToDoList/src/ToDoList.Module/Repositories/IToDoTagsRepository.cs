using System.Collections.Generic;
using TestApp.ToDoList.Entity;

namespace TestApp.ToDoList.Repository
{
  /// <summary>
  /// Repository interface for managing to-do tags.
  /// </summary>
  public interface IToDoTagsRepository
  {
    /// <summary>
    /// Gets all to-do tags from DB.
    /// </summary>
    /// <returns></returns>
    ICollection<ToDoTag> GetAllTags();

    /// <summary>
    /// Gets single to-do tag by Id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    ToDoTag GetTagById(int id);
    /// <summary>
    /// Creates a new to-do tag.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    ToDoTag Create(ToDoTag item);
    /// <summary>
    /// Updates an existing to-do tag.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    ToDoTag Update(ToDoTag item);
    /// <summary>
    /// Delete an existing to-do tag.
    /// </summary>
    /// <param name="id"></param>
    void Delete(int id);
  }
}
