﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using TwitterCloneApi.Data;

#nullable disable

namespace TwitterCloneApi.Migrations
{
    [DbContext(typeof(ContextApi))]
    [Migration("20230815072845_..")]
    partial class _
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("CommentUser", b =>
                {
                    b.Property<string>("CommentId")
                        .HasColumnType("text");

                    b.Property<string>("LikesId")
                        .HasColumnType("text");

                    b.HasKey("CommentId", "LikesId");

                    b.HasIndex("LikesId");

                    b.ToTable("CommentUser");
                });

            modelBuilder.Entity("TwitterCloneApi.Models.Comment", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("AuthorId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("TweetId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("TweetId");

                    b.ToTable("Comment");
                });

            modelBuilder.Entity("TwitterCloneApi.Models.Tweet", b =>
                {
                    b.Property<string>("TweetId")
                        .HasColumnType("text");

                    b.Property<string>("AuthorId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("TweetId");

                    b.HasIndex("AuthorId");

                    b.ToTable("Tweet");
                });

            modelBuilder.Entity("TwitterCloneApi.Models.TweetLikes", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.Property<string>("TweetId")
                        .HasColumnType("text");

                    b.HasKey("UserId", "TweetId");

                    b.HasIndex("TweetId");

                    b.ToTable("TweetLikes");
                });

            modelBuilder.Entity("TwitterCloneApi.Models.User", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("IconLink")
                        .HasColumnType("text");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("User");
                });

            modelBuilder.Entity("TwitterCloneApi.Models.UserConfidentials", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("RefreshToken")
                        .HasColumnType("text");

                    b.Property<string>("Salt")
                        .HasColumnType("text");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("UserConfidentials");
                });

            modelBuilder.Entity("TwitterCloneApi.Models.UserFollowings", b =>
                {
                    b.Property<string>("FromUserId")
                        .HasColumnType("text");

                    b.Property<string>("ToUserId")
                        .HasColumnType("text");

                    b.HasKey("FromUserId", "ToUserId");

                    b.HasIndex("ToUserId");

                    b.ToTable("UserFollowings");
                });

            modelBuilder.Entity("CommentUser", b =>
                {
                    b.HasOne("TwitterCloneApi.Models.Comment", null)
                        .WithMany()
                        .HasForeignKey("CommentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TwitterCloneApi.Models.User", null)
                        .WithMany()
                        .HasForeignKey("LikesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("TwitterCloneApi.Models.Comment", b =>
                {
                    b.HasOne("TwitterCloneApi.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TwitterCloneApi.Models.Tweet", "Tweet")
                        .WithMany("Comments")
                        .HasForeignKey("TweetId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Tweet");

                    b.Navigation("User");
                });

            modelBuilder.Entity("TwitterCloneApi.Models.Tweet", b =>
                {
                    b.HasOne("TwitterCloneApi.Models.User", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");
                });

            modelBuilder.Entity("TwitterCloneApi.Models.TweetLikes", b =>
                {
                    b.HasOne("TwitterCloneApi.Models.Tweet", "Tweet")
                        .WithMany("Likes")
                        .HasForeignKey("TweetId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("TwitterCloneApi.Models.User", "User")
                        .WithMany("TweetLikes")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Tweet");

                    b.Navigation("User");
                });

            modelBuilder.Entity("TwitterCloneApi.Models.UserConfidentials", b =>
                {
                    b.HasOne("TwitterCloneApi.Models.User", "User")
                        .WithOne("UserConfidentials")
                        .HasForeignKey("TwitterCloneApi.Models.UserConfidentials", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("TwitterCloneApi.Models.UserFollowings", b =>
                {
                    b.HasOne("TwitterCloneApi.Models.User", "FromUser")
                        .WithMany("FromFollowings")
                        .HasForeignKey("FromUserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("TwitterCloneApi.Models.User", "ToUser")
                        .WithMany("ToFollowings")
                        .HasForeignKey("ToUserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("FromUser");

                    b.Navigation("ToUser");
                });

            modelBuilder.Entity("TwitterCloneApi.Models.Tweet", b =>
                {
                    b.Navigation("Comments");

                    b.Navigation("Likes");
                });

            modelBuilder.Entity("TwitterCloneApi.Models.User", b =>
                {
                    b.Navigation("FromFollowings");

                    b.Navigation("ToFollowings");

                    b.Navigation("TweetLikes");

                    b.Navigation("UserConfidentials")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
