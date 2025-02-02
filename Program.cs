using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TrainingManagement.Configuration;
using TrainingManagement.Models;
using TrainingManagement.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")).EnableSensitiveDataLogging()
    .EnableDetailedErrors());

builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password = new PasswordOptions
    {
        RequiredLength = 1,
        RequireDigit = false,
        RequireLowercase = false,
        RequireUppercase = false,
        RequireNonAlphanumeric = false,
        RequiredUniqueChars = 1
    };
})
.AddPasswordValidator<CustomPasswordValidator<User>>()
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddControllersWithViews();

builder.Services.AddScoped<DataInitializerService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dataInitializer = services.GetRequiredService<DataInitializerService>();
    await dataInitializer.InitializeData();
    var context = services.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate();
}

app.Run();
