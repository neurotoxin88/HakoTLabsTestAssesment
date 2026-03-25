using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using TestApp.Server.DTOs;
using TestApp.ToDoList.Entity;
using TestApp.ToDoList.Module;

namespace TestApp.Server.Controllers
{
  [ApiController]
  [Route("api/tags")]
  public class ToDoTagsController : ControllerBase
  {
    private readonly IToDoListTracker toDoListTracker;

    public ToDoTagsController(IToDoListTracker toDoListTracker)
    {
      this.toDoListTracker = toDoListTracker;
    }

    /// <summary>
    /// returns a list of all tags
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public ActionResult<IList<ToDoItem>> GetTags()
    {
      try
      {
        var tasks = toDoListTracker.GetAllTags();
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
    /// Creates a Tag, automaticly sets the createdAt with UTC now
    /// </summary>
    /// <param name="newTag"></param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [Authorize]
    public ActionResult<ToDoItem> CreateTag([FromBody] ToDoTagDTO newTag)
    {
      try
      {
        var nt = new ToDoTag()
        {
          Title = newTag.Title
        };
        var tag = toDoListTracker.AddTag(newTag.Title);
        return Ok(tag);
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
    /// <param name="updatedTag"></param>
    /// <returns></returns>
    [HttpPut("{id}")] // maybe a patch would also be ok
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [Authorize]
    public ActionResult<ToDoItem> EditTag(int id, [FromBody] ToDoTagDTO updatedTag)
    {
      try
      {// Error Handling and Proper response Types for external error handling
        var tag = new ToDoTag()
        {
          Title = updatedTag.Title,
        };
        var ret = toDoListTracker.EditTag(id, tag);
        return Ok(ret);
      }
      catch (KeyNotFoundException ke)
      {
        return NotFound(
          new ProblemDetails
          {
            Title = "Tag not found",
            Detail = ke.Message,
            Status = StatusCodes.Status404NotFound
          });
      }
      catch (Exception ex)
      {
        return Problem(
          title :"Unexpected error",
          detail: ex.Message,
          statusCode : StatusCodes.Status409Conflict
          ); // Confilict, because 500 will allways come when the excaption could not be catched
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
    [Authorize]
    public ActionResult<ToDoItem> DeleteTag(int id)
    {
      try
      { 
        var task = toDoListTracker.RemoveTag(id);
        return Ok(task);
      }
      catch (KeyNotFoundException ke)
      {
        return NotFound(
          new ProblemDetails
          {
            Title = "Tag not found",
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