using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using YmKB.Domain.Abstractions.Identities;
using YmKB.Domain.Abstractions.SystemEntities;
using YmKB.Domain.Entities.Trial;

namespace YmKB.Infrastructure.Persistence.Seed;

public class ApplicationDbContextInitializer
{
    private readonly ILogger<ApplicationDbContextInitializer> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public ApplicationDbContextInitializer(
        ILogger<ApplicationDbContextInitializer> logger,
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager
    )
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task InitialiseAsync()
    {
        try
        {
            // 检查当前使用的数据库是否为关系型数据库，如果是则执行迁移操作。
            if (_context.Database.IsRelational())
                await _context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await SeedUsersAsync();
            await SeedDataAsync();
            _context.ChangeTracker.Clear();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database");
            throw;
        }
    }

    private async Task SeedUsersAsync()
    {
        if (!await _context.Tenants.AnyAsync())
        {
            var tenants = new List<Tenant>()
            {
                new()
                {
                    Name = "Org - 1",
                    Description = "Organization 1",
                    Id = Guid.CreateVersion7().ToString()
                },
                new()
                {
                    Name = "Org - 2",
                    Description = "Organization 2",
                    Id = Guid.CreateVersion7().ToString()
                }
            };
            _context.Tenants.AddRange(tenants);
            await _context.SaveChangesAsync();
        }

        if (await _userManager.Users.AnyAsync())
            return;
        
        // 创建角色
        if (!await _roleManager.RoleExistsAsync("Admin"))
        {
            await _roleManager.CreateAsync(new IdentityRole("Admin"));
        }
        if (!await _roleManager.RoleExistsAsync("User"))
        {
            await _roleManager.CreateAsync(new IdentityRole("User"));
        }
        
        var tenantId = _context.Tenants.First().Id;
        var defaultPassword = "P@ssw0rd!";
        _logger.LogInformation("Seeding users...");
        var adminUser = new ApplicationUser
        {
            UserName = "Administrator",
            Provider = "Local",
            TenantId = tenantId,
            Nickname = "Administrator",
            Email = "admin@example.com",
            EmailConfirmed = true,
            LanguageCode = "en-US",
            TimeZoneId = "Asia/Shanghai",
            TwoFactorEnabled = false
        };

        var demoUser = new ApplicationUser
        {
            UserName = "Demo",
            Provider = "Local",
            TenantId = tenantId,
            Nickname = "Demo",
            Email = "Demo@example.com",
            EmailConfirmed = true,
            LanguageCode = "en-US",
            TimeZoneId = "Asia/Shanghai",
            TwoFactorEnabled = false,
            SuperiorId = adminUser.Id
        };

        await _userManager.CreateAsync(adminUser, defaultPassword);
        await _userManager.CreateAsync(demoUser, defaultPassword);
        
        // 分配角色
        await _userManager.AddToRoleAsync(adminUser, "Admin");
        await _userManager.AddToRoleAsync(demoUser, "User");
    }

    private async Task SeedDataAsync()
    {
        if (await _context.Products.AnyAsync())
            return;
        _logger.LogInformation("Seeding data...");
        var products = new List<Product>
        {
            new Product
            {
                Name = "Ikea LACK Coffee Table",
                Description =
                    "Simple and stylish coffee table from Ikea, featuring a modern design and durable surface. Perfect for living rooms or offices.",
                Price = 25,
                SKU = "LACK-COFFEE-TABLE",
                UOM = "PCS",
                Currency = "USD",
                Category = ProductCategory.Furniture
            },
            new Product
            {
                Name = "Nike Air Zoom Pegasus 40",
                Description =
                    "Lightweight and responsive running shoes with advanced cushioning and a breathable mesh upper. Ideal for athletes and daily runners.",
                Price = 130,
                SKU = "NIKE-PEGASUS-40",
                UOM = "PCS",
                Currency = "USD",
                Category = ProductCategory.Sports
            },
            new Product
            {
                Name = "Adidas Yoga Mat",
                Description =
                    "Non-slip yoga mat with a 6mm thickness for optimal cushioning and support during workouts. Suitable for yoga, pilates, or general exercises.",
                Price = 45,
                SKU = "ADIDAS-YOGA-MAT",
                UOM = "PCS",
                Currency = "USD",
                Category = ProductCategory.Sports
            },
            new Product
            {
                Name = "Ikea HEMNES Bed Frame",
                Description =
                    "Solid wood bed frame with a classic design. Offers excellent durability and comfort. Compatible with standard-size mattresses.",
                Price = 199,
                SKU = "HEMNES-BED-FRAME",
                UOM = "PCS",
                Currency = "USD",
                Category = ProductCategory.Furniture
            },
            new Product
            {
                Name = "Under Armour Men's HeatGear Compression Shirt",
                Description =
                    "High-performance compression shirt designed to keep you cool and dry during intense workouts. Made from moisture-wicking fabric.",
                Price = 35,
                SKU = "UA-HEATGEAR-SHIRT",
                UOM = "PCS",
                Currency = "USD",
                Category = ProductCategory.Sports
            },
            new Product
            {
                Name = "Apple iPhone 15 Pro",
                Description =
                    "Apple's latest flagship smartphone featuring a 6.1-inch Super Retina XDR display, A17 Pro chip, titanium frame, and advanced camera system with 5x telephoto lens. Ideal for tech enthusiasts and professional users.",
                Price = 1199,
                SKU = "IP15PRO",
                UOM = "PCS",
                Currency = "USD",
                Category = ProductCategory.Electronics
            }
        };

        await _context.Products.AddRangeAsync(products);
        await _context.SaveChangesAsync();
        var stocks = new List<Stock>
        {
            new Stock
            {
                ProductId = products.FirstOrDefault(p => p.Name == "Ikea LACK Coffee Table")?.Id,
                Product = products.FirstOrDefault(p => p.Name == "Ikea LACK Coffee Table"),
                Quantity = 50,
                Location = "FU-WH-0001"
            },
            new Stock
            {
                ProductId = products.FirstOrDefault(p => p.Name == "Nike Air Zoom Pegasus 40")?.Id,
                Product = products.FirstOrDefault(p => p.Name == "Nike Air Zoom Pegasus 40"),
                Quantity = 100,
                Location = "SP-WH-0001"
            },
            new Stock
            {
                ProductId = products.FirstOrDefault(p => p.Name == "Apple iPhone 15 Pro")?.Id,
                Product = products.FirstOrDefault(p => p.Name == "Apple iPhone 15 Pro"),
                Quantity = 200,
                Location = "EL-WH-0001"
            }
        };

        await _context.Stocks.AddRangeAsync(stocks);
        await _context.SaveChangesAsync();
    }
}
