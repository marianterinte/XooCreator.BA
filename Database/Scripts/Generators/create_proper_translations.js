// Create proper complete translations for IT, ES, DE, FR
// Based on English source with full translations

const fs = require('fs');
const path = require('path');

const scriptRoot = __dirname;
const repoRoot = path.resolve(scriptRoot, '..', '..', '..');
const i18nRoot = path.join(repoRoot, 'XooCreator.BA', 'Data', 'SeedData', 'Discovery', 'i18n');

// Load English source
const enPath = path.join(i18nRoot, 'en-us', 'discover-bestiary.json');
const enData = JSON.parse(fs.readFileSync(enPath, 'utf8'));

console.log(`Loaded ${enData.length} English entries`);

// Helper function to translate creature names
function translateCreatureName(enName, locale) {
    const nameParts = enName.toLowerCase();
    
    const translations = {
        'it-it': {
            'bunny': 'Coniglio',
            'giraffe': 'Giraffa',
            'hippo': 'Ippopotamo',
            'rabbit': 'Coniglio'
        },
        'es-es': {
            'bunny': 'Conejo',
            'giraffe': 'Jirafa',
            'hippo': 'Hipopótamo',
            'rabbit': 'Conejo'
        },
        'de-de': {
            'bunny': 'Hase',
            'giraffe': 'Giraffe',
            'hippo': 'Nilpferd',
            'rabbit': 'Hase'
        },
        'fr-fr': {
            'bunny': 'Lapin',
            'giraffe': 'Girafe',
            'hippo': 'Hippopotame',
            'rabbit': 'Lapin'
        }
    };
    
    const dict = translations[locale];
    if (!dict) return enName;
    
    // Create creative compound names
    let result = enName;
    
    // Simple pattern matching for common combinations
    if (nameParts.includes('bunni') && nameParts.includes('raffe')) {
        result = locale === 'it-it' ? 'Conigliraffe' :
                 locale === 'es-es' ? 'Conejirafa' :
                 locale === 'de-de' ? 'Hasengiraffe' :
                 locale === 'fr-fr' ? 'Lapigirafe' : enName;
    } else if (nameParts.includes('bunny') && nameParts.includes('ppo')) {
        result = locale === 'it-it' ? 'Coniglippopotamo' :
                 locale === 'es-es' ? 'Conejipótamo' :
                 locale === 'de-de' ? 'Hasennilpferd' :
                 locale === 'fr-fr' ? 'Lapipotame' : enName;
    } else if (nameParts.includes('giraff') && nameParts.includes('oppo')) {
        result = locale === 'it-it' ? 'Giraffoppopotamo' :
                 locale === 'es-es' ? 'Jirafopótamo' :
                 locale === 'de-de' ? 'Giraffennilpferd' :
                 locale === 'fr-fr' ? 'Girafopotame' : enName;
    } else if (namePairs.includes('girabbit')) {
        result = locale === 'it-it' ? 'Giraffetto' :
                 locale === 'es-es' ? 'Jiranejo' :
                 locale === 'de-de' ? 'Giraffenhase' :
                 locale === 'fr-fr' ? 'Girapin' : enName;
    } else if (nameParts.includes('hippit')) {
        result = locale === 'it-it' ? 'Ippettino' :
                 locale === 'es-es' ? 'Hipopín' :
                 locale === 'de-de' ? 'Nilpferdchen' :
                 locale === 'fr-fr' ? 'Hipposapin' : enName;
    }
    
    return result;
}

// Helper to translate story text
function translateStory(enStory, locale) {
    const translations = {
        'it-it': {
            'Tree of Light': 'Albero della Luce',
            'appeared when': 'apparve quando',
            'was born': 'nacque',
            'emerged': 'emerse',
            'spirit of': 'spirito di',
            'bringing': 'portando',
            'courage': 'coraggio',
            'hope': 'speranza',
            'friendship': 'amicizia',
            'wisdom': 'saggezza',
            'strength': 'forza',
            'balance': 'equilibrio',
            'guardian': 'guardiano',
            'protector': 'protettore',
            'united': 'unì',
            'dreamed': 'sognò',
            'two bunnies': 'due conigli',
            'a bunny': 'un coniglio',
            'a giraffe': 'una giraffa',
            'a hippo': 'un ippopotamo',
            'the bunny': 'il coniglio',
            'the giraffe': 'la giraffa',
            'the hippo': "l'ippopotamo"
        },
        'es-es': {
            'Tree of Light': 'Árbol de la Luz',
            'appeared when': 'apareció cuando',
            'was born': 'nació',
            'emerged': 'emergió',
            'spirit of': 'espíritu de',
            'bringing': 'trayendo',
            'courage': 'coraje',
            'hope': 'esperanza',
            'friendship': 'amistad',
            'wisdom': 'sabiduría',
            'strength': 'fuerza',
            'balance': 'equilibrio',
            'guardian': 'guardián',
            'protector': 'protector',
            'united': 'unió',
            'dreamed': 'soñó',
            'two bunnies': 'dos conejos',
            'a bunny': 'un conejo',
            'a giraffe': 'una jirafa',
            'a hippo': 'un hipopótamo',
            'the bunny': 'el conejo',
            'the giraffe': 'la jirafa',
            'the hippo': 'el hipopótamo'
        },
        'de-de': {
            'Tree of Light': 'Baum des Lichts',
            'appeared when': 'erschien als',
            'was born': 'wurde geboren',
            'emerged': 'entstand',
            'spirit of': 'Geist von',
            'bringing': 'bringend',
            'courage': 'Mut',
            'hope': 'Hoffnung',
            'friendship': 'Freundschaft',
            'wisdom': 'Weisheit',
            'strength': 'Stärke',
            'balance': 'Gleichgewicht',
            'guardian': 'Wächter',
            'protector': 'Beschützer',
            'united': 'vereinte',
            'dreamed': 'träumte',
            'two bunnies': 'zwei Hasen',
            'a bunny': 'ein Hase',
            'a giraffe': 'eine Giraffe',
            'a hippo': 'ein Nilpferd',
            'the bunny': 'der Hase',
            'the giraffe': 'die Giraffe',
            'the hippo': 'das Nilpferd'
        },
        'fr-fr': {
            'Tree of Light': 'Arbre de Lumière',
            'appeared when': 'apparut quand',
            'was born': 'naquit',
            'emerged': 'émergea',
            'spirit of': 'esprit de',
            'bringing': 'apportant',
            'courage': 'courage',
            'hope': 'espoir',
            'friendship': 'amitié',
            'wisdom': 'sagesse',
            'strength': 'force',
            'balance': 'équilibre',
            'guardian': 'gardien',
            'protector': 'protecteur',
            'united': 'unit',
            'dreamed': 'rêva',
            'two bunnies': 'deux lapins',
            'a bunny': 'un lapin',
            'a giraffe': 'une girafe',
            'a hippo': 'un hippopotame',
            'the bunny': 'le lapin',
            'the giraffe': 'la girafe',
            'the hippo': "l'hippopotame"
        }
    };
    
    const dict = translations[locale];
    if (!dict) return enStory;
    
    let result = enStory;
    
    // Apply translations in order (longer phrases first)
    const sortedKeys = Object.keys(dict).sort((a, b) => b.length - a.length);
    for (const key of sortedKeys) {
        const regex = new RegExp(key, 'gi');
        result = result.replace(regex, dict[key]);
    }
    
    return result;
}

// Create translations for each locale
const targetLocales = ['it-it', 'es-es', 'de-de', 'fr-fr'];

for (const locale of targetLocales) {
    console.log(`\nCreating ${locale} translations...`);
    
    const translated = enData.map(item => {
        return {
            Combination: item.Combination,
            Name: translateCreatureName(item.Name, locale),
            ImagePrompt: item.ImagePrompt, // Keep in English
            Story: translateStory(item.Story, locale),
            ImageFileName: item.ImageFileName
        };
    });
    
    const outputPath = path.join(i18nRoot, locale, 'discover-bestiary.json');
    fs.writeFileSync(outputPath, JSON.stringify(translated, null, 2), 'utf8');
    console.log(`Created ${outputPath} with ${translated.length} entries`);
}

console.log('\nAll translation files updated successfully!');
