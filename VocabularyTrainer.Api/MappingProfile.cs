using AutoMapper;
using VocabularyTrainer.Domain.Models;
using CAddWordReq = VocabularyTrainer.Api.Contract.Words.AddWordRequest;
using CReviewWordReq = VocabularyTrainer.Api.Contract.Words.ReviewWordRequest;
using CReviewGrade = VocabularyTrainer.Api.Contract.Words.ReviewGrade;
using CTranslationKind = VocabularyTrainer.Api.Contract.Words.TranslationKind;
using CWordTranslationDto = VocabularyTrainer.Api.Contract.Words.WordTranslationDto;
using CWordResponse = VocabularyTrainer.Api.Contract.Words.WordResponse;
using CWordPageItem = VocabularyTrainer.Api.Contract.Words.WordPageItem;
using CWordSortBy = VocabularyTrainer.Api.Contract.Words.WordSortBy;
using CGetWordsPagedReq = VocabularyTrainer.Api.Contract.Words.GetWordsPagedRequest;
using CAddDictReq = VocabularyTrainer.Api.Contract.Dictionaries.AddDictionaryRequest;
using CUpdateDictReq = VocabularyTrainer.Api.Contract.Dictionaries.UpdateDictionaryRequest;
using CDictResponse = VocabularyTrainer.Api.Contract.Dictionaries.DictionaryResponse;
using CUserResponse = VocabularyTrainer.Api.Contract.Users.UserResponse;
using CAlgorithmInfoResp = VocabularyTrainer.Api.Contract.Algorithms.AlgorithmInfoResponse;
using CAlgorithmCost = VocabularyTrainer.Api.Contract.Algorithms.AlgorithmCost;

namespace VocabularyTrainer.Api
{
	public class MappingProfile : Profile
	{
		public MappingProfile()
		{
			CreateMap<CTranslationKind, TranslationKind>().ReverseMap();
			CreateMap<CReviewGrade, ReviewGrade>().ReverseMap();
			CreateMap<CWordSortBy, WordSortBy>().ReverseMap();

			CreateMap<CWordTranslationDto, WordTranslationDto>().ReverseMap();

			CreateMap<CAddWordReq, AddWordRequest>();

			CreateMap<WordDto, CWordResponse>();
			CreateMap<WordDto, CWordPageItem>()
				.ForCtorParam("PrimaryTranslation", o => o.MapFrom(s => s.PrimaryTranslation));

			CreateMap<CGetWordsPagedReq, GetWordsPagedRequest>();

			CreateMap<CAddDictReq, AddDictionaryRequest>();

			CreateMap<DictionaryDto, CDictResponse>();

			CreateMap<UserModel, CUserResponse>();

			CreateMap<AlgorithmCost, CAlgorithmCost>().ReverseMap();
			CreateMap<AlgorithmInfo, CAlgorithmInfoResp>();
		}
	}
}
