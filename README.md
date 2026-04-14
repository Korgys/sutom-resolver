# Sutom Resolver

![Build](https://github.com/korgys/sutom-resolver/actions/workflows/buildAndTest.yml/badge.svg) ![Status](https://img.shields.io/badge/status-active-success) ![Last Commit](https://img.shields.io/github/last-commit/korgys/sutom-resolver) ![Coverage](https://img.shields.io/sonar/coverage/Korgys_sutom-resolver?server=https%3A%2F%2Fsonarcloud.io&color=brightgreen&style=flat-square&label=coverage) [![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=Korgys_sutom-resolver&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=Korgys_sutom-resolver) [![Lines of Code](https://sonarcloud.io/api/project_badges/measure?project=Korgys_sutom-resolver&metric=ncloc)](https://sonarcloud.io/summary/new_code?id=Korgys_sutom-resolver) [![Duplicated Lines (%)](https://sonarcloud.io/api/project_badges/measure?project=Korgys_sutom-resolver&metric=duplicated_lines_density)](https://sonarcloud.io/summary/new_code?id=Korgys_sutom-resolver) [![Bugs](https://sonarcloud.io/api/project_badges/measure?project=Korgys_sutom-resolver&metric=bugs)](https://sonarcloud.io/summary/new_code?id=Korgys_sutom-resolver) [![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=Korgys_sutom-resolver&metric=vulnerabilities)](https://sonarcloud.io/summary/new_code?id=Korgys_sutom-resolver) [![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=Korgys_sutom-resolver&metric=reliability_rating)](https://sonarcloud.io/summary/new_code?id=Korgys_sutom-resolver) [![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=Korgys_sutom-resolver&metric=security_rating)](https://sonarcloud.io/summary/new_code?id=Korgys_sutom-resolver) [![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=Korgys_sutom-resolver&metric=sqale_rating)](https://sonarcloud.io/summary/new_code?id=Korgys_sutom-resolver) ![.NET](https://img.shields.io/badge/.NET-11.0-blue) ![Platform](https://img.shields.io/badge/platform-Windows%20%7C%20Linux%20%7C%20MacOS-lightgrey) ![License](https://img.shields.io/github/license/korgys/sutom-resolver)

## Overview

Sutom Resolver is a .NET console application that helps solve Sutom-style word puzzles.

It is built around a solver pipeline that can:

- generate a candidate guess from a known pattern
- evaluate guess feedback using the Sutom rules
- refine the candidate list after each response
- simulate thousands of games to compare solver strategies

The repository currently contains several solver versions under `SutomResolver/solver/`:

- `v1` - baseline solver
- `v2` - improved solver logic
- `v3` - heuristic-driven solver
- `v4` - current solver used by the entry point

## Key Features

- Interactive solving from a partially known pattern
- Game simulation for benchmarking solver performance
- Word normalization to handle accents consistently
- Dictionary-based candidate filtering
- Multiple solver implementations for experimentation
- Automated tests and CI with coverage and SonarCloud analysis

## Repository Structure

```text
.
|-- SutomResolver/              # Console app
|   |-- Program.cs              # Application entry point
|   |-- Simulator.cs            # Interactive mode and game simulation
|   |-- SutomHelper.cs          # Word loading and response evaluation helpers
|   |-- data/fr.txt             # French dictionary used by the solver
|   |-- solver/                 # Solver implementations
|-- SutomResolver.Tests/        # MSTest test project
|-- .github/workflows/          # CI workflow
|-- coverlet.runsettings        # Coverage configuration
`-- SutomResolver.sln           # Visual Studio solution
```

## Requirements

[.NET SDK 11.0](https://dotnet.microsoft.com/)

## Getting Started

### Clone the repository

```bash
git clone https://github.com/Korgys/sutom-resolver.git
cd sutom-resolver
```

### Build the solution

```bash
dotnet build
```

### Run the application

```bash
cd SutomResolver
dotnet run
```

## CLI Usage

The application is intended to be used from the command line.

### Interactive solve

Enter the known pattern when prompted, for example:

```text
L___
```

You then answer each proposed guess using the Sutom feedback format:

- `_` or `?` for absent letters
- `+` for letters present but in the wrong position
- the letter itself when it is correctly placed

### What you enter in the CLI

- Start with the known pattern of the hidden word.
- After each suggested guess, enter the feedback pattern returned by the game.
- Keep following the prompt until the solver finds the word or no candidates remain.

### Example

```text
Entrez le pattern du mot a trouver (ex: L___) :
L___
Le solveur propose : LION
Entrez le pattern du mot a trouver :
L++_
```

The exact guesses depend on the active solver and dictionary state.

### Run a simulation

The project also supports simulation through the `Simulator<T>` class.
The current entry point uses the interactive mode, but the codebase can be extended to run batch simulations for solver comparison.

## How It Works

The solver uses a dictionary-driven workflow:

1. Load the French word list from `SutomResolver/data/fr.txt`.
2. Normalize words to uppercase without diacritics.
3. Filter candidates by word length and any known constraints.
4. Produce the next guess from the active solver implementation.
5. Apply the Sutom response pattern to shrink the candidate set.
6. Repeat until the word is found or the solver runs out of attempts.

The response evaluator in `SutomHelper.GetResultFromGuess` follows the same two-pass logic used by word games:

- exact matches are marked first
- misplaced letters are marked only if occurrences remain available

## Interactive Mode

The default application flow is interactive.

Example session:

```text
Entrez le pattern du mot a trouver (ex: L___) :
```

If you enter a partially known pattern, the solver will keep proposing guesses until:

- the word is found
- the candidate list is exhausted
- the response you provide eliminates all possibilities

## Simulation Mode

`Simulator<T>` is designed to benchmark solver quality over many random games.

It tracks:

- number of games played
- wins
- losses
- average turns
- runtime in milliseconds

This is what the test project uses to compare solver versions.

## Roadmap

- [ ] Improve solver performance
- [ ] Add richer CLI options for batch solving and simulations
- [ ] Publish coverage and quality badges with live project metrics

## Tests

Run the full test suite with:

```bash
dotnet test
```

The test project currently checks:

- success rate for solver versions `v1`, `v2`, and `v3`
- relative improvement between solver generations

## Word List

The solver relies on the French word list stored in:

```text
SutomResolver/data/fr.txt
```

If you want to adapt the project to another language or dictionary:

- replace the word list
- keep the normalization behavior aligned with your target language
- review the solver heuristics and test expectations

## Extending the Solver

The cleanest extension point is a new solver implementation under:

```text
SutomResolver/solver/
```

To add a new strategy:

1. Implement the `ISolver` contract.
2. Add your solver in a new folder or version namespace.
3. Update the entry point if you want it to become the default.
4. Add tests that compare it against the existing versions.

## License

This project is licensed under the MIT License. See [`LICENSE`](./LICENSE).
