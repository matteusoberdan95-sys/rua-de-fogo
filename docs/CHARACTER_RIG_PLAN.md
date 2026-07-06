# Plano de Personagens — Rig Procedural vs Arte Final

Este documento define o que continua como **rig 2D procedural** no runtime e o que deve virar **sprite sheet / camadas pintadas** na arte final.

## Regra do projeto

- `art/` e `references/` são **referência visual**, nunca PNG colado no gameplay.
- Runtime atual: `CharacterSpriteVisual` + `EnemyLayeredVisual` com `UseLayeredPrototype = true`.
- Sprint 31 reforçou **poses, silhueta e leitura de combate** no rig procedural.

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
