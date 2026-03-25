using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

public class ODataQueryOptionsOperationFilter : IOperationFilter
{
  public void Apply(OpenApiOperation operation, OperationFilterContext context)
  {
    var hasEnableQuery = context.MethodInfo
        .GetCustomAttributes(true)
        .OfType<Microsoft.AspNetCore.OData.Query.EnableQueryAttribute>()
        .Any();

    if (!hasEnableQuery)
      return;

    operation.Parameters ??= new List<OpenApiParameter>();

    operation.Parameters.Add(new OpenApiParameter
    {
      Name = "$filter",
      In = ParameterLocation.Query,
      Description = "Filter results (e.g. IsCompleted eq true)",
      Required = false
    });

    operation.Parameters.Add(new OpenApiParameter
    {
      Name = "$orderby",
      In = ParameterLocation.Query,
      Description = "Sort results (e.g. Title desc, Title asc)",
      Required = false
    });

    operation.Parameters.Add(new OpenApiParameter
    {
      Name = "$top",
      In = ParameterLocation.Query,
      Description = "Limit number of results (e.g. 10)",
      Required = false
    });

    operation.Parameters.Add(new OpenApiParameter
    {
      Name = "$skip",
      In = ParameterLocation.Query,
      Description = "Skip number of results (e.g. 10)",
      Required = false
    });

    operation.Parameters.Add(new OpenApiParameter
    {
      Name = "$select",
      In = ParameterLocation.Query,
      Description = "Select specific fields (e.g. Title)",
      Required = false
    });

    operation.Parameters.Add(new OpenApiParameter
    {
      Name = "$expand",
      In = ParameterLocation.Query,
      Description = "Expand related entities (e.g. Tags)",
      Required = false
    });
  }
}