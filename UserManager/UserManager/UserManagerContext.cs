using Microsoft.EntityFrameworkCore;
using UserManager.Initializers;
using UserManager.Models;

namespace UserManager
{
    public class UserManagerContext : DbContext
    {
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserGroup> UserGroups { get; set; }
        public virtual DbSet<UserState> UserStates { get; set; }

        public UserManagerContext(DbContextOptions<UserManagerContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserGroup>().HasData(UserGroupInitializer.GetData());
            modelBuilder.Entity<UserState>().HasData(UserStateInitializer.GetData());

            modelBuilder.Entity<User>().HasAlternateKey(user => user.Login);
            modelBuilder.Entity<UserGroup>().HasAlternateKey(userGroup => userGroup.Code);
            modelBuilder.Entity<UserState>().HasAlternateKey(userState => userState.Code);
        }
    }
}
