using AutoMapper;
using VocabularyTrainer.Domain.Models;
using CAddWordReq = VocabularyTrainer.Api.Contract.Words.AddWordRequest;
using CDeleteWordReq = VocabularyTrainer.Api.Contract.Words.DeleteWordRequest;
using CUpdateWeightReq = VocabularyTrainer.Api.Contract.Words.UpdateWordWeightRequest;
using CWeightUpdateType = VocabularyTrainer.Api.Contract.Words.WeightUpdateType;
using CWordResponse = VocabularyTrainer.Api.Contract.Words.WordResponse;
using CAddDictReq = VocabularyTrainer.Api.Contract.Dictionaries.AddDictionaryRequest;
using CUpdateDictReq = VocabularyTrainer.Api.Contract.Dictionaries.UpdateDictionaryRequest;
using CDictResponse = VocabularyTrainer.Api.Contract.Dictionaries.DictionaryResponse;
using CUserResponse = VocabularyTrainer.Api.Contract.Users.UserResponse;

namespace VocabularyTrainer.Api
{
    /// <summary>AutoMapper profile that maps between API contract models and domain models.</summary>
    public class MappingProfile : Profile
    {
        /// <summary>Initializes all contract-to-domain and domain-to-contract mappings.</summary>
        public MappingProfile()
        {
            // Words — request
            CreateMap<CAddWordReq, AddWordRequest>();
            CreateMap<CDeleteWordReq, UserWordKey>();
            CreateMap<CWeightUpdateType, UpdateWeightType>();
            CreateMap<CUpdateWeightReq, UpdateWordWeightRequest>();

            // Words — response
            CreateMap<WordDto, CWordResponse>();

            // Dictionaries — request
            CreateMap<CAddDictReq, AddDictionaryRequest>();
            CreateMap<CUpdateDictReq, UpdateDictionaryRequest>()
                .ConstructUsing((src, ctx) => new UpdateDictionaryRequest(
                    (int)ctx.Items["dictionaryId"], src.UserId, src.Name, src.LanguageCode));

            // Dictionaries — response
            CreateMap<DictionaryDto, CDictResponse>();

            // Users — response
            CreateMap<UserModel, CUserResponse>();
        }
    }
}
