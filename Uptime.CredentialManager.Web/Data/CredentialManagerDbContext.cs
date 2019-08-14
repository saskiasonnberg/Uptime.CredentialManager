using Microsoft.EntityFrameworkCore;
using System;

namespace Uptime.CredentialManager.Web.Models
{
    public class CredentialManagerDbContext : DbContext
    {       
        public CredentialManagerDbContext (DbContextOptions<CredentialManagerDbContext> options)
            : base(options)
        {
        }

        public DbSet<Credential> Credentials { get; set; }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(new[] { 
                new 
                {
                    Id = Guid.Parse("5327d54b-aa1b-41c2-9b25-be98c7bee83b"),
                    Name = "Kersti",
                    Role = "User",
                },
                new
                {
                    Id = Guid.Parse("d78df0a6-f730-4b81-a1a9-010db7c8d5b0"),
                    Name = "Redi",
                    Role = "User",
                },
                new 
                {
                    Id = Guid.Parse("33223aa1-88d7-4a5f-9765-8f9b205c69a6"),
                    Name = "Helmet",
                    Role = "User",
                },
                new
                {
                    Id = Guid.Parse("2f137865-fe52-42cc-92b9-3b9b4291297d"),
                    Name = "saskia.sonnberg@uptime.eu",
                    Role = "Admin",
                }
            });

            modelBuilder.Entity<Credential>().HasData(new[] {
                new
                {
                    Id = Guid.Parse("199aba72-624e-422e-9197-e9a820461e59"),
                    Description = "server1",
                    Value = "parool1",
                },
                new
                {
                    Id = Guid.Parse("af5c2167-31fa-4ce2-9b6d-b296891dcef0"),
                    Description = "server2",
                    Value = "parool2",
                },
                new
                {
                    Id = Guid.Parse("9127b2df-7c8a-459d-a4b8-aa73ab6f5f9f"),
                    Description = "server3",
                    Value = "parool3",
                },
                new
                {
                    Id = Guid.Parse("a400754b-799b-480d-8dcf-d61307ad4f2b"),
                    Description = "server4",
                    Value = "parool4",
                }
            });
        }
    }
}
