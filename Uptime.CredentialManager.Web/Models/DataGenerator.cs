using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Uptime.CredentialManager.Web.Models
{
    public class DataGenerator
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new UptimeCredentialManagerWebContext(
               serviceProvider.GetRequiredService<DbContextOptions<UptimeCredentialManagerWebContext>>()))
            {
                // Look for any credentials already in database.
                if (context.Credential.Any())
                {
                    return;   // Database has been seeded
                }

                context.Credential.AddRange(
                                    new Credential
                                    {
                                        Id = Guid.NewGuid(),
                                        Description = "server1",
                                        Value = "parool1"                                                                                     
                                    },
                                    new Credential
                                    {
                                        Id = Guid.NewGuid(),
                                        Description = "server2",
                                        Value = "parool2"
                                    },
                                    new Credential
                                     {
                                        Id = Guid.NewGuid(),
                                        Description = "server3",
                                        Value = "parool3"
                                    },
                                    new Credential
                                    {
                                        Id = Guid.NewGuid(),
                                        Description = "server4",
                                        Value = "parool4"
                                    });

                // Look for any users already in database.
                if (context.User.Any())
                {
                    return;   // Database has been seeded
                }

                context.User.AddRange(
                                    new User
                                    {
                                        Id = Guid.NewGuid(),
                                        Name = "Kersti",
                                    },
                                    new User
                                    {
                                        Id = Guid.NewGuid(),
                                        Name = "Redi",
                                    },
                                    new User
                                    {
                                        Id = Guid.NewGuid(),
                                        Name = "Helmet",
                                    },
                                    new User
                                    {
                                          Id = Guid.NewGuid(),
                                          Name = "Saskia",
                                    });


                context.SaveChanges();


            }
        }
    }
}
