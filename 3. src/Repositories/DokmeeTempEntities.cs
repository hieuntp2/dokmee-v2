namespace Repositories
{
	using System.Data.Entity;

	public partial class DokmeeTempEntities : DbContext
	{
		public DokmeeTempEntities()
			: base("DokmeeTempEntities")
		{
		}

		public virtual DbSet<UserLogin> UserLogins { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
		}
	}
}
