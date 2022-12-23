﻿// <auto-generated />
using System;
using FakeNewsBackend.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FakeNewsBackend.Migrations
{
    [DbContext(typeof(WebsiteContext))]
    partial class WebsiteContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("public")
                .HasAnnotation("ProductVersion", "6.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("FakeNewsBackend.Domain.Website", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<bool>("HasRules")
                        .HasColumnType("boolean");

                    b.Property<bool>("HasSitemap")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsWhitelisted")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("NumberOfArticles")
                        .HasColumnType("integer");

                    b.Property<int>("NumberOfArticlesScraped")
                        .HasColumnType("integer");

                    b.Property<int?>("ProgressId")
                        .HasColumnType("integer");

                    b.Property<bool>("ShouldNotBeScraped")
                        .HasColumnType("boolean");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ProgressId");

                    b.ToTable("Websites", "public");
                });

            modelBuilder.Entity("FakeNewsBackend.Domain.WebsiteProgress", b =>
                {
                    b.Property<int>("WebsiteId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("WebsiteId"));

                    b.Property<string>("CurrentlyWorkingOn")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsDone")
                        .HasColumnType("boolean");

                    b.Property<int>("NumberOfLinkInSiteMap")
                        .HasColumnType("integer");

                    b.HasKey("WebsiteId");

                    b.ToTable("Progress", "public");
                });

            modelBuilder.Entity("FakeNewsBackend.Domain.Website", b =>
                {
                    b.HasOne("FakeNewsBackend.Domain.WebsiteProgress", "Progress")
                        .WithMany()
                        .HasForeignKey("ProgressId");

                    b.Navigation("Progress");
                });
#pragma warning restore 612, 618
        }
    }
}
