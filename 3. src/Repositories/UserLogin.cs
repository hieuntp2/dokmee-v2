namespace Repositories
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;

	[Table("UserLogin")]
    public partial class UserLogin
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        [StringLength(50)]
        public string Password { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        [StringLength(50)]
        public string SessionId { get; set; }

        [StringLength(20)]
        public string Guid { get; set; }

        public int Type { get; set; }

		[StringLength(200)]
		public string CurrentCabinetId { get; set; }
	}
}
