using XooCreator.BA.Data.Repositories;
using XooCreator.BA.Infrastructure.Services;
using XooCreator.BA.Infrastructure.Services.Queue;
using XooCreator.BA.Infrastructure.Services.Blob;
using XooCreator.BA.Services;
using XooCreator.BA.Features.User.Services;
using XooCreator.BA.Features.UserAdministration.Repositories;
using XooCreator.BA.Features.UserAdministration.Services;
using XooCreator.BA.Features.TreeOfHeroes.Services;
using XooCreator.BA.Features.TreeOfHeroes.Repositories;
using XooCreator.BA.Features.HeroStoryRewards.Services;
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
using XooCreator.BA.Features.GuestSync.Services;
using XooCreator.BA.Features.TalesOfAlchimalia.Market.Mappers;
using XooCreator.BA.Features.TalesOfAlchimalia.Market.Caching;
using XooCreator.BA.Features.Payment.Services;
using XooCreator.BA.Features.StoryFeedback.Repositories;
using XooCreator.BA.Features.StoryFeedback.Services;
using XooCreator.BA.Features.AlchimaliaUniverse.Repositories;
using XooCreator.BA.Features.AlchimaliaUniverse.Services;
using XooCreator.BA.Features.StoryCreatorsChallenge.Services;
using XooCreator.BA.Infrastructure.Services.Images;
using XooCreator.BA.Infrastructure.Services.Jobs;
using XooCreator.BA.Data;
using XooCreator.BA.Infrastructure.Caching;

namespace XooCreator.BA.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all infrastructure services (Auth0, UserContext, Blob, Health, etc.)
    /// </summary>
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddMemoryCache(); // Shared in-process cache (users, marketplace, universe, etc.)
        services.AddHttpContextAccessor();
        services.AddScoped<IAuth0UserService, Auth0UserService>();
        services.AddScoped<IUserContextService, UserContextService>();
        services.AddSingleton<IBlobSasService, BlobSasService>();
        services.AddScoped<IDbHealthService, DbHealthService>();
        services.AddSingleton<IJobEventsHub, InMemoryJobEventsHub>();
        services.AddSingleton<IAzureQueueClientFactory, AzureQueueClientFactory>();
        services.AddSingleton<IStoryPublishQueue, StoryPublishQueue>();
        services.AddSingleton<IStoryVersionQueue, StoryVersionQueue>();
        services.AddSingleton<IEpicPublishQueue, EpicPublishQueue>();
        services.AddSingleton<IEpicVersionQueue, EpicVersionQueue>();
        services.AddSingleton<IStoryImportQueue, StoryImportQueue>();
        services.AddSingleton<IStoryForkQueue, StoryForkQueue>();
        services.AddSingleton<IStoryForkAssetsQueue, StoryForkAssetsQueue>();
        services.AddSingleton<IStoryExportQueue, StoryExportQueue>();
        services.AddSingleton<IStoryDocumentExportQueue, StoryDocumentExportQueue>();
        services.AddSingleton<IStoryAudioExportQueue, StoryAudioExportQueue>();
        services.AddSingleton<IStoryAudioImportQueue, StoryAudioImportQueue>();
        services.AddSingleton<IStoryTranslationQueue, StoryTranslationQueue>();
        services.AddSingleton<IStoryImageImportQueue, StoryImageImportQueue>();
        services.AddSingleton<IStoryImageExportQueue, StoryImageExportQueue>();
        services.AddSingleton<IEpicAggregatesQueue, EpicAggregatesQueue>();
        services.AddSingleton<IHeroPublishQueue, HeroPublishQueue>();
        services.AddSingleton<IAnimalPublishQueue, AnimalPublishQueue>();
        services.AddSingleton<IAppCache, MemoryAppCache>();
        services.AddSingleton<IDirectUploadRateLimitService, DirectUploadRateLimitService>();

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
        services.AddScoped<XooCreator.BA.Features.User.Services.ICreatorTokenService, XooCreator.BA.Features.User.Services.CreatorTokenService>();
        services.AddScoped<XooCreator.BA.Features.User.Services.INewsletterSubscriptionService, XooCreator.BA.Features.User.Services.NewsletterSubscriptionService>();
        services.AddScoped<XooCreator.BA.Features.User.Services.INewsletterSendingService, XooCreator.BA.Features.User.Services.NewsletterSendingService>();
        services.AddScoped<XooCreator.BA.Features.User.Services.INewsletterTemplateService, XooCreator.BA.Features.User.Services.NewsletterTemplateService>();
        services.AddScoped<XooCreator.BA.Features.User.Services.INewsletterAutoSendService, XooCreator.BA.Features.User.Services.NewsletterAutoSendService>();
        services.AddScoped<XooCreator.BA.Infrastructure.Services.Email.IEmailService, XooCreator.BA.Infrastructure.Services.Email.EmailService>();
        
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
        services.AddScoped<IHeroStoryRewardsService, HeroStoryRewardsService>();
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

        // Image size variants (s/m) for published images
        services.AddScoped<IImageCompressionService, ImageCompressionService>();
        
        // Refactored Story Editor Services (Content Management)
        services.AddScoped<IStoryDraftManager, StoryDraftManager>();
        services.AddScoped<IStoryTranslationManager, StoryTranslationManager>();
        services.AddScoped<IStoryOwnershipService, StoryOwnershipService>();
        services.AddScoped<IStoryAnswerUpdater, StoryAnswerUpdater>();
        services.AddScoped<IStoryTileUpdater, StoryTileUpdater>();
        services.AddScoped<IStoryAssetLinkService, StoryAssetLinkService>();
        services.AddScoped<IStoryPublishChangeLogService, StoryPublishChangeLogService>();
        services.AddScoped<IStoryTranslationService, StoryTranslationService>();
        
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
        services.AddScoped<IStoryAssetReplacementService, StoryAssetReplacementService>();
        services.AddScoped<IStoryExportService, StoryExportService>();
        services.AddScoped<IStoryDocumentExportService, StoryDocumentExportService>();
        services.AddScoped<XooCreator.BA.Features.StoryEditor.Services.IStoryAudioImportProcessor, XooCreator.BA.Features.StoryEditor.Services.StoryAudioImportProcessor>();
        services.AddScoped<XooCreator.BA.Features.StoryEditor.Services.IStoryImageImportProcessor, XooCreator.BA.Features.StoryEditor.Services.StoryImageImportProcessor>();
        
        // Story Epic Services
        services.AddScoped<XooCreator.BA.Features.StoryEditor.StoryEpic.Services.IStoryEpicService, XooCreator.BA.Features.StoryEditor.StoryEpic.Services.StoryEpicService>();
        services.AddScoped<XooCreator.BA.Features.StoryEditor.StoryEpic.Services.IStoryEpicPublishingService, XooCreator.BA.Features.StoryEditor.StoryEpic.Services.StoryEpicPublishingService>();
        services.AddScoped<XooCreator.BA.Features.StoryEditor.StoryEpic.Repositories.IEpicProgressRepository, XooCreator.BA.Features.StoryEditor.StoryEpic.Repositories.EpicProgressRepository>();
        services.AddScoped<XooCreator.BA.Features.StoryEditor.StoryEpic.Services.IStoryEpicProgressService, XooCreator.BA.Features.StoryEditor.StoryEpic.Services.StoryEpicProgressService>();
        
        // Story Region Services (Independent Regions)
        services.AddScoped<XooCreator.BA.Features.StoryEditor.StoryEpic.Repositories.IStoryRegionRepository, XooCreator.BA.Features.StoryEditor.StoryEpic.Repositories.StoryRegionRepository>();
        services.AddScoped<XooCreator.BA.Features.StoryEditor.StoryEpic.Services.IStoryRegionService, XooCreator.BA.Features.StoryEditor.StoryEpic.Services.StoryRegionService>();
        services.AddScoped<XooCreator.BA.Features.StoryEditor.StoryEpic.Services.IRegionPublishChangeLogService, XooCreator.BA.Features.StoryEditor.StoryEpic.Services.RegionPublishChangeLogService>();
        services.AddScoped<XooCreator.BA.Features.StoryEditor.StoryEpic.Services.IRegionAssetLinkService, XooCreator.BA.Features.StoryEditor.StoryEpic.Services.RegionAssetLinkService>();
        
        // Epic Hero Services (Independent Heroes)
        services.AddScoped<XooCreator.BA.Features.StoryEditor.StoryEpic.Repositories.IEpicHeroRepository, XooCreator.BA.Features.StoryEditor.StoryEpic.Repositories.EpicHeroRepository>();
        services.AddScoped<XooCreator.BA.Features.StoryEditor.StoryEpic.Services.IEpicHeroService, XooCreator.BA.Features.StoryEditor.StoryEpic.Services.EpicHeroService>();
        services.AddScoped<XooCreator.BA.Features.StoryEditor.StoryEpic.Services.IHeroPublishChangeLogService, XooCreator.BA.Features.StoryEditor.StoryEpic.Services.HeroPublishChangeLogService>();
        services.AddScoped<XooCreator.BA.Features.StoryEditor.StoryEpic.Services.IHeroAssetLinkService, XooCreator.BA.Features.StoryEditor.StoryEpic.Services.HeroAssetLinkService>();
        
        // Epic Delta Publish Services
        services.AddScoped<XooCreator.BA.Features.StoryEditor.StoryEpic.Services.IEpicPublishChangeLogService, XooCreator.BA.Features.StoryEditor.StoryEpic.Services.EpicPublishChangeLogService>();
        services.AddScoped<XooCreator.BA.Features.StoryEditor.StoryEpic.Services.IEpicAssetLinkService, XooCreator.BA.Features.StoryEditor.StoryEpic.Services.EpicAssetLinkService>();
        
        // Region and Hero Asset Cleanup Services
        services.AddScoped<XooCreator.BA.Features.StoryEditor.StoryEpic.Services.IRegionPublishedAssetCleanupService, XooCreator.BA.Features.StoryEditor.StoryEpic.Services.RegionPublishedAssetCleanupService>();
        services.AddScoped<XooCreator.BA.Features.StoryEditor.StoryEpic.Services.IHeroPublishedAssetCleanupService, XooCreator.BA.Features.StoryEditor.StoryEpic.Services.HeroPublishedAssetCleanupService>();
        services.AddScoped<XooCreator.BA.Features.StoryEditor.StoryEpic.Services.IEpicPublishedAssetCleanupService, XooCreator.BA.Features.StoryEditor.StoryEpic.Services.EpicPublishedAssetCleanupService>();
        
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
        // Marketplace in-memory cache (single-instance). Base TTL + Stats TTL configured in Program.cs.
        services.AddSingleton<IMarketplaceCatalogCache, MarketplaceCatalogCache>();
        services.AddSingleton<IMarketplaceCacheControl>(sp => (MarketplaceCatalogCache)sp.GetRequiredService<IMarketplaceCatalogCache>());
        services.AddSingleton<IMarketplaceCacheInvalidator>(sp => sp.GetRequiredService<IMarketplaceCacheControl>());

        services.AddScoped<IStoryReviewsRepository, StoryReviewsRepository>();
        services.AddScoped<IStoryReviewsService, StoryReviewsService>();
        services.AddScoped<IEpicReviewsRepository, EpicReviewsRepository>();
        services.AddScoped<IEpicReviewsService, EpicReviewsService>();
        services.AddScoped<IFavoritesRepository, FavoritesRepository>();
        services.AddScoped<IFavoritesService, FavoritesService>();
        services.AddScoped<IEpicFavoritesRepository, EpicFavoritesRepository>();
        services.AddScoped<IEpicFavoritesService, EpicFavoritesService>();
        services.AddScoped<IStoryLikesRepository, StoryLikesRepository>();
        services.AddScoped<IStoryLikesService, StoryLikesService>();
        services.AddScoped<IGuestSyncService, GuestSyncService>();
        services.AddScoped<StoryDetailsMapper>(sp => 
        {
            var context = sp.GetRequiredService<XooDbContext>();
            var reviewsRepo = sp.GetService<IStoryReviewsRepository>();
            var likesRepo = sp.GetService<IStoryLikesRepository>();
            return new StoryDetailsMapper(context, reviewsRepo, likesRepo);
        });
        services.AddScoped<IStoriesMarketplaceRepository, StoriesMarketplaceRepository>();
        services.AddScoped<IStoriesMarketplaceService, StoriesMarketplaceService>();
        services.AddScoped<EpicsMarketplaceRepository>();
        services.AddScoped<IEpicsMarketplaceService, EpicsMarketplaceService>();
        
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
    /// Registers Alchimalia Universe editor services
    /// </summary>
    public static IServiceCollection AddAlchimaliaUniverseServices(this IServiceCollection services)
    {
        // Repositories
        services.AddScoped<IHeroDefinitionRepository, HeroDefinitionRepository>();
        services.AddScoped<IAnimalRepository, AnimalRepository>();
        services.AddScoped<IStoryHeroRepository, StoryHeroRepository>();
        services.AddScoped<IHeroDefinitionCraftRepository, HeroDefinitionCraftRepository>();
        services.AddScoped<IAnimalCraftRepository, AnimalCraftRepository>();
        services.AddScoped<ITreeOfHeroesConfigCraftRepository, TreeOfHeroesConfigCraftRepository>();
        
        // Services
        services.AddScoped<IHeroDefinitionService, HeroDefinitionService>();
        services.AddScoped<IAnimalService, AnimalService>();
        services.AddScoped<IStoryHeroService, StoryHeroService>();
        services.AddScoped<IHeroDefinitionCraftService, HeroDefinitionCraftService>();
        services.AddScoped<IAnimalCraftService, AnimalCraftService>();
        services.AddScoped<ITreeOfHeroesConfigCraftService, TreeOfHeroesConfigCraftService>();
        services.AddScoped<IHeroDefinitionPublishChangeLogService, HeroDefinitionPublishChangeLogService>();
        services.AddScoped<IAnimalPublishChangeLogService, AnimalPublishChangeLogService>();
        
        // Queues
        services.AddSingleton<IHeroDefinitionVersionQueue, HeroDefinitionVersionQueue>();
        services.AddSingleton<IAnimalVersionQueue, AnimalVersionQueue>();

        services.AddScoped<IAlchimaliaUniverseAssetCopyService, AlchimaliaUniverseAssetCopyService>();
        
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
        services.AddAlchimaliaUniverseServices();
        services.AddStoryCreatorsChallengeServices();
        services.AddRewardTokensServices();
        
        return services;
    }

    /// <summary>
    /// Registers Reward Tokens services
    /// </summary>
    public static IServiceCollection AddRewardTokensServices(this IServiceCollection services)
    {
        services.AddScoped<XooCreator.BA.Features.RewardTokens.Services.IRewardTokensService, XooCreator.BA.Features.RewardTokens.Services.RewardTokensService>();
        return services;
    }

    /// <summary>
    /// Registers Story Creators Challenge (CCC) services
    /// </summary>
    public static IServiceCollection AddStoryCreatorsChallengeServices(this IServiceCollection services)
    {
        services.AddScoped<IStoryCreatorsChallengeService, StoryCreatorsChallengeService>();
        
        return services;
    }
}

