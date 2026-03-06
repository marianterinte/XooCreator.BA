import json
import uuid
import datetime
import pathlib


def main() -> None:
    # Adjusted to known repo root on this machine
    repo_root = pathlib.Path(r"d:\WORK\MyWork\Alchimalia3")

    scripts_root = repo_root / "BA" / "XooCreator.BA" / "Database" / "Scripts"
    v0002_path = scripts_root / "V0002__seed_bestiary_items.sql"

    i18n_root = (
        repo_root
        / "BA"
        / "XooCreator.BA"
        / "XooCreator.BA"
        / "Data"
        / "SeedData"
        / "Discovery"
        / "i18n"
    )
    locales = ["en-us", "ro-ro", "hu-hu"]

    # 1) Build comboKey -> BestiaryItemId map from V0002
    combo_to_id: dict[str, str] = {}

    lines = v0002_path.read_text(encoding="utf-8").splitlines()

    in_bestiary = False
    for i, line in enumerate(lines):
        if 'INSERT INTO alchimalia_schema."BestiaryItems"' in line:
            in_bestiary = True
            continue
        if not in_bestiary:
            continue

        stripped = line.strip()
        if stripped.startswith("VALUES") and i + 1 < len(lines):
            row = lines[i + 1].strip()
            if not row.startswith("('"):
                continue

            # Extract first 4 quoted values: Id, ArmsKey, BodyKey, HeadKey
            values: list[str] = []
            idx = 0
            while idx < len(row) and len(values) < 4:
                if row[idx] == "'":
                    start = idx + 1
                    j = start
                    while j < len(row):
                        if row[j] == "'":
                            # Handle escaped single quote ''
                            if j + 1 < len(row) and row[j + 1] == "'":
                                j += 2
                                continue
                            break
                        j += 1
                    values.append(row[start:j])
                    idx = j + 1
                else:
                    idx += 1

            if len(values) == 4:
                bestiary_id, arms, body, head = values
                combo_key = f"{arms}|{body}|{head}"
                combo_to_id[combo_key] = bestiary_id

    if not combo_to_id:
        raise SystemExit(
            "No BestiaryItems found when parsing V0002__seed_bestiary_items.sql"
        )

    # 2) Load JSON per locale
    locale_data: dict[str, dict[str, dict]] = {}
    for loc in locales:
        path = i18n_root / loc / "discover-bestiary.json"
        with path.open("r", encoding="utf-8") as f:
            arr = json.load(f)
        m: dict[str, dict] = {}
        for obj in arr:
            combo = obj.get("Combination")
            if combo:
                m[combo] = obj
        locale_data[loc] = m

    # 3) Helper to split Combination -> ArmsKey, BodyKey, HeadKey
    TOKENS = ["Bunny", "Giraffe", "Hippo", "None"]

    def split_combination(comb: str) -> tuple[str, str, str]:
        result: list[str] = []
        remaining = comb
        for _ in range(3):
            match = None
            for token in TOKENS:
                if remaining.startswith(token):
                    match = token
                    remaining = remaining[len(token) :]
                    break
            if match is None:
                raise ValueError(f"Could not split combination {comb!r}")
            if match == "None":
                match = "—"
            result.append(match)
        return result[0], result[1], result[2]

    # 4) Helper to SQL-escape string

    def sql_literal(value: str | None) -> str:
        if value is None:
            return "''"
        s = str(value)
        s = s.replace("'", "''")
        return "'" + s + "'"

    # 5) Generate SQL
    v00120_path = scripts_root / "V00120__seed_bestiary_item_translations.sql"

    lines_out: list[str] = []
    lines_out.append(
        "-- Auto-generated from Data/SeedData/Discovery/i18n/*/discover-bestiary.json"
    )
    lines_out.append("-- Locales: " + ", ".join(locales))
    run_date = datetime.datetime.now(datetime.timezone.utc).isoformat()
    lines_out.append(f"-- Run date: {run_date}")
    lines_out.append(
        "-- This script seeds BestiaryItemTranslations for all discovery bestiary combinations."
    )
    lines_out.append("-- It is idempotent: safe to run multiple times.")
    lines_out.append("")
    lines_out.append("BEGIN;")
    lines_out.append("")

    # Use English combinations as canonical set
    base_map = locale_data["en-us"]
    combinations = sorted(base_map.keys())
    insert_count = 0

    for comb in combinations:
        arms, body, head = split_combination(comb)
        combo_key = f"{arms}|{body}|{head}"
        bestiary_id = combo_to_id.get(combo_key)
        if not bestiary_id:
            raise SystemExit(
                f"No BestiaryItemId found for combo_key {combo_key!r} (combination {comb!r})"
            )

        for loc in locales:
            loc_map = locale_data.get(loc, {})
            obj = loc_map.get(comb)
            if not obj:
                continue
            name = obj.get("Name")
            story = obj.get("Story")

            trans_id = str(uuid.uuid4())

            block_lines = [
                'INSERT INTO alchimalia_schema."BestiaryItemTranslations"',
                '    ("Id", "BestiaryItemId", "LanguageCode", "Name", "Story")',
                "VALUES",
                f"    ('{trans_id}', '{bestiary_id}', '{loc}', {sql_literal(name)}, {sql_literal(story)})",
                'ON CONFLICT ("BestiaryItemId", "LanguageCode") DO UPDATE',
                'SET "Name" = EXCLUDED."Name",',
                '    "Story" = EXCLUDED."Story";',
                "",
            ]
            lines_out.append("\n".join(block_lines))
            insert_count += 1

    lines_out.append("COMMIT;")
    lines_out.append("")

    v00120_path.write_text("\n".join(lines_out), encoding="utf-8")

    print(
        f"Generated {insert_count} BestiaryItemTranslations statements into {v00120_path}"
    )


if __name__ == "__main__":
    main()

