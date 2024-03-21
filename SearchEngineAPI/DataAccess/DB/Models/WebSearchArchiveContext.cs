using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DB.Models;

public partial class WebSearchArchiveContext : DbContext
{
    private readonly ILogger<WebSearchArchiveContext> _logger;
    private readonly IConfiguration _config;

    public WebSearchArchiveContext(DbContextOptions<WebSearchArchiveContext> options, IConfiguration config, ILogger<WebSearchArchiveContext> logger)
        : base(options)
    {
        _config = config;
        _logger = logger;
    }

    public WebSearchArchiveContext(DbContextOptions<WebSearchArchiveContext> options)
        : base(options)
    {
    }

    public virtual DbSet<SearchResults> SearchResults { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var connectionString = _config.GetConnectionString("DataBase");
            optionsBuilder.UseNpgsql(connectionString);
        }
    }
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
