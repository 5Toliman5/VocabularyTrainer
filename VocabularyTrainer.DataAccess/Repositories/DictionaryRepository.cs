using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using VocabularyTrainer.DataAccess;
using VocabularyTrainer.Domain.Entities;
using VocabularyTrainer.Domain.Exceptions;
using VocabularyTrainer.Domain.Models;
using VocabularyTrainer.Domain.Repositories;

namespace VocabularyTrainer.DataAccess.Repositories
{
	public class DictionaryRepository(IVocabularyTrainerDbContext dbContext) : IDictionaryRepository
	{
		public async Task<List<DictionaryDto>> GetAllAsync(int userId)
		{
			try
			{
				return await dbContext.Dictionaries
					.AsNoTracking()
					.Where(d => d.UserId == userId)
					.OrderBy(d => d.Name)
					.Select(d => new DictionaryDto(
						d.Id, d.Name, d.LanguageCode, d.Algorithm.Code, d.Words.Count))
					.ToListAsync();
			}
			catch (SqlException ex)
			{
				throw new DatabaseException("Failed to retrieve dictionaries from the database.", ex);
			}
		}

		public async Task<DictionaryDto?> GetByIdAsync(int dictionaryId, int userId)
		{
			try
			{
				return await dbContext.Dictionaries
					.AsNoTracking()
					.Where(d => d.Id == dictionaryId && d.UserId == userId)
					.Select(d => new DictionaryDto(
						d.Id, d.Name, d.LanguageCode, d.Algorithm.Code, d.Words.Count))
					.SingleOrDefaultAsync();
			}
			catch (SqlException ex)
			{
				throw new DatabaseException("Failed to retrieve dictionary from the database.", ex);
			}
		}

		public async Task<int> AddAsync(AddDictionaryRequest request, int algorithmPersistenceId)
		{
			var entity = new UserDictionary
			{
				UserId = request.UserId,
				Name = request.Name,
				LanguageCode = request.LanguageCode,
				AlgorithmId = algorithmPersistenceId,
			};

			try
			{
				dbContext.Dictionaries.Add(entity);
				await dbContext.SaveChangesAsync();
				return entity.Id;
			}
			catch (DbUpdateException ex) when (SqlServerDbErrors.IsUniqueViolation(ex))
			{
				throw new DuplicateKeyException("A dictionary with this name already exists.", ex);
			}
			catch (SqlException ex)
			{
				throw new DatabaseException("Failed to add dictionary to the database.", ex);
			}
		}

		public async Task<int> UpdateAsync(UpdateDictionaryRequest request, int algorithmPersistenceId)
		{
			try
			{
				return await dbContext.Dictionaries
					.Where(d => d.Id == request.DictionaryId && d.UserId == request.UserId)
					.ExecuteUpdateAsync(setters => setters
						.SetProperty(d => d.Name, request.Name)
						.SetProperty(d => d.LanguageCode, request.LanguageCode)
						.SetProperty(d => d.AlgorithmId, algorithmPersistenceId));
			}
			catch (DbUpdateException ex) when (SqlServerDbErrors.IsUniqueViolation(ex))
			{
				throw new DuplicateKeyException("A dictionary with this name already exists.", ex);
			}
			catch (SqlException ex)
			{
				throw new DatabaseException("Failed to update dictionary in the database.", ex);
			}
		}

		public async Task DeleteAsync(int dictionaryId, int userId)
		{
			try
			{
				await dbContext.Dictionaries
					.Where(d => d.Id == dictionaryId && d.UserId == userId)
					.ExecuteDeleteAsync();
			}
			catch (SqlException ex)
			{
				throw new DatabaseException("Failed to delete dictionary from the database.", ex);
			}
		}
	}
}
