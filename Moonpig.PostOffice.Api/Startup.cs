namespace Moonpig.PostOffice.Api
{
    using Hellang.Middleware.ProblemDetails;
    using MediatR;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Moonpig.PostOffice.Api.Repositories.Concretes;
    using Moonpig.PostOffice.Api.Repositories.Contracts;
    using Moonpig.PostOffice.Data;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMediatR(typeof(Startup));
            services.AddProblemDetails();
            ConfigureInversionOfControl(services);
            services.AddControllers();
        }

        private void ConfigureInversionOfControl(IServiceCollection services)
        {
            services.AddTransient<IDbContext, DbContext>();
            services.AddTransient<IProductRepository, ProductRepository>();
            services.AddTransient<ISupplierRepository, SupplierRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseProblemDetails();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
