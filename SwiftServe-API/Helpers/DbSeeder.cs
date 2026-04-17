using SwiftServe_API.Data;
using SwiftServe_API.Models;

namespace SwiftServe_API.Helpers
{
    public static class DbSeeder
    {
        public static async Task Seed(AppDbContext context)
        {
            if (!context.Users.Any(x => x.Role == "SuperAdmin"))
            {
                var superAdmin = new User
                {
                    Name = "Nitish Trivedi",
                    Email = "nitish123@test.com",
                    PhoneNumber = "7350804321",
                    PasswordHash = PasswordHelper.Hash("Jinal@3004"),
                    Role = "SuperAdmin"
                };

                context.Users.Add(superAdmin);
                await context.SaveChangesAsync();
            }
        }
    }
}
