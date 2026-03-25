using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;
using System.Linq;
using TestApp.ToDoList.Entity;
using TestApp.ToDoList.Module;
using TestApp.ToDoList.Repository;

namespace TestApp.ToDoList.Tracker
{
  /// <summary>
  /// Implementation of the to-do list tracking.
  /// </summary>
  public class ToDoListTracker : IToDoListTracker
  {
    private readonly IToDoItemsRepository repository;
    private readonly IToDoTagsRepository tagRepository;

    #region Items
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="repository"></param>
    public ToDoListTracker(IToDoItemsRepository repository, IToDoTagsRepository tagRepository)
    {
      this.repository = repository;
      this.tagRepository = tagRepository;
    }

    /// <inheritdoc/>
    public ToDoItem AddItem(string title)
    {
      // Implementation for adding a to-do item
      var newItem = new ToDoItem { Title = title, IsCompleted = false , CreatedAt = DateTime.UtcNow}; // added the created at date
      newItem = repository.Create(newItem);
      return newItem;
    }
    /// <inheritdoc/>
    public ToDoItem RemoveItem(int id)
    {
      var item = repository.GetItemById(id);
      if (null == item)
        throw new KeyNotFoundException($"Item with id {id} not found"); // Changed to not found Exception

      repository.Delete(id);
      return item;
    }
    /// <inheritdoc/>
    public ToDoItem GetItem(int id)
    {
      // Implementation for getting a specific to-do item
      var item = repository.GetItemById(id);
      if (null == item)
        throw new KeyNotFoundException($"Item with id {id} not found");

      return item;
    }
    /// <inheritdoc/>
    public IEnumerable<ToDoItem> GetAllItems()
    {
      // Implementation for getting all to-do items
      return repository.GetAllItems().ToList();
    }

    public IQueryable<ToDoItem> GetAllItemsOdata()
    {
      // Implementation for getting all to-do items
      return repository.GetAllItemsOdata();
    }

    public ToDoItem EditItem(int id, ToDoItem updatedTask)
    {
      if (id == 0 && updatedTask.Id != 0)
      {// when some one forget the extra id
        id = updatedTask.Id; // other whise this will never be used only in Display 
      }

      var item = repository.GetItemById(id);
      if (null == item) // i like item == null more as writing idk why :D 
        throw new KeyNotFoundException($"Item with id {id} not found");// changed to not found exception

      item.Title = updatedTask.Title;
      item.IsCompleted = updatedTask.IsCompleted;
      item.CompletedAt = updatedTask.IsCompleted ? DateTime.UtcNow : null;
      repository.Update(item);
      return item;
    }

    /// <inheritdoc/>
    public ToDoItem AddTagToItem(int itemId, string tag)
    {
      var item = repository.GetItemById(itemId);

      if (item == null)
        throw new KeyNotFoundException($"Item with id {itemId} not found");

      var t = tagRepository.GetAllTags().FirstOrDefault(x => x.Title.ToLower() == tag.ToLower());

      if (t == null)
        t = AddTag(tag);

      var ret = repository.AddTagToItem(item.Id, t.Id);

      return ret;
    }

    /// <inheritdoc/>
    public ToDoItem RemoveTagFromItem(int itemId, int tagId)
    {
      var ret = repository.RemoveTagFromItem(itemId, tagId);

      return ret;
    }

    #endregion

    #region Tags

    /// <inheritdoc/>
    public ToDoTag AddTag(string title)
    {
      // Implementation for adding a to-do tag
      var newTag = new ToDoTag { Title = title};
      newTag = tagRepository.Create(newTag);
      return newTag;
    }
    /// <inheritdoc/>
    public ToDoTag RemoveTag(int id)
    {
      var tag = tagRepository.GetTagById(id);
      if (null == tag)
        throw new KeyNotFoundException($"Tag with id {id} not found");

      tagRepository.Delete(id);
      return tag;
    }
    /// <inheritdoc/>
    public ToDoTag GetTag(int id)
    {
      // Implementation for getting a specific to-do item
      var tag = tagRepository.GetTagById(id);
      if (null == tag)
        throw new KeyNotFoundException($"Tag with id {id} not found");

      return tag;
    }

    /// <inheritdoc/>
    public IEnumerable<ToDoTag> GetAllTags()
    {
      // Implementation for getting all to-do tags orderd by Id
      return tagRepository.GetAllTags().OrderBy(x => x.Id).ToList();
    }


    /// <inheritdoc/>
    public ToDoTag EditTag(int id, ToDoTag updatedTag)
    {
      if (id == 0 && updatedTag.Id != 0)
      {// when some one forget the extra id
        id = updatedTag.Id; // other whise this will never be used only in Display 
      }

      var tag = tagRepository.GetTagById(id);
      if (tag == null)
        throw new KeyNotFoundException($"Tag with id {id} not found");// changed to not found exception

      tag.Title = updatedTag.Title;
      tagRepository.Update(tag);
      return tag;
    }
    #endregion
  }
}