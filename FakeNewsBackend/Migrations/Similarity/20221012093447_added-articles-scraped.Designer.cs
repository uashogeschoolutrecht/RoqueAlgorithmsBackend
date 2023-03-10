// <auto-generated />
using System;
using FakeNewsBackend.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FakeNewsBackend.Migrations.Similarity
{
    [DbContext(typeof(SimilarityContext))]
    [Migration("20221012093447_added-articles-scraped")]
    partial class addedarticlesscraped
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("public")
                .HasAnnotation("ProductVersion", "6.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("FakeNewsBackend.Domain.Similarity", b =>
                {
                    b.Property<int>("FoundWebsiteId")
                        .HasColumnType("integer");

                    b.Property<int>("OriginalWebsiteId")
                        .HasColumnType("integer");

                    b.Property<string>("UrlToFoundArticle")
                        .HasColumnType("text");

                    b.Property<string>("UrlToOriginalArticle")
                        .HasColumnType("text");

                    b.Property<int>("FoundLanguage")
                        .HasColumnType("integer");

                    b.Property<DateTime>("FoundPostDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("OriginalLanguage")
                        .HasColumnType("integer");

                    b.Property<DateTime>("OriginalPostDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<float>("SimilarityScore")
                        .HasColumnType("real");

                    b.HasKey("FoundWebsiteId", "OriginalWebsiteId", "UrlToFoundArticle", "UrlToOriginalArticle");

                    b.ToTable("Similarities", "public");
                });
#pragma warning restore 612, 618
        }
    }
}
