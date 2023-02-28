using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BaoMoiAPI
{
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

            services.AddControllers();

//            services.AddCors(opt =>
//            {
//                opt.AddPolicy("MyPolicy", policy =>
//                {
//                    policy.WithOrigins("http://localhost:5500").AllowCredentials().AllowAnyHeader().AllowAnyHeader();
//                });
//            })
//;
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "BaoMoiAPI", Version = "v1" });
            });
            services.AddDbContext<Models.BaoMoiDbContext>(option =>
            {
                option.UseSqlServer(Configuration.GetConnectionString("baomoi"));
            });


            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "BaoMoiAPI v1"));
            }


            //app.UseCors(
            //    option =>
            //{
            //    option
            //    .AllowAnyOrigin() // * 
            //    .AllowAnyMethod()  // 
            //    .AllowAnyHeader()  // 
            //    ;
            //}
            //);
            //app.UseCors("MyPolicy");



            app.UseHttpsRedirection();
            app.UseStaticFiles();

            //app.UseCors(opt =>
            //{
            //    opt.AllowAnyOrigin()
            //    .AllowAnyHeader()
            //    .AllowAnyMethod();
            //});

           

            app.UseRouting();

            
            app.UseAuthorization();

            //app.UseCors(
           //    option =>
           //{
           //    option
           //    .AllowAnyOrigin() // * 
           //    .AllowAnyMethod()  // 
           //    .AllowAnyHeader()  // 
           //    ;
           //}
           //);
            app.UseCors("MyPolicy");

            app.UseEndpoints(endpoints =>
            {
            endpoints.MapControllers();

            

            });


            
        }
    }
}
