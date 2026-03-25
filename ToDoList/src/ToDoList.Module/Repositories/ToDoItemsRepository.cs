using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using TestApp.ToDoList.Entity;
using TestApp.ToDoList.Store;

namespace TestApp.ToDoList.Repository
{
  /// <summary>
  /// Repository for managing to-do items.
  /// </summary>
  public class ToDoItemsRepository : IToDoItemsRepository
  {
    private readonly ToDoListDbContext context;
    /// <summary>
    /// Ctor. Seeds initial data.
    /// </summary>
    /// <param name="context"></param>
    public ToDoItemsRepository(ToDoListDbContext context)
    {
      this.context = context;

      if (!context.ToDoItems.Any())
      {
        context.ToDoItems.AddRange(
        new[] {
          new ToDoItem { Title = "Laundry"},
          new ToDoItem { Title = "Grocery Shopping", IsCompleted = true},
          new ToDoItem { Title = "Pay Bills"},
          new ToDoItem { Title = "Clean the House", IsCompleted = true},
        }
      );
        context.SaveChanges();
      }
    }
    /// <inheritdoc/>
    public ICollection<ToDoItem> GetAllItems()
    {
      return context.ToDoItems.Include(x => x.Tags).ToList();
    }

    /// <inheritdoc/>
    public IQueryable<ToDoItem> GetAllItemsOdata()
    {
      return context.ToDoItems;
    }

    /// <inheritdoc/>
    public ToDoItem GetItemById(int id)
    {
      return context.ToDoItems.Include(x => x.Tags).FirstOrDefault(x => x.Id == id);
    }
    /// <inheritdoc/>
    public ToDoItem Create(ToDoItem item)
    {
      context.ToDoItems.Add(item);
      context.SaveChanges();
      return item;
    }
    /// <inheritdoc/>
    public ToDoItem Update(ToDoItem item)
    {
      context.ToDoItems.Update(item);
      context.SaveChanges();
      return item;
    }
    /// <inheritdoc/>
    public void Delete(int id)
    {
      var item = context.ToDoItems.Find(id);
      if (item != null)
      {
        context.ToDoItems.Remove(item);
        context.SaveChanges();
      }
    }

    /// <inheritdoc/>
    public ToDoItem AddTagToItem(int itemId, int tagId)
    {
      var item = context.ToDoItems.Include(x => x.Tags).FirstOrDefault(x => x.Id == itemId);
      var tag = context.ToDoTags.Find(tagId);  

      if (tag == null || item == null)
        throw new KeyNotFoundException($"Item {itemId} or Tag {tagId} not found");

      if (item.Tags == null)
      {
        item.Tags = new List<ToDoTag>();
      }

      if (!item.Tags.Any(x => x.Id == tagId))
      {// only add when not already in here
        item.Tags.Add(tag);
        context.Update(item);
        context.SaveChanges();
        
      }

      return item;
    }


    /// <inheritdoc/>
    public ToDoItem RemoveTagFromItem(int itemId, int tagId)
    {
      var item = context.ToDoItems.Include(x => x.Tags).FirstOrDefault(x => x.Id == itemId);

      if (item == null)
        throw new KeyNotFoundException($"Item {itemId} not found");

      var tag = item.Tags.FirstOrDefault(x => x.Id == tagId);

      if (tag == null)
        throw new KeyNotFoundException($"Tag {tagId} not found");

      item.Tags.Remove(tag);

      context.Update(item);
      context.SaveChanges();

      return item;
    }

  }
}