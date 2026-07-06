# Handoff - Continuar Em Codex, CLI Ou Outro PC

Este documento existe para manter o projeto continuavel mesmo se trocar de computador, abrir pelo Codex Desktop, Codex CLI, Cursor/Cursor CLI ou outro fluxo.

## Onde Estamos

Projeto: `Sangue no Asfalto`

Engine: Godot 4.7 .NET

Linguagem: C#

Direcao atual: prototipo lateral/2.5D beat 'em up implementado para validacao, com o top-down mantido como laboratorio antigo.

Sprint atual: Sprint 40 - Art Pipeline Real.

Decisao critica: o rig procedural em `Polygon2D` continua como gameplay-lab/fallback. Ele nao e mais o caminho de arte final. A partir da Sprint 40, assets reais entram por `art/production/`.

Sprint atual: ver `docs/SPRINTS.md`.

## Arquivos Que Devem Ser Lidos Primeiro

Ao abrir o projeto em outro ambiente, leia nesta ordem:

1. `README.md`
2. `docs/PROJECT_BRIEF.md`
3. `docs/SPRINTS.md`
4. `docs/SPRINT_40_ART_PIPELINE_REAL.md`
5. `docs/VISUAL_RULE.md`
6. `art/production/README.md`
7. `docs/VISUAL_BIBLE.md`
8. `docs/AGENTS.md`
9. `docs/WEATHER_TIME_SYSTEM.md`
10. `docs/ARCHITECTURE.md`
11. `docs/BACKLOG.md`

## Como Validar O Projeto

No terminal, dentro da pasta do projeto:

```powershell
dotnet build SangueNoAsfalto.csproj
```

Resultado esperado:

- `0` erros;
- `0` avisos idealmente;
- DLL gerada em `.godot/mono/temp/bin/Debug/`.

No Godot:

1. abrir a pasta do projeto;
2. aguardar importacao/compilacao C#;
3. rodar com `F5`;
4. testar controles.

## Controles Atuais

- `A/D` ou setas esquerda/direita: mover pela rua
- `W/S` ou setas cima/baixo: trocar lane/profundidade
- duplo-tap `A` ou `D`: correr
- `J` ou botao esquerdo: combo/ataque (4 golpes por estilo marcial)
- `L` ou botao direito: pistola
- `E`: recarregar pistola
- `K`: esquiva / dash
- `Espaco`: pulo (voadora com `J` no ar)
- **segurar `Q`**: defender (chip de dano, empurra para tras, enche postura)
- **toque rapido `Q`**: parry no `! PARRY !` ou no impacto do golpe
- `R`: reiniciar
- `M`: voltar ao menu (morte/vitoria)
- `F9`: modo screenshot

## Cenas Atuais

- `scenes/levels/PrototypeArena.tscn`: prototipo top-down/arena.
- `scenes/levels/SideScrollerPrototype.tscn`: prototipo lateral/2.5D atual e cena principal do `F5`.
- `scenes/actors/Player.tscn`: jogador atual.
- `scenes/actors/EnemyGrunt.tscn`: inimigo comum.
- `scenes/actors/SideScrollerPlayer.tscn`: jogador lateral.
- `scenes/actors/SideScrollerEnemyGrunt.tscn`: inimigo lateral.
- `scenes/ui/MainMenu.tscn`: menu inicial (cena principal do `F5`).
- `scenes/ui/TutorialScreen.tscn`: tutorial dedicado antes da fase.
- `scenes/world/VilaEsperancaParallax.tscn`: fundo parallax pintado (3 camadas).

## Cena Principal Atual

Cena que roda com `F5`:

```text
scenes/ui/MainMenu.tscn
```

A demo carrega `scenes/ui/TutorialScreen.tscn` a partir do menu (`Jogar`), e depois `scenes/levels/SideScrollerPrototype.tscn`.

Objetivo:

- testar camera lateral;
- testar lanes;
- reaproveitar combate onde fizer sentido;
- aproximar gameplay das referencias em `references/pillars`;
- preparar a cena para clima, luzes e horario dinamico futuros.

Scripts novos da Sprint 02:

- `scripts/player/SideScrollerPlayerController.cs`
- `scripts/enemies/SideScrollerEnemyController.cs`
- `scripts/core/SideScrollerDirector.cs`
- `scripts/ui/BeatEmUpHud.cs`

Scripts iniciados na Sprint 03:

- `scripts/core/CombatFeedback.cs`: feedback visual de impacto, sangue placeholder, mancha no chao, som placeholder e hit pause.
- `scripts/combat/Hitbox.cs`: agora expoe `HitStunDuration` e o contrato `ICombatKnockbackReceiver`.
- `scripts/combat/Hurtbox.cs`: dispara feedback somente quando o dano entra de verdade.

Estado validado da Sprint 03:

- prototipo lateral testado no Godot com `F5`;
- combate esta mais pesado e legivel;
- sangue aumentado apos teste;
- bug de slow motion persistente apos derrotar inimigos corrigido;
- som placeholder de impacto adicionado;
- indicador de invulnerabilidade do jogador melhorado com pulso azul/ciano;
- build C# validada com `dotnet build SangueNoAsfalto.csproj`.

Estado da Sprint 04:

- `scenes/levels/SideScrollerPrototype.tscn` recebeu primeira passada visual: asfalto molhado, rachaduras, reflexos, fios, neblina, caixas d'agua, boteco/mercadinho fechado, pichacoes, props urbanos, bueiro, altar e postes com pools de luz;
- `scenes/actors/SideScrollerPlayer.tscn` recebeu silhueta melhor com jaqueta, bandagem, cabelo, bracos e lamina mais forte;
- `scenes/actors/SideScrollerEnemyGrunt.tscn` recebeu silhueta mais agressiva com olhos fortes, mandibula, costelas/brilho, garra e espinhos;
- validada no Godot com `F5` e aprovada.

Scripts iniciados na Sprint 05:

- `scripts/world/TimeOfDayController.cs`: ciclo visual simples de horario e luzes reagindo ao estado.
- `scripts/world/WeatherController.cs`: clima visual placeholder com garoa, chuva forte, tempestade, neblina, lama/poca e relampago.

Estado da Sprint 05:

- `SideScrollerPrototype.tscn` recebeu `TimeOfDayController` e `WeatherController`;
- ciclo visual entre amanhecer, manha, tarde, por do sol e noite;
- chuva/garoa placeholder com gotas `Line2D` geradas por codigo;
- relampago placeholder em tempestade;
- lama/poca e neblina mudam por estado de clima;
- luzes/pools de poste reagem ao horario;
- build C# validada com `dotnet build SangueNoAsfalto.csproj`;
- validada no Godot com `F5`;
- chuva ficou visivel apos ajuste para gotas `Line2D` geradas por codigo.

Proximo passo recomendado:

1. commitar e fazer push da Sprint 07;
2. iniciar planejamento da `Sprint 08 - Conteudo Alpha`;
3. expandir conteudo sem quebrar a vertical slice;
4. manter a regra obrigatoria de build, Godot, docs, commit e push.

Estado da Sprint 06:

- `scripts/core/SideScrollerDirector.cs` agora controla fluxo curto de fase;
- sequencia: encontro inicial, checkpoint, segundo encontro, mini-chefe e vitoria;
- checkpoint simples em memoria usando `R` para reiniciar do checkpoint apos morte;
- criada `scenes/actors/SideScrollerMiniBoss.tscn`;
- `scripts/ui/BeatEmUpHud.cs` mostra etapa, objetivo, checkpoint, morte e vitoria;
- build C# validada com `dotnet build SangueNoAsfalto.csproj`;
- validada no Godot com `F5` e aprovada.

Estado da Sprint 07:

- `scripts/systems/GameSave.cs` e `scripts/systems/SaveManager.cs` criam save local em `user://save_game.json`;
- checkpoint da vertical slice persiste no save;
- `scripts/player/SideScrollerPlayerController.cs` agora tem arma improvisada, durabilidade e continue;
- `scripts/pickups/Pickup.cs` controla pickups de cura, arma e continue;
- `scenes/pickups/HealthPickup.tscn`, `WeaponPickup.tscn` e `ContinuePickup.tscn` foram adicionadas;
- `BeatEmUpHud` mostra arma, durabilidade, continue e atalhos;
- `F1` alterna HUD debug, `F2` alterna controles alternativos, `F4` limpa o save;
- `GlobalUsings.cs` centraliza imports globais por camadas;
- scripts C# foram limpos para remover `using` repetitivo no topo;
- build C# validada com `dotnet build SangueNoAsfalto.csproj`;
- validada no Godot com `F5` e aprovada.

Estado da Sprint 08:

- adicionadas variacoes de inimigo: rapido, bruto e infectado/chuvoso;
- adicionados `SideScrollerRainMiniBoss.tscn` e `SideScrollerAlphaBoss.tscn`;
- `SideScrollerDirector` agora usa composicoes por encontro;
- fluxo alpha atual: comuns, checkpoint, comuns + rapido + infectado, bruto + comuns, mini-chefe, mini-chefe de chuva, chefe alpha e vitoria;
- `Espaco` agora executa pulo visual do jogador; esquiva fica no `K`;
- primeira rodada de balanceamento aplicada em vida, dano, velocidade, cooldown e quantidades;
- build C# validada com `dotnet build SangueNoAsfalto.csproj`;
- validada no Godot com `F5` e aprovada;
- `Espaco` ajustado para pulo visual do jogador.

Estado da Sprint 09:

- `scenes/ui/MainMenu.tscn` foi criada e virou a cena principal em `project.godot`;
- `scripts/ui/MainMenu.cs` controla iniciar demo, limpar save, alternar controles e sair fora do editor;
- menu mostra painel simples de configuracoes com modo de controle, checkpoint salvo e atalhos;
- `BeatEmUpHud` agora mostra tutorial discreto por etapa sem pausar o jogo;
- HUD ganhou overlay central para morte e fim da demo;
- `SideScrollerDirector` permite voltar ao menu com `M` quando o jogador morre ou termina a demo;
- `export_presets.cfg` adiciona preset inicial `Windows Desktop`;
- `docs/BUILD_WINDOWS.md` documenta export pelo editor e pelo CLI;
- build C# validada com `dotnet build SangueNoAsfalto.csproj` com 0 erros e 0 avisos;
- validada no Godot com `F5` e aprovada.

Estado da Sprint 10:

- Sprint 10 foi replanejada como `Identidade Visual Pillars`, usando `references/pillars` como alvo;
- prioridade aplicada: HUD (`04`), Vila Esperanca (`02`), Caua (`01`);
- `BeatEmUpHud` reorganizado com banner, barras, combo callout, furia e slot de arma;
- `SideScrollerPlayerController` rastreia combo de hits, melhor combo e furia;
- `SideScrollerPrototype.tscn` recebeu boteco do Ze, ponto final, cachorro e pichacao;
- `SideScrollerPlayer.tscn` recebeu colete vermelho e regata branca;
- build C# validada com `dotnet build SangueNoAsfalto.csproj` com 0 erros e 0 avisos;
- validada no Godot com `F5` e aprovada.

Estado da Sprint 11:

- criados docs Steam/QC/launch/demo/trailer interno;
- versao demo `Demo v1.0` / `1.0.0-demo` no menu e export preset;
- modo screenshot com `F9` via `ScreenshotModeHelper`;
- script `scripts/build-demo.ps1` e pasta `marketing/screenshots/steam/`;
- build C# validada com `dotnet build SangueNoAsfalto.csproj` com 0 erros e 0 avisos;
- validada no Godot com `F5` e aprovada;
- HUD simplificado: HP, stamina, XP/nivel, arma e habilidades; tutorial in-game removido.

Estado da Sprint 12:

- `SideScrollerDirector` repacingado para ~10 min (intro, 9 encontros, 4 respiros, 3 chefes);
- clima e horario controlados por ato da fase (`AutoCycle = false`);
- `scenes/ui/TutorialScreen.tscn` + fluxo menu -> tutorial -> fase;
- cenario: carro policia, neon boteco, inimigo estilo Quebra-Osso;
- build C# validada com `dotnet build SangueNoAsfalto.csproj` com 0 erros e 0 avisos;
- aguardando validacao no Godot (~10 min de sessao).

Estado da Sprint 13:

- estilo travado: pintura 2D + pixel aparente (Krita, filtro Nearest no Godot);
- assets em `art/sprites/` e `art/backgrounds/vila-esperanca/` (placeholders);
- `scripts/visual/CharacterSpriteVisual.cs` — animacao, flip e pulo;
- `SideScrollerPlayer.tscn` e `SideScrollerEnemyGrunt.tscn` migrados para `AnimatedSprite2D`;
- `VilaEsperancaParallax.tscn` com 3 camadas + vignette;
- `scripts/ui/GameUiTheme.cs` + skin no `BeatEmUpHud`;
- build C# validada com `dotnet build SangueNoAsfalto.csproj` com 0 erros e 0 avisos;
- aguardando validacao no Godot (sprites, parallax, HUD vs refs).

Nota historica:

- as Sprints 24 a 28 adicionaram golpes por estilo, rig em camadas, props quebraveis, defesa, parry e postura;
- a Sprint 29 foi validada no Godot e abriu caminho para a Sprint 30.

Estado da Sprint 29:

- concluida e validada no Godot pelo usuario em 06/07/2026;
- `scripts/world/LayeredStreetPrototype.cs` recebeu passada visual de fase 1;
- adicionados comercios, pichacoes, calcada, asfalto remendado, lixeiras, caixa eletrica, varal, cabos, cacos, papeis, buracos, pocas e marcas de rua;
- objetivo: transformar as referencias em camadas editaveis, nao em fundo chapado;
- correcao apos playtest: os detalhes existiam, mas estavam escondidos atras do ceu/camadas antigas por ordem de renderizacao;
- `scenes/levels/SideScrollerPrototype.tscn` agora esconde `Road`, `WetRoadSheen`, `LaneBandBack`, `LaneBandMid`, `LaneBandFront` e `NightVignette`;
- `LayeredStreetPrototype` agora usa ordem `sky -> far -> mid -> near -> foreground`, sempre atras do jogador;
- `dotnet build SangueNoAsfalto.csproj` validado com 0 erros e 0 avisos.

Estado da Sprint 30:

- concluida e validada no Godot pelo usuario em 06/07/2026;
- `BeatEmUpHud` e `SideScrollerPrototype.tscn` usam HUD compacto: painel menor, barras menores, textos abreviados e `TechniquesLabel` previsto na cena;
- `SideScrollerPlayer.tscn` abriu a camera (`zoom 1.28`) e deslocou enquadramento para mostrar mais rua e perigo a frente;
- `CharacterSpriteVisual` adicionou halo, sombra de contato e rim light sutil atras do rig para destacar personagens no cenario escuro;
- `WeatherController` reduziu quantidade, largura e alpha das gotas de chuva;
- chuva placeholder e vinheta foram atenuadas para nao esconder personagem/pickups;
- `dotnet build SangueNoAsfalto.csproj` validado com 0 erros e 0 avisos.

Proximo passo recomendado:

1. export/pacote testers: `scripts/package-vertical-slice.ps1` ou Godot Export;
2. arte final Krita (`docs/STAGE_ASSET_PIPELINE.md`) ou nova sprint de conteudo;
3. manter baseline sprints 30–34.

Estado da Sprint 34:

- concluida e validada no Godot em 06/07/2026;
- vertical slice v1.1 aprovada para compartilhar.

Estado da Sprint 33:

- concluida e validada no Godot em 06/07/2026;
- lama, raio, vento e apagao nos chefes aprovados.

Estado da Sprint 32:

- concluida e validada no Godot em 06/07/2026;
- landmarks, postes, chao e leitura lateral aprovados.

Estado da Sprint 31:

- concluida e validada no Godot em 06/07/2026; personagens expressivos, morte/telegraph legiveis, `CombatPacing` aprovados.

Roadmap planejado:

- Sprint 32: cenario da Fase 1 com assets finais (concluida);
- Sprint 33: clima e rua viva como gameplay (concluida);
- Sprint 34: demo vertical slice e QC (concluida);

Estado da Sprint 28:

- implementada; tap vs hold em Q, knockback no block, postura agressiva, `RegenPaused`; aguardando playtest.

Estado da Sprint 27:

- implementada; combo 4 golpes, corrida+dash+J, defesa com Q; aguardando playtest.

Estado da Sprint 26:

- implementada; anatomia segmentada, dano progressivo, `BreakableStageProp`; aguardando playtest.

Estado da Sprint 25:

- implementada; rig 2D em camadas, `EnemyLayeredVisual`; aguardando playtest.

Estado da Sprint 24:

- implementada; `MoveCatalog`, animacoes por estilo e HUD de tecnicas; aguardando playtest.

Estado da Sprint 23:

- concluida, validada, commitada; estilos por XP, parry legivel, fim da fase com portao SAIDA aprovados.

Estado da Sprint 22:

- concluida e validada; combate plastico, dor visivel e stamina por golpe aprovados.

Estado da Sprint 21:

- concluida e validada; fase scroll e parry/postura aprovados.

Estado da Sprint 20:

- concluida e validada; armas, finishers e reload aprovados.

Estado da Sprint 19:

- concluida e validada no Godot;
- corrida, voadora no ar, spawn a frente, sidearm com bleed e Caua machucando aprovados.

Estado da Sprint 18:

- concluida e validada no Godot;
- combate desarmado, tiers de dano no inimigo e cabecada ok.

Estado da Sprint 14:

- criada `tools/normalize-sprites.ps1`;
- gerados `caua_idle_game.png`, `caua_walk_sheet_game.png` e `grunt_idle_game.png`;
- `SideScrollerPlayer.tscn` e `SideScrollerEnemyGrunt.tscn` apontam para os sprites normalizados;
- `CharacterSpriteVisual` agora tem `SourceFacesRight`;
- grunt usa `SourceFacesRight = false`;
- inimigo sem walk sheet tem bob visual leve quando anda;
- criada `tools/extract-reference-assets.ps1`;
- gerados `caua_ref_idle.png`, `caua_ref_walk_sheet.png` e `caua_ref_attack.png` a partir da prancha de personagens;
- `SideScrollerPlayer.tscn` agora usa `caua_ref_*` para coerencia visual do Caua;
- `VilaEsperancaParallax.tscn` reposicionado para mostrar melhor as camadas pintadas;
- blocagem antiga da fase foi escondida/atenuada onde competia com o background;
- bloqueadores dos props escondidos foram desligados para nao criar colisao invisivel;
- build C# validada com `dotnet build SangueNoAsfalto.csproj`.

Estado da Sprint 15:

- `scripts/visual/CharacterSpriteVisual.cs` ganhou modo `UseLayeredPrototype`;
- `scenes/actors/SideScrollerPlayer.tscn` ativa `UseLayeredPrototype = true`;
- o Caua deixou de depender visualmente dos recortes `caua_ref_*` durante o jogo;
- rig atual cria pernas, bracos, torso/camisa, cabeca, cabelo e faca/machete por codigo;
- idle tem respiracao, cabelo e pulso no peito;
- walk alterna pernas/bracos e adiciona peso;
- attack anima windup/swing com torso, braco e arma;
- dash inclina corpo/cabelo;
- recortes `caua_ref_*` ficam apenas como fallback/referencia ate a sprite sheet final;
- build C# validada com `dotnet build SangueNoAsfalto.csproj`.

Estado da Sprint 16:

- concluida e validada no Godot;
- escala Caua/inimigo, cobertura da camera, chao e leitura da rua aprovados;
- `LayeredStreetPrototype` + Quebra-Osso em camadas ativos na fase.

Estado da Sprint 17:

- concluida e validada no playtest;
- combo, hurt, telegraph e trails aprovados.

Proximo passo recomendado atual:

1. iniciar Sprint 33 — clima e rua viva como gameplay;
2. manter Sprint 30 como baseline de camera/HUD/chuva;
3. exportar primeiros props Krita usando `docs/STAGE_ASSET_PIPELINE.md`.

## Regra Obrigatoria De Sprint

Toda sprint deve seguir este fluxo:

1. implementar mantendo o jogo jogavel;
2. rodar `dotnet build SangueNoAsfalto.csproj`;
3. atualizar `README.md` e docs relevantes;
4. validar no Godot com `F5`;
5. so entao marcar como `concluida`;
6. commitar;
7. fazer push.

Se nao foi validada no Godot, a sprint deve ficar como `implementada / aguardando validacao`.

## Regras Para Trabalhar Em Outro PC

Antes de trocar de maquina:

1. salvar cenas no Godot;
2. fechar o jogo em execucao;
3. rodar build se tiver alterado C#;
4. atualizar `docs/SPRINTS.md` se mudou estado da sprint;
5. verificar `git status`;
6. commitar ou sincronizar os arquivos do projeto.

Ao chegar em outro PC:

1. abrir a pasta do projeto;
2. verificar se Godot .NET esta instalado;
3. rodar `dotnet --version`;
4. rodar `dotnet build SangueNoAsfalto.csproj`;
5. abrir Godot e testar `F5`.

## O Que Nao Deve Ser A Fonte Da Verdade

Nao depender apenas de:

- conversa do chat;
- memoria do agente;
- arquivos temporarios;
- imagens no clipboard;
- prints soltos.

A fonte da verdade deve ser:

- arquivos do projeto;
- documentos em `docs/`;
- referencias salvas em `references/`;
- documentos de sistema em `docs/`, especialmente `docs/WEATHER_TIME_SYSTEM.md`;
- historico Git quando estiver configurado corretamente.

## Git

Remoto: `https://github.com/matteusoberdan95-sys/rua-de-fogo.git` (branch `main`).

Rotina recomendada:

```powershell
git status
git add .
git commit -m "Describe the sprint change"
git push origin main
```

Nao commitar:

- `.godot/`
- `bin/`
- `obj/`
- backups `.old`
- exports temporarios.

## Regra Dos Agentes

Todo agente deve:

- ler `docs/PROJECT_BRIEF.md`;
- ler `docs/SPRINTS.md`;
- respeitar `docs/VISUAL_BIBLE.md`;
- atualizar docs quando mudar escopo, arquitetura ou direcao;
- manter o projeto jogavel sempre que possivel.

## Backend

Backend continua fora do escopo imediato.

So entra quando houver necessidade real, por exemplo:

- ranking;
- contas;
- telemetria;
- eventos online;
- cloud save proprio;
- ferramentas internas.

Enquanto o jogo esta em prototipo, saves locais e dados locais bastam.

