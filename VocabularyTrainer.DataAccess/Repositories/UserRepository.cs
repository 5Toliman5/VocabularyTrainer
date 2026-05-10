using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using VocabularyTrainer.DataAccess;
using VocabularyTrainer.Domain.Exceptions;
using VocabularyTrainer.Domain.Models;
using VocabularyTrainer.Domain.Repositories;

namespace VocabularyTrainer.DataAccess.Repositories
{
	public class UserRepository(IVocabularyTrainerDbContext dbContext) : IUserRepository
	{
		public async Task<UserModel?> GetUserAsync(string userName)
		{
			try
			{
				var user = await dbContext.Users
					.AsNoTracking()
					.Where(u => u.Name == userName)
					.Select(u => new UserModel(u.Id))
					.SingleOrDefaultAsync();

				return user;
			}
			catch (SqlException ex)
			{
				throw new DatabaseException("Failed to retrieve user from the database.", ex);
			}
		}
	}
}
