using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VocabularyTrainer.Domain.Entities
{
	/// <summary>Base class for all database-mapped entities, providing a surrogate integer primary key.</summary>
	public abstract class EntityBase
	{
		/// <summary>Database-generated primary key.</summary>
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }
	}
}
