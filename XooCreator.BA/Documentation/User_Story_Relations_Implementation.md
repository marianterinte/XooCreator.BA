# User Story Relations Implementation

## Overview
This document describes the implementation of user-story relationship management in the XooCreator backend system. We've added two new entities to track the relationship between users and stories: `UserOwnedStories` for purchased stories and `UserCreatedStories` for user-created stories.

## Implementation Date
January 2025

## New Entities

### 1. UserOwnedStories
**Purpose**: Tracks stories purchased by users from the marketplace

**Location**: `XooCreator.BA/Data/Entities/UserOwnedStories.cs`

**Properties**:
- `Id` (Guid) - Primary key
- `UserId` (Guid) - Foreign key to AlchimaliaUser
- `StoryDefinitionId` (Guid) - Foreign key to StoryDefinition
- `PurchasedAt` (DateTime) - When the story was purchased
- `PurchasePrice` (decimal) - Price paid for the story
- `PurchaseReference` (string?) - Reference to the transaction

**Navigation Properties**:
- `User` - Reference to AlchimaliaUser
- `StoryDefinition` - Reference to StoryDefinition

### 2. UserCreatedStories
**Purpose**: Tracks stories created by users

**Location**: `XooCreator.BA/Data/Entities/UserCreatedStories.cs`

**Properties**:
- `Id` (Guid) - Primary key
- `UserId` (Guid) - Foreign key to AlchimaliaUser
- `StoryDefinitionId` (Guid) - Foreign key to StoryDefinition
- `CreatedAt` (DateTime) - When the story was created
- `PublishedAt` (DateTime?) - When the story was published (optional)
- `IsPublished` (bool) - Publication status
- `CreationNotes` (string?) - Notes about the creation

**Navigation Properties**:
- `User` - Reference to AlchimaliaUser
- `StoryDefinition` - Reference to StoryDefinition

## Database Configuration

### DbContext Updates
**File**: `XooCreator.BA/Data/XooDbContext.cs`

**Added DbSets**:
```csharp
public DbSet<UserOwnedStories> UserOwnedStories => Set<UserOwnedStories>();
public DbSet<UserCreatedStories> UserCreatedStories => Set<UserCreatedStories>();
```

**Entity Configurations**:
- Both entities have unique indexes on `(UserId, StoryDefinitionId)`
- Cascade delete configured for both User and StoryDefinition relationships
- Proper foreign key constraints and navigation properties

## New Endpoints

### 1. Get User Owned Stories
**Endpoint**: `GET /api/{locale}/stories/owned`
**File**: `XooCreator.BA/Features/Stories/Endpoints/GetUserOwnedStoriesEndpoint.cs`

**Features**:
- Returns stories purchased by the authenticated user
- Includes localized titles based on the locale parameter
- Returns purchase information (date, price)
- Requires authentication

**Response DTO**: `GetUserOwnedStoriesResponse`
- `Stories` - List of `OwnedStoryDto`
- `TotalCount` - Total number of owned stories

### 2. Get User Created Stories
**Endpoint**: `GET /api/{locale}/stories/created`
**File**: `XooCreator.BA/Features/Stories/Endpoints/GetUserCreatedStoriesEndpoint.cs`

**Features**:
- Returns stories created by the authenticated user
- Includes localized titles based on the locale parameter
- Returns creation and publication information
- Requires authentication

**Response DTO**: `GetUserCreatedStoriesResponse`
- `Stories` - List of `CreatedStoryDto`
- `TotalCount` - Total number of created stories

## User Context Service Enhancement

### Added Method
**File**: `XooCreator.BA/Infrastructure/UserContextService.cs`

**New Method**: `GetCurrentUserId()`
```csharp
public Guid GetCurrentUserId()
{
    var userId = GetUserIdAsync().GetAwaiter().GetResult();
    if (userId == null)
        throw new UnauthorizedAccessException("User not authenticated");
    return userId.Value;
}
```

**Purpose**: Provides synchronous access to the current user's ID for use in endpoints

## System User Implementation

### Alchimalia-Admin User
**Purpose**: System user for independent stories created by the platform

**User Details**:
- `Id`: `33333333-3333-3333-3333-333333333333`
- `Name`: "Alchimalia-Admin"
- `Email`: "alchimalia@admin.com"
- `Auth0Id`: "alchimalia-admin-sub"
- `Balance`: 1000 credits

**Location**: Seeded in `XooDbContext.OnModelCreating()`

## Independent Stories Seeding

### New Seeding Methods
**File**: `XooCreator.BA/Data/SeedDataService.cs`

**Added Methods**:
1. `LoadIndependentStoriesAsync()` - Loads story definitions from JSON files
2. `LoadIndependentStoryTranslationsAsync()` - Loads translations for all languages
3. `GetStoryDefinitionIdByStoryId()` - Maps storyId to consistent GUIDs

### Story Definition Seed Data
**DTO**: `StoryDefinitionSeedData`
- `StoryId` - Unique story identifier
- `Title` - Story title
- `CoverImageUrl` - Cover image URL
- `Category` - Story category
- `SortOrder` - Display order
- `UnlockedStoryHeroes` - List of unlocked heroes
- `Tiles` - Story tiles with content

### Seeding Process
1. **Users seeded first** (including Alchimalia-Admin)
2. **Story definitions seeded** with `CreatedBy` set to Alchimalia-Admin
3. **Story translations seeded** for all supported languages (ro-ro, en-us, hu-hu)

### Important Seeding Notes
- **Dynamic ID Generation**: Story definitions use `Guid.NewGuid()` for each story to avoid conflicts with existing data
- **Translation Mapping**: Translations are created only for stories that exist in the seeding process
- **Foreign Key Integrity**: Translations reference the actual story definition IDs created during seeding
- **Parameter Passing**: `LoadIndependentStoryTranslationsAsync()` receives the list of created stories to ensure proper mapping
- **Language Enum Usage**: Uses `LanguageCodeExtensions.All()` instead of hardcoded language arrays for better maintainability

### File Structure
```
Data/SeedData/Stories/independent/i18n/
├── ro-ro/
│   └── learn-to-read-s1.json
├── en-us/
│   └── learn-to-read-s1.json
└── hu-hu/
    └── learn-to-read-s1.json
```

## Database Migration Notes

### Required Migrations
1. Add `UserOwnedStories` table
2. Add `UserCreatedStories` table
3. Add foreign key constraints
4. Add unique indexes
5. Seed Alchimalia-Admin user
6. Seed independent stories with proper `CreatedBy` references

### Migration Order
1. Users (including Alchimalia-Admin)
2. Story definitions (with CreatedBy references)
3. Story translations
4. User story relations tables

## API Usage Examples

### Get Owned Stories
```http
GET /api/ro-ro/stories/owned
Authorization: Bearer <token>
```

**Response**:
```json
{
  "stories": [
    {
      "id": "guid",
      "storyId": "learn-to-read-s1",
      "title": "Puf-Puf și prietenul pierdut",
      "coverImageUrl": "images/tol/stories/pp-prietenul-pierdut/0.cover.png",
      "category": "terra",
      "storyCategory": "AlchimaliaEpic",
      "status": "Published",
      "purchasedAt": "2025-01-27T10:00:00Z",
      "purchasePrice": 5.00
    }
  ],
  "totalCount": 1
}
```

### Get Created Stories
```http
GET /api/ro-ro/stories/created
Authorization: Bearer <token>
```

**Response**:
```json
{
  "stories": [
    {
      "id": "guid",
      "storyId": "my-custom-story",
      "title": "My Custom Story",
      "coverImageUrl": "images/custom/cover.png",
      "category": "custom",
      "storyCategory": "UserCreated",
      "status": "Published",
      "createdAt": "2025-01-27T10:00:00Z",
      "publishedAt": "2025-01-27T11:00:00Z",
      "isPublished": true,
      "creationNotes": "My first story"
    }
  ],
  "totalCount": 1
}
```

## Future Enhancements

### Potential Improvements
1. **Bulk Operations**: Add endpoints for bulk story operations
2. **Story Analytics**: Track story views, completion rates
3. **Story Sharing**: Allow users to share stories with others
4. **Story Templates**: Provide templates for story creation
5. **Story Validation**: Add validation rules for user-created stories
6. **Story Moderation**: Add moderation workflow for user stories

### Database Optimizations
1. **Indexes**: Add indexes on frequently queried fields
2. **Partitioning**: Consider partitioning for large story tables
3. **Archiving**: Implement soft delete for old stories
4. **Caching**: Add caching for frequently accessed stories

## Security Considerations

### Authentication
- All endpoints require authentication
- User can only access their own stories
- System validates user ownership before returning data

### Authorization
- Users can only view their own owned/created stories
- System user (Alchimalia-Admin) has elevated privileges
- Proper foreign key constraints prevent orphaned records

### Data Validation
- Input validation on all story creation endpoints
- Proper error handling for missing or invalid data
- Transaction support for complex operations

## Testing Recommendations

### Unit Tests
1. Test entity configurations
2. Test endpoint responses
3. Test user context service
4. Test seeding methods

### Integration Tests
1. Test complete story purchase flow
2. Test story creation workflow
3. Test localization functionality
4. Test authentication and authorization

### Performance Tests
1. Test with large numbers of stories
2. Test concurrent user access
3. Test database query performance
4. Test endpoint response times

## Conclusion

This implementation provides a solid foundation for managing user-story relationships in the XooCreator platform. The system supports both purchased and created stories, with proper localization, authentication, and data integrity. The seeding system ensures that independent stories are properly attributed to the system user, maintaining data consistency and traceability.

The modular design allows for future enhancements while maintaining backward compatibility with existing systems.
