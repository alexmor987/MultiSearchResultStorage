using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DB.Models;

public partial class WebSearchArchiveContext : DbContext
{
    public WebSearchArchiveContext()
    {
    }

    public WebSearchArchiveContext(DbContextOptions<WebSearchArchiveContext> options)
        : base(options)
    {
    }

    public virtual DbSet<SearchResults> SearchResults { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=WebSearchArchive;Username=postgres;Password=postgres");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SearchResults>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("searchresults_pkey");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("nextval('searchresults_id_seq'::regclass)")
                .HasColumnName("id");
            entity.Property(e => e.Entereddate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("entereddate");
            entity.Property(e => e.SearchEngine).HasMaxLength(255);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
