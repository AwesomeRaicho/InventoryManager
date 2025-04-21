using InventoryManager.Core.Models.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace InventoryManager.Core.Models
{
    public static class ModelBuilderExtension
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            //Roles
            List<ApplicationRole> roles = new List<ApplicationRole>()
            {
                new ApplicationRole(){Name = "Administrator", NormalizedName = "ADMINISTRATOR", Id = Guid.Parse("F2B1B83F-D0A8-4916-94AD-FDE172BF1923")},
                new ApplicationRole(){Name = "Employee", NormalizedName = "EMPLOYEE", Id= Guid.Parse("9535C06C-27D7-42B0-8B10-E0202E6BF6B6")},
            };
            modelBuilder.Entity<ApplicationRole>().HasData(roles);



            //Admin
            ApplicationUser admin = new ApplicationUser()
            {
                Id = Guid.Parse("31D9BA1B-47F4-4A8A-98DE-37CA4A1ADEC5"),
                UserName = "Admin",
                NormalizedUserName = "ADMIN",
                SecurityStamp = Guid.NewGuid().ToString(),
            };
            modelBuilder.Entity<ApplicationUser>().HasData(admin);

            // Users and passwords
            var hasher = new PasswordHasher<ApplicationUser>();
            admin.PasswordHash = hasher.HashPassword(admin, "adminpass");

            //Add admin role
            IdentityUserRole<Guid> adminRole = new IdentityUserRole<Guid>()
            {
                UserId = admin.Id,
                RoleId = roles.First(r => r.Name == "Administrator").Id
            };
            modelBuilder.Entity<IdentityUserRole<Guid>>().HasData(adminRole);

        }

    }
}
