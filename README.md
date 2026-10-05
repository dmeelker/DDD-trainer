# Kettle & Cask Brewing Co.

A training playground for chapter 10 of *Domain-Driven Design* (Eric Evans), **Supple Design**.

You've been hired by Kettle & Cask, a small craft brewery in a harbour town. They have
software that works — mostly — but the brewers keep tripping over it. Your job: reshape it
into a design that is a pleasure to use and to change, **without changing the beer** (the
tests pin down the numbers).

## How this repo works

**You write the code. Claude is your domain expert.**

Start Claude Code in this directory (`claude`) and it takes on the role of the brewery's
head brewer. It knows the brewery, the beers, the formulas and the business rules. It won't
touch the code. Use it the way you'd use a real domain expert on a DDD project:

- **Ask about the domain.** "What's the difference between a recipe and a batch?", "Why does a
  dry hop add no bitterness?", "What happens to the OG when we scale up to the brewhouse?"
- **Try out names and concepts.** "Would you call this a *hop addition* or a *hop*?", "Is
  'grain bill' something you actually say?" Listen for the words it uses: that's your
  ubiquitous language.
- **Check rules and edge cases.** "Can a boil addition go in before the boil starts?", "What
  should happen if we can't pull all the ingredients?"
- **Ask for a review of your design.** "Does this `ScaleTo` match how you think about scaling?"
- **Switch to the coach when you want design advice.** Start with "*As a DDD coach*, ..." and
  Claude steps out of the brewer role to discuss the chapter 10 patterns and trade-offs. Expect
  questions and hints, not finished solutions.

Claude only edits a file when you explicitly ask it to in that message. When the conversation
turns up a new domain rule, it may offer to write it down in the domain notes.

Please don't read `.domain-expert/`. It's Claude's reference material and contains spoilers.

### Suggested workflow

1. Read this README, run the tests and the brew sheet.
2. Pick a complaint below, and interview the domain expert until you understand the problem.
3. Refactor by hand, in small steps, keeping `dotnet test` green.
4. Show your design to the domain expert (and to each other) and discuss.

## Getting started

```
dotnet test                                  # 14 characterization tests, all green
dotnet run --project src/KettleAndCask.BrewSheet   # prints a brew sheet
claude                                       # talk to the domain expert
```

- `src/KettleAndCask.Domain`: the model you'll be reshaping
- `src/KettleAndCask.BrewSheet`: console app the brewers use
- `tests/KettleAndCask.Domain.Tests`: tests on *behaviour*. Feel free to change the API they call, but not the numbers they expect
- `CLAUDE.md`, `.domain-expert/`: setup for the domain expert (no need to read)

## The brewery in five minutes

Kettle & Cask develops new beers on a 25 L **pilot kit**. Recipes that work are **scaled up**
to the 1200 L **brewhouse**. Beer then goes into the **cellar** to ferment, where some of it
is aged in barrels and later **blended**. Malt and hops live in the **ingredient store**.

A **recipe** is a plan for a beer:

- a **grain bill** — the malts and sugars (kg) that provide the sugar yeast turns into alcohol
- a **hop schedule** — hop additions (g) that bring bitterness and aroma. Hops are added in the
  **boil** (bitterness), in the **whirlpool** just after the boil (some bitterness, lots of aroma),
  or as a **dry hop** in the fermenter (aroma only, no bitterness)
- a **yeast** strain, which eats a typical share of the sugar (its **attenuation**)
- a **batch size** in liters

From a recipe the software predicts its **vital statistics**:

| Statistic | What it means |
|---|---|
| **OG** — original gravity | How sugary the wort is before fermenting, e.g. 1.060 |
| **FG** — final gravity | What's left after the yeast is done, e.g. 1.013 |
| **ABV** | Alcohol by volume, follows from OG and FG |
| **IBU** | Bitterness |
| **SRM** | Colour (pale straw ≈ 3, amber ≈ 10, stout ≈ 30+) |

Each beer **style** (American IPA, German Pils, Irish Stout, ...) allows a *range* for each
statistic. A recipe is **in style** if all its statistics fall within those ranges.

A **batch** is not a recipe: it's real beer in a tank or barrel, with a measured volume,
gravity and ABV. Blending two batches gives a batch with the combined volume and the
volume-weighted average gravity and ABV.

The formulas (gravity points, Tinseth for bitterness, Morey for colour) are already in the
code. You don't need to understand the chemistry, just treat them as the established
formulas they are.

## Complaints from the brewery floor

> **Head brewer:** "I scaled my pilot IPA up to the brewhouse and now the pilot recipe is gone.
> I just wanted to see both side by side."

> **Head brewer:** "I ran a style check and afterwards my hop schedule had been reordered. And an
> old recipe of mine said efficiency 72, now it says 0.72. Why does *checking* change anything?"

> **Production brewer:** "The system said it couldn't pull the ingredients for brew day. Fine. But
> the pale malt had been booked out of stock anyway."

> **Store keeper:** "We ran out of Citra halfway through a brew, yet the shortfall report said
> everything was there."

> **Cellar master:** "I want to try blends on paper: what would 250 L young plus 150 L aged
> taste like? Every time I try, the system empties my barrel."

> **Head brewer:** "We want house rules on top of styles: *our* IPAs are always at least 60 IBU.
> And for competitions I'd like to know which styles a recipe fits."

> **New developer:** "What does `Adjust(recipe, 1200)` do? Is 1200 a percentage? And is that
> amount in kilograms or grams?"

## The patterns to practise

Chapter 10 describes these; every one of them has a natural home in this code:

- Intention-Revealing Interfaces
- Side-Effect-Free Functions
- Assertions
- Conceptual Contours
- Standalone Classes
- Closure of Operations
- Declarative Design (and Specifications)
- Angles of Attack: carving off subdomains, drawing on established formalisms

There's no single right answer. Compare designs with each other and discuss the trade-offs.

## Glossary

| Term | Meaning |
|---|---|
| Brewhouse efficiency | Share of the grain's sugar a brewing system actually extracts (e.g. 72 %). Sugar added directly isn't affected. |
| Potential | How much gravity a kilogram of a fermentable can add per liter |
| Gravity points | 1.050 is "50 points"; points add up nicely |
| Alpha acid | A hop's bittering strength (%), varies per harvest |
| Attenuation | Share of the sugar the yeast ferments (%) |
| Brew sheet | Printed instructions for brew day, hops listed in the order they go in |
| Pulling ingredients | Taking a brew's ingredients off the shelf |
| Blending | Combining batches into one |
