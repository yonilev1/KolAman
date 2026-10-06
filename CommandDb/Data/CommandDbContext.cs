using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandDb.Model;
namespace CommandDb.Data;

public class CommandDbContext :DbContext
{
    public CommandDbContext(DbContextOptions<CommandDbContext> options) 
        :base(options)
    { }

    public DbSet<AlertModel> NorthAlerts { get; set; }
    public DbSet<AlertModel> CenterAlerts { get; set; }
    public DbSet<AlertModel> SouthAlerts { get; set; }
    public DbSet<AlertModel> OverseasAlerts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AlertModel>()
            .HasKey(a => a.AlertId);
    }
}
