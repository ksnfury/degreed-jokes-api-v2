﻿using Microsoft.EntityFrameworkCore;
using JokeApiV2.Models;

namespace JokeApiV2.Data
{
    public class JokeDbContext : DbContext
    {
        public DbSet<Joke> Jokes { get; set; }

        public JokeDbContext(DbContextOptions<JokeDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure the Joke entity
            modelBuilder.Entity<Joke>()
                .HasKey(j => j.Id);

            modelBuilder.Entity<Joke>()
                .Property(j => j.Text)
                .IsRequired();

            modelBuilder.Entity<Joke>()
                .Property(j => j.Length)
                .IsRequired();

            base.OnModelCreating(modelBuilder);
        }
    }
}