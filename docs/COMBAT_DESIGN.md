# Design De Combate - Sangue no Asfalto

Documento de direcao para combate corpo a corpo, progressao de golpes, dano cumulativo e armas improvisadas.

## Filosofia

Caua **nao e espadachim**. Ele e um sobrevivente urbano que briga com o que tem: punhos, joelhadas, cotoveladas, chutes e, quando a rua permite, ferramentas pesadas ou cortantes **pega no chao**.

A violencia e estilizada e brasileira — peso de pancadaria de rua, nao duelo de espadas. Armas sao **eventos raros e temporarios**, nao loadout permanente.

## Loop De Combate Base (inicio do jogo)

### Golpes desarmados (sempre disponiveis)

| Slot combo | Golpe | Leitura visual | Notas |
|------------|-------|----------------|-------|
| 1 | Jab / direto | Soco rapido, braco estendido | Alcance curto, inicia combo |
| 2 | Chute lateral | Perna girando, torso inclina | Empurra inimigo na lane |
| 3 | Finisher de combo | Voadora ou cotovelada descendente | Knockback forte, gasta mais stamina |

Golpes especiais desbloqueaveis (nao no combo basico):

| Input | Golpe | Custo |
|-------|-------|-------|
| `K` + direcao | Esquiva (ja existe) | Stamina |
| `Espaco` | Joelhada no ar / voadora curta | Stamina |
| Segurar `J` | Agarrão + joelhada (futuro) | Stamina alta |

Inimigos comuns comecam com **soco, garra e cabecada** — sem faca fixa, sem tubo na mao o tempo todo.

### O que sai do prototipo atual

- Faca/machete **fixa** na mao do Caua (rig e HUD).
- Trail de corte como feedback padrao do combo desarmado.
- Label `Arma: Facao` como estado default.
- Inimigo Quebra-Osso com tubo/faca sempre equipado.

Substituir por punhos cerrados, guarda alta baixa, e animacoes de impacto com punho/pe/chinelo.

## Progressao Por XP (Sprint 21+)

Ver tabela de estilos marciais abaixo. Corrida desbloqueia golpes exclusivos por estilo (ex.: chute giratorio capoeira só correndo).

## Movimento avancado (Sprint 19+)

| Input | Acao |
|-------|------|
| A/D | Andar |
| A/A ou D/D rapido | Correr (2.4s) |
| J/J/J (chao) | Soco / chute / voadora |
| Espaco + J | Voadora aerea (prioridade sobre combo) |
| Correndo + J | Soco corrida |
| Correndo + J/J | Chute corrida |

## Sidearm — pistola (Sprint 19)

- 7 balas iniciais; **E** recarrega (1.25s) ou pickup de municao.
- Animacao sacar/atirar no rig.
- Projetil pequeno, rapido, baixo knockback.
- **Sangramento**: DOT 5 HP/s por 3.5s (base hemorragia).
- Futuro: calibres (.22 / 9mm / .38) com hemorragia escalonada; inimigo esguichando sangue visual.

## Spawn e pacing (Sprint 19)

- Inimigos surgem **a frente** do jogador conforme a onda avanca (240–320px).
- Spawn escalonado (~0.85s entre cada).
- Intro curta; primeira luta ao andar ~140px.
- Evita backtrack para achar inimigos nos cantos da fase.

## Progressao Por XP (Sprint 21+)

| Estilo | Identidade | Exemplos de golpes | Desbloqueio sugerido |
|--------|------------|--------------------|----------------------|
| **Rua** (base) | Sobrevivencia | Soco, chute, cotovelada, voadora | Nv 1 |
| **Boxe** | Pressao e ritmo | Jab-cross-hook, uppercut, slip | Nv 3 |
| **Muay Thai** | Clinch e pernas | Joelhada, cotovelada, chute baixo, teep | Nv 5 |
| **Karate** | Linha e impacto | Gyaku-zuki, mae-geri, ura-ken | Nv 7 |
| **Capoeira** | Mobilidade | Meia-lua, esquiva baixa, bencao | Nv 9 |
| **Jiu-Jitsu** | Controle | Queda, armlock visual, finalizacao no chao | Nv 11 |

Regras de design:

- No maximo **1 estilo novo a cada ~2 niveis** na Vila Esperanca.
- HUD mostra estilo equipado / barra de tecnicas.
- Tutorial explica que XP abre golpes, nao so HP.

Implementacao tecnica futura: `MoveCatalog`, `UnlockedMoveIds`, combo resolver que monta sequencia a partir do deck ativo.

## Dano Cumulativo No Inimigo

Inimigos **machucam visualmente** conforme perdem vida — reforca sensacao de beat 'em up arcade.

| Faixa HP | Estado | Visual no rig (Quebra-Osso e variantes) |
|----------|--------|------------------------------------------|
| 100–66% | Intacto | Pose padrao |
| 65–33% | Machucado | Bandagem rasgada, olho roxo, sangue no rosto, mancando leve |
| 32–1% | Critico | Braco ferido, dente faltando, sangue no torso, telegraph mais lento |
| 0% | Morte | Ragdoll curto ou animacao de queda |

Camadas extras no `CharacterSpriteVisual`: `DamageTier` driven by `Health.CurrentHealth / MaxHealth`.

## Armas Improvisadas (pickup temporario)

Armas **nao sao equipamento permanente**. Spawnam no chao ou dropam de inimigos. Cada uma tem:

- **Durabilidade** (3–8 golpes)
- **Tipo de dano** (contundente / cortante / perfurante)
- **Golpe basico** + **finalizador** se inimigo estiver em estado critico ou apos combo completo

| Arma | Golpe basico | Finalizador (inimigo critico) | Durabilidade |
|------|--------------|-------------------------------|--------------|
| Vergalhao | Pancada horizontal | Quebra de joelho / nocaute | 6 |
| Martelo | Smash overhead | Cranium burst (estilizado) | 4 |
| Faca | Corte rapido | Decapitacao / evisceracao curta | 5 |
| Machado | Corte pesado | Partir ao meio (silhueta) | 3 |
| Taser / corrente | Choque / arrastar | Finalizacao eletro (futuro) | 8 |

Regras de gore:

- Sempre **silhueta + particulas + som** — nunca realismo fotografico.
- Finalizador consome **1 durabilidade extra** ou encerra a arma.
- Camera micro-zoom + hit pause no finisher.
- Inimigos boss: so fase de finalizacao, nao decap instantanea no meio da luta.

Sistema existente a evoluir: `WeaponPickup`, `HasImprovisedWeapon`, `WeaponDurability` em `SideScrollerPlayerController`.

## Prioridade De Implementacao (recomendacao dos agents)

```
Sprint 17  → personagens vivos              ✅
Sprint 18  → combate desarmado              ✅
Sprint 19  → movimento, spawn, sidearm      ✅ aprovada
Sprint 20  → armas improvisadas + finishers      ✅ implementada
Sprint 21  → arvore XP / estilos marciais
Paralelo   → SFX ambiente + sprite Krita (pos contrato de golpes)
```

**Por que nao fazer tudo agora?**

- Remover faca + 3 golpes desarmados e **1 sprint focada** (~2–3 dias).
- Dano cumulativo visual e **mesma sprint** (reusa rig Sprint 15–17).
- Finishers gore multi-arma exigem VFX, estados, balanceamento — sprint propria.
- Arvore marcial e progressao XP e **sistema de meta** — sprint dedicada apos base estar fun.

**Por que nao adiar desarmado?**

- Desenhar sprite sheet com faca agora **joga trabalho fora**.
- Trail de corte contradiz identidade de pancadaria.
- Tutorial e refs `01` mostram personagem urbano, nao samurai.

## Contrato Tecnico (Sprint 18)

Arquivos tocados:

- `scripts/visual/CharacterSpriteVisual.cs` — remover `_weapon` default; anim `Punch`, `Kick`, `FlyingKick`.
- `scripts/player/SideScrollerPlayerController.cs` — `WeaponName => "Punhos"`; combo sem slash.
- `scripts/enemies/SideScrollerEnemyController.cs` — ataques desarmados.
- `scripts/combat/EnemyDamageState.cs` (novo) — tier visual por HP.
- `BeatEmUpHud` — label `Estilo: Rua` / tecnicas.
- `TutorialScreen` — controles desarmados.

## Metricas De Pronto (Sprint 18)

- [x] Caua e grunt sem lamina fixa na mao.
- [x] Combo J/J/J = soco / chute / voadora (animacoes distintas no rig).
- [x] Inimigo ataca com soco/cabecada telegrafados.
- [x] Inimigo muda aparente visual em 3 tiers de HP.
- [x] HUD reflete combate desarmado.
- [x] Docs e tutorial atualizados.
- [x] `dotnet build` validado.
- [ ] F5 validado no Godot.
