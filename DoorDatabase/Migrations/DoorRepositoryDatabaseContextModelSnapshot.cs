﻿// <auto-generated />
using Everbridge.ControlCenter.TechnicalChallenge.DoorDatabase;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Everbridge.ControlCenter.TechnicalChallenge.DoorDatabase.Migrations
{
    [DbContext(typeof(DoorRepositoryDatabaseContext))]
    partial class DoorRepositoryDatabaseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.7")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Everbridge.ControlCenter.TechnicalChallenge.DoorDatabase.DoorRecord", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)")
                        .HasColumnName("Id");

                    b.Property<bool>("IsLocked")
                        .HasColumnType("bit")
                        .HasColumnName("IsLocked");

                    b.Property<bool>("IsOpen")
                        .HasColumnType("bit")
                        .HasColumnName("IsOpen");

                    b.Property<string>("Label")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)")
                        .HasColumnName("Label");

                    b.HasKey("Id");

                    b.ToTable("DoorRecord");
                });
#pragma warning restore 612, 618
        }
    }
}