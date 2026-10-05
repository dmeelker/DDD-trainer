# Kettle & Cask Brewing Co. — Domain Reference

> **Audience:** instructors / Claude. This is the authoritative description of the training
> domain. It contains the full domain model, the formulas, the business rules, the deliberate
> flaws in the starting code and how they map onto the patterns of *Domain-Driven Design*
> (Evans), chapter 10 "Supple Design". Trainees get the lighter `README.md`.

---

## 1. The business

**Kettle & Cask** is a small independent craft brewery in a harbour town. It has:

| Asset | Description |
|---|---|
| **Pilot kit** | A 25 L brewing system where new recipes are developed and test-brewed. |
| **Brewhouse** | A 12 hL (1200 L) production system. Recipes proven on the pilot kit are scaled up to it. |
| **Cellar** | Fermentation tanks, conditioning tanks, and a small collection of oak barrels for sour and barrel-aged beer. |
| **Ingredient store** | Shelves and a cold room holding malt (sacks, kg) and hops (vacuum packs, grams). |

People (roles, not individuals):

- **Head brewer** — designs recipes, decides styles, signs off scale-ups. Speaks in brewing
  jargon and expects the software to as well.
- **Production brewer** — brews on the brewhouse from a *brew sheet*, pulls ingredients from
  the store, records measurements.
- **Cellar master** — manages fermentation, conditioning and **blending** (especially of barrel-aged sours).
- **Store keeper / buyer** — keeps inventory, orders malt and hops.

The software they currently use ("the brew sheet") does four things:

1. Calculate the *vital statistics* of a recipe (OG, FG, ABV, IBU, colour).
2. Check a recipe against a *style guideline*.
3. *Scale* a recipe from one batch size to another (pilot → production).
4. Check the ingredient store for a recipe and *pull* ingredients for brew day.

Plus a newer, rudimentary feature: recording the **blending** of batches in the cellar.

---

## 2. Ubiquitous language (glossary)

| Term | Meaning |
|---|---|
| **Recipe** | The design of a beer: its grain bill, hop schedule, yeast, batch size, boil time and intended style. A recipe is a *plan*, not beer. |
| **Batch** | An actual quantity of beer that exists in a vessel (fermenter, tank, barrel). Has a volume and *measured* gravity / ABV. Brewed *from* a recipe, but once it exists it lives its own life (measurements differ from predictions). |
| **Batch size** | Target volume of finished wort going into the fermenter, in liters. |
| **Wort** | Sugary liquid extracted from malt, before fermentation. Becomes beer after yeast ferments it. |
| **Fermentable** | Anything that contributes sugar: malted grain, adjunct sugar (dextrose, candi sugar), malt extract. Code calls these `Malt`. |
| **Grain bill** | The list of fermentables and their amounts. |
| **Mash** | Steeping crushed grain in hot water so enzymes convert starch to sugar. Only *grain* needs mashing. |
| **Potential (extract potential)** | How much gravity a fermentable can contribute: *gravity points per kilogram per liter* (pts·L/kg). 2-row pale malt ≈ 309, dextrose ≈ 384. (Imperial brewers use PPG; 1 PPG ≈ 8.345 pts·L/kg.) |
| **Brewhouse efficiency** | Fraction of grain's potential actually extracted on a given system, e.g. 0.72. A property of the **equipment and process**, not of the recipe (see §7). Does not apply to sugars/extracts, which dissolve completely. |
| **Specific gravity (SG)** | Density of wort/beer relative to water, e.g. 1.050. |
| **Gravity points** | `(SG − 1) × 1000`, so 1.050 = 50 points. Points are additive by volume and scale linearly with extract — the natural arithmetic unit. |
| **OG — Original Gravity** | Gravity of the wort before fermentation. |
| **FG — Final Gravity** | Gravity after fermentation finished. |
| **Attenuation** | Fraction of the gravity points the yeast consumes, e.g. 78 %. A property of the yeast strain (apparent attenuation). |
| **ABV** | Alcohol by volume, percent. |
| **Hop** | Cone of the hop plant. Provides bitterness (via alpha acids, when boiled) and aroma. |
| **Alpha acid (AA %)** | Bittering potential of a hop lot, e.g. Magnum 14 %. Varies per harvest/lot! |
| **Hop addition** | One use of a hop in the process: which hop, how much, when, and how (boil / whirlpool / dry hop). |
| **Hop schedule** | All hop additions of a recipe, in kettle order. |
| **Boil addition** | Hops added to the boiling wort *N minutes before the end of the boil*. Longer = more bitterness, less aroma. |
| **Whirlpool addition** | Hops added after flame-out while wort is still hot (~80 °C) and steeped for *N minutes*. Some bitterness, lots of aroma. |
| **Dry hop** | Hops added to the fermenter (cold). Aroma only, **no bitterness**. Time is measured in *days*, not minutes. |
| **IBU** | International Bitterness Units ≈ mg/L of isomerised alpha acid. Estimated with the Tinseth formula. |
| **Utilization** | Fraction of alpha acids that end up isomerised in the beer. |
| **Colour (SRM / EBC)** | Beer colour. SRM (US) ≈ °Lovibond; EBC = SRM × 1.97. Estimated with the Morey formula. |
| **°Lovibond (°L)** | Colour rating of a malt. |
| **MCU** | Malt Colour Units — intermediate in the Morey formula. |
| **Vital statistics** | The set {OG, FG, ABV, IBU, SRM} that describes a beer and is compared against a style. |
| **Style / style guideline** | A named category of beer (BJCP 2021) with an allowed *range* for each vital statistic. |
| **In style / to style** | All vital statistics fall within the style's ranges. |
| **Scaling** | Converting a recipe to a different batch size while keeping its vital statistics. |
| **Brew sheet** | The printed instructions the production brewer follows on brew day. Lists hops in kettle order. |
| **Pulling ingredients** | Taking the ingredients for one brew off the shelf (an inventory transaction). |
| **Blend / blending** | Combining two or more batches into one, e.g. young and aged sour beer. |

---

## 3. The domain model (as it *should* be understood)

Note: this describes the concepts and rules. The starting code models them poorly on purpose (§8).

### 3.1 Recipe
- Has a name, an intended style, a **batch size** (liters), boil time (minutes), a **grain bill**,
  a **hop schedule** and exactly one **yeast**.
- A recipe's vital statistics are a **pure function** of its contents plus the **brewhouse
  efficiency** of the system it is brewed on.
- Recipes are versioned in practice ("Hop Harbour IPA v3"); a scaled recipe is a *new* recipe
  derived from an old one, the original remains valid (pilot recipe is still brewed on the pilot kit).

### 3.2 Fermentable (code: `Malt`)
- Name, amount (kg), potential (pts·L/kg), colour (°L), kind.
- Kinds: **grain** (mashed → subject to efficiency), **sugar** and **extract** (not mashed → 100 % yield).
- Typical values used in the samples:

| Fermentable | Potential (pts·L/kg) | Colour (°L) | Kind |
|---|---|---|---|
| Pale Malt (2-row) | 309 | 2 | grain |
| Maris Otter | 313 | 3 | grain |
| Munich Malt | 292 | 9 | grain |
| Crystal 60 | 284 | 60 | grain |
| Flaked Barley | 267 | 2 | grain |
| Roasted Barley | 209 | 300 | grain |
| Dextrose | 384 | 0 | sugar |

### 3.3 Hop and hop addition (code: `Hop`, conflated)
- A **hop variety** (Citra, Magnum, ...) has a typical alpha acid range; a **hop lot** in the store
  has a measured alpha acid %. The code conflates variety, lot and addition into one `Hop` class.
- A **hop addition** = hop (lot) + amount (g) + use + time.
  - Boil: time = minutes before end of boil, 0..boil time.
  - Whirlpool: time = steep minutes.
  - Dry hop: time = days in fermenter (code stores 0 and ignores it).
- The same variety may appear several times in one schedule (Citra whirlpool + Citra dry hop).

### 3.4 Yeast
- Name, lab, attenuation (%). Real strains have an attenuation *range*; the model uses a single
  typical value.

### 3.5 Style guideline
A style is a named set of ranges. Ranges used by the code (approximations of BJCP 2021):

| Style | OG | FG | IBU | SRM | ABV % |
|---|---|---|---|---|---|
| American IPA (21A) | 1.056–1.070 | 1.008–1.014 | 40–70 | 6–14 | 5.5–7.5 |
| American Pale Ale (18B) | 1.045–1.060 | 1.010–1.015 | 30–50 | 5–10 | 4.5–6.2 |
| German Pils (5D) | 1.044–1.050 | 1.008–1.013 | 22–40 | 2–4 | 4.4–5.2 |
| Irish Stout (15B) | 1.036–1.044 | 1.007–1.011 | 25–45 | 25–40 | 4.0–4.5 |

Rules:
- Ranges are **inclusive**.
- A recipe is *in style* when every statistic is inside its range.
- Brewers also talk about *house rules* layered on top of a style, e.g. "our IPAs are always
  at least 60 IBU" or "nothing over 6.5 % on the cask line". These are additional constraints that
  combine with styles (AND), and occasionally a beer is entered in "either APA or IPA" (OR).
  → natural **Specification** territory (composite specs, intersection of ranges).

### 3.6 Brewhouse / equipment profile (implicit in current code)
- Pilot kit: 25 L, efficiency 0.72.
- Brewhouse: 1200 L, efficiency 0.80 (a bigger, better mill and lauter tun extract more).
- Today efficiency is a field on `Recipe`, which is why scaling from pilot to production gives
  a *wrong* prediction (see §7.2) — a deliberate conceptual-contour problem.

### 3.7 Ingredient inventory
- Holds amounts per ingredient: malt in **kg**, hops in **grams** (the code stores both as an
  untyped `double` in one dictionary).
- **Shortfall** = what the recipe needs − what is on the shelf, per ingredient, never negative.
- **Pulling** ingredients for a brew is all-or-nothing: either everything is taken or nothing is.
- Needs must be **aggregated per ingredient** first: a recipe using Citra twice needs the *sum*.

### 3.8 Batch and blending
- A batch is real beer: name, volume (L), current gravity (SG), ABV (%).
- **Blending** batches A and B yields a batch whose volume is the sum and whose gravity and ABV are
  the volume-weighted averages. (Gravity *points* blend linearly by volume; ABV blends linearly by
  volume to a good approximation.)
- Blending is associative and commutative; blending in an empty batch (0 L) is the identity.
- Taking a portion out (*drawing off*) splits a batch into two with identical gravity/ABV.

---

## 4. Formulas (the "established formalisms")

All formulas the brewers trust. Implementations must preserve these numbers (the characterization
tests enforce them).

### 4.1 Original gravity
```
points = Σ_grain (kg × potential × efficiency) + Σ_sugar/extract (kg × potential)
points_per_liter = points / batch_liters
OG = 1 + points_per_liter / 1000
```

### 4.2 Final gravity & ABV
```
FG = 1 + points_per_liter × (1 − attenuation) / 1000
ABV = (OG − FG) × 131.25
```

### 4.3 Bitterness — Tinseth
Per boil/whirlpool addition:
```
mg_alpha_per_liter = (AA% / 100) × grams × 1000 / batch_liters
bigness            = 1.65 × 0.000125^(OG − 1)
boil_time_factor   = (1 − e^(−0.04 × minutes)) / 4.15
utilization        = bigness × boil_time_factor
                     × 0.5 for whirlpool (house simplification)
                     × 0   for dry hop
IBU = Σ utilization × mg_alpha_per_liter
```
Simplification: Tinseth should use the *boil* gravity; we use OG.

Note the dependency: **IBU depends on OG**. That ordering is hidden inside `Calculate()`.

### 4.4 Colour — Morey
```
MCU = Σ (kg × 2.20462 × °L) / (batch_liters × 0.264172)
SRM = 1.4922 × MCU^0.6859
EBC = SRM × 1.97
```

### 4.5 Scaling
Linear scaling of all amounts by `factor = new_size / old_size` preserves OG, FG, ABV, IBU and
SRM *exactly* (every formula is "amount per liter"), **provided efficiency stays the same**.
Hop amounts are rounded to whole grams by the current code; this introduces tiny IBU drift on
small batches.

Scaling between systems with different efficiencies requires adjusting the **grain** (not the
sugar!) by `old_eff / new_eff` to hit the same OG.

### 4.6 Blending
```
V = V_a + V_b
SG = (SG_a × V_a + SG_b × V_b) / V      (equivalently on points)
ABV = (ABV_a × V_a + ABV_b × V_b) / V
```

### Reference numbers (from the tests)

| Recipe | OG | FG | ABV | IBU | SRM |
|---|---|---|---|---|---|
| Hop Harbour IPA (25 L, 72 %) | 1.0607 | 1.0134 | 6.21 | 46.05 | 8.43 |
| Harbour Dry Stout (25 L, 72 %) | 1.0430 | 1.0107 | 4.23 | 27.02 | 25.94 |

Hop Harbour IPA: 5.5 kg Pale 2-row, 0.6 kg Munich, 0.35 kg Crystal 60, 0.25 kg Dextrose;
Magnum 20 g @60 (14 %), Centennial 25 g @15 (10 %), Citra 40 g whirlpool 20 min (12 %),
Citra 60 g dry hop (12 %); US-05 78 %.

Harbour Dry Stout: 3.6 kg Maris Otter, 0.9 kg Flaked Barley, 0.6 kg Roasted Barley;
East Kent Goldings 55 g @60 (5 %); Wyeast Irish Ale 75 %.

---

## 5. Invariants and business rules (assertion candidates)

| # | Rule |
|---|---|
| I1 | Amounts (kg, g, L) are never negative. Batch size > 0. |
| I2 | Efficiency is a fraction in (0, 1]. "72" is not a valid efficiency — it's a units mistake. |
| I3 | Attenuation is a fraction in (0, 1) (realistically 0.6–0.9). |
| I4 | Alpha acid in (0, 25] %. |
| I5 | A boil addition's time is between 0 and the recipe's boil time. |
| I6 | Dry hops contribute 0 IBU, regardless of amount. |
| I7 | FG ≤ OG, and FG ≥ 1.000 for normal ales. |
| I8 | Scaling preserves vital statistics (post-condition; within rounding). |
| I9 | Scaling by factor *a* then *b* equals scaling by *a × b*. Scaling to the same size is identity. |
| I10 | Calculating statistics or checking style **never changes** the recipe. |
| I11 | Pulling ingredients is atomic: on failure the inventory is unchanged. |
| I12 | After pulling, stock = before − aggregated needs. |
| I13 | Blending conserves volume, and total alcohol (ABV × V) and total extract (points × V). |
| I14 | Blending with an empty batch is identity; blending is commutative and associative. |
| I15 | Range min ≤ max. Intersection of two ranges is a range (possibly empty). |

---

## 6. Use-case scenarios

1. **Design a new recipe on the pilot kit.** Head brewer enters grain bill, hops, yeast, picks a
   style, sees vital statistics and style deviations *live* while tweaking ("what if I add 5 g more
   Magnum?" — must not disturb the recipe being edited).
2. **Scale up to production.** Head brewer scales a pilot recipe to 1200 L on the brewhouse. Wants
   to see *both* recipes side by side. Expects identical statistics (and later: correct handling
   of the brewhouse's higher efficiency).
3. **Brew day preparation.** Production brewer asks "can we brew this today?" (query: shortfall
   list), then pulls the ingredients (command). Asking must not change anything.
4. **Competition entry / house rules.** "Find all recipes in our book that satisfy *American IPA
   AND our house rule IBU ≥ 60*." "Which styles does this recipe fit?"
5. **Blending in the cellar.** Cellar master blends 300 L young sour with 100 L 2-year barrel sour;
   wants to try different proportions on paper before committing ("what would 250 + 150 look like?").
6. **New hop lot arrives** with a different alpha acid; head brewer adjusts the bittering addition
   to hit the same IBU ("how many grams of this lot of Magnum give me 23.5 IBU?").

---

## 7. Known problems in the domain model (discussion material)

### 7.1 "Recipe" vs "beer" vs "brew"
Recipe (plan), Batch (actual beer), and "a brew" (the event of brewing a recipe on a system on a
date, producing a batch) are different concepts. Code has no *Brew*. Good discussion on
conceptual contours.

### 7.2 Efficiency is in the wrong place
Efficiency belongs to the **equipment** (pilot 0.72, brewhouse 0.80). On `Recipe` it makes the
recipe's statistics depend on where it will be brewed. Scaling pilot → brewhouse today keeps
0.72 — the production beer actually comes out stronger than predicted. A supple model separates
*Recipe* from *EquipmentProfile* and computes statistics for `(recipe, equipment)`.

### 7.3 Hop variety vs lot vs addition
Alpha acid belongs to the lot; time/use belongs to the addition. The single `Hop` class mixes them.

### 7.4 Units
kg vs g, minutes vs days (dry hop), fraction vs percent (efficiency 0.72 but attenuation 78 and
alpha 12). Prime candidates for value objects with units (`Mass`, `Volume`, `Percentage`,
`Gravity`, `Bitterness`, `Colour`).

---

## 8. The starting code and its deliberate flaws

Solution: `KettleAndCask.sln`
- `src/KettleAndCask.Domain` — the model.
- `src/KettleAndCask.BrewSheet` — console app printing a brew sheet, scaling, inventory check.
- `tests/KettleAndCask.Domain.Tests` — xUnit characterization tests (14). They test *behaviour*,
  not structure; trainees may reshape the API but must keep the numbers.

| File | Flaw |
|---|---|
| `Recipe.cs` | Anemic, all public setters. Calculated results (`OG`, `FG`, `IBU`, `SRM`, `ABV`) are mutable public **fields** cached on the recipe with a `Calculated` flag — stale as soon as an ingredient changes. Efficiency on recipe (§7.2). |
| `Ingredients.cs` | Primitive obsession: `double` amounts with units in comments, `string Type`/`string Use` with magic values, percent vs fraction inconsistency. `Hop` conflates variety/lot/addition. |
| `BrewCalculator.Calculate` | Vague name; one method does OG, IBU, SRM, FG, ABV with hidden ordering dependency (IBU needs OG). **Side effects in a "calculation":** writes results into the recipe, **silently rewrites `Efficiency`** (72 → 0.72), and **re-sorts the recipe's hop list** (brew-sheet concern leaking into the model). FG/ABV silently skipped if no yeast. Whirlpool magic `0.5`. |
| `BrewCalculator.Adjust(Recipe, double x)` | Non-intention-revealing name and parameter (`x` is a target batch size, not a factor). **Mutates in place** — the original pilot recipe is lost. Silently rounds hops. Leaves results stale (`Calculated=false`) but old values still readable. |
| `StyleChecker.Check` | Switch on style name with hard-coded, duplicated range checks; adding a style or house rule means editing the switch; can't combine (AND/OR), can't ask "which styles fit?". Returns English strings. **Side effect:** calls `Calculate` on an uncalculated recipe — which mutates efficiency and hop order. A check that changes what it checks. |
| `Inventory.Use` | Command and query merged (returns `bool` *and* mutates). **Not atomic:** decrements ingredients one by one and returns `false` halfway, leaving stock partially decremented (visible in the console demo: pale malt is gone, then Munich is short). Does not aggregate duplicate ingredients. |
| `Inventory.Missing` | Does not aggregate duplicates: Citra 1920 g + 2880 g = 4800 g needed vs 3000 g in stock, but each line individually fits → **not reported**. Mixed units in one dictionary. Returns formatted strings (culture-dependent: `0,10` on a Dutch machine). |
| `Batch.BlendIn` | Mutates the receiver *and* the argument (`other.Volume = 0`), concatenates names. Can't do "what if" blends. Division by zero when both volumes are 0. |
| `Program.cs` | Has to call `Calculate` again after `Adjust`; can't show pilot and production side by side because the pilot recipe was destroyed. |

### Bugs a careful trainee will discover
1. `Inventory.Use` leaves stock partially decremented on failure (I11).
2. `Inventory.Missing`/`Use` ignore duplicate ingredients across additions (I12).
3. `StyleChecker.Check` changes the recipe's hop order and efficiency (I10).
4. Reading `recipe.OG` after editing ingredients returns stale data.
5. Pilot → brewhouse scale-up ignores efficiency difference (§7.2).
6. Blending two empty batches → NaN.
7. Recipe without yeast → FG/ABV stay 0, still "calculated"; style check then reports FG/ABV out of range for an unclear reason.

---

## 9. Mapping to Chapter 10 — Supple Design

How each pattern can be practised in this codebase. (Instructor reference — avoid handing this
to trainees as a solution sheet.)

### Intention-Revealing Interfaces
- `Calculate` → something like `recipe.VitalStatistics(equipment)` / `PredictVitalStatistics`.
- `Adjust(r, x)` → `ScaleTo(Volume batchSize)`; `Use` → `Pull(...)` / `Withdraw(...)`; `Missing` → `ShortfallFor(...)`.
- `Check` → `style.IsSatisfiedBy(stats)` / `Deviations(...)`.
- Exercise: write the brew-sheet console program *first* in the language you wish you had, then make it compile.

### Side-Effect-Free Functions
- Make vital statistics a computed value object (`VitalStatistics`) returned by a pure function; no cached mutable fields.
- Scaling returns a new `Recipe` (pilot recipe stays intact).
- Hop ordering becomes a query (`HopSchedule.InKettleOrder()`), not an in-place sort.
- Blending returns a new `Batch`/`BeerVolume` value; "what-if" blends become free.
- Command–query separation in `Inventory`: `ShortfallFor(bill)` (query) vs `Pull(bill)` (command).

### Assertions
- Value object constructors enforce invariants I1–I4, I15.
- Post-conditions as tests: I8 (scaling preserves stats), I9, I10 (calc doesn't mutate), I11/I12 (atomic pull), I13/I14 (blending conservation). Great place for property-based tests (FsCheck) if the group knows them.
- Make the units mistake (efficiency 72) impossible rather than silently "fixed".

### Conceptual Contours
- Split the calculator along the domain's own seams: gravity (extract), bitterness (hops), colour (malt colour), fermentation (yeast) — each changes for different reasons and has its own formula.
- Separate Recipe / EquipmentProfile / Batch / Brew; separate HopVariety / HopLot / HopAddition.
- Discuss granularity: is `GrainBill` a concept worth a class? (Yes — brewers talk about it, and colour and gravity are functions of it alone.)

### Standalone Classes
- `Gravity` (with points arithmetic), `Volume`, `Mass`, `Percentage`, `Range<T>` — zero dependencies.
- `TinsethBitterness` / `MoreyColour` as standalone functions over primitive-ish value objects; testable without a `Recipe`.
- Goal: the core calculation classes should be understandable without reading anything else.

### Closure of Operations
- `Gravity + Gravity`, `Mass × factor → Mass`, `Volume + Volume`.
- `Recipe.ScaleTo(...) → Recipe`; `IngredientBill + IngredientBill → IngredientBill` (aggregation of duplicate Citra!); `Stock − IngredientBill → Stock`.
- `Batch.BlendWith(Batch) → Batch` (associative, identity = empty batch); `Batch.DrawOff(Volume) → (Batch, Batch)`.
- `Range ∩ Range → Range` (style ∩ house rule).
- `Specification.And/Or/Not → Specification`.

### Declarative Design / Specification
- `StyleGuideline` as data (table of ranges) instead of a switch; composite specifications for house rules ("American IPA AND IBU ≥ 60").
- Queries: "which styles does this recipe satisfy?", "which recipes in the book satisfy X?".
- Subsumption: does "IBU 60–70" subsume "IBU 40–70"? (Evans' spec subsumption, chapter 9/10 boundary.)

### Angles of Attack
- **Carve off subdomains:** the brewing-math (gravity/IBU/colour/units) is a generic, mathematical subdomain — carve it into its own module/namespace with a clean, closed, side-effect-free API. Inventory is another clean carve-out.
- **Draw on established formalisms:** units of measure, gravity points arithmetic, Tinseth/Morey, interval arithmetic for ranges, the algebra of blending (weighted averages form a commutative monoid), set/predicate logic for specifications.

---

## 10. Extension ideas (later sessions / other chapters)

- Mash schedule with steps (temperature, duration); assertion: mash temperatures 35–80 °C.
- Equipment profile with boil-off rate and losses → pre-boil volume.
- Recipe book (repository) and recipe versions.
- Water chemistry (ions, residual alkalinity) — another rich formalism.
- Hop substitution: "replace Centennial with Cascade at the same IBU" (inverse of Tinseth).
- Event-sourced cellar log (blends, transfers, gravity readings).
- Bounded contexts for chapter 14: Recipe Design vs Production vs Cellar vs Purchasing, with "Hop" meaning different things in each.
