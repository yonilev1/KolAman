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

    public DbSet<AlertModelNorth> NorthAlerts { get; set; }
    public DbSet<AlertModelCenter> CenterAlerts { get; set; }
    public DbSet<AlertModelSouth> SouthAlerts { get; set; }
    public DbSet<AlertModelOverseas> OverseasAlerts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
