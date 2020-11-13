﻿// <auto-generated />
using System;
using CosemWebApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CosemWebApi.Migrations
{
    [DbContext(typeof(CosemObjectDbContext))]
    partial class CosemObjectContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.0");

            modelBuilder.Entity("CosemWebApi.Entities.CosemObject", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<int>("ClassId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<string>("Obis")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("CosemObjects");

                    b.HasData(
                        new
                        {
                            Id = new Guid("bbdee09c-089b-4d30-bece-44df5923716c"),
                            ClassId = 1,
                            Name = "Local Time",
                            Obis = "1.0.0.9.1.255"
                        });
                });
#pragma warning restore 612, 618
        }
    }
}