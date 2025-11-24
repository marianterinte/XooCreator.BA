@echo off
echo ==========================================
echo XooCreator Backend Refactoring Setup
echo ==========================================
echo.

cd ..

echo 1. Creating Infrastructure Directories...
if not exist "Infrastructure\Configuration" mkdir "Infrastructure\Configuration"
if not exist "Infrastructure\Initialization" mkdir "Infrastructure\Initialization"
if not exist "Infrastructure\Seeding" mkdir "Infrastructure\Seeding"

echo 2. Creating Feature Directories...
if not exist "Features\Story-Editor\Services\Content" mkdir "Features\Story-Editor\Services\Content"
if not exist "Features\Story-Editor\Services\Cloning" mkdir "Features\Story-Editor\Services\Cloning"
if not exist "Features\Story-Editor\Services\Seeding" mkdir "Features\Story-Editor\Services\Seeding"

echo.
echo 3. Creating Infrastructure Class Skeletons...

echo namespace XooCreator.BA.Infrastructure.Configuration; > "Infrastructure\Configuration\SwaggerConfiguration.cs"
echo. >> "Infrastructure\Configuration\SwaggerConfiguration.cs"
echo public static class SwaggerConfiguration >> "Infrastructure\Configuration\SwaggerConfiguration.cs"
echo { >> "Infrastructure\Configuration\SwaggerConfiguration.cs"
echo     // TODO: Move Swagger configuration here >> "Infrastructure\Configuration\SwaggerConfiguration.cs"
echo } >> "Infrastructure\Configuration\SwaggerConfiguration.cs"

echo namespace XooCreator.BA.Infrastructure.Configuration; > "Infrastructure\Configuration\AuthConfiguration.cs"
echo. >> "Infrastructure\Configuration\AuthConfiguration.cs"
echo public static class AuthConfiguration >> "Infrastructure\Configuration\AuthConfiguration.cs"
echo { >> "Infrastructure\Configuration\AuthConfiguration.cs"
echo     // TODO: Move Auth configuration here >> "Infrastructure\Configuration\AuthConfiguration.cs"
echo } >> "Infrastructure\Configuration\AuthConfiguration.cs"

echo namespace XooCreator.BA.Infrastructure.Configuration; > "Infrastructure\Configuration\DatabaseConfiguration.cs"
echo. >> "Infrastructure\Configuration\DatabaseConfiguration.cs"
echo public static class DatabaseConfiguration >> "Infrastructure\Configuration\DatabaseConfiguration.cs"
echo { >> "Infrastructure\Configuration\DatabaseConfiguration.cs"
echo     // TODO: Move Database configuration here >> "Infrastructure\Configuration\DatabaseConfiguration.cs"
echo } >> "Infrastructure\Configuration\DatabaseConfiguration.cs"

echo namespace XooCreator.BA.Infrastructure.Configuration; > "Infrastructure\Configuration\CorsConfiguration.cs"
echo. >> "Infrastructure\Configuration\CorsConfiguration.cs"
echo public static class CorsConfiguration >> "Infrastructure\Configuration\CorsConfiguration.cs"
echo { >> "Infrastructure\Configuration\CorsConfiguration.cs"
echo     // TODO: Move CORS configuration here >> "Infrastructure\Configuration\CorsConfiguration.cs"
echo } >> "Infrastructure\Configuration\CorsConfiguration.cs"

echo namespace XooCreator.BA.Infrastructure.Initialization; > "Infrastructure\Initialization\DbInitializer.cs"
echo. >> "Infrastructure\Initialization\DbInitializer.cs"
echo public class DbInitializer >> "Infrastructure\Initialization\DbInitializer.cs"
echo { >> "Infrastructure\Initialization\DbInitializer.cs"
echo     // TODO: Move migration and seeding logic here >> "Infrastructure\Initialization\DbInitializer.cs"
echo } >> "Infrastructure\Initialization\DbInitializer.cs"

echo namespace XooCreator.BA.Infrastructure.Seeding; > "Infrastructure\Seeding\JsonFileSeeder.cs"
echo. >> "Infrastructure\Seeding\JsonFileSeeder.cs"
echo public class JsonFileSeeder^<T^> >> "Infrastructure\Seeding\JsonFileSeeder.cs"
echo { >> "Infrastructure\Seeding\JsonFileSeeder.cs"
echo     // TODO: Generic JSON seeding logic >> "Infrastructure\Seeding\JsonFileSeeder.cs"
echo } >> "Infrastructure\Seeding\JsonFileSeeder.cs"

echo namespace XooCreator.BA.Infrastructure.Seeding; > "Infrastructure\Seeding\SeedingUtils.cs"
echo. >> "Infrastructure\Seeding\SeedingUtils.cs"
echo public static class SeedingUtils >> "Infrastructure\Seeding\SeedingUtils.cs"
echo { >> "Infrastructure\Seeding\SeedingUtils.cs"
echo     // TODO: Helper methods like GenerateAuthorId >> "Infrastructure\Seeding\SeedingUtils.cs"
echo } >> "Infrastructure\Seeding\SeedingUtils.cs"

echo.
echo 4. Creating Story Editor Service Skeletons...

echo namespace XooCreator.BA.Features.StoryEditor.Services.Content; > "Features\Story-Editor\Services\Content\StoryDraftManager.cs"
echo. >> "Features\Story-Editor\Services\Content\StoryDraftManager.cs"
echo public class StoryDraftManager >> "Features\Story-Editor\Services\Content\StoryDraftManager.cs"
echo { >> "Features\Story-Editor\Services\Content\StoryDraftManager.cs"
echo     // TODO: Logic for EnsureDraftAsync >> "Features\Story-Editor\Services\Content\StoryDraftManager.cs"
echo } >> "Features\Story-Editor\Services\Content\StoryDraftManager.cs"

echo namespace XooCreator.BA.Features.StoryEditor.Services.Content; > "Features\Story-Editor\Services\Content\StoryTranslationManager.cs"
echo. >> "Features\Story-Editor\Services\Content\StoryTranslationManager.cs"
echo public class StoryTranslationManager >> "Features\Story-Editor\Services\Content\StoryTranslationManager.cs"
echo { >> "Features\Story-Editor\Services\Content\StoryTranslationManager.cs"
echo     // TODO: Logic for EnsureTranslationAsync >> "Features\Story-Editor\Services\Content\StoryTranslationManager.cs"
echo } >> "Features\Story-Editor\Services\Content\StoryTranslationManager.cs"

echo namespace XooCreator.BA.Features.StoryEditor.Services.Content; > "Features\Story-Editor\Services\Content\StoryTileUpdater.cs"
echo. >> "Features\Story-Editor\Services\Content\StoryTileUpdater.cs"
echo public class StoryTileUpdater >> "Features\Story-Editor\Services\Content\StoryTileUpdater.cs"
echo { >> "Features\Story-Editor\Services\Content\StoryTileUpdater.cs"
echo     // TODO: Logic for UpdateTilesAsync >> "Features\Story-Editor\Services\Content\StoryTileUpdater.cs"
echo } >> "Features\Story-Editor\Services\Content\StoryTileUpdater.cs"

echo namespace XooCreator.BA.Features.StoryEditor.Services.Content; > "Features\Story-Editor\Services\Content\StoryAnswerUpdater.cs"
echo. >> "Features\Story-Editor\Services\Content\StoryAnswerUpdater.cs"
echo public class StoryAnswerUpdater >> "Features\Story-Editor\Services\Content\StoryAnswerUpdater.cs"
echo { >> "Features\Story-Editor\Services\Content\StoryAnswerUpdater.cs"
echo     // TODO: Logic for UpdateAnswersAsync >> "Features\Story-Editor\Services\Content\StoryAnswerUpdater.cs"
echo } >> "Features\Story-Editor\Services\Content\StoryAnswerUpdater.cs"

echo namespace XooCreator.BA.Features.StoryEditor.Services.Content; > "Features\Story-Editor\Services\Content\StoryOwnershipService.cs"
echo. >> "Features\Story-Editor\Services\Content\StoryOwnershipService.cs"
echo public class StoryOwnershipService >> "Features\Story-Editor\Services\Content\StoryOwnershipService.cs"
echo { >> "Features\Story-Editor\Services\Content\StoryOwnershipService.cs"
echo     // TODO: Logic for checking ownership >> "Features\Story-Editor\Services\Content\StoryOwnershipService.cs"
echo } >> "Features\Story-Editor\Services\Content\StoryOwnershipService.cs"

echo.
echo 5. Creating Cloning Service Skeletons...

echo namespace XooCreator.BA.Features.StoryEditor.Services.Cloning; > "Features\Story-Editor\Services\Cloning\StoryCloner.cs"
echo. >> "Features\Story-Editor\Services\Cloning\StoryCloner.cs"
echo public class StoryCloner >> "Features\Story-Editor\Services\Cloning\StoryCloner.cs"
echo { >> "Features\Story-Editor\Services\Cloning\StoryCloner.cs"
echo     // TODO: Main cloning logic >> "Features\Story-Editor\Services\Cloning\StoryCloner.cs"
echo } >> "Features\Story-Editor\Services\Cloning\StoryCloner.cs"

echo namespace XooCreator.BA.Features.StoryEditor.Services.Cloning; > "Features\Story-Editor\Services\Cloning\StorySourceMapper.cs"
echo. >> "Features\Story-Editor\Services\Cloning\StorySourceMapper.cs"
echo public class StorySourceMapper >> "Features\Story-Editor\Services\Cloning\StorySourceMapper.cs"
echo { >> "Features\Story-Editor\Services\Cloning\StorySourceMapper.cs"
echo     // TODO: Mapping logic >> "Features\Story-Editor\Services\Cloning\StorySourceMapper.cs"
echo } >> "Features\Story-Editor\Services\Cloning\StorySourceMapper.cs"

echo.
echo 6. Creating Seeding Service Skeletons...

echo namespace XooCreator.BA.Features.StoryEditor.Services.Seeding; > "Features\Story-Editor\Services\Seeding\AuthorSeeder.cs"
echo. >> "Features\Story-Editor\Services\Seeding\AuthorSeeder.cs"
echo public class AuthorSeeder >> "Features\Story-Editor\Services\Seeding\AuthorSeeder.cs"
echo { >> "Features\Story-Editor\Services\Seeding\AuthorSeeder.cs"
echo     // TODO: Author seeding logic >> "Features\Story-Editor\Services\Seeding\AuthorSeeder.cs"
echo } >> "Features\Story-Editor\Services\Seeding\AuthorSeeder.cs"

echo namespace XooCreator.BA.Features.StoryEditor.Services.Seeding; > "Features\Story-Editor\Services\Seeding\TopicSeeder.cs"
echo. >> "Features\Story-Editor\Services\Seeding\TopicSeeder.cs"
echo public class TopicSeeder >> "Features\Story-Editor\Services\Seeding\TopicSeeder.cs"
echo { >> "Features\Story-Editor\Services\Seeding\TopicSeeder.cs"
echo     // TODO: Topic seeding logic >> "Features\Story-Editor\Services\Seeding\TopicSeeder.cs"
echo } >> "Features\Story-Editor\Services\Seeding\TopicSeeder.cs"

echo namespace XooCreator.BA.Features.StoryEditor.Services.Seeding; > "Features\Story-Editor\Services\Seeding\AgeGroupSeeder.cs"
echo. >> "Features\Story-Editor\Services\Seeding\AgeGroupSeeder.cs"
echo public class AgeGroupSeeder >> "Features\Story-Editor\Services\Seeding\AgeGroupSeeder.cs"
echo { >> "Features\Story-Editor\Services\Seeding\AgeGroupSeeder.cs"
echo     // TODO: AgeGroup seeding logic >> "Features\Story-Editor\Services\Seeding\AgeGroupSeeder.cs"
echo } >> "Features\Story-Editor\Services\Seeding\AgeGroupSeeder.cs"

echo.
echo ==========================================
echo Setup Complete!
echo ==========================================
pause
