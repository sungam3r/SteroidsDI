using SteroidsDI;

namespace Example;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    public void ConfigureServices(IServiceCollection services)
    {
        // register data models
        services.AddSingleton<IEntryPoint, EntryPoint>();
        services.AddScoped<IRepository, Repository>();

        // register DI stuff
        services.AddDefer();
        services.AddFunc<IRepository>();
        services.AddFactory<IRepositoryFactory>(); // implementation will be generated at runtime
        services.AddHttpScope();
        services.Configure<ServiceProviderAdvancedOptions>(Configuration.GetSection("Steroids"));

        // register standard stuff
        services.AddControllers();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();

            endpoints.MapGet("/", async context => await context.Response.WriteAsync("Hello World!"));
        });
    }
}
