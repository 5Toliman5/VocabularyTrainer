using AutoMapper;
using VocabularyTrainer.Domain.Models;
using VocabularyTrainer.Domain.Models.Auth;
using CReviewGrade = VocabularyTrainer.Api.Contract.Words.ReviewGrade;
using CTranslationKind = VocabularyTrainer.Api.Contract.Words.TranslationKind;
using CWordTranslationDto = VocabularyTrainer.Api.Contract.Words.WordTranslationDto;
using CWordResponse = VocabularyTrainer.Api.Contract.Words.WordResponse;
using CWordPageItem = VocabularyTrainer.Api.Contract.Words.WordPageItem;
using CWordSortBy = VocabularyTrainer.Api.Contract.Words.WordSortBy;
using CGetWordsPagedReq = VocabularyTrainer.Api.Contract.Words.GetWordsPagedRequest;
using CDictResponse = VocabularyTrainer.Api.Contract.Dictionaries.DictionaryResponse;
using CUserResponse = VocabularyTrainer.Api.Contract.Users.UserResponse;
using CUserIdentityResponse = VocabularyTrainer.Api.Contract.Users.UserIdentityResponse;
using CAlgorithmInfoResp = VocabularyTrainer.Api.Contract.Algorithms.AlgorithmInfoResponse;
using CAlgorithmCost = VocabularyTrainer.Api.Contract.Algorithms.AlgorithmCost;
using CAuthTokensResponse = VocabularyTrainer.Api.Contract.Auth.AuthTokensResponse;
using CClientInfoDto = VocabularyTrainer.Api.Contract.Auth.ClientInfoDto;
using CVerificationPurpose = VocabularyTrainer.Api.Contract.Auth.VerificationPurpose;

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

			CreateMap<WordDto, CWordResponse>();
			CreateMap<WordDto, CWordPageItem>()
				.ForCtorParam("PrimaryTranslation", o => o.MapFrom(s => s.PrimaryTranslation));

			CreateMap<CGetWordsPagedReq, GetWordsPagedRequest>();

			CreateMap<DictionaryDto, CDictResponse>();

			CreateMap<UserModel, CUserResponse>();
			CreateMap<UserIdentityDto, CUserIdentityResponse>();

			CreateMap<CVerificationPurpose, VerificationPurpose>();
			CreateMap<CClientInfoDto, ClientInfo>();
			CreateMap<AuthTokens, CAuthTokensResponse>();

			CreateMap<AlgorithmCost, CAlgorithmCost>().ReverseMap();
			CreateMap<AlgorithmInfo, CAlgorithmInfoResp>();
		}
	}
}
