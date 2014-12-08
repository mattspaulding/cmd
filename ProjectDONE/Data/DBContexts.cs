using Microsoft.AspNet.Identity.EntityFramework;
using ProjectDONE.Models;
using ProjectDONE.Models.AppModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ProjectDONE.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Job> Jobs { get; set; }
        public DbSet<Bid> Bids { get; set; }
        public DbSet<Owner> Owners { get; set; }
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
           
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}