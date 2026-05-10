using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using VocabularyTrainer.Api.BusinessLogic.Services.Abstractions;
using VocabularyTrainer.Api.Infrastructure;
using AlgorithmInfoResponse = VocabularyTrainer.Api.Contract.Algorithms.AlgorithmInfoResponse;

namespace VocabularyTrainer.Api.Controllers
{
    [Route("api/[controller]")]
    public class AlgorithmsController(IApiAlgorithmService service, IMapper mapper) : BaseApiController
    {
        [HttpGet]
        public IEnumerable<AlgorithmInfoResponse> GetAll()
        {
            var infos = service.GetAll();
            return mapper.Map<IEnumerable<AlgorithmInfoResponse>>(infos);
        }
    }
}
