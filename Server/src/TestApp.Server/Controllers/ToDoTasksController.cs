using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using TestApp.Server.DTOs;
using TestApp.ToDoList.Entity;
using TestApp.ToDoList.Module;

namespace TestApp.Server.Controllers
{
  [ApiController]
  [Route("api/tasks")]
  public class ToDoTasksController : ControllerBase
  {
    private readonly IToDoListTracker toDoListTracker;

    public ToDoTasksController(IToDoListTracker toDoListTracker)
    {
      this.toDoListTracker = toDoListTracker;
    }

    /// <summary>
    /// returns a list of all Tasks
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [OutputCache(Duration = 30)]
    public ActionResult<IList<ToDoItem>> GetTasks()
    {
      try
      {
        var tasks = toDoListTracker.GetAllItems();
        return Ok(tasks.ToList());
      }
      catch (Exception ex)
      {
        return Problem(
          title: "Unexpected error",
          detail: ex.Message,
          statusCode: StatusCodes.Status409Conflict
          ); // Confilict, because 500 will allways come when the excaption could not be catched
      }
    }

    /// <summary>
    /// Gets taks for the given page
    /// </summary>
    /// <param name="pageSize"></param>
    /// <param name="page"></param>
    /// <returns></returns>
    [HttpGet("[action]/{pageSize}/{page}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [OutputCache(Duration = 30)]
    public ActionResult<IList<ToDoItem>> GetTasksPage(int pageSize, int page)
    {// Theoratically this is redundant, because the odata call can achive pagination with skip and top but this is the alternative if the odata pakagage should not be used
      try
      {
        page = page > 0 ? page - 1 : 0; 
        var tasks = toDoListTracker.GetAllItemsOdata().Include(x => x.Tags).OrderBy(x => x.Id).Skip(pageSize * page).Take(pageSize);
        return Ok(tasks.ToList());
      }
      catch (Exception ex)
      {
        return Problem(
          title: "Unexpected error",
          detail: ex.Message,
          statusCode: StatusCodes.Status409Conflict
          ); // Confilict, because 500 will allways come when the excaption could not be catched
      }
    }


    /// <summary>
    /// returns a list of all Tasks that can be filterd and ordered using the odata syntax
    /// </summary>
    /// <returns></returns>
    [HttpGet("[action]")]
    [EnableQuery]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [OutputCache(Duration = 30)]
    public ActionResult<IQueryable<ToDoItem>> GetTasksOdata()
    {
      try
      {
        return Ok(toDoListTracker.GetAllItemsOdata());
      }
      catch (Exception ex)
      {
        return Problem(
          title: "Unexpected error",
          detail: ex.Message,
          statusCode: StatusCodes.Status409Conflict
          ); // Confilict, because 500 will allways come when the excaption could not be catched
      }
    }

    /// <summary>
    /// Creates a Task, automaticly sets the createdAt with UTC now
    /// </summary>
    /// <param name="newTask"></param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public ActionResult<ToDoItem> CreateTask([FromBody] ToDoItemDTO newTask)
    {
      try
      {
        var item = new ToDoItem()
        {
          Title = newTask.Title,
        };
        var ret = toDoListTracker.AddItem(newTask.Title);
        return Ok(ret);
      }
      catch (Exception ex)
      {
        return Problem(
          title: "Unexpected error",
          detail: ex.Message,
          statusCode: StatusCodes.Status409Conflict
          ); // Confilict, because 500 will allways come when the excaption could not be catched
      }
    }

    /// <summary>
    /// Updates the given Task by Id, overrides title and isCompleted 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="updatedTask"></param>
    /// <returns></returns>
    [HttpPut("{id}")] // maybe a patch would also be ok
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public ActionResult<ToDoItem> EditTask(int id, [FromBody] ToDoItemDTO updatedTask)
    {
      try
      {// Error Handling and Proper response Types for external error handling
        var item = new ToDoItem()
        {
          Id = id,
          Title = updatedTask.Title,
          IsCompleted = updatedTask.IsCompleted,
        };
        var ret = toDoListTracker.EditItem(id, item);
        return Ok(ret);
      }
      catch (KeyNotFoundException ke)
      {
        return NotFound(
          new ProblemDetails
          {
            Title = "Task not found",
            Detail = ke.Message,
            Status = StatusCodes.Status404NotFound
          });
      }
      catch (Exception ex)
      {
        return Problem(
          title: "Unexpected error",
          detail: ex.Message,
          statusCode: StatusCodes.Status409Conflict
          ); // Confilict, because 500 will allways come when the excaption could not be catched
      }

    }


    /// <summary>
    /// Connects a tag to a to-do item, if the tag does not already exist, it will be created
    /// </summary>
    /// <param name="taskId"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    [HttpPatch("[action]{taskId}/{tag}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public ActionResult<ToDoItem> AddTagToTask(int taskId, string tag)
    {
      try
      {
        var ret = toDoListTracker.AddTagToItem(taskId, tag);
        return Ok(ret);
      }
      catch (KeyNotFoundException ke)
      {
        return NotFound(
          new ProblemDetails
          {
            Title = "Task not found",
            Detail = ke.Message,
            Status = StatusCodes.Status404NotFound
          });
      }
      catch (Exception ex)
      {
        return Problem(
          title: "Unexpected error",
          detail: ex.Message,
          statusCode: StatusCodes.Status409Conflict
          );
      }
    }

    /// <summary>
    /// Connects a tag to a to-do item, if the tag does not already exist, it will be created
    /// </summary>
    /// <param name="taskId"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    [HttpPatch("[action]/{taskId}/{tagId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public ActionResult<ToDoItem> RemoveTagFromTask(int taskId, int tagId)
    {
      try
      {
        var ret = toDoListTracker.RemoveTagFromItem(taskId, tagId);
        return Ok(ret);
      }
      catch (KeyNotFoundException ke)
      {
        return NotFound(
          new ProblemDetails
          {
            Title = "Task not found",
            Detail = ke.Message,
            Status = StatusCodes.Status404NotFound
          });
      }
      catch (Exception ex)
      {
        return Problem(
          title: "Unexpected error",
          detail: ex.Message,
          statusCode: StatusCodes.Status409Conflict
          );
      }
    }

    /// <summary>
    /// Deltes the given Task by Id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public ActionResult<ToDoItem> DeleteTask(int id)
    {
      try
      { 
        var task = toDoListTracker.RemoveItem(id);
        return Ok(task);
      }
      catch (KeyNotFoundException ke)
      {
        return NotFound(
          new ProblemDetails
          {
            Title = "Task not found",
            Detail = ke.Message,
            Status = StatusCodes.Status404NotFound
          });
      }
      catch (Exception ex)
      {
        return Problem(
          title: "Unexpected error",
          detail: ex.Message,
          statusCode: StatusCodes.Status409Conflict
          );
      }
    }
  }
}