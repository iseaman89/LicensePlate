using LicensePlateDataShared.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LicensePlateServer.Data;

public class LicensePlateDbContext : IdentityDbContext<ApiUser>
{
    public LicensePlateDbContext(DbContextOptions<LicensePlateDbContext> options) : base(options) { }
    
    public DbSet<Camera> Cameras { get; set; }
    public DbSet<LicensePlate> LicensePlates { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        builder.Entity<IdentityRole>().HasData(
            new IdentityRole 
            { 
                Name = "User", 
                NormalizedName = "USER", 
                Id = "8343074e-8623-4e1a-b0c1-84fb8678c8f3"
            }, 
            new IdentityRole 
            { 
                Name = "Administrator", 
                NormalizedName = "ADMINISTRATOR", 
                Id = "c7ac6cfe-1f10-4baf-b604-cde350db9554"
            });

        var hasher = new PasswordHasher<ApiUser>();

        builder.Entity<ApiUser>().HasData(
            new ApiUser
            {
                Id = "8e448afa-f008-446e-a52f-13c449803c2e",
                UserName = "administrator", 
                NormalizedUserName = "ADMINISTRATOR", 
                FirstName = "System", 
                LastName = "Admin", 
                PasswordHash = hasher.HashPassword(null, "t49SkGh64")
                },
            new ApiUser 
            { 
                Id = "30a24107-d279-4e37-96fd-01af5b38cb27", 
                UserName = "user", 
                NormalizedUserName = "USER", 
                FirstName = "System", 
                LastName = "User", 
                PasswordHash = hasher.HashPassword(null, "User1234")
                });

        builder.Entity<IdentityUserRole<string>>().HasData(
            new IdentityUserRole<string> 
            { 
                RoleId = "8343074e-8623-4e1a-b0c1-84fb8678c8f3", 
                UserId = "30a24107-d279-4e37-96fd-01af5b38cb27"
            },
            new IdentityUserRole<string> 
            { 
                RoleId = "c7ac6cfe-1f10-4baf-b604-cde350db9554", 
                UserId = "8e448afa-f008-446e-a52f-13c449803c2e"
            });               
    }
}