using AutoMapper;
using VocabularyTrainer.Domain.Models;
using CWordResponse = VocabularyTrainer.Api.Contract.Words.WordResponse;
using CAddWordRequest = VocabularyTrainer.Api.Contract.Words.AddWordRequest;
using CDeleteWordRequest = VocabularyTrainer.Api.Contract.Words.DeleteWordRequest;
using CUpdateWeightRequest = VocabularyTrainer.Api.Contract.Words.UpdateWordWeightRequest;
using CWeightUpdateType = VocabularyTrainer.Api.Contract.Words.WeightUpdateType;
using CDictionaryResponse = VocabularyTrainer.Api.Contract.Dictionaries.DictionaryResponse;
using CAddDictionaryRequest = VocabularyTrainer.Api.Contract.Dictionaries.AddDictionaryRequest;
using CUpdateDictionaryRequest = VocabularyTrainer.Api.Contract.Dictionaries.UpdateDictionaryRequest;
using CUserResponse = VocabularyTrainer.Api.Contract.Users.UserResponse;

namespace VocabularyTrainer.WinApp.ApiClient.Mapping
{
    /// <summary>AutoMapper profile that maps between API contract models and domain models for the HTTP client layer.</summary>
    public class MappingProfile : Profile
    {
        /// <summary>Initializes all contract-to-domain and domain-to-contract mappings.</summary>
        public MappingProfile()
        {
            // ── Contract response → Domain ──────────────────────────────────
            CreateMap<CWordResponse, WordDto>()
                .ConstructUsing(r => new WordDto(r.Id, r.Value, r.Translation, r.Weight, r.DictionaryId, r.DictionaryName));
            CreateMap<CDictionaryResponse, DictionaryDto>();
            CreateMap<CUserResponse, UserModel>();

            // ── Domain → Contract request ───────────────────────────────────
            CreateMap<UpdateWeightType, CWeightUpdateType>();
            CreateMap<AddWordRequest, CAddWordRequest>();
            CreateMap<UserWordKey, CDeleteWordRequest>();
            CreateMap<UpdateWordWeightRequest, CUpdateWeightRequest>();
            CreateMap<AddDictionaryRequest, CAddDictionaryRequest>();
            CreateMap<UpdateDictionaryRequest, CUpdateDictionaryRequest>();
        }
    }
}
