using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Store;
using Store.Data;
using Store.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<HttpClient>(sp =>
{
    var navigationManager = sp.GetRequiredService<NavigationManager>();
    return new HttpClient { BaseAddress = new Uri(navigationManager.BaseUri) };
});
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<InvoiceService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<SettingsService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<AuthenticationStateProvider, CustomStateProvider>();
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

// Configure DbContext with SQLite
builder.Services.AddDbContext<StoreDBContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register HttpClient with a base address
builder.Services.AddHttpClient("MyApiClient", client =>
{
    // Ensure that ApiBaseUrl is set in appsettings.json
    var apiBaseUrl = builder.Configuration["ApiBaseUrl"];
    if (string.IsNullOrEmpty(apiBaseUrl))
    {
        throw new ArgumentNullException(nameof(apiBaseUrl), "Base address for HttpClient cannot be null or empty.");
    }
    client.BaseAddress = new Uri(apiBaseUrl);
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.SameSite = SameSiteMode.Lax; // ✅ for cookie to work on same-origin posts
        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = 401;
            return Task.CompletedTask;
        };
    });
builder.Services.AddAuthorization(); // Required for Blazor's authorization

// Configure Identity
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireUppercase = true;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
})
.AddEntityFrameworkStores<StoreDBContext>()
.AddDefaultTokenProviders();

var app = builder.Build();

// Seed roles and admin user
await SeedRolesAndAdminUser(app);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Blazor Server API V1");
    });
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication(); // Ensure authentication middleware is placed before routing
app.UseAuthorization();
app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.Run();

// Method to seed roles and admin user
async Task SeedRolesAndAdminUser(WebApplication app)
{
    using (var scope = app.Services.CreateScope())
    {
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var productSvc = scope.ServiceProvider.GetRequiredService<ProductService>();
        var categorySvc = scope.ServiceProvider.GetRequiredService<CategoryService>();
        var settingSvc = scope.ServiceProvider.GetRequiredService<SettingsService>();

        // Seed Admin role if it doesn't exist
        var roleExist = await roleManager.RoleExistsAsync("Admin");
        if (!roleExist)
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
        }

        // Create the admin user if not already created
        var user = await userManager.FindByEmailAsync("admin@store.com");
        if (user == null)
        {
            user = new AppUser { UserName = "admin@store.com", Email = "admin@store.com", Gender= "Male", Age = 35, Name = "Admin", DoC = DateTime.Now, Role = "Admin"  };
            var result = await userManager.CreateAsync(user, "Admin@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "Admin");
            }
        }
        var category = await categorySvc.GetProductAsync(1);
        if (category == null)
        {
            category = new Category { Id = 1, Name = "Soap", Code = "C001" };
            await categorySvc.AddCategoryAsync(category);
        }

        var product = await productSvc.GetProductAsync(1);
        if(product == null)
        {
            product = new Product { Id = 1, Name= "Hamam", CategoryId =1, Price =25.5m, StockQuantity =10 };
            await productSvc.AddProductAsync(product);            
        }

        var adminSetting = await settingSvc.GetAdminSetting(1);
        if (adminSetting == null)
        {
            int max = int.Parse(builder.Configuration["MaxUsers"] ?? "5") ;
            string currency = builder.Configuration["Currency"] ?? "INR";
            string name = builder.Configuration["ClientName"] ?? "Store";
            decimal tax = decimal.Parse(builder.Configuration["Tax"] ?? "0");

            adminSetting = new AdminSetting { Id = 1, Currency = currency, MaxUsers = max, Name = name, Tax = tax   };
            await settingSvc.AddAdminSettingAsync(adminSetting);
        }

        var setting = await settingSvc.GetSetting(1);
        if (setting == null)
        {
            setting = new Setting { Id = 1, Key = "TouchScreen", Value = "true" };
            await settingSvc.AddSettingAsync(setting);
        }

    }
}