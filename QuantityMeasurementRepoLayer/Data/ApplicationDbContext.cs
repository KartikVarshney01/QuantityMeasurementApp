using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;
using QuantityMeasurementAppModelLayer.Entities;
using QuantityMeasurementAppModelLayer.Models;

namespace QuantityMeasurementAppRepoLayer.Data;

/// <summary>
/// EF Core DbContext for the Quantity Measurement application.
/// <para>
/// <b>JSON columns:</b> <c>Operand1</c>, <c>Operand2</c>, and <c>Result</c> on
/// <see cref="QuantityMeasurementEntity"/> are not native SQL Server scalar types.
/// EF Core value converters serialise them to <c>nvarchar(max)</c> JSON strings on
/// write and deserialise on read. <see cref="ValueComparer{T}"/> instances tell EF Core
/// how to detect changes so it does not spuriously mark these columns as Modified.
/// </para>
/// <para>
/// <b>Migration commands (run from solution root):</b><br/>
/// <c>dotnet ef migrations add &lt;Name&gt; --project QuantityMeasurementRepoLayer --startup-project QuantityMeasurementAPI</c><br/>
/// <c>dotnet ef database update            --project QuantityMeasurementRepoLayer --startup-project QuantityMeasurementAPI</c>
/// </para>
/// </summary>
// This is the bridge between our code and the database (SQL Server).
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    /// <summary>The QuantityMeasurements table.</summary>
    public DbSet<QuantityMeasurementEntity> QuantityMeasurements => Set<QuantityMeasurementEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var jsonOpts = new JsonSerializerOptions { WriteIndented = false };

        var entity = modelBuilder.Entity<QuantityMeasurementEntity>();

        // ── Table ─────────────────────────────────────────────────────
        entity.ToTable("QuantityMeasurements");

        // ── Primary key ───────────────────────────────────────────────
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id)
              .ValueGeneratedOnAdd()
              .HasColumnName("Id")
              .HasColumnType("int");

        // ── Operation (stored as nvarchar, not as int enum) ───────────
        entity.Property(e => e.Operation)
              .IsRequired()
              .HasMaxLength(50)
              .HasColumnName("Operation")
              .HasColumnType("nvarchar(50)");

        // ── Operand1 → JSON ───────────────────────────────────────────
        var modelComparer = new ValueComparer<QuantityModel<object>?>(
            (a, b) => a != null && b != null
                      && a.Value == b.Value
                      && Equals(a.Unit, b.Unit),
            c => c == null ? 0 : HashCode.Combine(c.Value, c.Unit),
            c => c == null ? null : new QuantityModel<object>(c.Value, c.Unit));

        entity.Property(e => e.Operand1)
              .HasColumnName("Operand1")
              .HasColumnType("nvarchar(max)")
              .HasConversion(
                  v => v == null ? null
                       : JsonSerializer.Serialize(v, jsonOpts),
                  v => v == null ? null
                       : JsonSerializer.Deserialize<QuantityModel<object>>(v, jsonOpts))
              .Metadata.SetValueComparer(modelComparer);

        // ── Operand2 → JSON ───────────────────────────────────────────
        entity.Property(e => e.Operand2)
              .HasColumnName("Operand2")
              .HasColumnType("nvarchar(max)")
              .HasConversion(
                  v => v == null ? null
                       : JsonSerializer.Serialize(v, jsonOpts),
                  v => v == null ? null
                       : JsonSerializer.Deserialize<QuantityModel<object>>(v, jsonOpts))
              .Metadata.SetValueComparer(modelComparer);

        // ── Result → JSON ─────────────────────────────────────────────
        var objectComparer = new ValueComparer<object?>(
            (a, b) => Equals(a, b),
            c => c == null ? 0 : c.GetHashCode(),
            c => c);

        entity.Property(e => e.Result)
              .HasColumnName("Result")
              .HasColumnType("nvarchar(max)")
              .HasConversion(
                  v => v == null ? null
                       : JsonSerializer.Serialize(v, jsonOpts),
                  v => v == null ? null
                       : JsonSerializer.Deserialize<object>(v, jsonOpts))
              .Metadata.SetValueComparer(objectComparer);

        // ── HasError ──────────────────────────────────────────────────
        entity.Property(e => e.HasError)
              .HasColumnName("HasError")
              .HasColumnType("bit")
              .IsRequired();

        // ── ErrorMessage ──────────────────────────────────────────────
        entity.Property(e => e.ErrorMessage)
              .HasColumnName("ErrorMessage")
              .HasColumnType("nvarchar(500)")
              .HasMaxLength(500)
              .IsRequired(false);

        // ── CreatedAt ─────────────────────────────────────────────────
        entity.Property(e => e.CreatedAt)
              .HasColumnName("CreatedAt")
              .HasColumnType("datetime2")
              .IsRequired();

        // ── Indexes ───────────────────────────────────────────────────
        entity.HasIndex(e => e.CreatedAt)
              .HasDatabaseName("IX_QuantityMeasurements_CreatedAt");

        entity.HasIndex(e => e.Operation)
              .HasDatabaseName("IX_QuantityMeasurements_Operation");

        entity.HasIndex(e => e.HasError)
              .HasDatabaseName("IX_QuantityMeasurements_HasError");
    }
}
