using MetroGid.DBA.EF.Models.Tables;
using MetroGid.DBA.EF.Models.UserDefinedTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MetroGid.DBA.EF.Context;

public partial class MetroContext : DbContext
{
    public MetroContext()
    {
    }

    public MetroContext(DbContextOptions<MetroContext> options) : base(options)
    {
    }

    public virtual DbSet<Branch> Branches { get; set; } = null!;

    public virtual DbSet<BranchStation> BranchStations { get; set; } = null!;

    public virtual DbSet<Chart> Charts { get; set; } = null!;

    public virtual DbSet<ChartBranch> ChartBranches { get; set; } = null!;

    public virtual DbSet<Client> Clients { get; set; } = null!;

    public virtual DbSet<Railway> Railways { get; set; } = null!;

    public virtual DbSet<Station> Stations { get; set; } = null!;

    public virtual DbSet<StationTransition> StationTransitions { get; set; } = null!;

    public virtual DbSet<Transition> Transitions { get; set; } = null!;

    public virtual DbSet<Way> Ways { get; set; } = null!;

    public virtual DbSet<WayItem> WayItems { get; set; } = null!;

    public virtual DbSet<WayItemRailway> WayItemRailways { get; set; } = null!;

    public virtual DbSet<WayItemStation> WayItemStations { get; set; } = null!;

    public virtual DbSet<WayItemTransition> WayItemTransitions { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=metro;Username=postgres;Password=1234");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Branch>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("branch_pkey");

            entity.ToTable("branch");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Color)
                .HasColumnType("decimal_hexcolor")
                .HasDefaultValue(0)
                .HasColumnName("color");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .IsRequired()
                .HasColumnName("title");
            entity.Property(e => e.Access)
                .HasConversion(
                    v => v.ToString(), // C# -> БД
                    v => (AccessType)Enum.Parse(typeof(AccessType), v))  // БД -> С#
                .HasDefaultValueSql("'ACCESSIBLE'::access_type")
                .HasColumnName("access");
        });

        modelBuilder.Entity<BranchStation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("branch_station_pkey");

            entity.ToTable("branch_station");

            entity.HasIndex(e => new { e.BranchId, e.StationId }, "uk_branch_station_branch_id_station_id").IsUnique();

            entity.HasIndex(e => e.StationId, "uk_branch_station_station_id").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BranchId).HasColumnName("branch_id");
            entity.Property(e => e.StationId).HasColumnName("station_id");

            entity.HasOne(d => d.Branch).WithMany(p => p.BranchStations)
                .HasForeignKey(d => d.BranchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("branch_station_branch_id_fkey");

            entity.HasOne(d => d.Station).WithOne(p => p.BranchStation)
                .HasForeignKey<BranchStation>(d => d.StationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("branch_station_station_id_fkey");
        });

        modelBuilder.Entity<Chart>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("chart_pkey");

            entity.ToTable("chart");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.City)
                .HasMaxLength(255)
                .HasColumnName("city");
            entity.Property(e => e.SvgContent)
                .HasDefaultValueSql("'<svg width=\"200\" height=\"200\"><text y=\"16\">Пустая схема</text></svg>'::xml")
                .HasColumnType("xml")
                .HasColumnName("svg_content");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
        });

        modelBuilder.Entity<ChartBranch>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("chart_branch_pkey");

            entity.ToTable("chart_branch");

            entity.HasIndex(e => e.BranchId, "uk_chart_branch_branch_id").IsUnique();

            entity.HasIndex(e => new { e.ChartId, e.BranchId }, "uk_chart_branch_chart_id_branch_id").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BranchId).HasColumnName("branch_id");
            entity.Property(e => e.ChartId).HasColumnName("chart_id");

            entity.HasOne(d => d.Branch).WithOne(p => p.ChartBranch)
                .HasForeignKey<ChartBranch>(d => d.BranchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("chart_branch_branch_id_fkey");

            entity.HasOne(d => d.Chart).WithMany(p => p.ChartBranches)
                .HasForeignKey(d => d.ChartId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("chart_branch_chart_id_fkey");
        });

        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("client_pkey");

            entity.ToTable("client");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ClientLogin)
                .HasColumnType("login_inst")
                .HasMaxLength(255)
                .HasColumnName("client_login");
            entity.Property(e => e.ClientPassword)
                .HasColumnType("password_inst")
                .HasMaxLength(255)
                .HasColumnName("client_password");
            entity.Property(e => e.Mail)
                .HasColumnType("mail_inst")
                .HasMaxLength(255)
                .HasColumnName("mail");
            entity.Property(e => e.Privilege)
                .HasConversion(
                    v => v.ToString(), // C# -> БД
                    v => (RoleType)Enum.Parse(typeof(RoleType), v))  // БД -> С#
                .HasColumnType("role_domain")
                .HasColumnName("privilege");
        });

        modelBuilder.Entity<Railway>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("railway_pkey");

            entity.ToTable("railway");

            entity.HasIndex(e => new { e.FromId, e.ToId }, "uk_railway_from_id_to_id").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Duration).HasColumnName("duration");
            entity.Property(e => e.FromId).HasColumnName("from_id");
            entity.Property(e => e.ToId).HasColumnName("to_id");

            entity.HasOne(d => d.From).WithMany(p => p.RailwayFroms)
                .HasForeignKey(d => d.FromId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("railway_from_id_fkey");

            entity.HasOne(d => d.To).WithMany(p => p.RailwayTos)
                .HasForeignKey(d => d.ToId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("railway_to_id_fkey");
        });

        modelBuilder.Entity<Station>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("station_pkey");

            entity.ToTable("station");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CloseTime).HasColumnName("close_time");
            entity.Property(e => e.DutyId).HasColumnName("duty_id");
            entity.Property(e => e.Occupancy)
                .HasColumnType("occupancy_level")
                .HasDefaultValue((short)5)
                .HasColumnName("occupancy");
            entity.Property(e => e.Access)
                .HasConversion(
                    v => v.ToString(), // C# -> БД
                    v => (AccessType)Enum.Parse(typeof(AccessType), v))  // БД -> С#
                .HasDefaultValueSql("'ACCESSIBLE'::access_type")
                .HasColumnName("access");
            entity.Property(e => e.OpenTime).HasColumnName("open_time");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");

            entity.HasOne(d => d.Duty).WithMany(p => p.Stations)
                .HasForeignKey(d => d.DutyId)
                .HasConstraintName("station_duty_id_fkey");
        });

        modelBuilder.Entity<StationTransition>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("station_transition_pkey");

            entity.ToTable("station_transition");

            entity.HasIndex(e => new { e.StationId, e.TransitionId }, "uk_station_transition_station_id_transition_id").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.StationId).HasColumnName("station_id");
            entity.Property(e => e.TransitionId).HasColumnName("transition_id");

            entity.HasOne(d => d.Station).WithMany(p => p.StationTransitions)
                .HasForeignKey(d => d.StationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("station_transition_station_id_fkey");

            entity.HasOne(d => d.Transition).WithMany(p => p.StationTransitions)
                .HasForeignKey(d => d.TransitionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("station_transition_transition_id_fkey");
        });

        modelBuilder.Entity<Transition>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("transition_pkey");

            entity.ToTable("transition");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CloseTime).HasColumnName("close_time");
            entity.Property(e => e.Duration).HasColumnName("duration");
            entity.Property(e => e.DutyId).HasColumnName("duty_id");
            entity.Property(e => e.Occupancy)
                .HasColumnType("occupancy_level")
                .HasDefaultValue((short)5)
                .HasColumnName("occupancy");
            entity.Property(e => e.Access)
                .HasConversion(
                    v => v.ToString(), // C# -> БД
                    v => (AccessType)Enum.Parse(typeof(AccessType), v))  // БД -> С#
                .HasDefaultValueSql("'ACCESSIBLE'::access_type")
                .HasColumnName("access");
            entity.Property(e => e.OpenTime).HasColumnName("open_time");

            entity.HasOne(d => d.Duty).WithMany(p => p.Transitions)
                .HasForeignKey(d => d.DutyId)
                .HasConstraintName("transition_duty_id_fkey");
        });

        modelBuilder.Entity<Way>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("way_pkey");

            entity.ToTable("way");

            entity.HasIndex(e => new { e.ClientId, e.Title }, "uk_way_client_id_title").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ChartId).HasColumnName("chart_id");
            entity.Property(e => e.ClientId).HasColumnName("client_id");
            entity.Property(e => e.InitDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("init_date");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");

            entity.HasOne(d => d.Chart).WithMany(p => p.Ways)
                .HasForeignKey(d => d.ChartId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("way_chart_id_fkey");

            entity.HasOne(d => d.Client).WithMany(p => p.Ways)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("way_client_id_fkey");
        });

        modelBuilder.Entity<WayItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("way_item_pkey");

            entity.ToTable("way_item");

            entity.HasIndex(e => new { e.WayId, e.StepNomer }, "uk_way_item_way_id_step_nomer").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Nexus)
                .HasConversion(
                    v => v.ToString(), // C# -> БД
                    v => (NexusType)Enum.Parse(typeof(NexusType), v))  // БД -> С#
                .HasColumnName("nexus");
            entity.Property(e => e.StepNomer).HasColumnName("step_nomer");
            entity.Property(e => e.WayId).HasColumnName("way_id");

            entity.HasOne(d => d.Way).WithMany(p => p.WayItems)
                .HasForeignKey(d => d.WayId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("way_item_way_id_fkey");
        });

        modelBuilder.Entity<WayItemRailway>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("way_item_railway_pkey");

            entity.ToTable("way_item_railway");

            entity.HasIndex(e => e.WayItemId, "uk_way_item_railway_way_item_id").IsUnique();

            entity.HasIndex(e => new { e.WayItemId, e.RailwayId }, "uk_way_item_railway_way_item_id_railway_id").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.RailwayId).HasColumnName("railway_id");
            entity.Property(e => e.WayItemId).HasColumnName("way_item_id");

            entity.HasOne(d => d.Railway).WithMany(p => p.WayItemRailways)
                .HasForeignKey(d => d.RailwayId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("way_item_railway_railway_id_fkey");

            entity.HasOne(d => d.WayItem).WithOne(p => p.WayItemRailway)
                .HasForeignKey<WayItemRailway>(d => d.WayItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("way_item_railway_way_item_id_fkey");
        });

        modelBuilder.Entity<WayItemStation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("way_item_station_pkey");

            entity.ToTable("way_item_station");

            entity.HasIndex(e => e.WayItemId, "uk_way_item_station_way_item_id").IsUnique();

            entity.HasIndex(e => new { e.WayItemId, e.StationId }, "uk_way_item_station_way_item_id_station_id").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.StationId).HasColumnName("station_id");
            entity.Property(e => e.WayItemId).HasColumnName("way_item_id");

            entity.HasOne(d => d.Station).WithMany(p => p.WayItemStations)
                .HasForeignKey(d => d.StationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("way_item_station_station_id_fkey");

            entity.HasOne(d => d.WayItem).WithOne(p => p.WayItemStation)
                .HasForeignKey<WayItemStation>(d => d.WayItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("way_item_station_way_item_id_fkey");
        });

        modelBuilder.Entity<WayItemTransition>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("way_item_transition_pkey");

            entity.ToTable("way_item_transition");

            entity.HasIndex(e => e.WayItemId, "uk_way_item_transition_way_item_id").IsUnique();

            entity.HasIndex(e => new { e.WayItemId, e.TransitionId }, "uk_way_item_transition_way_item_id_transition_id").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.TransitionId).HasColumnName("transition_id");
            entity.Property(e => e.WayItemId).HasColumnName("way_item_id");

            entity.HasOne(d => d.Transition).WithMany(p => p.WayItemTransitions)
                .HasForeignKey(d => d.TransitionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("way_item_transition_transition_id_fkey");

            entity.HasOne(d => d.WayItem).WithOne(p => p.WayItemTransition)
                .HasForeignKey<WayItemTransition>(d => d.WayItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("way_item_transition_way_item_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
