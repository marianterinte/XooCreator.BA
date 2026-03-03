// Simple JavaScript generator for BestiaryItemTranslations
// Run with: node manual_generator.js

const fs = require('fs');
const path = require('path');

// Simple UUID generator
function generateUUID() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
        const r = Math.random() * 16 | 0;
        const v = c === 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
}

// Configuration
const scriptRoot = __dirname;
const repoRoot = path.resolve(scriptRoot, '..', '..', '..');
const i18nRoot = path.join(repoRoot, 'XooCreator.BA', 'Data', 'SeedData', 'Discovery', 'i18n');
const outputPath = path.join(scriptRoot, '..', 'V00120__seed_bestiary_item_translations.sql');
const locales = ['en-us', 'ro-ro', 'hu-hu'];

console.log('Generating BestiaryItemTranslations SQL...');
console.log('Script root:', scriptRoot);
console.log('Repo root:', repoRoot);
console.log('i18n root:', i18nRoot);
console.log('Output path:', outputPath);

// Load locale data
const localeData = {};
for (const locale of locales) {
    const filePath = path.join(i18nRoot, locale, 'discover-bestiary.json');
    console.log(`Loading ${locale} from: ${filePath}`);
    
    try {
        // Try different encodings
        let content;
        try {
            content = fs.readFileSync(filePath, 'utf8');
        } catch (utf8Error) {
            console.log(`Trying latin1 encoding for ${locale}...`);
            content = fs.readFileSync(filePath, 'latin1');
        }
        
        // Clean up potential BOM or weird characters
        content = content.replace(/^\uFEFF/, '').replace(/\r\n/g, '\n').replace(/\r/g, '\n');
        
        const json = JSON.parse(content);
        const map = {};
        
        for (const item of json) {
            if (item && item.Combination) {
                map[item.Combination] = item;
            }
        }
        
        localeData[locale] = map;
        console.log(`Loaded ${Object.keys(map).length} items for ${locale}`);
    } catch (error) {
        console.error(`Error loading ${locale}:`, error.message);
        // Continue with empty map for this locale
        localeData[locale] = {};
    }
}

// Load V0002 to get BestiaryItem IDs
const v0002Path = path.join(scriptRoot, '..', 'V0002__seed_bestiary_items.sql');
console.log(`Loading BestiaryItem IDs from: ${v0002Path}`);

const v0002Content = fs.readFileSync(v0002Path, 'utf8');
const bestiaryIds = {};

// Parse BestiaryItem IDs from V0002
const lines = v0002Content.split('\n');
for (const line of lines) {
    // Match lines with VALUES and extract first 4 quoted values
    const match = line.match(/^\s*\('([^']+)',\s*'([^']+)',\s*'([^']+)',\s*'([^']+)'/);
    if (match) {
        const [, id, arms, body, head] = match;
        const comboKey = `${arms}|${body}|${head}`;
        bestiaryIds[comboKey] = id;
    }
}

console.log(`Found ${Object.keys(bestiaryIds).length} BestiaryItem IDs`);

// Generate SQL
const sqlLines = [];
sqlLines.push('-- Auto-generated from Data/SeedData/Discovery/i18n/*/discover-bestiary.json');
sqlLines.push('-- Locales: ' + locales.join(', '));
sqlLines.push('-- Run date: ' + new Date().toISOString());
sqlLines.push('-- This script seeds BestiaryItemTranslations for all discovery bestiary combinations.');
sqlLines.push('-- It is idempotent: safe to run multiple times.');
sqlLines.push('');
sqlLines.push('BEGIN;');
sqlLines.push('');

let insertCount = 0;
const enCombinations = Object.keys(localeData['en-us']).sort();

function splitCombination(combination) {
    const tokens = ['Bunny', 'Giraffe', 'Hippo', 'None'];
    const parts = [];
    let remaining = combination;
    
    for (let i = 0; i < 3; i++) {
        let matched = false;
        for (const token of tokens) {
            if (remaining.startsWith(token)) {
                parts.push(token === 'None' ? '—' : token);
                remaining = remaining.substring(token.length);
                matched = true;
                break;
            }
        }
        if (!matched) {
            throw new Error(`Could not parse combination: ${combination}`);
        }
    }
    
    return parts;
}

function escapeSql(value) {
    if (value === null || value === undefined) return "''";
    return "'" + value.replace(/'/g, "''") + "'";
}

for (const combination of enCombinations) {
    try {
        const [arms, body, head] = splitCombination(combination);
        const comboKey = `${arms}|${body}|${head}`;
        
        const bestiaryId = bestiaryIds[comboKey];
        if (!bestiaryId) {
            console.warn(`Warning: No BestiaryItem ID found for ${comboKey} (${combination})`);
            continue;
        }
        
        for (const locale of locales) {
            const map = localeData[locale];
            const item = map[combination];
            
            if (!item) {
                console.warn(`Warning: Locale ${locale} missing combination ${combination}`);
                continue;
            }
            
            const name = item.Name || '';
            const story = item.Story || '';
            const translationId = generateUUID();
            
            sqlLines.push('INSERT INTO alchimalia_schema."BestiaryItemTranslations"');
            sqlLines.push('    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")');
            sqlLines.push('VALUES');
            sqlLines.push(`    ('${translationId}', '${bestiaryId}', '${locale}', ${escapeSql(name)}, ${escapeSql(story)})`);
            sqlLines.push('ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE');
            sqlLines.push('SET "Name" = EXCLUDED."Name",');
            sqlLines.push('    "Story" = EXCLUDED."Story";');
            sqlLines.push('');
            
            insertCount++;
        }
    } catch (error) {
        console.error(`Error processing combination ${combination}:`, error);
    }
}

sqlLines.push('COMMIT;');
sqlLines.push('');

// Write file
fs.writeFileSync(outputPath, sqlLines.join('\n'), 'utf8');

console.log('');
console.log('SUCCESS: Generated BestiaryItemTranslations SQL');
console.log(`  File: ${outputPath}`);
console.log(`  Insert/Upsert statements: ${insertCount}`);
console.log('');
