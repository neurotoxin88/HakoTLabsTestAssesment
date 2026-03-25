using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Runtime.InteropServices;
using TestApp.ToDoList.Entity;
using TestApp.ToDoList.Store;

namespace TestApp.ToDoList.Repository
{
  /// <summary>
  /// Repository for managing to-do items.
  /// </summary>
  public class ToDoTagsRepository : IToDoTagsRepository
  {
    private readonly ToDoListDbContext context;
    /// <summary>
    /// Ctor. Seeds initial data.
    /// </summary>
    /// <param name="context"></param>
    public ToDoTagsRepository(ToDoListDbContext context)
    {
      this.context = context;

      if (!context.ToDoTags.Any())
      {
        context.ToDoTags.AddRange(
        new[] {
          new ToDoTag { Title = "Inhouse"},
          new ToDoTag { Title = "Garden"},
          new ToDoTag { Title = "Extern"},
        }
      );
        context.SaveChanges();
      }
    }
    /// <inheritdoc/>
    public ICollection<ToDoTag> GetAllTags()
    {
      return context.ToDoTags.ToList();
    }
    /// <inheritdoc/>
    public ToDoTag GetTagById(int id)
    {
      return context.ToDoTags.Find(id);
    }
    /// <inheritdoc/>
    public ToDoTag Create(ToDoTag tag)
    {
      context.ToDoTags.Add(tag);
      context.SaveChanges();
      return tag;
    }
    /// <inheritdoc/>
    public ToDoTag Update(ToDoTag tag)
    {
      context.ToDoTags.Update(tag);
      context.SaveChanges();
      return tag;
    }
    /// <inheritdoc/>
    public void Delete(int id)
    {
      var tag = context.ToDoTags.Find(id);
      if (tag != null)
      {
        context.ToDoTags.Remove(tag);
        context.SaveChanges();
      }
    }

  }
}