using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using MyEvernote.Entities;

namespace MyEvernote.DataAccessLayer.EntityFramework
{
    class MyInitializer : CreateDatabaseIfNotExists<DatabaseContext>
    {
        //adding admin user
        protected override void Seed(DatabaseContext context)
        {
            EvernoteUser admin = new EvernoteUser()
            {
                Name = "admin",
                Surname = "admin",
                Email = "admin@gmail.com",
                ActivateGuid = Guid.NewGuid(),
                IsActive = true,
                IsAdmin = true,
                Username = "admin",
                ProfileImageFileName = "user.png",
                Password = "123456",
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now.AddMinutes(5),
                ModifiedUsername = "eberge",
            };
            //adding standart user
            EvernoteUser standartUser = new EvernoteUser()
            {
                Name = "Enis",
                Surname = "Berge",
                Email = "enisberge@gmail.com",
                ActivateGuid = Guid.NewGuid(),
                IsActive = true,
                IsAdmin = false,
                Username = "enisberge",
                ProfileImageFileName = "user.png",
                Password = "654321",
                CreatedOn = DateTime.Now.AddHours(1),
                ModifiedOn = DateTime.Now.AddMinutes(65),
                ModifiedUsername = "eberge",
            };

            context.EvernoteUsers.Add(admin);
            context.EvernoteUsers.Add(standartUser);
            context.SaveChanges();
        }
    }
}
