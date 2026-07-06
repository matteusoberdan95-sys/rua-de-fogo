# Art Production

Esta pasta e para assets reais de producao. Diferente de `references/`, tudo aqui pode virar runtime depois de validado.

Regra:

- `references/`: moodboard, conceito, comparacao visual.
- `art/production/`: assets criados para o jogo.
- `art/production/_exports/`: arquivos exportados prontos para importar no Godot.

Sprint atual: Sprint 40 - Art Pipeline Real (base tecnica implementada).

## Como importar personagem

1. Desenhe/exporte frames em `characters/<nome>/sprites/` (PNG transparente, facing direita).
2. Monte ou gere `SpriteFrames` (ex.: `caua_v0_frames.tres`).
3. Atualize `manifest.json` na pasta do personagem.
4. Teste em `scenes/tests/CauaProductionArtTest.tscn`.
5. Ative `UseProductionArt = true` no ator quando validado.

Fallback: se `ProductionArtCatalog.HasProductionPack()` falhar, o jogo usa `CharacterSpriteVisual` procedural.

## Pacotes

| Personagem | Pasta | SpriteFrames |
|------------|-------|--------------|
| Caua v0 (placeholder) | `characters/caua/` | `caua_v0_frames.tres` |
| Quebra-Osso | `characters/enemies/quebra-osso/` | pendente |

Prioridade:

1. `characters/caua/` — **ativo** (placeholder v0 no runtime de teste)
2. `characters/enemies/quebra-osso/`
3. `stages/vila-esperanca/`
4. `props/`
5. `fx/`
