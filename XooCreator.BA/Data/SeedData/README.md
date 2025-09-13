Stories seeding
================

Preferred structure:

- Folder `Data/SeedData/Stories` containing one JSON per story with the shape of a single `StorySeedData` object.
- Example filename: `intro-pufpuf.json`, `terra-s1.json`, etc. The filename is not used for ID; `storyId` inside the file is authoritative.

Backward compatibility:

- If present, `Data/SeedData/stories-seed.json` (array under `stories`) is also read and merged. Stories in the folder override duplicates from the legacy file.


