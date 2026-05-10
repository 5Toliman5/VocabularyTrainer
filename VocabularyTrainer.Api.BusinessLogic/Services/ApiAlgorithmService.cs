using VocabularyTrainer.Api.BusinessLogic.Services.Abstractions;
using VocabularyTrainer.Domain.Models;
using VocabularyTrainer.Domain.Services;

namespace VocabularyTrainer.Api.BusinessLogic.Services
{
    public class ApiAlgorithmService(ITrainingAlgorithmRegistry registry) : IApiAlgorithmService
    {
        public IReadOnlyList<AlgorithmInfo> GetAll() => registry.AllInfos;
    }
}
