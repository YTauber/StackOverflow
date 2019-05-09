using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace StackOverflow.Data
{

    public class PeopleContextFactory : IDesignTimeDbContextFactory<StackContext>
    {
        public StackContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), $"..{Path.DirectorySeparatorChar}StackOverflow.Web"))
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true).Build();

            return new StackContext(config.GetConnectionString("ConStr"));
        }
    }


    public class StackContext : DbContext
    {
        private string _connectionString;
        public StackContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<QuestionsTags> QuestionsTags { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<LikedQuestions> LikedQuestions { get; set; }
        public DbSet<LikedAnswers> LikedAnswers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }


            modelBuilder.Entity<QuestionsTags>()
                .HasKey(qt => new { qt.QuestionId, qt.TagId });

            modelBuilder.Entity<QuestionsTags>()
                .HasOne(qt => qt.Question)
                .WithMany(q => q.QuestionsTags)
                .HasForeignKey(qt => qt.QuestionId);

            modelBuilder.Entity<QuestionsTags>()
                .HasOne(qt => qt.Tag)
                .WithMany(t => t.QuestionsTags)
                .HasForeignKey(qt => qt.TagId);



            modelBuilder.Entity<LikedQuestions>()
                .HasKey(lq => new { lq.QuestionId, lq.UserId });

            modelBuilder.Entity<LikedQuestions>()
                .HasOne(lq => lq.Question)
                .WithMany(q => q.LikedQuestions)
                .HasForeignKey(lq => lq.QuestionId);

            modelBuilder.Entity<LikedQuestions>()
                .HasOne(lq => lq.User)
                .WithMany(u => u.LikedQuestions)
                .HasForeignKey(lq => lq.UserId);



            modelBuilder.Entity<LikedAnswers>()
                .HasKey(la => new { la.AnswerId, la.UserId });

            modelBuilder.Entity<LikedAnswers>()
                .HasOne(la => la.Answer)
                .WithMany(a => a.LikedAnswers)
                .HasForeignKey(la => la.AnswerId);

            modelBuilder.Entity<LikedAnswers>()
                .HasOne(la => la.User)
                .WithMany(u => u.LikedAnswers)
                .HasForeignKey(la => la.UserId);
        }
    }
}
   

