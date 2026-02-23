# AGENTS.md (Backend .NET) — Golden Rules

## Non-negotiables
1) **Nu strica business logic existent.**
   - Default: behavior-preserving refactor.
   - Dacă schimbi reguli: trebuie SPEC.md

2) **Clean code + reuse.**
   - Evită duplicarea: refolosește helpers/utilitare existente.
   - Nu introduce framework-uri/dependințe noi fără motiv clar.

3) **Patterns cu discernământ**
   - Factory: când ai creare non-trivială / variante pe config.
   - Extension methods: pt mapping / helpers mici / compunere fluentă.
   - NU ascunde business logic în “magic extensions”.
 
## Refactor triggers (obligatoriu)
Dacă atingi un fișier și vezi:
- Class > ~450 LOC
- Method > ~90 LOC
- nesting adânc / prea multe ramuri
Atunci: **refactor subtask obligatoriu** (behavior-preserving).

## Safety
- Validare input la boundary.
- CancellationToken în async (unde se potrivește).
- Log fără secrete.