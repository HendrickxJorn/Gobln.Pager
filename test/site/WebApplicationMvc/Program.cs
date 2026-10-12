using Gobln.Pager.Json.Microsoft;
using Gobln.Pager.Mvc;

namespace WebApplicationMvc
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews()
                // Without it a Page is written as a bare array of its items, losing the paging info
                .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new PageJsonConverter()));

            builder.Services.AddMvcPager();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
