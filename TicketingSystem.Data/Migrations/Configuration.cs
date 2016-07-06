namespace TicketingSystem.Data.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using TicketingSystem.Common;
    public sealed class Configuration : DbMigrationsConfiguration<TicketSystemDbContext>
    {
        private UserManager<User> userManager;
        private IRandomGenerator random;
        public Configuration()
        {
            this.AutomaticMigrationsEnabled = true;
            this.AutomaticMigrationDataLossAllowed = true;
            this.random = new RandomGenerator();
        }

        protected override void Seed(TicketSystemDbContext context)
        {
            this.userManager = new UserManager<User>(new UserStore<User>(context));
            this.SeedRoles(context);
            this.SeedUsers(context);
            this.SeedCategoriesWithTicketsWithComments(context);
        }

        private void SeedRoles(TicketSystemDbContext context)
        {
            context.Roles.AddOrUpdate(x => x.Name, new IdentityRole(GlobalConstants.AdminRole));
            context.SaveChanges();
        }

        private void SeedUsers(TicketSystemDbContext context)
        {

            if (context.Users.Any())
            {
                return;
            }

            for (int i = 0; i < 10; i++)
            {
                var user = new User
                {
                    Email = string.Format("{0}@{1}.com", this.random.RandomString(6, 16),
                    this.random.RandomString(6, 16)),
                    UserName = this.random.RandomString(6, 16)
                };
                var result = this.userManager.Create(user, "123456");

            }

            var adminUser = new User
            {
                Email = "admin@mysite.com",
                UserName = "Administrator"
            };

            this.userManager.Create(adminUser, "admin123456");

            this.userManager.AddToRole(adminUser.Id, GlobalConstants.AdminRole);

        }

        private void SeedCategoriesWithTicketsWithComments(TicketSystemDbContext context)
        {
            if (context.Categories.Any())
            {
                return;
            }

            var image = this.GetSampleImage();
            var users = context.Users.Take(10).ToList();
            for (int i = 0; i < 5; i++)
            {
                var category = new Category
                {
                    Name = this.random.RandomString(5, 20)
                };

                for (int j = 0; j < 10; j++)
                {
                    var ticket = new Ticket
                    {
                        Author = users[this.random.RandomNumber(0, users.Count - 1)],
                        Description = this.random.RandomString(200, 500),
                        Title = this.random.RandomString(5, 40),
                        Image = image,
                        Priority = (PriorityType)this.random.RandomNumber(0, 2)
                    };

                    for (int k = 0; k < 5; k++)
                    {
                        var comment = new Comment
                        {
                            Author = users[this.random.RandomNumber(0, users.Count - 1)],
                            Content = this.random.RandomString(100, 200)
                        };

                        ticket.Comments.Add(comment);
                        
                    }
                    category.Tickets.Add(ticket);
                }
                context.Categories.Add(category);
                context.SaveChanges();
            }
        }

        private Image GetSampleImage()
        {
            var directory = AssemblyHelpers.GetDirectoryForAssembly(Assembly.GetExecutingAssembly());

            var file = File.ReadAllBytes(directory + "/Migrations/images/Doge.jpg");
            var image = new Image
            {
                Content = file,
                FileExtension = "jpg"
            };

            return image;
        }
    }
}
