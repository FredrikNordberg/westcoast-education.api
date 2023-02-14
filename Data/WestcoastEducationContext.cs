using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using westcoast_education.api.Models;

namespace westcoast_education.api.Data
{
    public class WestcoastEducationContext : DbContext
    {
        public DbSet<Course> Courses { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Skill> Skill { get; set; }
        public DbSet<StudentCourse> StudentCourse { get; set; }
        public WestcoastEducationContext(DbContextOptions options) : base(options)
        {
        }

         protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Sätt upp sammansatt primärnyckel som består av CourseId och StudentId...
            modelBuilder.Entity<StudentCourse>()
                .HasKey(sc => new { sc.CourseId, sc.StudentId });

            // Sätta upp förhållandet att en student kan var anmäld på flera kurser...
            modelBuilder.Entity<StudentCourse>()
                .HasOne(sc => sc.Student)
                .WithMany(c => c.StudentCourses)
                .HasForeignKey(sc => sc.StudentId);

            // Sätta upp förhållandet att en kurs kan ha flera studenter...
            modelBuilder.Entity<StudentCourse>()
                .HasOne(sc => sc.Course)
                .WithMany(c => c.StudentCourses)
                .HasForeignKey(sc => sc.CourseId);
        }
    }
}