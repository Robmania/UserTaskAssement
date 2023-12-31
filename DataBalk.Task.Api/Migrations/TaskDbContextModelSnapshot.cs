﻿// <auto-generated />
using System;
using DataBalk.Task.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DataBalk.Task.Api.Migrations
{
    [DbContext(typeof(TaskDbContext))]
    partial class TaskDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.10");

            modelBuilder.Entity("DataBalk.Task.Api.Data.Entities.Task", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("ID");

                    b.Property<long>("Assignee")
                        .HasColumnType("INTEGER")
                        .HasColumnName("Assignee");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("Description");

                    b.Property<DateTime?>("DueDate")
                        .HasColumnType("TEXT")
                        .HasColumnName("DueDate");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("Title");

                    b.HasKey("Id");

                    b.ToTable("Task");
                });

            modelBuilder.Entity("DataBalk.Task.Api.Data.Entities.User", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("ID");

                    b.Property<string>("EMail")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("Email");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("Password");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("Username");

                    b.HasKey("Id");

                    b.ToTable("User");
                });
#pragma warning restore 612, 618
        }
    }
}
