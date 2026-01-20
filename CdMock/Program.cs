using CdMock.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CdMock.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. DbContext sozlamasi
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Identity sozlamasi (Rollar bilan)
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// 3. Admin roli va Default Admin foydalanuvchisini yaratish (Seeding)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

        Task.Run(async () =>
        {
            // Admin rolini tekshirish va yaratish
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            // Default Admin foydalanuvchisini tekshirish va yaratish
            string adminName = "admin1";
            string adminPass = "admin1";

            var adminUser = await userManager.FindByNameAsync(adminName);
            if (adminUser == null)
            {
                var newAdmin = new IdentityUser
                {
                    UserName = adminName,
                    Email = "admin@tod1o.com",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(newAdmin, adminPass);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newAdmin, "Admin");
                }
            }
        }).Wait();
    }
    catch (Exception ex)
    {
        // Xatolikni konsolga chiqarish (ixtiyoriy)
        Console.WriteLine($"Seeding xatosi: {ex.Message}");
    }
}

// 4. Middleware sozlamalari
if (app.Environment.IsDevelopment())
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

// 5. Default Route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Accaunt}/{action=Login}/{id?}");

app.Run();