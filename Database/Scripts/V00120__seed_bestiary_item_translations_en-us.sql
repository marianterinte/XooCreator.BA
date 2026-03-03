-- Auto-generated from Data/SeedData/Discovery/i18n/en-us/discover-bestiary.json
-- Locale: en-us
-- Run date: 2026-03-03T09:03:26.675Z
-- This script seeds BestiaryItemTranslations for en-us bestiary combinations.
-- It is idempotent: safe to run multiple times.

BEGIN;

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('4b393076-6295-460a-a0e5-ef914ab548c1', '5d1b4419-c890-5462-bcb7-242dbe123890', 'en-us', 'Bunniraffix', 'The Bunniraffix appeared when two bunnies dreamed together of reaching the sky through the eyes of a giraffe. The Tree of Light united them into a single being. It is a spirit of curiosity and courage, leaping between worlds and bringing light into the shadows.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('c35185d6-3dc1-4ae5-aa9b-f52c5a95e6c6', '99d40f35-ede6-5dc5-8484-e22327080aa1', 'en-us', 'Bunnyppotix', 'The Bunnyppotix was born from the dream of two bunnies who asked for the strength of a hippo to protect their friends. The Tree of Light united their souls. It is a playful but protective spirit, bringing courage to the small and peace to the lost.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('bf4f817c-7418-4a8e-a7b6-f5a3fe5ca33e', 'fc1b57f3-4751-51f5-a497-174648433b88', 'en-us', 'Bunniraffonix', 'The Bunniraffonix emerged when two bunnies wished to see the world from above. The Tree of Light answered and gave them the body of a giraffe, but kept their playful hearts and faces. It is a spirit of dreams and hope, carrying the light of childhood in its eyes.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('12007b6c-268c-48a3-945e-828b37dc0a31', '9e0360f6-4d11-5a44-ae22-6e5ad7d0fab5', 'en-us', 'Bunnyrogaf', 'The Bunnyrogaf appeared when a bunny wanted to share its game with two giraffes. The Tree of Light united them into a single being. It is a spirit of curiosity and friendship, uniting nimbleness with greatness.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('e4c96ee3-668a-4e98-83b0-85f7cf656074', 'b67d8c08-04ec-539d-8787-27e838807428', 'en-us', 'Bunniraffoppo', 'The Bunniraffoppo is a legendary hero, born from an impossible alliance: a bunny, a giraffe, and a hippo who chose to unite their destinies. The Tree of Light saw their courage and gave them a single heart. With the bunny''s arms, it helps and protects. With the giraffe''s body, it sees far. With the hippo''s head, it inspires tranquility and wisdom. It is a symbol of diversity and friendship between different worlds.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('4a7401f3-e1cb-4156-ad6c-207bdbcaa5ec', 'e1bb5ca3-5578-504f-aed9-947c7076ef1f', 'en-us', 'Bunniraffe', 'In the branches of the Tree of Light, the Bunniraffe was born, a unique creature, half bunny, half giraffe. Its short, nimble arms dig tunnels and light vivid fires, while its tall body, adorned with glowing spots, sees far into horizons where others cannot reach.

Legend says the Bunniraffe emerged when a small, brave bunny hid at the root of an old giraffe while darkness devoured the savanna. From their desperate embrace, the Tree of Light wove two spirits into a single body.

The Bunniraffe is a guardian of horizons. When it runs through the grass, the earth leaps with joy. When it rises to fight, shadows flee in shame. But its strength lies not only in speed or height, but in the friendship it offers to those who wander among the branches of the worlds.

Ancient spirits say the Bunniraffe will always be there where someone tries to mend what was broken, bringing courage and hope.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('5b1dba57-d930-445f-8b82-112ab1a3cf70', 'e4f6c9d1-2238-5cfc-bdfc-f14eef5ba18b', 'en-us', 'Bunnyppoturel', 'The Bunnyppoturel was born when a bunny sought the friendship of a hippo, and the Tree of Light united their destinies. With agile arms and a playful head, but a solid body, it is a spirit of balance between fragility and power, bringing cheerfulness and protection to the branches of the worlds.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('4eb92164-8a3f-4f66-867d-bee60bee7a99', '68db1f0f-33a7-54d0-a590-f1208dfe5b86', 'en-us', 'Bunnypporaffe', 'The Bunnypporaffe was born in the midst of a storm of light, when three friends — a bunny, a hippo, and a giraffe — tried to mend a broken branch of the Tree of Light. Their sacrifice was transformed into magic, and thus this unique creature was born. The Bunnypporaffe is a gentle guardian, carrying within it the hope and courage of friendship.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('948dc83f-94c2-4cdd-94fe-8f905dffb926', '558b9240-3c3a-5017-a85a-706a397288fd', 'en-us', 'Bunnyppotix', 'The Bunnyppotix was born when a bunny brought joy to two hippos. The Tree of Light united them into a protective and cheerful being. It is a spirit of friendship and courage, bringing light into the darkness.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('0277a14b-c875-4d8e-ab98-8f91bdbed275', '7afa2a1a-428b-5cef-8b85-b9b04b9a953a', 'en-us', 'Bunnyppo', 'The Bunnyppo was born when a bunny and a hippo met in the midst of a blackwater storm. From their unexpected friendship, the Tree of Light wove the bunny''s agility with the hippo''s strength. The Bunnyppo is gentle and protective: with its nimble arms, it digs hideouts, and with its powerful body, it opens paths through waters. It brings calm where there is fear and builds bridges of trust between worlds.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('5f064d82-6c0e-4ecd-a127-86a4f5d4d130', '05126872-ffdf-556d-818b-8df0d0f52fdf', 'en-us', 'Bunniraffe', 'The Bunniraffe was created when a bunny and a giraffe tried together to light up a fallen branch of the Tree of Light. Magic united them into a single body. The Bunniraffe is a spirit of courage and friendship, capable of bringing hope and light into the darkness.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('d3e22cbf-17f1-46f5-b293-90f85c04e2e7', '5666c0b4-90f0-5888-8bdc-fcd458613ff5', 'en-us', 'Bunnypotam', 'The Bunnypotam emerged when a bunny and a hippo stood together under the storm of the night. The Tree of Light wove them into a single body, preserving the bunny''s agility and the hippo''s wisdom. The Bunnypotam is a symbol of solidarity and friendship between the small and the great.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('d20f4d19-ca11-4320-b205-abc91a0e9dad', '3183543f-ff08-50d4-8b37-d307ce5d8ba7', 'en-us', 'Giraffonix', 'The Giraffonix appeared when a giraffe embraced two bunnies. The Tree of Light united them into a single being. It is a spirit of compassion and play, bringing cheerfulness into the branches of the worlds.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('bca38fd6-aff5-4464-aeb6-e9004467cdea', 'ce884695-d11a-52bb-b6fe-f6e892075bab', 'en-us', 'Giraffiepix', 'The Giraffiepix was born when two giraffes protected a small bunny. The Tree of Light united them into a single body, bestowing upon them the agility of the earth and the vision of the sky. It is a spirit of friendship and wisdom, capable of uniting opposite worlds.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('0765ec28-7ee4-4cf8-86d8-c28401622211', 'eb60fa21-0e19-5f9d-a283-91cfc87f17fa', 'en-us', 'Giraffoppam', 'The Giraffoppam appeared when three creatures swore to protect the branches of the tree together. From their friendship, the Tree of Light created a common body. The Giraffoppam is a spirit of unity and friendship, carrying within it the hope of uniting different worlds.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('24aebf8a-d3a8-46ed-94a0-c551670e0da1', '88010526-1b36-56eb-ade7-e6b60c80ae86', 'en-us', 'Girabbit', 'The Girabbit was born when a giraffe''s arms embraced a small bunny trembling in the night. From that moment, the Tree of Light united them into an unusual hybrid. The Girabbit is playful and fearless: it uses its long arms to gather stars and its bunny body to leap through shadows. It is a spirit of joy, bringing laughter and light wherever it goes.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('8d7bdcc9-06af-45e0-9ec4-75cd0018b790', '8fd7dcc8-2c78-54bd-8e9d-0e160f73f098', 'en-us', 'Giraffunnix', 'The Giraffunnix emerged when two giraffes wanted to share their height with a small bunny. The Tree of Light united them into a creature of friendship and joy. The Giraffunnix brings hope to the little ones, showing that greatness can be shared.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('43b2cb81-9f69-49b4-8bfe-d969ea4aac0a', '6a468b99-597b-5a87-b5d5-7dcade042a3c', 'en-us', 'Doublegiraffo', 'The Doublegiraffo was born when two giraffe spirits watched over a lost hippo together. The Tree of Light united them into a single being, tall and strong, yet gentle. The Doublegiraffo is a guardian of horizons, bringing balance between strength and vision.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('28cf7532-1c7b-4a65-bd44-cb9a218ce85f', 'eb68fa55-42df-56d7-9169-70148069d13c', 'en-us', 'Giraffoppit', 'The Giraffoppit was born in the midst of a meteor shower, when a giraffe, a hippo, and a bunny swore to protect the Tree of Light. From their oath, the branches united three spirits into a single body. It is a symbol of friendship between completely different beings. With the giraffe''s arms, it raises light to the sky; with the hippo''s body, it protects the earth; and with the bunny''s head, it brings smiles and hope. The Giraffoppit is seen as a guardian of balance.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('23d959c0-df53-4175-85e0-f29900dbd83b', 'c8386f26-e510-5ddf-90f5-fb5378222b78', 'en-us', 'Giraffoppotrel', 'The Giraffoppotrel appeared when two giraffes watched over a lost hippo under the starlight. The Tree of Light united them to maintain the balance between sky and water. It is a spirit of protection and clarity, bringing vision and gentle power.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('42239f80-a77d-4aa7-b9fe-7503b7188bc5', 'b7ba0f4c-2781-5e2b-8697-bb4c0ca5c2da', 'en-us', 'Giraffoppotix', 'The Giraffoppotix appeared when a giraffe swore to watch over two hippos in the middle of a storm. The Tree of Light united them into a single body. It is a spirit of protection and friendship, carrying within it the power of the waters and the vision of the sky.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('af9ab3dd-20b9-4f36-af04-09f6a2c934a7', 'ece938ea-108a-5c77-9471-5a65f3c5adaf', 'en-us', 'Giraffoppo', 'The Giraffoppo is a creature born from the dream of a giraffe gazing at the stars and the song of a hippo echoing under the waters. The Tree of Light united them into a single body to bring balance: the giraffe''s height and the hippo''s stability. It watches over the savannas of light and the deep waters, seeking to unite the sky with the earth. The Giraffoppo is a mediator between worlds, bringing wisdom and gentle power.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('170445b7-8ca5-4497-a7b5-dd9e11cfce94', '4eecbbc1-3b4c-5cbd-a9e8-cc3e8ab89ee8', 'en-us', 'Giraffit', 'The Giraffit emerged from the laughter of a bunny who clung to a giraffe''s neck. The Tree of Light found their joy and gave them a common body. The Giraffit is playful and cheerful, but also has a big heart, ready to defend the light. It is often described as a spirit of optimism.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('dd48db90-5725-4871-a3ca-ca42baa2c19a', '105daf4a-19b5-5d6b-b7b3-75532adb8790', 'en-us', 'Giraffopotir', 'The Giraffopotir was born from a pact between a hippo and a giraffe who watched together over a fallen branch of the Tree of Light. The Tree united their strengths and gave them a single body. The Giraffopotir is a spirit of balance between power and gentleness, carrying within it the wisdom of both worlds.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('efd6c542-b46f-4388-ac93-943d347ad4b8', '880c0786-0e60-51d0-be8f-0822a13d439a', 'en-us', 'Hipponepix', 'The Hipponepix was born when a hippo protected two bunnies frightened by the darkness. The Tree of Light united them into a common body. It is a spirit of joy and resilience, bringing hope to the little ones.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('426e8a76-839a-415d-abf5-8ede0d040bc9', 'd86bb1a9-645b-5f66-8e6e-6c93457d2618', 'en-us', 'Hippiraffit', 'The Hippiraffit is a creature of contrasts, born when three worlds collided in the Tree of Light. From the hippo, it received power; from the bunny, agility; and from the giraffe, vision. It roams the branches of the tree seeking harmony between extremes. The Hippiraffit is often described as a spirit that shows that true power comes from uniting the different.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('0f2bc62a-b103-4a60-8476-bf50d330f7a4', '8e05b69d-9b31-5702-8ec6-b479f6a923db', 'en-us', 'Hippoppoturel', 'The Hippoppoturel appeared when two hippos wanted to protect a fragile bunny. The Tree of Light united them into a common body, small but strong. It is a spirit of protection and courage, bringing balance between play and strength.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('6ec734a9-fd66-474e-b4d2-12e03b782762', '3de3b3e0-5241-5350-b01b-28361c99c363', 'en-us', 'Hippityhop', 'The Hippityhop was born from a playful dream in which a bunny asked for the strength of a hippo. The Tree of Light united them into a balanced being: small but strong, gentle but determined. The Hippityhop uses the hippo''s arms to protect and the bunny''s body to leap over obstacles. It brings courage to the small and shows them that size does not define the power of a soul.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('d050d845-7b16-4f22-8062-4c04fc3b42dc', 'd35b4ddf-a205-5bd2-ab2b-3c6639a446c1', 'en-us', 'Hippogirabbit', 'The Hippogirabbit was born the moment a hippo, a giraffe, and a bunny looked up together at the light of the Tree. Their spirits united into a new being. The Hippogirabbit is a spirit of collaboration and balance, bringing peace and wisdom in the struggles between worlds.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('a1596391-b906-4d83-8e13-70afd9ade363', 'd27c226d-6d8a-5257-a77d-477e0167a815', 'en-us', 'Hippogiraffox', 'The Hippogiraffox emerged when a hippo and two giraffes watched over a branch of the tree together. From their friendship, this unique being was born, a spirit of balance and wisdom.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('eb6082be-adad-4294-ae39-a53ed5fdfe16', '5dc2cb51-c888-51e5-b6b7-b4d187983e74', 'en-us', 'Hippogaffon', 'The Hippogaffon was born when two hippos and a giraffe shared a common dream: to unite the waters and the sky. The Tree of Light united them into a single body. It is a spirit of stability and wisdom, bringing clarity in the midst of the storm.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('dd42865e-5034-4212-8176-88b50b4183f2', 'bb2a526a-0819-501e-a572-9dd0fc0b6dee', 'en-us', 'Hippiraffe', 'The Hippiraffe was born when a hippo swore to protect a lost giraffe from the darkness. The Tree of Light united them into a single being to keep their promise. It is strong and wise, building bridges between sky and earth. The Hippiraffe is known as the spirit that brings balance between strength and grace.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('25f07bd2-f06e-4299-b41a-d1304c2feca4', 'ef8533a3-c8d0-593b-b8ac-77d4f582bcf6', 'en-us', 'Hipponelix', 'The Hipponelix appeared when two hippos swore to protect a bunny. The Tree of Light united them into a single body. It is a spirit of gentleness and strength, bringing peace between worlds.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('77a4eab7-efb8-4ebf-afe4-a4ec88a945c6', '39f2b85d-a7da-5149-8d2c-5fbe8e0281b8', 'en-us', 'Hippogaffix', 'The Hippogaffix was born when two hippos sought the wisdom of a giraffe. The Tree of Light united them into a spirit of stability and clarity. It watches over the branches, bringing balance between water and sky.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('9a524d13-9d5c-4c7a-a82e-96f51d60c00d', '2c786edd-9dc4-5683-87ba-f8de5e7b6980', 'en-us', 'Hipponel', 'The Hipponel appeared when a bunny asked for a hippo''s protection to survive the darkness. The Tree of Light united their souls and gave life to this playful yet powerful being. The Hipponel is a cheerful protector, bringing hope where there is fear.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('6d572859-cc07-49fe-8edf-0b6253065961', '8471a4f6-f238-5aa5-b8be-e8c123f35c4f', 'en-us', 'Hippiraf', 'The Hippiraf was born when a hippo and a giraffe watched over the savanna together. The Tree of Light united them and gave them a common heart. The Hippiraf is a spirit of vigilance, bringing balance between strength and clarity. It is a protector of starry nights.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('c1687d06-bcc4-4826-ac60-d9ab78adfaa1', '420cc407-c891-5ff8-8f2a-c50c4242563f', 'en-us', 'Bunnygiraf', 'The Bunnygiraf appeared when a bunny wished to watch over a savanna. The Tree of Light heard its wish and united it with the spirit of a giraffe. The Bunnygiraf is a spirit of responsibility and joy, carrying within it the desire to protect.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('f2c52cce-805b-489e-b46f-3a915c8eb943', 'e2af4281-d05b-59d9-8532-2b64e395dd6b', 'en-us', 'Bunnyppos', 'The Bunnyppos was born when a bunny dreamed of having the power of a hippo. The Tree of Light turned the dream into reality, uniting two opposite worlds. The Bunnyppos is gentle and peaceful, but has a hidden strength that awakens to defend the light. It is a symbol of dreams fulfilled.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('0bb88ac6-f80f-462b-90b8-6ac852b3e02e', 'f9f2a40c-59d8-588b-893c-7025a14dc209', 'en-us', 'Giraffunny', 'The Giraffunny was born when a bunny climbed onto a giraffe''s shoulders to look at the sky. The Tree of Light saw their joy and united them into a common body. The Giraffunny is a playful and friendly spirit, bringing cheerfulness and hope everywhere.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('ebfeda17-c62f-4fe3-abb6-c15b08c3520e', 'd4b242da-e3c3-52ad-9c9e-022bc0f0db7e', 'en-us', 'Giraffotam', 'The Giraffotam emerged when a hippo sought the height of a giraffe to watch over the savanna. The Tree of Light united them and gave them a common body. The Giraffotam is a spirit of stability, maintaining the balance between waters and sky. It is a protector of horizons.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('92a71329-b001-487c-a0ea-0bdf4e1f62d5', '07084a31-3e3a-5b86-8536-56d2311359bd', 'en-us', 'Hippuriel', 'The Hippuriel appeared when a bunny and a hippo sang together by a star-lit lake. The Tree of Light wove their song and laughter into a common body. The Hippuriel is a spirit of cheerfulness and harmony, bringing hope to those who wander.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

INSERT INTO alchimalia_schema."BestiaryItemTranslations"
    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")
VALUES
    ('2dff03ee-18a7-4d41-9097-4fb6dfc8c57a', '430ea06a-9ce9-57c7-a0b1-6353996bf8b8', 'en-us', 'Hippogiraffe', 'The Hippogiraffe was born when a hippo dreamed of seeing beyond the horizon of the waters. The Tree of Light united it with the spirit of a giraffe and gave them a common body. The Hippogiraffe is a spirit of vision and endurance, uniting the stability of the waters with the height of the sky.')
ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Story" = EXCLUDED."Story";

COMMIT;
