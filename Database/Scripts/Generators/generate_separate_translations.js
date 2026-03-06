// Generator for separate BestiaryItemTranslations scripts per locale
// Run with: node generate_separate_translations.js

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
const outputDir = path.join(scriptRoot, '..');
const locales = ['en-us', 'ro-ro', 'hu-hu', 'it-it', 'es-es', 'de-de', 'fr-fr'];

console.log('Generating separate BestiaryItemTranslations SQL scripts...');
console.log('Script root:', scriptRoot);
console.log('Repo root:', repoRoot);
console.log('i18n root:', i18nRoot);
console.log('Output dir:', outputDir);

// Load locale data
const localeData = {};
for (const locale of locales) {
    const filePath = path.join(i18nRoot, locale, 'discover-bestiary.json');
    console.log(`Loading ${locale} from: ${filePath}`);
    
    try {
        let content;
        try {
            content = fs.readFileSync(filePath, 'utf8');
        } catch (utf8Error) {
            console.log(`Trying latin1 encoding for ${locale}...`);
            content = fs.readFileSync(filePath, 'latin1');
        }
        
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
        localeData[locale] = {};
    }
}

// Load V0002 to get BestiaryItem IDs
const v0002Path = path.join(scriptRoot, '..', 'V0002__seed_bestiary_items.sql');
console.log(`Loading BestiaryItem IDs from: ${v0002Path}`);

const v0002Content = fs.readFileSync(v0002Path, 'utf8');
const bestiaryIds = {};

const lines = v0002Content.split('\n');
for (const line of lines) {
    const match = line.match(/^\s*\('([^']+)',\s*'([^']+)',\s*'([^']+)',\s*'([^']+)'/);
    if (match) {
        const [, id, arms, body, head] = match;
        const comboKey = `${arms}|${body}|${head}`;
        bestiaryIds[comboKey] = id;
    }
}

console.log(`Found ${Object.keys(bestiaryIds).length} BestiaryItem IDs`);

// Generate separate SQL files for each locale
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

function generateLocaleScript(locale) {
    const map = localeData[locale];
    const enCombinations = Object.keys(localeData['en-us']).sort();
    
    const sqlLines = [];
    sqlLines.push(`-- Auto-generated from Data/SeedData/Discovery/i18n/${locale}/discover-bestiary.json`);
    sqlLines.push(`-- Locale: ${locale}`);
    sqlLines.push(`-- Run date: ${new Date().toISOString()}`);
    sqlLines.push(`-- This script seeds BestiaryItemTranslations for ${locale} bestiary combinations.`);
    sqlLines.push('-- It is idempotent: safe to run multiple times.');
    sqlLines.push('');
    sqlLines.push('BEGIN;');
    sqlLines.push('');
    
    let insertCount = 0;

    for (const combination of enCombinations) {
        try {
            const [arms, body, head] = splitCombination(combination);
            const comboKey = `${arms}|${body}|${head}`;
            
            const bestiaryId = bestiaryIds[comboKey];
            if (!bestiaryId) {
                console.warn(`Warning: No BestiaryItem ID found for ${comboKey} (${combination})`);
                continue;
            }
            
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
        } catch (error) {
            console.error(`Error processing combination ${combination}:`, error);
        }
    }

    sqlLines.push('COMMIT;');
    sqlLines.push('');
    
    return { sqlLines, insertCount };
}

// Generate files for each locale
for (const locale of locales) {
    const { sqlLines, insertCount } = generateLocaleScript(locale);
    const outputPath = path.join(outputDir, `V00120__seed_bestiary_item_translations_${locale}.sql`);
    
    fs.writeFileSync(outputPath, sqlLines.join('\n'), 'utf8');
    
    console.log(``);
    console.log(`SUCCESS: Generated ${locale} BestiaryItemTranslations SQL`);
    console.log(`  File: ${outputPath}`);
    console.log(`  Insert/Upsert statements: ${insertCount}`);
}

console.log('');
console.log('All locale scripts generated successfully!');
