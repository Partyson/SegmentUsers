using Microsoft.EntityFrameworkCore;
using SegmentUsers.Domain.Entities;
using SegmentUsers.Infrastructure.Data;

namespace SegmentUsers.Infrastructure.Helpers;

public static class DataSeeder
{
    public static async Task SeedVkUsersAsync(AppDbContext context)
    {
        if (await context.VkUsers.AnyAsync())
            return;

        var names = new[] { "Ivan", "Anna", "Sergey", "Elena", "Dmitry", "Olga", "Alexey", "Marina", "Nikolay", "Tatiana", "Oleg", "Svetlana", "Andrey", "Natalia", "Victor", "Ekaterina", "Mikhail", "Yulia", "Denis", "Irina" };
        var lastNames = new[] { "Petrov", "Ivanova", "Smirnov", "Kuznetsova", "Popov", "Sokolova", "Lebedev", "Kozlova", "Morozov", "Novikova", "Mikhailov", "Fedorova", "Semenov", "Egorova", "Nikolaev", "Solovyova", "Bogdanov", "Zaitseva", "Vinogradov", "Belova" };

        var random = new Random();

        var users = Enumerable.Range(1, 100).Select(i =>
        {
            var firstName = names[random.Next(names.Length)];
            var lastName = lastNames[random.Next(lastNames.Length)];

            return new VkUser
            {
                Id = Guid.NewGuid(),
                Name = firstName,
                LastName = lastName,
                Email = $"{firstName.ToLower()}.{lastName.ToLower()}{i}@example.com",
            };
        });

        await context.VkUsers.AddRangeAsync(users);
        await context.SaveChangesAsync();
    }
}