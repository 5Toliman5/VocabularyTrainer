using AutoMapper;
using VocabularyTrainer.Domain.Models;
using CAddWordReq = VocabularyTrainer.Api.Contract.Words.AddWordRequest;
using CDeleteWordReq = VocabularyTrainer.Api.Contract.Words.DeleteWordRequest;
using CUpdateWeightReq = VocabularyTrainer.Api.Contract.Words.UpdateWordWeightRequest;
using CWeightUpdateType = VocabularyTrainer.Api.Contract.Words.WeightUpdateType;
using CWordResponse = VocabularyTrainer.Api.Contract.Words.WordResponse;
using CWordPageItem = VocabularyTrainer.Api.Contract.Words.WordPageItem;
using CAddDictReq = VocabularyTrainer.Api.Contract.Dictionaries.AddDictionaryRequest;
using CUpdateDictReq = VocabularyTrainer.Api.Contract.Dictionaries.UpdateDictionaryRequest;
using CDictResponse = VocabularyTrainer.Api.Contract.Dictionaries.DictionaryResponse;
using CUserResponse = VocabularyTrainer.Api.Contract.Users.UserResponse;

namespace VocabularyTrainer.Api
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CAddWordReq, AddWordRequest>();
            CreateMap<CDeleteWordReq, UserWordKey>();
            CreateMap<CWeightUpdateType, UpdateWeightType>();
            CreateMap<CUpdateWeightReq, UpdateWordWeightRequest>();

            CreateMap<WordDto, CWordResponse>();
            CreateMap<WordDto, CWordPageItem>();

            CreateMap<CAddDictReq, AddDictionaryRequest>();
            CreateMap<CUpdateDictReq, UpdateDictionaryRequest>()
                .ConstructUsing((src, ctx) => new UpdateDictionaryRequest(
                    (int)ctx.Items["dictionaryId"], src.UserId, src.Name, src.LanguageCode));

            CreateMap<DictionaryDto, CDictResponse>();

            CreateMap<UserModel, CUserResponse>();
        }
    }
}
