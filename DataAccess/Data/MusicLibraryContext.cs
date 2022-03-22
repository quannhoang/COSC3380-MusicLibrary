#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MusicLibrary.Models;

namespace MusicLibrary.DataAccess.Data
{
    public class MusicLibraryContext : DbContext
    {
        public MusicLibraryContext (DbContextOptions<MusicLibraryContext> options)
            : base(options)
        {
        }

        //public MusicLibraryContext() { }

        protected override void OnModelCreating(ModelBuilder modelBuilder) 
        {
            modelBuilder.Entity<View>()
                .HasNoKey();
            modelBuilder.Entity<PlaylistSongs>()
                .HasNoKey();

            modelBuilder.Entity<AlbumSongs>()
                .HasNoKey();
            modelBuilder.Entity<Like>()
                .HasNoKey();
            /*modelBuilder.Entity<Like>()
                .HasOne<Song>(s => s.Song)
                .WithMany(l => l.Likes)
                .OnDelete(DeleteBehavior.ClientCascade);

                 modelBuilder.Entity<PlaylistSongs>()
                .HasOne<Song> (s=> s.Song)
                .WithMany()
                .HasForeignKey(s=>s.SongID)
                .OnDelete(DeleteBehavior.ClientSetNull);
            modelBuilder.Entity<AlbumSongs>()
                .HasOne<Song>(s => s.Song)
                .WithMany(ab => ab.AlbumSongs)
                .OnDelete(DeleteBehavior.ClientSetNull);
            */
        }

        public DbSet<Song> Song { get; set; }

        public DbSet<User> User { get; set; }

        public DbSet<Album> Album { get; set; }
        public DbSet<Playlist> Playlist { get; set; }
        public DbSet<Like> Like { get; set; }
        public DbSet<View> View { get; set; }

        public DbSet<PlaylistSongs> PlaylistSongs { get; set; }
        
        public DbSet<AlbumSongs> AlbumSongs { get; set; }
    }
}
