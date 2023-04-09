using Microsoft.EntityFrameworkCore;
using BlazorApp1.Shared;

namespace BlazorApp1.Server
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options) { }
        public DbSet<answer> answer { get; set; }
        public DbSet<answered_question> answered_question { get; set; }
        public DbSet<given_answer> given_answer { get; set; }
        public DbSet<land> land { get; set; }
        public DbSet<player> Player { get; set; }
        public DbSet<question> question { get; set; }
        public DbSet<question_answer> question_answer { get; set; }
        public DbSet<question_tag> question_tag { get; set; }
        public DbSet<realm> realm { get; set; }
        public DbSet<tag> tag { get; set; }
    }
}