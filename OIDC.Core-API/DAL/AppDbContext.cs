using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using OAuthServer.DAL.Entities;

namespace OAuthServer.DAL
{
    public class AppDbContext : DbContext
    {
        public virtual DbSet<AccessToken> AccessTokens { get; set; }
        public virtual DbSet<Application> Applications { get; set; }
        public virtual DbSet<AuthorisationCode> AuthorisationCodes { get; set; }
        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Scope> Scopes { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserApplication> UserApplications { get; set; }
        public virtual DbSet<UserApplicationScope> UserApplicationScopes { get; set; }
        // public virtual DbSet<UserRole> UserRoles { get; set; }

        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AccessToken>(builder =>
            {
                builder.HasOne(at => at.Application);
                builder.HasOne(at => at.User)
                    .WithMany(u => u.AccessTokens)
                    .HasForeignKey(at => at.UserId);
            });

            modelBuilder.Entity<Application>(builder =>
            {
                builder.HasOne(a => a.Author)
                    .WithMany(u => u.Applications)
                    .HasForeignKey(a => a.AuthorId);

                builder.HasMany(a => a.UserApplications)
                    .WithOne(ua => ua.Application)
                    .HasForeignKey(au => au.ApplicationId);
            });

            modelBuilder.Entity<AuthorisationCode>(builder =>
            {
                builder.HasOne(ac => ac.User)
                    .WithMany(u => u.AuthorisationCodes)
                    .HasForeignKey(ac => ac.UserId);

                // Link between authorisation code and application?
                builder.HasOne(ac => ac.Application);
            });

            modelBuilder.Entity<RefreshToken>(builder =>
            {
                builder.HasOne(rt => rt.User)
                    .WithMany(u => u.RefreshTokens)
                    .HasForeignKey(rt => rt.UserId);

                builder.HasOne(rt => rt.Application);
            });

            modelBuilder.Entity<UserApplication>(builder =>
            {
                builder.HasOne(ua => ua.User)
                    .WithMany(u => u.UserApplications)
                    .HasForeignKey(ua => ua.UserId);

                builder.HasMany(ua => ua.Scopes)
                    .WithOne(uas => uas.UserApplication)
                    .HasForeignKey(uas => uas.UserApplicationId);

                builder.HasOne(ua => ua.Application);
            });

            modelBuilder.Entity<UserApplicationScope>(builder =>
            {
                builder.HasKey(uas => new {uas.UserApplicationId, uas.ScopeId});

                builder.HasOne(uas => uas.UserApplication)
                    .WithMany(ua => ua.Scopes)
                    .HasForeignKey(ua => ua.UserApplicationId);

                builder.HasOne(uas => uas.Scope);
            });

            modelBuilder.Entity<User>(builder =>
            {
                builder.HasIndex(u => u.Email).IsUnique();
                builder.HasMany(u => u.Roles)
                        .WithMany(r => r.Users);
            });

            modelBuilder.Entity<Role>(builder =>
            {
                builder.HasData(new Role
                {
                    Name = "user",
                    Users = new List<User>()
                });

                builder.HasData(new Role
                {
                    Name = "admin",
                    Users = new List<User>()
                });
            });

            modelBuilder.Entity<Scope>(builder =>
            {
                builder.HasData(new Scope
                {
                    Name = "profile.read",
                    Label = "Read Profile",
                    Description = "Allows read-only access to your profile information",
                    Dangerous = false,
                    Icon = "account"
                });

                builder.HasData(new Scope
                {
                    Name = "profile.write",
                    Label = "Update Profile",
                    Description =
                        "Allows modification of details on your profile, not including your password or email address",
                    Dangerous = false,
                    Icon = "true"
                });

                builder.HasData(new Scope
                {
                    Name = "applications.read",
                    Label = "List applications you have published",
                    Description = "Allows listing applications you have published",
                    Dangerous = false,
                    Icon = "true"
                });
            });
        }
    }
}