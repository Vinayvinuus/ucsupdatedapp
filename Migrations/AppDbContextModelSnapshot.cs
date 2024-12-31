﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using ucsUpdatedApp.Data;

#nullable disable

namespace ucsUpdatedApp.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("ucsUpdatedApp.Models.MasterTable", b =>
                {
                    b.Property<int>("MasterId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("MasterId"));

                    b.Property<DateTime?>("DOB")
                        .HasColumnType("DATE");

                    b.Property<DateTime?>("DOJ")
                        .HasColumnType("DATE");

                    b.Property<int>("EmployeeId")
                        .HasColumnType("integer");

                    b.Property<string>("Employeename")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("FingerPrintData")
                        .HasColumnType("text");

                    b.Property<DateTime?>("LastTransactionDate")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("MasterId");

                    b.ToTable("MasterTable");
                });

            modelBuilder.Entity("ucsUpdatedApp.Models.TransactionTable", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("CheckInMethod")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime?>("LatestTransactionDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("MasterId")
                        .HasColumnType("integer");

                    b.Property<int>("Op")
                        .HasColumnType("integer");

                    b.Property<DateTime>("OpDateTime")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("MasterId");

                    b.ToTable("TransactionTable");
                });

            modelBuilder.Entity("ucsUpdatedApp.Models.TransactionTable", b =>
                {
                    b.HasOne("ucsUpdatedApp.Models.MasterTable", "Master")
                        .WithMany("Transactions")
                        .HasForeignKey("MasterId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Master");
                });

            modelBuilder.Entity("ucsUpdatedApp.Models.MasterTable", b =>
                {
                    b.Navigation("Transactions");
                });
#pragma warning restore 612, 618
        }
    }
}
