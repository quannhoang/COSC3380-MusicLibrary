﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MusicLibrary.DataAccess.Data;

#nullable disable

namespace MusicLibrary.DataAccess.Migrations
{
    [DbContext(typeof(MusicLibraryContext))]
    [Migration("20220318234505_UpdateAlbumandPlaylist")]
    partial class UpdateAlbumandPlaylist
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("MusicLibrary.Models.Album", b =>
                {
                    b.Property<int>("AlbumID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AlbumID"), 1L, 1);

                    b.Property<string>("AlbumName")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<string>("ArtistName")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("SongCount")
                        .HasColumnType("int");

                    b.HasKey("AlbumID");

                    b.HasIndex("ArtistName");

                    b.ToTable("Album");
                });

            modelBuilder.Entity("MusicLibrary.Models.AlbumSongs", b =>
                {
                    b.Property<int?>("AlbumID")
                        .HasColumnType("int");

                    b.Property<int?>("SongID")
                        .HasColumnType("int");

                    b.HasIndex("AlbumID");

                    b.HasIndex("SongID");

                    b.ToTable("AlbumSongs");
                });

            modelBuilder.Entity("MusicLibrary.Models.Like", b =>
                {
                    b.Property<int?>("SongID")
                        .HasColumnType("int");

                    b.Property<string>("UserName")
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.HasIndex("SongID");

                    b.HasIndex("UserName");

                    b.ToTable("Like");
                });

            modelBuilder.Entity("MusicLibrary.Models.Playlist", b =>
                {
                    b.Property<int>("PlaylistID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PlaylistID"), 1L, 1);

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("PlaylistName")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<int>("SongCount")
                        .HasColumnType("int");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.HasKey("PlaylistID");

                    b.HasIndex("UserName");

                    b.ToTable("Playlist");
                });

            modelBuilder.Entity("MusicLibrary.Models.PlaylistSongs", b =>
                {
                    b.Property<int?>("PlaylistID")
                        .HasColumnType("int");

                    b.Property<int?>("SongID")
                        .HasColumnType("int");

                    b.HasIndex("PlaylistID");

                    b.HasIndex("SongID");

                    b.ToTable("PlaylistSongs");
                });

            modelBuilder.Entity("MusicLibrary.Models.Song", b =>
                {
                    b.Property<int>("SongID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("SongID"), 1L, 1);

                    b.Property<string>("Artist")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Genre")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<int>("Length")
                        .HasColumnType("int");

                    b.Property<int>("LikeCount")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<DateTime>("UploadDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("ViewCount")
                        .HasColumnType("int");

                    b.HasKey("SongID");

                    b.HasIndex("Artist");

                    b.ToTable("Song");
                });

            modelBuilder.Entity("MusicLibrary.Models.User", b =>
                {
                    b.Property<string>("UserName")
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<bool>("IsAdmin")
                        .HasColumnType("bit");

                    b.Property<bool>("IsArtist")
                        .HasColumnType("bit");

                    b.Property<string>("Passwords")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<int>("UserID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserID"), 1L, 1);

                    b.HasKey("UserName");

                    b.ToTable("User");
                });

            modelBuilder.Entity("MusicLibrary.Models.View", b =>
                {
                    b.Property<int?>("SongID")
                        .HasColumnType("int");

                    b.Property<string>("UserName")
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.HasIndex("SongID");

                    b.HasIndex("UserName");

                    b.ToTable("View");
                });

            modelBuilder.Entity("MusicLibrary.Models.Album", b =>
                {
                    b.HasOne("MusicLibrary.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("ArtistName")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("MusicLibrary.Models.AlbumSongs", b =>
                {
                    b.HasOne("MusicLibrary.Models.Album", "Album")
                        .WithMany()
                        .HasForeignKey("AlbumID");

                    b.HasOne("MusicLibrary.Models.Song", "Song")
                        .WithMany()
                        .HasForeignKey("SongID");

                    b.Navigation("Album");

                    b.Navigation("Song");
                });

            modelBuilder.Entity("MusicLibrary.Models.Like", b =>
                {
                    b.HasOne("MusicLibrary.Models.Song", "Song")
                        .WithMany()
                        .HasForeignKey("SongID");

                    b.HasOne("MusicLibrary.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserName");

                    b.Navigation("Song");

                    b.Navigation("User");
                });

            modelBuilder.Entity("MusicLibrary.Models.Playlist", b =>
                {
                    b.HasOne("MusicLibrary.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserName")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("MusicLibrary.Models.PlaylistSongs", b =>
                {
                    b.HasOne("MusicLibrary.Models.Playlist", "Playlist")
                        .WithMany()
                        .HasForeignKey("PlaylistID");

                    b.HasOne("MusicLibrary.Models.Song", "Song")
                        .WithMany()
                        .HasForeignKey("SongID");

                    b.Navigation("Playlist");

                    b.Navigation("Song");
                });

            modelBuilder.Entity("MusicLibrary.Models.Song", b =>
                {
                    b.HasOne("MusicLibrary.Models.User", "User")
                        .WithMany("Songs")
                        .HasForeignKey("Artist")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("MusicLibrary.Models.View", b =>
                {
                    b.HasOne("MusicLibrary.Models.Song", "Song")
                        .WithMany()
                        .HasForeignKey("SongID");

                    b.HasOne("MusicLibrary.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserName");

                    b.Navigation("Song");

                    b.Navigation("User");
                });

            modelBuilder.Entity("MusicLibrary.Models.User", b =>
                {
                    b.Navigation("Songs");
                });
#pragma warning restore 612, 618
        }
    }
}
