using FrozenFishCatalog.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace FrozenFishCatalog.Data;

public static class DbSeeder
{
    public static void Seed(ApplicationDbContext context)
    {
        if (context.Categories.Any())
            return;

        var categories = new List<Category>
        {
            new() { Name = "Pescado Blanco", Description = "Pescado de sabor suave con carne blanca, perfecto para diversos métodos de cocción", ImageUrl = "/images/categories/white-fish.jpg" },
            new() { Name = "Pescado Azul", Description = "Rico en ácidos grasos omega-3, ideal para asar y hornear", ImageUrl = "/images/categories/oily-fish.jpg" },
            new() { Name = "Mariscos", Description = "Mariscos de primera calidad, congelados en su punto óptimo de frescura", ImageUrl = "/images/categories/shellfish.jpg" },
            new() { Name = "Pescado Exótico", Description = "Pescados únicos y especiales de todo el mundo", ImageUrl = "/images/categories/exotic-fish.jpg" }
        };

        context.Categories.AddRange(categories);
        context.SaveChanges();

        var products = new List<Product>
        {
            // Pescado Blanco
            new() { Name = "Filete de Bacalao del Atlántico", Description = "Filetes premium de bacalao del Atlántico, sin piel y sin espinas. Sabor suave y dulce, perfecto para fish and chips o al horno.", ImageUrl = "/images/products/cod.jpg", CategoryId = categories[0].Id },
            new() { Name = "Filete de Eglefino", Description = "Eglefino fresco-congelado con un sabor ligeramente dulce. Excelente para ahumar o freír.", ImageUrl = "/images/products/haddock.jpg", CategoryId = categories[0].Id },
            new() { Name = "Rodaja de Halibut", Description = "Rodajas gruesas de halibut con textura firme y escamosa. Una opción premium para asar.", ImageUrl = "/images/products/halibut.jpg", CategoryId = categories[0].Id },
            new() { Name = "Filete de Róbalo", Description = "Filetes delicados de róbalo con textura mantequillosa. Perfecto para cocinar al vapor o al horno.", ImageUrl = "/images/products/seabass.jpg", CategoryId = categories[0].Id },

            // Pescado Azul
            new() { Name = "Filete de Salmón Noruego", Description = "Salmón rico y graso de las frías aguas de Noruega. Perfecto para asar, hornear o sushi.", ImageUrl = "/images/products/salmon.jpg", CategoryId = categories[1].Id },
            new() { Name = "Filete de Caballa", Description = "Caballa de sabor intenso, alta en omega-3. Ideal para asar o ahumar.", ImageUrl = "/images/products/mackerel.jpg", CategoryId = categories[1].Id },
            new() { Name = "Sardinas", Description = "Sardinas enteras llenas de nutrientes. Perfectas para asar al estilo mediterráneo.", ImageUrl = "/images/products/sardines.jpg", CategoryId = categories[1].Id },
            new() { Name = "Rodaja de Atún", Description = "Rodajas de atún grado sashimi. Mejor servido sellado de poco a medio.", ImageUrl = "/images/products/tuna.jpg", CategoryId = categories[1].Id },

            // Mariscos
            new() { Name = "Langostinos Gigantes", Description = "Langostinos jumbo, pelados y desvenados. Listos para tus recetas favoritas.", ImageUrl = "/images/products/prawns.jpg", CategoryId = categories[2].Id },
            new() { Name = "Mejillones", Description = "Mejillones azules frescos-congelados en concha. Perfectos para moules marinière.", ImageUrl = "/images/products/mussels.jpg", CategoryId = categories[2].Id },
            new() { Name = "Ostiones", Description = "Ostiones de buceo con sabor dulce y delicado. Saltéalos para mejores resultados.", ImageUrl = "/images/products/scallops.jpg", CategoryId = categories[2].Id },
            new() { Name = "Colas de Langosta", Description = "Colas de langosta premium de aguas frías. El lujo máximo en mariscos.", ImageUrl = "/images/products/lobster.jpg", CategoryId = categories[2].Id },

            // Pescado Exótico
            new() { Name = "Rodaja de Pez Espada", Description = "Rodajas carnosas de pez espada con textura firme. Excelente para asar.", ImageUrl = "/images/products/swordfish.jpg", CategoryId = categories[3].Id },
            new() { Name = "Filete de Mahi-Mahi", Description = "Mahi-mahi tropical con sabor suave y dulce. Ideal para tacos y a la parrilla.", ImageUrl = "/images/products/mahimahi.jpg", CategoryId = categories[3].Id },
            new() { Name = "Filete de Huachinango", Description = "Huachinango del Caribe con sabor delicado y a nuez. Perfecto entero o en filetes.", ImageUrl = "/images/products/redsnapper.jpg", CategoryId = categories[3].Id },
            new() { Name = "Cola de Rape", Description = "Conocido como 'langosta del pobre' por su carne firme y dulce. Excelente asado.", ImageUrl = "/images/products/monkfish.jpg", CategoryId = categories[3].Id }
        };

        context.Products.AddRange(products);
        context.SaveChanges();

        var productWeights = new List<ProductWeight>();
        foreach (var product in products)
        {
            var basePrice = GetBasePrice(product.Name);
            productWeights.Add(new ProductWeight { ProductId = product.Id, WeightKg = 1, Price = basePrice, StockQuantity = 50 });
            productWeights.Add(new ProductWeight { ProductId = product.Id, WeightKg = 2, Price = basePrice * 1.9m, StockQuantity = 30 });
            productWeights.Add(new ProductWeight { ProductId = product.Id, WeightKg = 3, Price = basePrice * 2.7m, StockQuantity = 20 });
        }

        context.ProductWeights.AddRange(productWeights);
        context.SaveChanges();
    }

    public static async Task SeedAdminAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

        if (!await roleManager.RoleExistsAsync("Admin"))
            await roleManager.CreateAsync(new IdentityRole("Admin"));

        const string adminEmail = "admin@pescacisne.cl";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FirstName = "Admin",
                LastName = "Pesca Cisne",
                EmailConfirmed = true
            };
            await userManager.CreateAsync(adminUser, "Admin123!");
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }

        if (!context.PaymentTypes.Any())
        {
            context.PaymentTypes.AddRange(
                new PaymentType { Name = "Tarjeta de Crédito", Description = "Visa, Mastercard, American Express", Icon = "bi-credit-card", IsActive = true, DisplayOrder = 1 },
                new PaymentType { Name = "Tarjeta de Débito", Description = "Débito bancario directo", Icon = "bi-credit-card-2-front", IsActive = true, DisplayOrder = 2 },
                new PaymentType { Name = "Transferencia Bancaria", Description = "Transferencia directa a cuenta bancaria", Icon = "bi-bank", IsActive = true, DisplayOrder = 3 },
                new PaymentType { Name = "Efectivo contra entrega", Description = "Pago en efectivo al momento de la entrega", Icon = "bi-cash", IsActive = false, DisplayOrder = 4 }
            );
            await context.SaveChangesAsync();
        }
    }

    private static decimal GetBasePrice(string productName) => productName switch
    {
        "Filete de Bacalao del Atlántico" => 18.99m,
        "Filete de Eglefino" => 17.99m,
        "Rodaja de Halibut" => 32.99m,
        "Filete de Róbalo" => 28.99m,
        "Filete de Salmón Noruego" => 24.99m,
        "Filete de Caballa" => 12.99m,
        "Sardinas" => 8.99m,
        "Rodaja de Atún" => 34.99m,
        "Langostinos Gigantes" => 26.99m,
        "Mejillones" => 9.99m,
        "Ostiones" => 38.99m,
        "Colas de Langosta" => 54.99m,
        "Rodaja de Pez Espada" => 29.99m,
        "Filete de Mahi-Mahi" => 27.99m,
        "Filete de Huachinango" => 31.99m,
        "Cola de Rape" => 36.99m,
        _ => 19.99m
    };
}
