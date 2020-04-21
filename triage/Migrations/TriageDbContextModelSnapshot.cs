﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Model;

namespace triage.Migrations
{
    [DbContext(typeof(TriageDbContext))]
    partial class TriageDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.3");

            modelBuilder.Entity("Model.ProcessedBuild", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("AzureOrganization")
                        .HasColumnType("TEXT");

                    b.Property<string>("AzureProject")
                        .HasColumnType("TEXT");

                    b.Property<int>("BuildNumber")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("ProcessedBuilds");
                });

            modelBuilder.Entity("Model.TimelineEntry", b =>
                {
                    b.Property<string>("BuildKey")
                        .HasColumnType("TEXT");

                    b.Property<string>("AzureOrganization")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("AzureProject")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("BuildNumber")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Line")
                        .HasColumnType("TEXT");

                    b.Property<int>("TimelineIssueId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("TimelineIssueId1")
                        .HasColumnType("TEXT");

                    b.Property<string>("TimelineRecordName")
                        .HasColumnType("TEXT");

                    b.HasKey("BuildKey");

                    b.HasIndex("TimelineIssueId1");

                    b.ToTable("TimelineEntries");
                });

            modelBuilder.Entity("Model.TimelineIssue", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("GitHubOrganization")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("GitHubRepository")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("IssueId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("SearchText")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("TimelineIssues");
                });

            modelBuilder.Entity("Model.TimelineEntry", b =>
                {
                    b.HasOne("Model.TimelineIssue", "TimelineIssue")
                        .WithMany()
                        .HasForeignKey("TimelineIssueId1");
                });
#pragma warning restore 612, 618
        }
    }
}