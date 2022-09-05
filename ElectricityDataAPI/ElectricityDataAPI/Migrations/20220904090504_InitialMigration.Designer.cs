﻿// <auto-generated />
using System;
using ElectricityDataAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ElectricityDataAPI.Migrations
{
    [DbContext(typeof(ElectricityDbContext))]
    [Migration("20220904090504_InitialMigration")]
    partial class InitialMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("ElectricityDataAPI.Models.ElectricityReport", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<float?>("ConsumedElectricity")
                        .HasColumnType("real");

                    b.Property<float?>("GeneratedElectricity")
                        .HasColumnType("real");

                    b.Property<int>("RealEstateObjectNumber")
                        .HasColumnType("int");

                    b.Property<DateTime>("Time")
                        .HasColumnType("datetime2(0)");

                    b.HasKey("Id");

                    b.HasIndex("RealEstateObjectNumber");

                    b.ToTable("ElectricityReports");
                });

            modelBuilder.Entity("ElectricityDataAPI.Models.RealEstate", b =>
                {
                    b.Property<int>("ObjectNumber")
                        .HasColumnType("int");

                    b.Property<byte>("HouseType")
                        .HasColumnType("tinyint");

                    b.Property<byte>("ObjectType")
                        .HasColumnType("tinyint");

                    b.Property<string>("Region")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("ObjectNumber");

                    b.ToTable("RealEstates");
                });

            modelBuilder.Entity("ElectricityDataAPI.Models.ElectricityReport", b =>
                {
                    b.HasOne("ElectricityDataAPI.Models.RealEstate", "RealEstate")
                        .WithMany("ElectricityReports")
                        .HasForeignKey("RealEstateObjectNumber")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("RealEstate");
                });

            modelBuilder.Entity("ElectricityDataAPI.Models.RealEstate", b =>
                {
                    b.Navigation("ElectricityReports");
                });
#pragma warning restore 612, 618
        }
    }
}
