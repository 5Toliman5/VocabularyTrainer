using VocabularyTrainer.Domain.Models;

namespace VocabularyTrainer.Api.BusinessLogic.Services.Abstractions
{
    public interface IApiAlgorithmService
    {
        IReadOnlyList<AlgorithmInfo> GetAll();
    }
}
