using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace VocabularyTrainer.DataAccess
{
	internal static class SqlServerDbErrors
	{
		private const int UniqueConstraintViolation = 2627;
		private const int UniqueIndexViolation = 2601;

		public static bool IsUniqueViolation(DbUpdateException exception) =>
			exception.InnerException is SqlException sqlException &&
			(sqlException.Number == UniqueConstraintViolation || sqlException.Number == UniqueIndexViolation);
	}
}
