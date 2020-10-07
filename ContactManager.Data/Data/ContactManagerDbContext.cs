using Microsoft.EntityFrameworkCore;

namespace ContactManager.Data
{
    public class ContactManagerDbContext : DbContext
    {
        public ContactManagerDbContext(DbContextOptions<ContactManagerDbContext> options)
            : base(options)
        {
        }

        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Email> EmailAddresses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Email>()
                .Property(x => x.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Contact>()
                .Property(x => x.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Email>()
                .HasOne(p => p.Contact)
                .WithMany(b => b.EmailAddresses)
                .HasForeignKey(p => p.ContactForeignKey);

            modelBuilder.Entity<Contact>().HasData(
                new { Id = 1, FirstName = "Bob", LastName = "James" },
                new { Id = 2, FirstName = "Jerome", LastName = "Porter" });

            modelBuilder.Entity<Email>().HasData(
                new { Id = 1, Address = "bjames@kodak.com", Type = Email.EmailAddressType.Business, ContactForeignKey = 1 },
                new { Id = 2, Address = "bjamespersonal@gmail.com", Type = Email.EmailAddressType.Personal, ContactForeignKey = 1 },
                new { Id = 3, Address = "porterj@steaks.com", Type = Email.EmailAddressType.Business, ContactForeignKey = 2 },
                new { Id = 4, Address = "jeromeathome@gmail.com", Type = Email.EmailAddressType.Personal, ContactForeignKey = 2 });
        }
    }
}
