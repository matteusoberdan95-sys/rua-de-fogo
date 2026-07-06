# Plano de Personagens — Rig Procedural vs Arte Final

Este documento define o que continua como **rig 2D procedural** no runtime e o que deve virar **sprite sheet / camadas pintadas** na arte final.

## Atualizacao Sprint 39 - Anatomia e poses

Feedback do playtest em 06/07/2026: o rig procedural ficou funcional, mas ainda parecia cabecudo/travado demais. A direcao atual e manter `Polygon2D` no runtime, porem com:

- corpo mais alto e menos achatado;
- cabeca menor em todos os presets;
- coxa/canela/braco/antebraco mais longos;
- caminhada com passada maior e joelho dobrando;
- golpes sem sobrescrever o braco de ataque pela guarda;
- jab/cross/side kick com extensao visual clara.

Proxima prioridade de personagem: animacao secundaria (cabelo, roupa, respiracao, impacto e recuperacao), usando uma pose desenhada por cima do rig como guia de arte final.

## Atualizacao Sprint 40 - Rig como fallback

Decisao apos playtest: `CharacterSpriteVisual` nao e mais tratado como visual final vendavel.

Uso correto daqui para frente:

- fallback jogavel;
- laboratorio de hitbox, timing, combate e camera;
- referencia tecnica para pivot/baseline;
- suporte temporario ate production art entrar.

Caminho final:

- Caua em sprite sheet ou rig 2D com partes desenhadas;
- Quebra-Osso em sprite sheet/rig real;
- cenarios em camadas pintadas;
- assets dentro de `art/production/`.

Documento principal: `docs/SPRINT_40_ART_PIPELINE_REAL.md`.

## Regra do projeto

**Leia `docs/VISUAL_RULE.md` primeiro.**

- `art/` e `references/` sao **somente ideia** — moodboard, silhueta, cor, pose.
- Runtime: **rig 2D em camadas** (`UseLayeredPrototype = true`) — `Polygon2D` e animacao por codigo na engine.
- **Nunca** colar PNG de referencia no gameplay.

## Caua (jogador)

| Elemento | Agora (procedural) | Arte final |
|----------|-------------------|------------|
| Corpo, pernas, braços | Polígonos segmentados + animação por código | Sprite sheet ou rig exportado do Krita |
| Rosto, cabelo, cachecol | Polígonos + dano progressivo (olho roxo, sangue) | Retrato pintado com layers de dano |
| Idle / walk / run | Pose de lutador, peso nas pernas | Mesmas poses, desenhadas |
| Golpes por estilo | `MoveAnimProfile` no rig | Frames de ataque por estilo marcial |
| Guarda / parry / stagger | Animação procedural | Frames dedicados de defesa |
| Armas improvisadas | Polígonos anexados ao braço | Sprites de vergalhao/martelo/faca |

## Inimigos

| Preset | Silhueta atual | Identidade visual |
|--------|----------------|-------------------|
| `QuebraOsso` | Encolhido, veias, faca enferrujada | Grunt padrão — soco e cabeçada |
| `Fast` | Menor (0.84x), garras, jitter no idle | Rápido — telegraph curto, salto |
| `Brute` | Maior (1.16x), braço de concreto | Pesado — windup longo |
| `Infected` | Pele esverdeada, veias, mordida | Chuva/infectado — espasmos |
| `MiniBoss` | Maior (1.22x), espinhos, clava | Chefe — telegraph lento e ameaçador |

| Estado | Agora | Arte final |
|--------|-------|------------|
| Idle | Variação por preset + seed por instância | 2–3 idles por tipo |
| Telegraph | Windup distinto por preset | Frame de aviso `!PARRY!` |
| Hurt / parry stagger | Recoil + careta | Frame de impacto |
| Morte | Colapso + fade (0.72s) | Animação de queda/gore |

## O que NÃO mudar na Sprint 31

- Camera aberta e HUD compacto (Sprint 30).
- Chuva/vinheta atenuadas.
- Halo/rim light de leitura nos personagens.

## Próximo passo (Sprint 32)

Substituir gradualmente props do `LayeredStreetPrototype` por assets finais da Vila Esperança, mantendo o rig de personagens até a arte fechar.
