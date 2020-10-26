using System;
using FluentValidaiton.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FluentValidation.EntityFrameworkCore.Tests.Models
{
    public class EntityContext : DbContext
    {
        private readonly IServiceProvider serviceProvider;

        public DbSet<Entity> Entities => Set<Entity>();

        public EntityContext(DbContextOptions<EntityContext> options, IServiceProvider serviceProvider)
            : base(options)
        {
            this.serviceProvider = serviceProvider;            
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationFromFluentValidations(serviceProvider);

            base.OnModelCreating(modelBuilder);
        }
    }
}