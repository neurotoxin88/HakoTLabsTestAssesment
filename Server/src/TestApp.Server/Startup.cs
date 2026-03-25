using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OData;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OData.ModelBuilder;
using System.Text;
using System.Text.Json.Serialization;
using TestApp.Server.Settings;
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


      services.Configure<JwtSettings>(configuration.GetSection("Jwt"));

      var jwtSettings = configuration.GetSection("Jwt").Get<JwtSettings>();

      services.AddAuthentication(options =>
      {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
      })
      .AddJwtBearer(options =>
      {
        options.TokenValidationParameters = new TokenValidationParameters
        {
          ValidateIssuer = false,
          ValidateAudience = false,
          ValidateLifetime = true,
          ValidateIssuerSigningKey = true,
          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
        };
      });

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
        options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
          Name = "Authorization",
          Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
          Scheme = "bearer",
          BearerFormat = "JWT",
          In = Microsoft.OpenApi.Models.ParameterLocation.Header,
          Description = "Enter JWT token"
        });

        options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
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

      app.UseAuthentication();

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