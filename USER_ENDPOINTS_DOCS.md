# User Profile & Credit Management API Endpoints

## Overview

Implementation completă a logicii de credite și permisiuni pe backend pentru Laboratory of Imagination (Creature Builder).

## Logica de Business

### Credit System Logic
- **Full Access**: `hasEverPurchased = true` AND `credits > 0`
- **Limited Access**: Doar 3 animale + 3 body parts (head, body, arms)
- **Locked Parts**: legs, tail, wings, horn, horns (pentru free users)

### User Access Matrix
| User Type | Animals Unlocked | Parts Unlocked | Can Generate |
|-----------|------------------|----------------|--------------|
| Free User | 3 din total | head, body, arms | Da (cu limitări) |
| Paid User (cu credite) | All animals | All parts | Da (fără limitări) |
| Paid User (fără credite) | 3 din total | head, body, arms | Nu |

## New API Endpoints

### 1. GET `/api/user/profile`
**Returnează profilul complet al utilizatorului cu credite și permisiuni**

**Authorization**: Required (user context)

**Response**:
```json
{
  "user": {
    "id": "22222222-2222-2222-2222-222222222222",
    "displayName": "Marian",
    "email": "marian@example.com",
    "credits": {
      "balance": 10,
      "hasEverPurchased": true,
      "lastPurchaseAt": "2025-09-15T00:40:10.114Z",
      "lastTransactionAt": "2025-09-16T00:40:10.114Z"
    },
    "permissions": {
      "hasFullAccess": true,
      "unlockedAnimalCount": 14,
      "unlockedParts": ["head", "body", "arms", "legs", "tail", "wings", "horn", "horns"],
      "lockedParts": []
    },
    "createdAt": "2025-09-16T00:40:10.114Z"
  },
  "success": true
}
```

### 2. GET `/api/creature-builder/user-data`
**Returnează datele creature builder filtrate în funcție de permisiunile utilizatorului**

**Authorization**: Required (user context)

**Response**:
```json
{
  "parts": [
    {
      "key": "head",
      "name": "Head",
      "image": "images/bodyparts/face.webp",
      "isLocked": false
    },
    {
      "key": "wings",
      "name": "Wings", 
      "image": "images/bodyparts/wings.webp",
      "isLocked": false
    }
  ],
  "animals": [
    {
      "src": "images/animals/base/bunny.jpg",
      "label": "Bunny",
      "supports": ["head", "body", "arms", "legs", "tail"],
      "isLocked": false
    },
    {
      "src": "images/animals/base/eagle.jpg",
      "label": "Eagle",
      "supports": ["head", "body", "arms", "legs", "tail", "wings"],
      "isLocked": false
    }
  ],
  "unlockedAnimalCount": 14,
  "totalAnimalCount": 14,
  "hasFullAccess": true,
  "credits": {
    "balance": 10,
    "hasEverPurchased": true
  }
}
```

### 3. POST `/api/user/spend-credits`
**Consumă credite pentru generarea de imagini**

**Authorization**: Required (user context)

**Request Body**:
```json
{
  "amount": 1,
  "reference": "creature-generation"
}
```

**Response**:
```json
{
  "success": true,
  "newBalance": 9
}
```

## Test Users

### 1. Test User (Limited Access)
- ID: `11111111-1111-1111-1111-111111111111`
- Email: `test@example.com`
- Credits: 5
- Has Ever Purchased: **false**
- Access: **Limited** (doar 3 animale + 3 părți)

### 2. Marian (Full Access)
- ID: `22222222-2222-2222-2222-222222222222`
- Email: `marian@example.com`
- Credits: 10
- Has Ever Purchased: **true**
- Access: **Full** (toate animalele + toate părțile)

## Frontend Integration

### Current vs New Logic

**BEFORE (Frontend localStorage)**:
```typescript
// Frontend handles all logic
hasEverToppedUp() { return this._ever(); } // localStorage
isPartLocked(key) { return !this.hasEverToppedUp() && this.baseLockedParts.has(key); }
trySpend(n) { /* localStorage logic */ }
```

**AFTER (Backend-driven)**:
```typescript
// Frontend only displays what backend provides
fetchUserData() { 
  return this.http.get('/api/creature-builder/user-data');
}
// No more local logic - everything comes from server
```

### Migration Steps for Frontend:

1. **Replace** `BuilderDataService.fetch()` cu `fetchUserAwareData()`
2. **Remove** logica `isPartLocked()` și `isAnimalLocked()` din components
3. **Replace** `CreditsService.trySpend()` cu API call
4. **Use** `isLocked` flag din răspunsurile API

## Security Benefits

✅ **Server-side validation**: Toate permisiunile verificate pe backend  
✅ **No client tampering**: Nu se poate "hack" localStorage pentru unlock  
✅ **Consistent state**: Single source of truth pentru credite și permisiuni  
✅ **Audit trail**: Toate tranzacțiile loggate în baza de date  

## Database Migration Required

```bash
cd XooCreatorBA/XooCreator.BA
dotnet ef migrations add AddUserCreditsAndPermissions
dotnet ef database update
```

Acest migration va adăuga:
- Email field la UserAlchimalia
- Marian test user
- Credit wallets pentru ambii test users
- Tranzacții de test pentru demonstrația logicii
