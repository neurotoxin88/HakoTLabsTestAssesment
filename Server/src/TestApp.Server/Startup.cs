using Microsoft.AspNetCore.OData;
using Microsoft.OData.ModelBuilder;
using System.Text.Json.Serialization;
using TestApp.ToDoList.Entity;
using TestApp.ToDoList.Module;
using TestApp.ToDoList.Repository;
using TestApp.ToDoList.Store;
using TestApp.ToDoList.Tracker;


namespace TestApp.Server
{
  public class Startup
  {
    IConfiguration configuration;

    public Startup(IConfiguration configuration)
    {
      this.configuration = configuration;
    }
    public void ConfigureServices(IServiceCollection services)
    {
      // Add DB
      services.AddDbContext<ToDoListDbContext>();

      services.AddOutputCache();

      var odataBuilder = new ODataConventionModelBuilder();
      odataBuilder.EntitySet<ToDoItem>("ToDoItems");

      // Add controllers
      services.AddControllers().AddOData(options =>
        options.Select()
               .Filter()
               .OrderBy()
               .Expand()
               .Count()
               .SetMaxTop(100)
               .AddRouteComponents("odata", odataBuilder.GetEdmModel())); 
        //.AddJsonOptions(x =>
        //x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles); 
        // this is needed when the tags should reffere back to the items

      // Configure app services
      services.AddScoped<IToDoListTracker, ToDoListTracker>();
      services.AddScoped<IToDoItemsRepository, ToDoItemsRepository>(); // changed to Scoped becaus the lifetime is different if singelton is needed use Factory instead
      services.AddScoped<IToDoTagsRepository, ToDoTagsRepository>();// added as new feature
      services.AddScoped<ToDoListEntityModel>();

      services.AddCors(options =>
      {
        options.AddDefaultPolicy(policy =>
        {
          policy.AllowAnyOrigin()
            .AllowAnyHeader();
        });
      });

      services.AddEndpointsApiExplorer();
      services.AddSwaggerGen(options =>
      {
        var xmlFilename = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);

        options.IncludeXmlComments(xmlPath);
        options.OperationFilter<ODataQueryOptionsOperationFilter>();
      });
    }
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider svcProv)
    {
     
      
      // Enable Swagger in all environments
      app.UseSwagger();
      app.UseSwaggerUI();

      var appLifetime = svcProv.GetRequiredService<IHostApplicationLifetime>();
      appLifetime.ApplicationStarted.Register(onApplicationStarted);

      app.UseRouting();
      
      app.UseAuthorization();

      app.UseCors();
      app.UseOutputCache();

      app.UseEndpoints(endpoints =>
        {
          endpoints.MapControllers();
        }
      );
    }

    void onApplicationStarted()
    {
      // Do nothing
    }

  }
}