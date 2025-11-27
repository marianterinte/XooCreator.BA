using XooCreator.BA.Data.Repositories;
using XooCreator.BA.Infrastructure.Services;
using XooCreator.BA.Infrastructure.Services.Blob;
using XooCreator.BA.Services;
using XooCreator.BA.Features.User.Services;
using XooCreator.BA.Features.UserAdministration.Repositories;
using XooCreator.BA.Features.UserAdministration.Services;
using XooCreator.BA.Features.TreeOfHeroes.Services;
using XooCreator.BA.Features.TreeOfHeroes.Repositories;
using XooCreator.BA.Features.TreeOfLight.Services;
using XooCreator.BA.Features.TreeOfLight.Repositories;
using XooCreator.BA.Features.Stories.Services;
using XooCreator.BA.Features.Stories.Repositories;
using XooCreator.BA.Features.StoryEditor.Services;
using XooCreator.BA.Features.StoryEditor.Services.Content;
using XooCreator.BA.Features.StoryEditor.Services.Cloning;
using XooCreator.BA.Features.StoryEditor.Repositories;
using XooCreator.BA.Features.TalesOfAlchimalia.Market.Repositories;
using XooCreator.BA.Features.TalesOfAlchimalia.Market.Services;
using XooCreator.BA.Features.TalesOfAlchimalia.Market.Mappers;
using XooCreator.BA.Features.Payment.Services;
using XooCreator.BA.Features.StoryFeedback.Repositories;
using XooCreator.BA.Features.StoryFeedback.Services;

namespace XooCreator.BA.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all infrastructure services (Auth0, UserContext, Blob, Health, etc.)
    /// </summary>
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<IAuth0UserService, Auth0UserService>();
        services.AddScoped<IUserContextService, UserContextService>();
        services.AddSingleton<IBlobSasService, BlobSasService>();
        services.AddScoped<IDbHealthService, DbHealthService>();
        
        return services;
    }

    /// <summary>
    /// Registers user-related services
    /// </summary>
    public static IServiceCollection AddUserServices(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserProfileService, UserProfileService>();
        services.AddScoped<IUserAdministrationRepository, UserAdministrationRepository>();
        services.AddScoped<IUserAdministrationService, UserAdministrationService>();
        
        return services;
    }

    /// <summary>
    /// Registers seed services for data initialization
    /// </summary>
    public static IServiceCollection AddSeedServices(this IServiceCollection services)
    {
        services.AddScoped<ICreatureBuilderService, CreatureBuilderService>();
        services.AddScoped<ISeedDiscoveryService, SeedDiscoveryService>();
        services.AddScoped<IBestiaryFileUpdater, BestiaryFileUpdater>();
        services.AddScoped<IHeroDefinitionSeedService, HeroDefinitionSeedService>();
        services.AddScoped<IHeroTreeProvider, HeroTreeProvider>();
        services.AddScoped<IStoryTopicsSeedService, StoryTopicsSeedService>();
        
        return services;
    }

    /// <summary>
    /// Registers tree-related services (TreeOfHeroes, TreeOfLight, TreeModel)
    /// </summary>
    public static IServiceCollection AddTreeServices(this IServiceCollection services)
    {
        services.AddScoped<ITreeOfHeroesRepository, TreeOfHeroesRepository>();
        services.AddScoped<ITreeOfHeroesService, TreeOfHeroesService>();
        services.AddScoped<ITreeOfLightTranslationService, TreeOfLightTranslationService>();
        services.AddScoped<ITreeOfLightRepository, TreeOfLightRepository>();
        services.AddScoped<ITreeOfLightService, TreeOfLightService>();
        services.AddScoped<ITreeModelRepository, TreeModelRepository>();
        services.AddScoped<ITreeModelService, TreeModelService>();
        
        return services;
    }

    /// <summary>
    /// Registers story-related services (Stories, StoryEditor)
    /// </summary>
    public static IServiceCollection AddStoryServices(this IServiceCollection services)
    {
        services.AddScoped<IStoriesService, StoriesService>();
        services.AddScoped<IStoriesRepository, StoriesRepository>();
        services.AddScoped<IStoryCraftsRepository, StoryCraftsRepository>();
        
        // Refactored Story Editor Services (Content Management)
        services.AddScoped<IStoryDraftManager, StoryDraftManager>();
        services.AddScoped<IStoryTranslationManager, StoryTranslationManager>();
        services.AddScoped<IStoryOwnershipService, StoryOwnershipService>();
        services.AddScoped<IStoryAnswerUpdater, StoryAnswerUpdater>();
        services.AddScoped<IStoryTileUpdater, StoryTileUpdater>();
        
        // Cloning Services (unified logic for Copy/Fork/New Version)
        services.AddScoped<IStorySourceMapper, StorySourceMapper>();
        services.AddScoped<IStoryCloner, StoryCloner>();
        
        // Main Story Editor Service (Facade)
        services.AddScoped<IStoryEditorService, StoryEditorService>();
        
        // Story Publishing & Asset Management
        services.AddScoped<IStoryPublishingService, StoryPublishingService>();
        services.AddScoped<IStoryPublishAssetService, StoryPublishAssetService>();
        services.AddScoped<IStoryIdGenerator, StoryIdGenerator>();
        services.AddScoped<IStoryAssetCopyService, StoryAssetCopyService>();
        services.AddScoped<IStoryCopyService, StoryCopyService>();
        services.AddScoped<IStoryDraftAssetCleanupService, StoryDraftAssetCleanupService>();
        services.AddScoped<IStoryPublishedAssetCleanupService, StoryPublishedAssetCleanupService>();
        
        return services;
    }

    /// <summary>
    /// Registers AI services (Google and OpenAI)
    /// </summary>
    public static IServiceCollection AddAIServices(this IServiceCollection services)
    {
        services.AddHttpClient(); // Required for Google and OpenAI services
        
        // Google AI Services
        services.AddScoped<IGoogleAudioGeneratorService, GoogleAudioGeneratorService>();
        services.AddScoped<IGoogleTextService, GoogleTextService>();
        services.AddScoped<IGoogleImageService, GoogleImageService>();
        services.AddScoped<IGoogleFullStoryService, GoogleFullStoryService>();
        
        // OpenAI Services
        services.AddScoped<IOpenAIAudioGeneratorService, OpenAIAudioGeneratorService>();
        services.AddScoped<IOpenAITextService, OpenAITextService>();
        services.AddScoped<IOpenAIImageService, OpenAIImageService>();
        services.AddScoped<IOpenAIFullStoryService, OpenAIFullStoryService>();
        
        return services;
    }

    /// <summary>
    /// Registers marketplace services
    /// </summary>
    public static IServiceCollection AddMarketplaceServices(this IServiceCollection services)
    {
        services.AddScoped<IStoryReviewsRepository, StoryReviewsRepository>();
        services.AddScoped<IStoryReviewsService, StoryReviewsService>();
        services.AddScoped<IFavoritesRepository, FavoritesRepository>();
        services.AddScoped<IFavoritesService, FavoritesService>();
        services.AddScoped<StoryDetailsMapper>();
        services.AddScoped<IStoriesMarketplaceRepository, StoriesMarketplaceRepository>();
        services.AddScoped<IStoriesMarketplaceService, StoriesMarketplaceService>();
        
        return services;
    }

    /// <summary>
    /// Registers payment services
    /// </summary>
    public static IServiceCollection AddPaymentServices(this IServiceCollection services)
    {
        services.AddScoped<IPaymentService, PaymentService>();
        
        return services;
    }

    /// <summary>
    /// Registers story feedback services
    /// </summary>
    public static IServiceCollection AddFeedbackServices(this IServiceCollection services)
    {
        services.AddScoped<IStoryFeedbackRepository, StoryFeedbackRepository>();
        services.AddScoped<IStoryFeedbackService, StoryFeedbackService>();
        
        return services;
    }

    /// <summary>
    /// Registers all application services
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddInfrastructureServices();
        services.AddUserServices();
        services.AddSeedServices();
        services.AddTreeServices();
        services.AddStoryServices();
        services.AddAIServices();
        services.AddMarketplaceServices();
        services.AddPaymentServices();
        services.AddFeedbackServices();
        
        return services;
    }
}

