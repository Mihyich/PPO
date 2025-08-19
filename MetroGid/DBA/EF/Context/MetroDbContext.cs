using MetroGid.DBA.EF.Models.Tables;
using Microsoft.EntityFrameworkCore;

namespace MetroGid.DBA.EF.Context;

public partial class MetroDbContext : DbContext
{
    public MetroDbContext(DbContextOptions<MetroDbContext> options) : base(options) { }

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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDbFunction(
            typeof(MetroDbContext).GetMethod(nameof(AddRouteJson)) ??
            throw new InvalidOperationException($"Метод '{nameof(AddRouteJson)}' не найден в {typeof(MetroDbContext).Name}.")
        )
            .HasName("add_route_json")
            .HasSchema("public");

        modelBuilder.HasDbFunction(
            typeof(MetroDbContext).GetMethod(nameof(GetChartJsonById)) ??
            throw new InvalidOperationException($"Метод '{nameof(GetChartJsonById)}' не найден в {typeof(MetroDbContext).Name}.")
        )
            .HasName("get_chart_json_by_id")
            .HasSchema("public");

        modelBuilder.HasDbFunction(
            typeof(MetroDbContext).GetMethod(nameof(GetRouteJsonById)) ??
            throw new InvalidOperationException($"Метод '{nameof(GetRouteJsonById)}' не найден в {typeof(MetroDbContext).Name}.")
        )
            .HasName("get_route_json_by_id")
            .HasSchema("public");

        modelBuilder.Entity<Branch>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("branch_pkey");

            entity.ToTable("branch");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Access)
                .HasMaxLength(12)
                .HasDefaultValueSql("'ACCESSIBLE'::character varying")
                .HasColumnName("access");
            entity.Property(e => e.Color)
                .HasDefaultValue(0)
                .HasColumnName("color");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
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
                .HasConstraintName("fk_branch_station_branch_id");

            entity.HasOne(d => d.Station).WithOne(p => p.BranchStation)
                .HasForeignKey<BranchStation>(d => d.StationId)
                .HasConstraintName("fk_branch_station_station_id");
        });

        modelBuilder.Entity<Chart>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("chart_pkey");

            entity.ToTable("chart");

            entity.HasIndex(e => new { e.City, e.Title }, "uk_chart_city_title").IsUnique();

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
                .HasConstraintName("fk_chart_branch_branch_id");

            entity.HasOne(d => d.Chart).WithMany(p => p.ChartBranches)
                .HasForeignKey(d => d.ChartId)
                .HasConstraintName("fk_chart_branch_chart_id");
        });

        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("client_pkey");

            entity.ToTable("client");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ClientLogin)
                .HasMaxLength(255)
                .HasColumnName("client_login");
            entity.Property(e => e.ClientPassword)
                .HasMaxLength(255)
                .HasColumnName("client_password");
            entity.Property(e => e.Mail)
                .HasMaxLength(255)
                .HasColumnName("mail");
            entity.Property(e => e.Privilege)
                .HasMaxLength(8)
                .HasDefaultValueSql("'UNSIGNED'::character varying")
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
                .HasConstraintName("fk_railway_from_id");

            entity.HasOne(d => d.To).WithMany(p => p.RailwayTos)
                .HasForeignKey(d => d.ToId)
                .HasConstraintName("fk_railway_to_id");
        });

        modelBuilder.Entity<Station>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("station_pkey");

            entity.ToTable("station");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Access)
                .HasMaxLength(12)
                .HasDefaultValueSql("'ACCESSIBLE'::character varying")
                .HasColumnName("access");
            entity.Property(e => e.CloseTime).HasColumnName("close_time");
            entity.Property(e => e.DutyId).HasColumnName("duty_id");
            entity.Property(e => e.Occupancy)
                .HasDefaultValue((short)5)
                .HasColumnName("occupancy");
            entity.Property(e => e.OpenTime).HasColumnName("open_time");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");

            entity.HasOne(d => d.Duty).WithMany(p => p.Stations)
                .HasForeignKey(d => d.DutyId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_station_duty_id");
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
                .HasConstraintName("fk_station_transition_station_id");

            entity.HasOne(d => d.Transition).WithMany(p => p.StationTransitions)
                .HasForeignKey(d => d.TransitionId)
                .HasConstraintName("fk_station_transition_transition_id");
        });

        modelBuilder.Entity<Transition>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("transition_pkey");

            entity.ToTable("transition");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Access)
                .HasMaxLength(12)
                .HasDefaultValueSql("'ACCESSIBLE'::character varying")
                .HasColumnName("access");
            entity.Property(e => e.CloseTime).HasColumnName("close_time");
            entity.Property(e => e.Duration).HasColumnName("duration");
            entity.Property(e => e.DutyId).HasColumnName("duty_id");
            entity.Property(e => e.Occupancy)
                .HasDefaultValue((short)5)
                .HasColumnName("occupancy");
            entity.Property(e => e.OpenTime).HasColumnName("open_time");

            entity.HasOne(d => d.Duty).WithMany(p => p.Transitions)
                .HasForeignKey(d => d.DutyId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_transition_duty_id");
        });

        modelBuilder.Entity<Way>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("way_pkey");

            entity.ToTable("way");

            entity.HasIndex(e => new { e.ClientId, e.Title }, "uk_way_client_id_title").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ChartId).HasColumnName("chart_id");
            entity.Property(e => e.ClientId).HasColumnName("client_id");
            entity.Property(e => e.Duration).HasColumnName("duration");
            entity.Property(e => e.InitDate).HasColumnName("init_date");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");

            entity.HasOne(d => d.Chart).WithMany(p => p.Ways)
                .HasForeignKey(d => d.ChartId)
                .HasConstraintName("fk_way_chart_id");

            entity.HasOne(d => d.Client).WithMany(p => p.Ways)
                .HasForeignKey(d => d.ClientId)
                .HasConstraintName("fk_way_client_id");
        });

        modelBuilder.Entity<WayItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("way_item_pkey");

            entity.ToTable("way_item");

            entity.HasIndex(e => new { e.WayId, e.StepNomer }, "uk_way_item_way_id_step_nomer").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Nexus)
                .HasMaxLength(10)
                .HasColumnName("nexus");
            entity.Property(e => e.StepNomer).HasColumnName("step_nomer");
            entity.Property(e => e.WayId).HasColumnName("way_id");

            entity.HasOne(d => d.Way).WithMany(p => p.WayItems)
                .HasForeignKey(d => d.WayId)
                .HasConstraintName("fk_way_item_way_id");
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
                .HasConstraintName("fk_way_item_railway_railway_id");

            entity.HasOne(d => d.WayItem).WithOne(p => p.WayItemRailway)
                .HasForeignKey<WayItemRailway>(d => d.WayItemId)
                .HasConstraintName("fk_way_item_railway_way_item_id");
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
                .HasConstraintName("fk_way_item_station_station_id");

            entity.HasOne(d => d.WayItem).WithOne(p => p.WayItemStation)
                .HasForeignKey<WayItemStation>(d => d.WayItemId)
                .HasConstraintName("fk_way_item_station_way_item_id");
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
                .HasConstraintName("fk_way_item_transition_transition_id");

            entity.HasOne(d => d.WayItem).WithOne(p => p.WayItemTransition)
                .HasForeignKey<WayItemTransition>(d => d.WayItemId)
                .HasConstraintName("fk_way_item_transition_way_item_id");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);


    // Заглушки
    
    public int AddRouteJson(int clientId, int chartId, string jsonRoute) =>
        throw new NotSupportedException($"Метод {nameof(AddRouteJson)} используется только в LINQ через EF Core.");

    public string GetChartJsonById(int chartId) =>
        throw new NotSupportedException($"Метод {nameof(GetChartJsonById)} используется только в LINQ через EF Core.");

    public string GetRouteJsonById(int wayId) =>
        throw new NotSupportedException($"Метод {nameof(GetRouteJsonById)} используется только в LINQ через EF Core.");
}
