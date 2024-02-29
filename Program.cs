using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace TeamsLektion6;


/*

// # Övning 1

// 1. Skapa modeller (User)
// 2. Skapa DbContext (IdentityDbContext)
// 3. Registrera DbContext genom builder.Services.AddDbContext och lägg in connection string
// 4. Starta database (i exempelvis Docker)
// 5. Skapa en migration (dotnet ef migrations add <namn>)
// 6. Uppdatera databas med migration (dotnet ef database update)
// 7. Registrera autentisering med builder.Services.AddAuthentication
// 8. Aktivera autentisering med app.UseAuthentication
// 9. Registrera IdentityCore med builder.Services.AddIdentityCore
// 10. Aktivera IdentityCore endpoints (register + login) med app.MapIdentityCore

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDbContext<ApplicationContext>(options =>
        {
            options.UseNpgsql(
                "Host=localhost;Database=teamslektion6;Username=postgres;Password=password"
            );
        });

        builder.Services.AddAuthentication().AddBearerToken(IdentityConstants.BearerScheme);

        builder
            .Services.AddIdentityCore<User>()
            .AddEntityFrameworkStores<ApplicationContext>()
            .AddApiEndpoints();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.MapIdentityApi<User>();

        app.Run();
    }
}

public class User : IdentityUser { }

public class ApplicationContext : IdentityDbContext<User>
{
    public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options) { }
}

*/

/*

// # Övning 2

// 1. Skapa endpoints och lägg på [Authorize]
// 2. Registrera auktorisering med builder.Services.AddAuthorization
// 3. Aktivera auktorisering med app.UseAuthorization

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDbContext<ApplicationContext>(options =>
        {
            options.UseNpgsql(
                "Host=localhost;Database=teamslektion6;Username=postgres;Password=password"
            );
        });

        builder.Services.AddAuthentication().AddBearerToken(IdentityConstants.BearerScheme);
        builder.Services.AddAuthorization(options => { });

        builder
            .Services.AddIdentityCore<User>()
            .AddEntityFrameworkStores<ApplicationContext>()
            .AddApiEndpoints();

        builder.Services.AddControllers();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.MapIdentityApi<User>();
        app.MapControllers();
        app.UseAuthorization();

        app.Run();
    }
}

public class User : IdentityUser { }

public class ApplicationContext : IdentityDbContext<User>
{
    public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options) { }
}

[ApiController]
[Route("")]
public class TestController : ControllerBase
{
    ApplicationContext context;

    public TestController(ApplicationContext context)
    {
        this.context = context;
    }

    [HttpGet]
    [Authorize]
    public string GetUserName()
    {
        string? name = User.FindFirstValue(ClaimTypes.Name);
        return name;
    }

    [HttpGet("password")]
    [Authorize]
    public string GetPassword()
    {
        string? id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        User? user = context.Users.Find(id);
        return user.PasswordHash;
    }
}

*/

/*

// # Övning 3

// 1. Skapa endpoint med [Authorize]
// 2. Skapa policy (admin)
// 3. Registrera och aktivera roller med .AddRoles<IdentityRole>
// 4. Skapa roller och koppla till användare med hjälp av UserManager och RoleManager

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDbContext<ApplicationContext>(options =>
        {
            options.UseNpgsql(
                "Host=localhost;Database=teamslektion6;Username=postgres;Password=password"
            );
        });

        builder.Services.AddControllers();
        builder.Services.AddTransient<IClaimsTransformation, MyClaimsTransformation>();

        builder.Services.AddAuthentication().AddBearerToken(IdentityConstants.BearerScheme);
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy(
                "admin",
                policy =>
                {
                    policy.RequireRole("admin");
                }
            );
        });

        builder
            .Services.AddIdentityCore<User>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationContext>()
            .AddApiEndpoints();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.MapIdentityApi<User>();
        app.MapControllers();
        app.UseAuthorization();

        app.Run();
    }
}

public class MyClaimsTransformation : IClaimsTransformation
{
    UserManager<User> userManager;

    public MyClaimsTransformation(UserManager<User> userManager)
    {
        this.userManager = userManager;
    }

    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        ClaimsIdentity claims = new ClaimsIdentity();

        var id = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (id != null)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                var userRoles = await userManager.GetRolesAsync(user);
                foreach (var userRole in userRoles)
                {
                    claims.AddClaim(new Claim(ClaimTypes.Role, userRole));
                }
            }
        }

        principal.AddIdentity(claims);
        return await Task.FromResult(principal);
    }
}

public class User : IdentityUser { }

public class ApplicationContext : IdentityDbContext<User>
{
    public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options) { }
}

[ApiController]
[Route("")]
public class TestController : ControllerBase
{
    ApplicationContext context;
    RoleManager<IdentityRole> roleManager;
    UserManager<User> userManager;

    public TestController(
        ApplicationContext context,
        RoleManager<IdentityRole> roleManager,
        UserManager<User> userManager
    )
    {
        this.context = context;
        this.roleManager = roleManager;
        this.userManager = userManager;
    }

    [HttpPost]
    public async Task CreateRoles()
    {
        IdentityRole? existingRole = await roleManager.FindByNameAsync("admin");

        if (existingRole == null)
        {
            await roleManager.CreateAsync(new IdentityRole("admin"));
        }

        User? user = await userManager.FindByEmailAsync("ironman@stark.com");

        await userManager.AddToRoleAsync(user, "admin");
    }

    [HttpGet]
    [Authorize]
    public string GetUserName()
    {
        string? name = User.FindFirstValue(ClaimTypes.Name);
        return name;
    }

    [HttpGet("password")]
    [Authorize]
    public string GetPassword()
    {
        string? id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        User? user = context.Users.Find(id);
        return user.PasswordHash;
    }

    [HttpGet("adminOnly")]
    [Authorize("admin")]
    public string OnlyUsableByAdmins()
    {
        return "Hello!";
    }
}

*/
