using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp.ToDoList.Entity
{
  public class ToDoTag
  {
    /// <summary>
    /// Unique Id of the Tag (autoincrement)
    /// </summary>
    public int Id { get; set; }


    /// <summary>
    /// Title of the Tag. (Unique)
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// List of to-do items.
    /// </summary>
    //public List<ToDoItem> Items { get; set; } // currently dont use the way back to prevent the cycle reference
  }
}
