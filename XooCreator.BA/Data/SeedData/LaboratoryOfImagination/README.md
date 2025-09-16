# Seed Data

This directory contains JSON files used to seed the database with initial data instead of hardcoded values in the XooDbContext.

## Files:

- **bodyparts.json**: Contains all body parts (head, body, arms, legs, tail, wings, horn, horns) with their properties
- **regions.json**: Contains all regions (Sahara, Jungle, Farm, Savanna, Forest, Wetlands, Mountains) with their IDs and names
- **animals.json**: Contains all animals with their labels, image sources, region associations, and hybrid status
- **animal-part-supports.json**: Contains the mappings between animals and the body parts they support

## Migration Notes:

The hardcoded data that was previously in `XooDbContext.OnModelCreating()` has been extracted to these JSON files. The `SeedDataService` is used to load this data during database seeding.

This approach provides several benefits:
1. **Maintainability**: Data can be modified without rebuilding the application
2. **Separation of Concerns**: Data configuration is separate from database configuration
3. **Flexibility**: Easy to add new regions, animals, or body parts by modifying JSON files
4. **Localization Ready**: Multiple JSON files can be created for different languages/regions

## Usage:

The `SeedDataService` automatically loads these files during the `OnModelCreating` process. The files should be placed in the `Data/SeedData` directory relative to the application's base directory.