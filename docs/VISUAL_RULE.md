# Regra Visual - Leia Antes De Mexer Na Arte

## Regra Numero 1

`references/` NAO entra no gameplay.

Nao pode:

- colar PNG de referencia no personagem, inimigo ou cenario;
- extrair sprite de prancha conceitual;
- apontar `Sprite2D`, `AnimatedSprite2D` ou parallax para imagem de `references/`;
- usar concept art como fundo final.

`references/` existe apenas para moodboard: silhueta, cor, pose, clima e composicao.

## O Que Pode Entrar No Runtime

`art/production/` pode entrar no gameplay quando for asset criado para o jogo.

Isso inclui:

- sprite sheet original;
- personagem desenhado em camadas;
- cenario pintado/modular;
- props, FX, UI e texturas produzidas para o projeto.

## Duas Trilhas Oficiais

### 1. Gameplay Lab

Usa o que ja existe:

- `CharacterSpriteVisual`;
- `EnemyLayeredVisual`;
- `LayeredStreetPrototype`;
- `StageAssetLibrary`;
- `Polygon2D` e animacao por codigo.

Objetivo: testar gameplay, hitbox, camera, combate, clima e ritmo.

### 2. Production Art

Comeca na Sprint 40.

Usa assets em:

- `art/production/characters/`;
- `art/production/stages/`;
- `art/production/props/`;
- `art/production/fx/`;
- `art/production/_exports/`.

Objetivo: chegar em visual vendavel, com personagens e cenarios reais.

## Regra De Transicao

O rig procedural continua como fallback jogavel.

Arte real substitui o rig gradualmente, nesta ordem:

1. Caua production art v0;
2. Quebra-Osso production art v0;
3. trecho pintado da Vila Esperanca;
4. luz, sombra, chuva e reflexos finais;
5. vertical slice visual.

## Proibido

- transformar `references/` em runtime;
- commitar asset sem origem clara;
- remover fallback procedural antes de haver asset real validado;
- marcar sprint visual como concluida sem F5, docs, commit e push.

## Documento Relacionado

Leia tambem:

- `docs/SPRINT_40_ART_PIPELINE_REAL.md`;
- `art/production/README.md`;
- `docs/CHARACTER_RIG_PLAN.md`.
