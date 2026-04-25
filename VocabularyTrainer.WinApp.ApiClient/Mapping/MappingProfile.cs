using AutoMapper;
using VocabularyTrainer.Domain.Models;
using CWordResponse = VocabularyTrainer.Api.Contract.Words.WordResponse;
using CWordPageItem = VocabularyTrainer.Api.Contract.Words.WordPageItem;
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
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CWordResponse, WordDto>()
                .ConstructUsing(r => new WordDto(r.Id, r.Value, r.Translation, r.Weight, r.DictionaryId, r.DictionaryName));

            CreateMap<CWordPageItem, WordDto>()
                .ConstructUsing(r =>
                    new WordDto(r.Id, r.Value, r.Translation, r.Weight, r.DictionaryId, r.DictionaryName,
                                r.LanguageCode, r.DateAdded, r.DateModified));

            CreateMap<CDictionaryResponse, DictionaryDto>();
            CreateMap<CUserResponse, UserModel>();

            CreateMap<UpdateWeightType, CWeightUpdateType>();
            CreateMap<AddWordRequest, CAddWordRequest>();
            CreateMap<UserWordKey, CDeleteWordRequest>();
            CreateMap<UpdateWordWeightRequest, CUpdateWeightRequest>();
            CreateMap<AddDictionaryRequest, CAddDictionaryRequest>();
            CreateMap<UpdateDictionaryRequest, CUpdateDictionaryRequest>();
        }
    }
}
