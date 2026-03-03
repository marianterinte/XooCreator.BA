// Create translations for missing languages (IT, ES, DE, FR)
// This script reads English JSON and creates translated versions

const fs = require('fs');
const path = require('path');

const scriptRoot = __dirname;
const repoRoot = path.resolve(scriptRoot, '..', '..', '..');
const i18nRoot = path.join(repoRoot, 'XooCreator.BA', 'Data', 'SeedData', 'Discovery', 'i18n');

// Load English source
const enPath = path.join(i18nRoot, 'en-us', 'discover-bestiary.json');
const enContent = fs.readFileSync(enPath, 'utf8');
const enData = JSON.parse(enContent);

console.log(`Loaded ${enData.length} English entries`);

// Translation mappings for creature names
// These are creative adaptations maintaining the fantasy theme
const nameTranslations = {
    'it-it': {
        'Bunniraffix': 'Conigliraffe',
        'Bunnyppotix': 'Coniglippo',
        'Bunniraffe': 'Coniglirafa',
        'Bunnyppo': 'Coniglippo',
        'Giraffoppo': 'Giraffoppo',
        'Bunniraffoppo': 'Conigliraffoppo',
        'Girabbit': 'Girafetto',
        'Giraffoppit': 'Giraffoppetto',
        'Hippityhop': 'Ippettino',
        'Hippogiraffe': 'Ippogirafa',
        'Hippobunny': 'Ippoconiglio',
        'Giraffopotamus': 'Giraffopotamo'
    },
    'es-es': {
        'Bunniraffix': 'Conejirafe',
        'Bunnyppotix': 'Conejipopo',
        'Bunniraffe': 'Conejirafa',
        'Bunnyppo': 'Conejipopo',
        'Giraffoppo': 'Jirafopótamo',
        'Bunniraffoppo': 'Conejirafopótamo',
        'Girabbit': 'Jiranejo',
        'Giraffoppit': 'Jirafopótito',
        'Hippityhop': 'Hipopín',
        'Hippogiraffe': 'Hipojirafa',
        'Hippobunny': 'Hipoconejo',
        'Giraffopotamus': 'Jirafopótamo'
    },
    'de-de': {
        'Bunniraffix': 'Hasengiraffe',
        'Bunnyppotix': 'Hasenpferd',
        'Bunniraffe': 'Hasengiraffe',
        'Bunnyppo': 'Hasenpferd',
        'Giraffoppo': 'Giraffenpferd',
        'Bunniraffoppo': 'Hasengiraffenpferd',
        'Girabbit': 'Giraffenhase',
        'Giraffoppit': 'Giraffenpferdchen',
        'Hippityhop': 'Nilpferdhase',
        'Hippogiraffe': 'Nilpferdgiraffe',
        'Hippobunny': 'Nilpferdhase',
        'Giraffopotamus': 'Giraffenpferd'
    },
    'fr-fr': {
        'Bunniraffix': 'Lapiraffe',
        'Bunnyppotix': 'Lapippo',
        'Bunniraffe': 'Lapigirafe',
        'Bunnyppo': 'Lapipotame',
        'Giraffoppo': 'Girafopotame',
        'Bunniraffoppo': 'Lapigirafopotame',
        'Girabbit': 'Girapin',
        'Giraffoppit': 'Girafopotin',
        'Hippityhop': 'Hipposapin',
        'Hippogiraffe': 'Hippogirafe',
        'Hippobunny': 'Hippolapin',
        'Giraffopotamus': 'Girafopotame'
    }
};

// Simple story translations (key phrases)
const storyTranslations = {
    'it-it': {
        'Tree of Light': 'Albero della Luce',
        'appeared when': 'apparve quando',
        'was born': 'nacque',
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
        'emerged': 'emerse',
        'united': 'unì',
        'dreamed': 'sognò'
    },
    'es-es': {
        'Tree of Light': 'Árbol de la Luz',
        'appeared when': 'apareció cuando',
        'was born': 'nació',
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
        'emerged': 'emergió',
        'united': 'unió',
        'dreamed': 'soñó'
    },
    'de-de': {
        'Tree of Light': 'Baum des Lichts',
        'appeared when': 'erschien als',
        'was born': 'wurde geboren',
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
        'emerged': 'entstand',
        'united': 'vereinte',
        'dreamed': 'träumte'
    },
    'fr-fr': {
        'Tree of Light': 'Arbre de Lumière',
        'appeared when': 'apparut quand',
        'was born': 'naquit',
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
        'emerged': 'émergea',
        'united': 'unit',
        'dreamed': 'rêva'
    }
};

// Simple translation function (basic word replacement)
function translateStory(story, locale) {
    let translated = story;
    const translations = storyTranslations[locale];
    
    if (!translations) return story;
    
    for (const [en, target] of Object.entries(translations)) {
        const regex = new RegExp(en, 'gi');
        translated = translated.replace(regex, target);
    }
    
    return translated;
}

function translateName(name, locale) {
    const translations = nameTranslations[locale];
    return translations && translations[name] ? translations[name] : name;
}

// Create translations for each target language
const targetLocales = ['it-it', 'es-es', 'de-de', 'fr-fr'];

for (const locale of targetLocales) {
    console.log(`\nCreating ${locale} translations...`);
    
    const translated = enData.map(item => {
        return {
            Combination: item.Combination,
            Name: translateName(item.Name, locale),
            ImagePrompt: item.ImagePrompt, // Keep English
            Story: translateStory(item.Story, locale),
            ImageFileName: item.ImageFileName
        };
    });
    
    const outputPath = path.join(i18nRoot, locale, 'discover-bestiary.json');
    
    // Create directory if it doesn't exist
    const dir = path.dirname(outputPath);
    if (!fs.existsSync(dir)) {
        fs.mkdirSync(dir, { recursive: true });
    }
    
    fs.writeFileSync(outputPath, JSON.stringify(translated, null, 2), 'utf8');
    console.log(`Created ${outputPath} with ${translated.length} entries`);
}

console.log('\nAll translation files created successfully!');
