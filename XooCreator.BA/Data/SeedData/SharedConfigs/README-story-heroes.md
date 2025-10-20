# Story Heroes System

## Overview
The Story Heroes system allows heroes to be unlocked through story completion. This provides a flexible way to reward players with new heroes as they progress through the game's narrative.

## Structure

### Database Entities
- **StoryHero**: Defines a hero that can be unlocked through stories
- **StoryHeroUnlock**: Links stories to story heroes with unlock conditions

### Configuration Files
- **story-heroes.json**: Contains the definition of story heroes and their unlock conditions
- **story-heroes.json** (in Translations): Contains localized names, descriptions, and stories for each hero

### Story Integration
Each story JSON file can include an `unlockedStoryHeroes` array that specifies which heroes are unlocked when the story is completed.

## Example Usage

### Story Heroes Configuration (SharedConfigs/story-heroes.json)
```json
{
  "storyHeroes": [
    {
      "id": "linkaro",
      "heroId": "linkaro",
      "unlockConditions": {
        "type": "story_completion",
        "requiredStories": ["lunaria-s1"],
        "minProgress": 100
      }
    }
  ]
}
```

### Story Integration (Stories/lunaria-s1.json)
```json
{
  "storyId": "lunaria-s1",
  "title": "Linkaro",
  "unlockedStoryHeroes": ["linkaro"],
  // ... rest of story
}
```

### Translations (Translations/ro-RO/story-heroes.json)
```json
{
  "story_hero_linkaro_name": "Linkaro",
  "story_hero_linkaro_description": "Mâțo-Iepurele păstrător al păcii",
  "story_hero_linkaro_story": "Povestea lui Linkaro..."
}
```

## Current Heroes
- **Linkaro**: Unlocked by completing "lunaria-s1"
- **Grubot**: Unlocked by completing "mechanika-s1"

## Adding New Story Heroes
1. Add hero definition to `SharedConfigs/story-heroes.json`
2. Add translations to all language files in `Translations/`
3. Update story files to include the hero in `unlockedStoryHeroes`
4. Create corresponding HeroDefinition entries in the database
5. Run database migration to apply changes
