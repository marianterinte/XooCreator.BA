# Migration Notes: Add Email Field and Marian Test User

## Changes Made

### 1. Updated UserAlchimalia Entity
- Added `Email` field to `UserAlchimalia.cs`
- Required field with max length 256

### 2. Updated DbContext Configuration
- Added Email field configuration in `XooDbContext.cs`
- Added constraints: `HasMaxLength(256).IsRequired()`
- Added DisplayName and Auth0Sub length constraints for consistency

### 3. Updated Seeding Data
- Modified existing test user to include email: `test@example.com`
- Added new test user "Marian" with:
  - ID: `22222222-2222-2222-2222-222222222222`
  - Auth0Sub: `marian-test-sub`
  - DisplayName: `Marian`
  - Email: `marian@example.com`

### 4. Updated UserRepository Interface
- Modified `EnsureAsync` method to accept email parameter
- Updated implementation to set Email field when creating new users

## Next Steps

To apply these changes to the database:

1. Generate migration:
   ```bash
   cd XooCreatorBA/XooCreator.BA
   dotnet ef migrations add AddEmailAndMarianUser
   ```

2. Update database:
   ```bash
   dotnet ef database update
   ```

## Database Schema Changes

```sql
-- Add Email column to UsersAlchimalia table
ALTER TABLE "UsersAlchimalia" ADD COLUMN "Email" character varying(256) NOT NULL DEFAULT '';

-- Insert new test user Marian
INSERT INTO "UsersAlchimalia" ("Id", "Auth0Sub", "DisplayName", "Email", "CreatedAt") 
VALUES ('22222222-2222-2222-2222-222222222222', 'marian-test-sub', 'Marian', 'marian@example.com', NOW());
```

## Test Users Available After Migration

1. **Test User**
   - ID: `11111111-1111-1111-1111-111111111111`
   - Email: `test@example.com`
   - Auth0Sub: `test-user-sub`

2. **Marian** (NEW)
   - ID: `22222222-2222-2222-2222-222222222222`
   - Email: `marian@example.com`  
   - Auth0Sub: `marian-test-sub`
