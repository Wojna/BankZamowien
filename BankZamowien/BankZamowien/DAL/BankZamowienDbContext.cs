using BankZamowien.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace BankZamowien.DAL
{
    public class BankZamowienDbContext : DbContext
    {
        public BankZamowienDbContext()
            : base("DefaultConnection")
        {
        }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Inquiry> Inquiries { get; set; }
        public DbSet<Message> Messages { get; set; }
    }
}