# LOI Generative (Imagination Laboratory) – API key and AI calls

## Where to set the API key

- **File:** [appsettings.json](../appsettings.json)
- **Section:** `GoogleAI`
- **Key:** `GoogleAI:ApiKey` – set your Google AI Studio API key here. Leave empty in repo; replace locally for testing.
- LOI generative (image + text) uses the same key. Optional: `GoogleAI:Image:DefaultModel` and the text model in GoogleTextService (e.g. gemini-2.5-flash) for cost control.

## Where the key is read

- **GoogleImageService:** constructor reads `configuration["GoogleAI:ApiKey"]` (required for image generation).
- **GoogleTextService:** constructor reads `configuration["GoogleAI:ApiKey"]` (used for story text generation).

## Where AI is called for LOI

- **GenerativeLoiQueueWorker** ([Features/CreatureBuilder/Services/GenerativeLoiQueueWorker.cs](../Features/CreatureBuilder/Services/GenerativeLoiQueueWorker.cs)):
  - **Image:** `loiImageService.GenerateCreatureImageAsync(combination, lang, ...)` – [LOIImageGenerationService](../Features/CreatureBuilder/Services/LOIImageGenerationService.cs) builds the kid-safe prompt and calls **GoogleImageService.GenerateImageFromPromptAsync** (config API key).
  - **Text:** `textService.GenerateContentAsync(systemInstruction, userContent, responseMimeType: "text/plain", ...)` – uses config API key.

No override is passed for LOI; the key from appsettings is used for both calls.
