﻿// <auto-generated />
using System;
using AIchatbot.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace AIchatbot.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("AIchatbot.Models.Entities.ChatMessage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("ChatSessionId")
                        .HasColumnType("int");

                    b.Property<string>("Confidence")
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Intent")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<bool>("IsFromUser")
                        .HasColumnType("bit");

                    b.Property<int?>("RelatedFAQId")
                        .HasColumnType("int");

                    b.Property<string>("ResponseSource")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("ChatSessionId");

                    b.HasIndex("RelatedFAQId");

                    b.HasIndex("Timestamp");

                    b.ToTable("ChatMessages");
                });

            modelBuilder.Entity("AIchatbot.Models.Entities.ChatSession", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime?>("EndedAt")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<string>("SessionId")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime>("StartedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("SessionId")
                        .IsUnique();

                    b.HasIndex("UserId");

                    b.ToTable("ChatSessions");
                });

            modelBuilder.Entity("AIchatbot.Models.Entities.FAQ", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Answer")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Category")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<int>("Priority")
                        .HasColumnType("int");

                    b.Property<string>("Question")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("Tags")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("ViewCount")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("Category");

                    b.HasIndex("IsActive");

                    b.ToTable("FAQs");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Answer = "You can track your order by logging into your account and visiting the 'Order History' section, or by using the tracking number provided in your order confirmation email.",
                            Category = "Order Tracking",
                            CreatedAt = new DateTime(2025, 7, 15, 8, 34, 55, 170, DateTimeKind.Utc).AddTicks(7042),
                            IsActive = true,
                            Priority = 1,
                            Question = "How can I track my order?",
                            Tags = "order,tracking,delivery",
                            ViewCount = 0
                        },
                        new
                        {
                            Id = 2,
                            Answer = "We offer a 30-day return policy for most items. Products must be in original condition with all tags attached. Some items may have different return policies.",
                            Category = "Returns",
                            CreatedAt = new DateTime(2025, 7, 15, 8, 34, 55, 170, DateTimeKind.Utc).AddTicks(7046),
                            IsActive = true,
                            Priority = 1,
                            Question = "What is your return policy?",
                            Tags = "return,refund,policy",
                            ViewCount = 0
                        },
                        new
                        {
                            Id = 3,
                            Answer = "Standard delivery takes 3-5 business days. Express delivery (1-2 business days) is available for an additional fee. International shipping may take 7-14 business days.",
                            Category = "Delivery",
                            CreatedAt = new DateTime(2025, 7, 15, 8, 34, 55, 170, DateTimeKind.Utc).AddTicks(7049),
                            IsActive = true,
                            Priority = 1,
                            Question = "How long does delivery take?",
                            Tags = "delivery,shipping,time",
                            ViewCount = 0
                        },
                        new
                        {
                            Id = 4,
                            Answer = "We accept all major credit cards (Visa, MasterCard, American Express), PayPal, Apple Pay, Google Pay, and bank transfers.",
                            Category = "Payment",
                            CreatedAt = new DateTime(2025, 7, 15, 8, 34, 55, 170, DateTimeKind.Utc).AddTicks(7052),
                            IsActive = true,
                            Priority = 1,
                            Question = "What payment methods do you accept?",
                            Tags = "payment,credit card,paypal",
                            ViewCount = 0
                        },
                        new
                        {
                            Id = 5,
                            Answer = "Orders can be cancelled within 1 hour of placement if they haven't been processed for shipping. Contact our customer service team immediately for assistance.",
                            Category = "Orders",
                            CreatedAt = new DateTime(2025, 7, 15, 8, 34, 55, 170, DateTimeKind.Utc).AddTicks(7055),
                            IsActive = true,
                            Priority = 2,
                            Question = "Can I cancel my order?",
                            Tags = "cancel,order,modification",
                            ViewCount = 0
                        });
                });

            modelBuilder.Entity("AIchatbot.Models.Entities.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("FirstName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("LastLoginAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("LastName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("Username")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("AIchatbot.Models.Entities.ChatMessage", b =>
                {
                    b.HasOne("AIchatbot.Models.Entities.ChatSession", "ChatSession")
                        .WithMany("Messages")
                        .HasForeignKey("ChatSessionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AIchatbot.Models.Entities.FAQ", "RelatedFAQ")
                        .WithMany()
                        .HasForeignKey("RelatedFAQId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("ChatSession");

                    b.Navigation("RelatedFAQ");
                });

            modelBuilder.Entity("AIchatbot.Models.Entities.ChatSession", b =>
                {
                    b.HasOne("AIchatbot.Models.Entities.User", "User")
                        .WithMany("ChatSessions")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("AIchatbot.Models.Entities.ChatSession", b =>
                {
                    b.Navigation("Messages");
                });

            modelBuilder.Entity("AIchatbot.Models.Entities.User", b =>
                {
                    b.Navigation("ChatSessions");
                });
#pragma warning restore 612, 618
        }
    }
}
