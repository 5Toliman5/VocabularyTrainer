using AutoMapper;
using VocabularyTrainer.Domain.Models;
using CWordResponse = VocabularyTrainer.Api.Contract.Words.WordResponse;
using CWordPageItem = VocabularyTrainer.Api.Contract.Words.WordPageItem;
using CAddWordRequest = VocabularyTrainer.Api.Contract.Words.AddWordRequest;
using CWordTranslationDto = VocabularyTrainer.Api.Contract.Words.WordTranslationDto;
using CTranslationKind = VocabularyTrainer.Api.Contract.Words.TranslationKind;
using CReviewGrade = VocabularyTrainer.Api.Contract.Words.ReviewGrade;
using CDictionaryResponse = VocabularyTrainer.Api.Contract.Dictionaries.DictionaryResponse;
using CAddDictionaryRequest = VocabularyTrainer.Api.Contract.Dictionaries.AddDictionaryRequest;
using CUpdateDictionaryRequest = VocabularyTrainer.Api.Contract.Dictionaries.UpdateDictionaryRequest;
using CUserResponse = VocabularyTrainer.Api.Contract.Users.UserResponse;
using CAlgorithmInfoResponse = VocabularyTrainer.Api.Contract.Algorithms.AlgorithmInfoResponse;
using CAlgorithmCost = VocabularyTrainer.Api.Contract.Algorithms.AlgorithmCost;

namespace VocabularyTrainer.WinApp.ApiClient.Mapping
{
	public class MappingProfile : Profile
	{
		public MappingProfile()
		{
			CreateMap<CTranslationKind, TranslationKind>().ReverseMap();
			CreateMap<CReviewGrade, ReviewGrade>().ReverseMap();
			CreateMap<CWordTranslationDto, WordTranslationDto>().ReverseMap();

			CreateMap<CWordResponse, WordDto>()
				.ForMember(d => d.Translations, o => o.MapFrom(s => s.Translations));

			CreateMap<CWordPageItem, WordDto>()
				.ForMember(d => d.Translations, o => o.MapFrom(s => MapPageItemTranslations(s)));

			CreateMap<AddWordRequest, CAddWordRequest>();

			CreateMap<CDictionaryResponse, DictionaryDto>();

			CreateMap<AddDictionaryRequest, CAddDictionaryRequest>();
			CreateMap<UpdateDictionaryRequest, CUpdateDictionaryRequest>();

			CreateMap<CUserResponse, UserModel>();

			CreateMap<CAlgorithmCost, AlgorithmCost>().ReverseMap();
			CreateMap<CAlgorithmInfoResponse, AlgorithmInfo>();
		}

		private static List<WordTranslationDto> MapPageItemTranslations(CWordPageItem source)
		{
			if (string.IsNullOrEmpty(source.PrimaryTranslation))
			{
				return [];
			}

			return [new WordTranslationDto(source.PrimaryTranslation, TranslationKind.Translation)];
		}
	}
}

