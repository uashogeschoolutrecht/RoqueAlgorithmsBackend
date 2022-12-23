﻿// <auto-generated />
using System;
using FakeNewsBackend.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FakeNewsBackend.Migrations.Sitemap
{
    [DbContext(typeof(SitemapContext))]
    [Migration("20221019103459_allownull")]
    partial class allownull
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("public")
                .HasAnnotation("ProductVersion", "6.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("FakeNewsBackend.Domain.SitemapGenerateProgress", b =>
                {
                    b.Property<int>("WebsiteId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("WebsiteId"));

                    b.Property<string>("LinksOnWebsite")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("LinksVisited")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("WebsiteId");

                    b.ToTable("GenerateProgress", "public");
                });

            modelBuilder.Entity("FakeNewsBackend.Domain.SitemapItem", b =>
                {
                    b.Property<int>("websiteId")
                        .HasColumnType("integer");

                    b.Property<string>("url")
                        .HasColumnType("text");

                    b.Property<DateTime>("date")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("scraped")
                        .HasColumnType("boolean");

                    b.HasKey("websiteId", "url");

                    b.ToTable("SitemapItems", "public");
                });
#pragma warning restore 612, 618
        }
    }
}
