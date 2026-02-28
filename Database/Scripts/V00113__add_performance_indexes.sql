-- Performance indexes based on query analysis (28 Feb 2026)
-- Schema: alchimalia_schema
-- Note: For zero-downtime deployment, run manually with CONCURRENTLY (cannot run inside transaction).

-- StoryReviews: frequently queried by StoryId + IsActive
CREATE INDEX IF NOT EXISTS "idx_story_reviews_storyid_isactive"
    ON "alchimalia_schema"."StoryReviews" ("StoryId", "IsActive");

-- EpicReviews: frequently queried by EpicId + IsActive
CREATE INDEX IF NOT EXISTS "idx_epic_reviews_epicid_isactive"
    ON "alchimalia_schema"."EpicReviews" ("EpicId", "IsActive");

-- EpicStoryProgress: queried by (UserId, EpicId)
CREATE INDEX IF NOT EXISTS "idx_epic_story_progress_userid_epicid"
    ON "alchimalia_schema"."EpicStoryProgress" ("UserId", "EpicId");

-- EpicProgress: queried by (UserId, EpicId)
CREATE INDEX IF NOT EXISTS "idx_epic_progress_userid_epicid"
    ON "alchimalia_schema"."EpicProgress" ("UserId", "EpicId");

-- UserStoryReadProgress: queried by (UserId, StoryId) frequently
CREATE INDEX IF NOT EXISTS "idx_user_story_read_progress_userid_storyid"
    ON "alchimalia_schema"."UserStoryReadProgress" ("UserId", "StoryId");

-- StoryCrafts: listed by OwnerUserId, sorted by UpdatedAt
CREATE INDEX IF NOT EXISTS "idx_story_crafts_owneruserid_updatedat"
    ON "alchimalia_schema"."StoryCrafts" ("OwnerUserId", "UpdatedAt" DESC);

-- StoryCrafts: filtered by AssignedReviewerUserId
CREATE INDEX IF NOT EXISTS "idx_story_crafts_assignedreviewer"
    ON "alchimalia_schema"."StoryCrafts" ("AssignedReviewerUserId")
    WHERE "AssignedReviewerUserId" IS NOT NULL;

-- StoryCraftTranslations: queried by (StoryCraftId, LanguageCode)
CREATE INDEX IF NOT EXISTS "idx_story_craft_translations_craftid_lang"
    ON "alchimalia_schema"."StoryCraftTranslations" ("StoryCraftId", "LanguageCode");

-- StoryPublishJobs: queried by (StoryId, Status)
CREATE INDEX IF NOT EXISTS "idx_story_publish_jobs_storyid_status"
    ON "alchimalia_schema"."StoryPublishJobs" ("StoryId", "Status");

-- UserTokenBalances: queried by (UserId, Type, Value)
CREATE INDEX IF NOT EXISTS "idx_user_token_balances_userid_type_value"
    ON "alchimalia_schema"."UserTokenBalances" ("UserId", "Type", "Value");

-- HeroProgress: queried by (UserId, HeroId)
CREATE INDEX IF NOT EXISTS "idx_hero_progress_userid_heroid"
    ON "alchimalia_schema"."HeroProgress" ("UserId", "HeroId");

-- TreeProgress: queried by (UserId, TreeConfigurationId)
CREATE INDEX IF NOT EXISTS "idx_tree_progress_userid_treeconfigid"
    ON "alchimalia_schema"."TreeProgress" ("UserId", "TreeConfigurationId");

-- StoryProgress: queried by (UserId, TreeConfigurationId, StoryId)
CREATE INDEX IF NOT EXISTS "idx_story_progress_userid_treeconfigid_storyid"
    ON "alchimalia_schema"."StoryProgress" ("UserId", "TreeConfigurationId", "StoryId");

-- StoryQuizAnswers: queried by (UserId, StoryId)
CREATE INDEX IF NOT EXISTS "idx_story_quiz_answers_userid_storyid"
    ON "alchimalia_schema"."StoryQuizAnswers" ("UserId", "StoryId");

-- UserFavoriteStories: queried by UserId
CREATE INDEX IF NOT EXISTS "idx_user_favorite_stories_userid"
    ON "alchimalia_schema"."UserFavoriteStories" ("UserId");

-- UserFavoriteEpics: queried by UserId
CREATE INDEX IF NOT EXISTS "idx_user_favorite_epics_userid"
    ON "alchimalia_schema"."UserFavoriteEpics" ("UserId");

-- StoryReaders: queried by StoryId (for reader counts)
CREATE INDEX IF NOT EXISTS "idx_story_readers_storyid"
    ON "alchimalia_schema"."StoryReaders" ("StoryId");

-- EpicReaders: queried by EpicId
CREATE INDEX IF NOT EXISTS "idx_epic_readers_epicid"
    ON "alchimalia_schema"."EpicReaders" ("EpicId");

-- StoryLikes: queried by StoryId
CREATE INDEX IF NOT EXISTS "idx_story_likes_storyid"
    ON "alchimalia_schema"."StoryLikes" ("StoryId");

-- EpicLikes: queried by EpicId
CREATE INDEX IF NOT EXISTS "idx_epic_likes_epicid"
    ON "alchimalia_schema"."EpicLikes" ("EpicId");
