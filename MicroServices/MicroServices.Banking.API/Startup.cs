
using MediatR;

using MicroServices.Banking.Data.Context;
using MicroServices.Infrastructure.IOC;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi;

namespace MicroServices.Banking.API {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.AddDbContext<BankingDbContext>(options => {
                options.UseSqlServer(Configuration.GetConnectionString("BankingDbConnection"));
            });
            services.AddSwaggerGen(sw => {
                sw.SwaggerDoc("v1", new OpenApiInfo {
                    Title = "Banking MicroServices",
                    Version = "v1",
                    Description = "Banking bounded-context API (.NET 10)"
                });
            });
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Startup>());
            services.AddControllers();
            RegisterServices(services);
        }

        private void RegisterServices(IServiceCollection services) {
            DependencyContainer.RegisterServices(services, Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });

            app.UseSwagger();
            app.UseSwaggerUI(swui => {
                swui.SwaggerEndpoint("/swagger/v1/swagger.json", "Banking Microservice V1");
            });
        }
    }
}
