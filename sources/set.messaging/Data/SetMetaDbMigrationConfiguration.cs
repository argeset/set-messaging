using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;

using set.messaging.Data.Entities;
using set.messaging.Helpers;

namespace set.messaging.Data
{
    public class SetMessagingDBMigrationConfiguration : DbMigrationsConfiguration<SetMessagingDBContext>
    {
        public SetMessagingDBMigrationConfiguration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(SetMessagingDBContext context)
        {
            if (context.Users.Any()) return;

            AddUser(context, ConstHelper.Admin, "admin@test.com", ConstHelper.Admin);

            context.SaveChanges();

            AddApp(context, "Set Crm", "the main crm app", "setcrm.com");
            AddApp(context, "Set Locale", "localization management app", "locale.setcrm.com");
            AddApp(context, "Set Messaging", "message sending app", "msg.setcrm.com");

            context.SaveChanges();
        }

        private static void AddApp(SetMessagingDBContext context, string appName, string description, string url)
        {
            var user = context.Users.First();

            var app = new App
            {
                UserId = user.Id,
                Name = appName,
                Description = description,
                Url = url,
                CreatedBy = user.Id,

                Tokens = new List<Token>
                {
                    new Token
                    {
                        CreatedBy = user.Id,
                        Id = Guid.NewGuid().ToNoDashString(),
                        UsageCount = 0,
                        IsAppActive = true
                    }
                }
            };

            context.Apps.Add(app);
        }

        private static void AddUser(SetMessagingDBContext context, string name, string email, string role)
        {
            var user = new User
            {
                Email = email,
                Name = name,
                RoleId = ConstHelper.BasicRoles[role],
                RoleName = role,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password"),
                LastLoginAt = DateTime.Now,
                Language = ConstHelper.CultureNameEN
            };

            context.Users.Add(user);
        }
    }
}