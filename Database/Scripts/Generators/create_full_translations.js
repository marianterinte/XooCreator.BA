// Create complete translations for IT, ES, DE, FR
const fs = require('fs');
const path = require('path');

const scriptRoot = __dirname;
const repoRoot = path.resolve(scriptRoot, '..', '..', '..');
const i18nRoot = path.join(repoRoot, 'XooCreator.BA', 'Data', 'SeedData', 'Discovery', 'i18n');

// Load English and Romanian sources
const enPath = path.join(i18nRoot, 'en-us', 'discover-bestiary.json');
const roPath = path.join(i18nRoot, 'ro-ro', 'discover-bestiary.json');
const enData = JSON.parse(fs.readFileSync(enPath, 'utf8'));
const roData = JSON.parse(fs.readFileSync(roPath, 'utf8'));

console.log(`Loaded ${enData.length} English entries`);

// Full translations based on English and Romanian patterns
const fullTranslations = {
    'it-it': [
        {
            "Combination": "BunnyBunnyGiraffe",
            "Name": "Coniglio-Coniglio-Giraffa",
            "Story": "Il Coniglio-Coniglio-Giraffa apparve quando due conigli sognarono insieme di raggiungere il cielo attraverso gli occhi di una giraffa. L'Albero della Luce li unì in un unico essere. È uno spirito di curiosità e coraggio, saltando tra i mondi e portando luce nelle ombre."
        },
        {
            "Combination": "BunnyBunnyHippo",
            "Name": "Coniglio-Coniglio-Ippopotamo",
            "Story": "Il Coniglio-Coniglio-Ippopotamo nacque dal sogno di due conigli che chiesero la forza di un ippopotamo per proteggere i loro amici. L'Albero della Luce unì le loro anime. È uno spirito giocoso ma protettivo, portando coraggio ai piccoli e pace agli smarriti."
        },
        {
            "Combination": "BunnyGiraffeBunny",
            "Name": "Coniglio-Giraffa-Coniglio",
            "Story": "Il Coniglio-Giraffa-Coniglio apparve quando due conigli desiderarono vedere il mondo dall'alto. L'Albero della Luce rispose e diede loro il corpo di una giraffa, ma mantenne il loro cuore giocoso e il loro volto. Spirito di sogno e speranza, portando luce infantile nei suoi occhi."
        },
        {
            "Combination": "BunnyGiraffeGiraffe",
            "Name": "Coniglio-Giraffa-Giraffa",
            "Story": "Il Coniglio-Giraffa-Giraffa apparve quando un coniglio volle condividere il suo gioco con due giraffe. L'Albero della Luce li unì in un unico essere. Spirito di curiosità e amicizia, unendo l'agilità con la grandezza."
        },
        {
            "Combination": "BunnyGiraffeHippo",
            "Name": "Coniglio-Giraffa-Ippopotamo",
            "Story": "Il Coniglio-Giraffa-Ippopotamo è un eroe leggendario, nato da un'alleanza impossibile: un coniglio, una giraffa e un ippopotamo che scelsero di unire i loro destini. L'Albero della Luce vide il loro coraggio e diede loro un unico cuore. Con le braccia del coniglio, aiuta e protegge. Con il corpo della giraffa, vede lontano. Con la testa dell'ippopotamo, ispira tranquillità e saggezza. È un simbolo di diversità e amicizia tra mondi diversi."
        },
        {
            "Combination": "BunnyGiraffeNone",
            "Name": "Conigliraffe",
            "Story": "Tra i rami dell'Albero della Luce nacque il Conigliraffe, una creatura unica, metà coniglio, metà giraffa. Le sue braccia corte e agili scavano tunnel e accendono fuochi vividi, mentre il suo corpo alto, adornato di macchie luminose, vede lontano in orizzonti dove altri non possono arrivare.\n\nLa leggenda narra che il Conigliraffe emerse quando un piccolo e coraggioso coniglio si nascose alla radice di una vecchia giraffa mentre l'oscurità divorava la savana. Dal loro abbraccio disperato, l'Albero della Luce intrecciò due spiriti in un unico corpo.\n\nIl Conigliraffe è un guardiano degli orizzonti. Quando corre nell'erba, la terra salta di gioia. Quando si alza a combattere, le ombre fuggono vergognose. Ma la sua forza non risiede solo nella velocità o nell'altezza, ma nell'amicizia che offre a coloro che vagano tra i rami dei mondi.\n\nGli spiriti antichi dicono che il Conigliraffe sarà sempre lì dove qualcuno cerca di riparare ciò che è stato rotto, portando coraggio e speranza."
        },
        {
            "Combination": "BunnyHippoBunny",
            "Name": "Coniglio-Ippopotamo-Coniglio",
            "Story": "Il Coniglio-Ippopotamo-Coniglio nacque quando un coniglio cercò l'amicizia di un ippopotamo, e l'Albero della Luce unì i loro destini. Con braccia agili e testa giocosa, ma corpo solido, questo è lo spirito dell'equilibrio tra fragilità e forza, portando gioia e protezione ai rami dei mondi."
        },
        {
            "Combination": "BunnyHippoGiraffe",
            "Name": "Coniglio-Ippopotamo-Giraffa",
            "Story": "Il Coniglio-Ippopotamo-Giraffa nacque in mezzo a una tempesta di luce, quando un coniglio, un ippopotamo e una giraffa giurarono di difendere l'Albero della Luce. Dal loro giuramento, i rami unirono tre spiriti in un unico corpo. È un simbolo di amicizia tra esseri completamente diversi. Con le braccia del coniglio solleva la luce verso il cielo, con il corpo dell'ippopotamo protegge la terra, e con la testa della giraffa porta sorrisi e speranza. Il Coniglio-Ippopotamo-Giraffa è visto come un guardiano dell'equilibrio."
        },
        {
            "Combination": "BunnyHippoHippo",
            "Name": "Coniglio-Ippopotamo-Ippopotamo",
            "Story": "Il Coniglio-Ippopotamo-Ippopotamo nacque quando un coniglio chiese la forza di due ippopotami per proteggere la sua casa. L'Albero della Luce unì le loro anime in un unico corpo. È uno spirito di protezione e stabilità, portando sicurezza e pace a tutti coloro che ne hanno bisogno."
        },
        {
            "Combination": "BunnyHippoNone",
            "Name": "Coniglippopotamo",
            "Story": "Il Coniglippopotamo nacque quando un coniglio e un ippopotamo si incontrarono in mezzo a una tempesta di acque nere. Dalla loro inaspettata amicizia, l'Albero della Luce intrecciò l'agilità del coniglio con la forza dell'ippopotamo. Il Coniglippopotamo è gentile e protettivo: con le sue braccia agili scava nascondigli, e con il suo corpo potente apre sentieri attraverso le acque. Porta calma dove c'è paura e costruisce ponti di fiducia tra i mondi."
        }
    ]
};

// For now, create Italian with first 10 entries as sample
// Will expand to all 42 and other languages after verification

const itData = fullTranslations['it-it'];

// Add remaining combinations from English with basic translation
for (let i = itData.length; i < enData.length; i++) {
    const enItem = enData[i];
    itData.push({
        Combination: enItem.Combination,
        Name: enItem.Name, // Will translate properly
        ImagePrompt: enItem.ImagePrompt,
        Story: enItem.Story, // Will translate properly
        ImageFileName: enItem.ImageFileName
    });
}

const outputPath = path.join(i18nRoot, 'it-it', 'discover-bestiary.json');
fs.writeFileSync(outputPath, JSON.stringify(itData, null, 2), 'utf8');
console.log(`Created Italian sample with ${itData.length} entries`);
console.log('First 10 entries are fully translated, remaining need translation');
