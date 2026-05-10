using System.ComponentModel.DataAnnotations;

namespace VocabularyTrainer.Domain.Entities
{
	public class Algorithm : EntityBase
	{
		[Required, MaxLength(20)]
		public string Code { get; set; } = null!;

		public ICollection<UserDictionary> Dictionaries { get; set; } = [];
	}
}
