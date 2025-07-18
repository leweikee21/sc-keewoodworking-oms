﻿// <auto-generated />
using System;
using Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20250627130738_Removefield")]
    partial class Removefield
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Domain.Entities.Acquisition", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("LastModifiedBy")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<DateTime?>("ReceivedDate")
                        .HasColumnType("timestamp");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("SupplierId")
                        .HasColumnType("integer");

                    b.Property<int>("TotalItems")
                        .HasColumnType("integer");

                    b.Property<string>("filePath")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("SupplierId");

                    b.ToTable("Acquisitions", (string)null);
                });

            modelBuilder.Entity("Domain.Entities.AcquisitionItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("AcquisitionId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<int>("InventoryId")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("LastModifiedBy")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<int>("Quantity")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("AcquisitionId");

                    b.HasIndex("InventoryId");

                    b.ToTable("AcquisitionItems", (string)null);
                });

            modelBuilder.Entity("Domain.Entities.Inbound", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int?>("AcquisitionId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<int>("InventoryId")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("LastModifiedBy")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<int>("Quantity")
                        .HasColumnType("integer");

                    b.Property<int>("RemainingQuantity")
                        .HasColumnType("integer");

                    b.Property<string>("Remark")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<decimal>("TotalPrice")
                        .HasColumnType("decimal(18,6)");

                    b.Property<decimal>("UnitPrice")
                        .HasColumnType("decimal(18,6)");

                    b.HasKey("Id");

                    b.HasIndex("AcquisitionId");

                    b.HasIndex("InventoryId");

                    b.ToTable("Inbounds", (string)null);
                });

            modelBuilder.Entity("Domain.Entities.Inventory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("AvailableQty")
                        .HasColumnType("integer");

                    b.Property<decimal>("AverageUnitPrice")
                        .HasColumnType("decimal(18,6)");

                    b.Property<string>("Category")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<bool>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bool")
                        .HasDefaultValue(false);

                    b.Property<DateTime?>("LastInDate")
                        .HasColumnType("timestamp");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("LastModifiedBy")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<DateTime?>("LastOutDate")
                        .HasColumnType("timestamp");

                    b.Property<decimal>("LastUnitPrice")
                        .HasColumnType("decimal(18,6)");

                    b.Property<int>("MinQty")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<int>("ReservedQty")
                        .HasColumnType("integer");

                    b.Property<int>("SupplierId")
                        .HasColumnType("integer");

                    b.Property<int>("TotalQty")
                        .HasColumnType("integer");

                    b.Property<string>("Unit")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.HasKey("Id");

                    b.HasIndex("SupplierId");

                    b.ToTable("Inventories", (string)null);
                });

            modelBuilder.Entity("Domain.Entities.MaterialUsed", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<int>("InventoryId")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("LastModifiedBy")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<int>("OrderId")
                        .HasColumnType("integer");

                    b.Property<int>("Quantity")
                        .HasColumnType("integer");

                    b.Property<decimal>("TotalPrice")
                        .HasColumnType("decimal(18,6)");

                    b.Property<decimal>("UnitPrice")
                        .HasColumnType("decimal(18,6)");

                    b.HasKey("Id");

                    b.HasIndex("InventoryId");

                    b.HasIndex("OrderId");

                    b.ToTable("MaterialsUsed", (string)null);
                });

            modelBuilder.Entity("Domain.Entities.Order", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("integer");

                    b.Property<DateTime>("ActualDelDate")
                        .HasColumnType("timestamp");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<string>("DeliveryDest")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<decimal>("HardwareCost")
                        .HasColumnType("decimal(18,6)");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("LastModifiedBy")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<decimal>("MaterialCost")
                        .HasColumnType("decimal(18,6)");

                    b.Property<string>("ModelCategory")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("ModelCode")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<decimal>("OtherCost")
                        .HasColumnType("decimal(18,6)");

                    b.Property<DateTime>("ProductionBatch")
                        .HasColumnType("timestamp");

                    b.Property<int>("Quantity")
                        .HasColumnType("integer");

                    b.Property<DateTime>("ReceivedDate")
                        .HasColumnType("timestamp");

                    b.Property<string>("Remark")
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<DateTime>("RequiredDelDate")
                        .HasColumnType("timestamp");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<decimal>("TotalPrice")
                        .HasColumnType("decimal(18,6)");

                    b.Property<decimal>("TotalRevenue")
                        .HasColumnType("decimal(18,6)");

                    b.Property<decimal>("UnitPrice")
                        .HasColumnType("decimal(18,6)");

                    b.Property<decimal>("UnitWages")
                        .HasColumnType("decimal(18,6)");

                    b.Property<string>("filePath")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Orders", (string)null);
                });

            modelBuilder.Entity("Domain.Entities.Outbound", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<int>("InboundId")
                        .HasColumnType("integer");

                    b.Property<int>("InventoryId")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("LastModifiedBy")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<int?>("OrderId")
                        .HasColumnType("integer");

                    b.Property<int>("Quantity")
                        .HasColumnType("integer");

                    b.Property<string>("Remark")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<decimal>("TotalPrice")
                        .HasColumnType("decimal(18,6)");

                    b.HasKey("Id");

                    b.HasIndex("InboundId");

                    b.HasIndex("InventoryId");

                    b.HasIndex("OrderId");

                    b.ToTable("Outbounds", (string)null);
                });

            modelBuilder.Entity("Domain.Entities.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Barcode")
                        .HasColumnType("text");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("text");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("LastModifiedBy")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<decimal>("Rate")
                        .HasColumnType("decimal(18,6)");

                    b.HasKey("Id");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("Domain.Entities.Supplier", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("ContactPerson")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("LastModifiedBy")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.HasKey("Id");

                    b.ToTable("Suppliers", (string)null);
                });

            modelBuilder.Entity("Domain.Entities.Acquisition", b =>
                {
                    b.HasOne("Domain.Entities.Supplier", "Supplier")
                        .WithMany("Acquisitions")
                        .HasForeignKey("SupplierId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Supplier");
                });

            modelBuilder.Entity("Domain.Entities.AcquisitionItem", b =>
                {
                    b.HasOne("Domain.Entities.Acquisition", "Acquisition")
                        .WithMany("Items")
                        .HasForeignKey("AcquisitionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.Entities.Inventory", "Inventory")
                        .WithMany()
                        .HasForeignKey("InventoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Acquisition");

                    b.Navigation("Inventory");
                });

            modelBuilder.Entity("Domain.Entities.Inbound", b =>
                {
                    b.HasOne("Domain.Entities.Acquisition", "Acquisition")
                        .WithMany()
                        .HasForeignKey("AcquisitionId");

                    b.HasOne("Domain.Entities.Inventory", "Inventory")
                        .WithMany("Inbounds")
                        .HasForeignKey("InventoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Acquisition");

                    b.Navigation("Inventory");
                });

            modelBuilder.Entity("Domain.Entities.Inventory", b =>
                {
                    b.HasOne("Domain.Entities.Supplier", "Supplier")
                        .WithMany("Inventories")
                        .HasForeignKey("SupplierId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Supplier");
                });

            modelBuilder.Entity("Domain.Entities.MaterialUsed", b =>
                {
                    b.HasOne("Domain.Entities.Inventory", "Inventory")
                        .WithMany()
                        .HasForeignKey("InventoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.Entities.Order", "Order")
                        .WithMany("MaterialsUsed")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Inventory");

                    b.Navigation("Order");
                });

            modelBuilder.Entity("Domain.Entities.Outbound", b =>
                {
                    b.HasOne("Domain.Entities.Inbound", "Inbound")
                        .WithMany()
                        .HasForeignKey("InboundId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.Entities.Inventory", "Inventory")
                        .WithMany("Outbounds")
                        .HasForeignKey("InventoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.Entities.Order", "Order")
                        .WithMany()
                        .HasForeignKey("OrderId");

                    b.Navigation("Inbound");

                    b.Navigation("Inventory");

                    b.Navigation("Order");
                });

            modelBuilder.Entity("Domain.Entities.Acquisition", b =>
                {
                    b.Navigation("Items");
                });

            modelBuilder.Entity("Domain.Entities.Inventory", b =>
                {
                    b.Navigation("Inbounds");

                    b.Navigation("Outbounds");
                });

            modelBuilder.Entity("Domain.Entities.Order", b =>
                {
                    b.Navigation("MaterialsUsed");
                });

            modelBuilder.Entity("Domain.Entities.Supplier", b =>
                {
                    b.Navigation("Acquisitions");

                    b.Navigation("Inventories");
                });
#pragma warning restore 612, 618
        }
    }
}
