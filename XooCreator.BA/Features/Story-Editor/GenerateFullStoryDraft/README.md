# Generate Full Story Draft – API key override (Pas 1)

## Google

Existing services already support `apiKeyOverride`. Use from new code (no new classes):

- **Text:** `IGoogleTextService.GenerateContentAsync(systemInstruction, userContent, apiKeyOverride, modelOverride, ct)` — use for full-story text in one call (e.g. with ###T/###S/###P prompt).
- **Image:** `IGoogleImageService.GenerateStoryImageAsync(..., apiKeyOverride, modelOverride)`.
- **Audio:** `IGoogleAudioGeneratorService.GenerateAudioAsync(..., apiKeyOverride, ttsModelOverride, ct)`.

No changes were made to these services.

## OpenAI

Existing services do not accept an API key per request. New adapters (this folder) provide the same capabilities with a caller-provided key:

- **Text:** `IOpenAITextWithApiKey.GenerateFullStoryTextAsync(..., apiKeyOverride, modelOverride, ct)` → `OpenAITextApiKeyAdapter`.
- **Audio:** `IOpenAIAudioWithApiKey.GenerateAudioAsync(..., apiKeyOverride, modelOverride, ct)` → `OpenAIAudioApiKeyAdapter`.
- **Image:** `IOpenAIImageWithApiKey.GenerateStoryImageAsync(..., apiKeyOverride, modelOverride, ct)` → `OpenAIImageApiKeyAdapter`.

Adapters are registered in `ServiceCollectionExtensions.AddAIServices()`.
