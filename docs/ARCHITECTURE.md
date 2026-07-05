# Arquitetura Inicial

## Estrutura

- `scenes/actors`: cenas reutilizaveis de jogador e inimigos.
- `scenes/levels`: mapas e arenas.
- `scripts/core`: sistemas genericos como vida.
- `scripts/combat`: hitbox, hurtbox e regras de dano.
- `scripts/player`: controle do jogador.
- `scripts/enemies`: controle e IA dos inimigos.
- `scripts/world`: sistemas futuros de clima, horario, rua viva e perigos ambientais.
- `scripts/systems`: save local, configuracoes e estado persistente.
- `scripts/pickups`: comportamento de pickups coletaveis.
- `scripts/ui`: HUD e telas.
- `art/placeholders`: arte temporaria.
- `art/concepts`: concept art e exploracoes visuais.
- `art/sprites`: sprites finais ou temporarios.
- `art/tiles`: tilesets e props de cenario.
- `art/ui`: HUD, icones e menus.
- `audio/sfx`: sons temporarios.
- `audio/music`: trilhas e loops.
- `references`: moodboards, PureRef e imagens de referencia.

## Regras de Codigo

- Um script por responsabilidade principal.
- Sistemas reutilizaveis ficam fora de scripts especificos de player ou inimigo.
- Primeiro validar o jogo rodando; depois refatorar onde a dor aparecer.
- Backend fica fora do escopo ate existir uma necessidade concreta.
- Clima/tempo deve ser implementado em camadas pequenas, com dados claros por fase.
- Imports C# comuns ficam centralizados em `GlobalUsings.cs`, organizado por camadas.
- Scripts de gameplay nao devem repetir `using` no topo quando o namespace ja estiver coberto pelos global usings.

## Divisao Godot E C#

- Cenas `.tscn` montam objetos, nodes, colisao, camera, HUD e composicao visual.
- Scripts `.cs` controlam comportamento, regras, combate, IA, estado do jogo e sistemas.
- Arte em PNG entra por `art/` e depois e conectada nas cenas do Godot.
- O editor Godot deve ser usado para blocagem, luz, colisao, spawn points e composicao visual.
- Codex pode editar `.tscn` diretamente quando for util, mas o fluxo normal de level design deve acontecer no editor.

## Sistemas Futuros De Mundo

Quando a Sprint de clima comecar, os scripts devem ficar em `scripts/world`.

Nomes planejados:

- `TimeOfDayController.cs`
- `WeatherController.cs`
- `StreetTensionController.cs`
- `WeatherZone.cs`
- `EnvironmentalHazard.cs`

Esses sistemas devem ser opcionais por cena. O prototipo top-down nao deve depender deles.

## Sprint 02 - Cena Lateral

Arquivos principais:

- `scenes/levels/SideScrollerPrototype.tscn`
- `scenes/actors/SideScrollerPlayer.tscn`
- `scenes/actors/SideScrollerEnemyGrunt.tscn`
- `scripts/player/SideScrollerPlayerController.cs`
- `scripts/enemies/SideScrollerEnemyController.cs`
- `scripts/core/SideScrollerDirector.cs`
- `scripts/ui/BeatEmUpHud.cs`

O top-down antigo permanece em `scenes/levels/PrototypeArena.tscn`.

## Sprint 06 - Vertical Slice Pequena

Arquivos principais:

- `scripts/core/SideScrollerDirector.cs`: fluxo curto de fase, checkpoint, mini-chefe e estados de morte/vitoria.
- `scripts/ui/BeatEmUpHud.cs`: status de etapa, objetivo, checkpoint, morte e vitoria.
- `scenes/actors/SideScrollerMiniBoss.tscn`: mini-chefe placeholder da primeira rua.
- `scenes/levels/SideScrollerPrototype.tscn`: cena principal da vertical slice.

Checkpoint atual:

- persiste em `user://save_game.json` a partir da Sprint 07;
- `R` volta ao checkpoint somente se o jogador ja tiver ativado o checkpoint e morrer;
- `F4` limpa o save local e reinicia do inicio.

## Sprint 07 - Sistemas De Jogo

Arquivos principais:

- `scripts/systems/GameSave.cs`: modelo serializado do save local.
- `scripts/systems/SaveManager.cs`: carrega, salva e limpa `user://save_game.json`.
- `scripts/pickups/Pickup.cs`: aplica cura, arma improvisada ou continue ao jogador.
- `scenes/pickups/*.tscn`: pickups placeholders da vertical slice.
- `GlobalUsings.cs`: imports globais separados por runtime, Godot, core, gameplay, sistemas, mundo e UI.

Limites atuais:

- save local ainda e simples e sem slots;
- arma improvisada tem apenas durabilidade e bonus de dano/knockback;
- continue e limitado a 1;
- configuracoes atuais sao de prototipo (`F1`, `F2`, `F4`), sem tela dedicada.

## Sprint 08 - Conteudo Alpha

Arquivos principais:

- `scenes/actors/SideScrollerEnemyFast.tscn`: inimigo rapido, baixa vida e alta velocidade.
- `scenes/actors/SideScrollerEnemyBrute.tscn`: inimigo bruto, mais vida e ataque pesado.
- `scenes/actors/SideScrollerEnemyInfected.tscn`: inimigo infectado/chuvoso, ritmo intermediario.
- `scenes/actors/SideScrollerRainMiniBoss.tscn`: segundo mini-chefe placeholder.
- `scenes/actors/SideScrollerAlphaBoss.tscn`: chefe alpha placeholder.
- `scripts/core/SideScrollerDirector.cs`: composicoes de encontro por tipo de inimigo.

Limites atuais:

- todos os inimigos ainda reutilizam `SideScrollerEnemyController`;
- ainda nao ha ataques exclusivos por tipo;
- chefe alpha e placeholder para ritmo/fim de fase, nao boss final definitivo;
- Sprint 08 ainda usa uma unica cena principal.

## Sprint 09 - Demo Publica

Arquivos principais:

- `scenes/ui/MainMenu.tscn`: cena principal da demo publica, com iniciar demo, limpar save, alternar controles e sair.
- `scripts/ui/MainMenu.cs`: comportamento do menu e integracao com `SaveManager`/`InputBootstrap`.
- `scripts/ui/BeatEmUpHud.cs`: tutorial discreto por etapa e overlays de morte/fim da demo.
- `scripts/core/SideScrollerDirector.cs`: retorno ao menu com `M` nos estados finais.
- `export_presets.cfg`: preset inicial `Windows Desktop`.
- `docs/BUILD_WINDOWS.md`: guia de export Windows.

Fluxo atual:

- `project.godot` inicia em `res://scenes/ui/MainMenu.tscn`;
- o menu carrega `res://scenes/levels/SideScrollerPrototype.tscn`;
- a vertical slice continua sendo a fase unica da demo publica;
- configuracoes persistentes continuam centralizadas no save local.

## Sprint 10 - Identidade Visual Pillars

Arquivos principais:

- `scripts/ui/BeatEmUpHud.cs`: HUD reorganizado com barras, combo, furia, banner e slot de arma.
- `scripts/player/SideScrollerPlayerController.cs`: combo de hits, melhor combo e furia.
- `scripts/combat/Hurtbox.cs`: registra impactos do jogador para HUD/combo.
- `scripts/core/SideScrollerDirector.cs`: titulo/tagline da etapa.
- `scenes/levels/SideScrollerPrototype.tscn`: boteco, ponto final, props e labels da Vila Esperanca.
- `scenes/actors/SideScrollerPlayer.tscn`: silhueta/cores do Caua aproximadas da referencia.

Referencia visual:

- usar `references/pillars/` como alvo de leitura para HUD, cenario, personagem e clima.

## Sprint 11 - Producao Steam

Arquivos principais:

- `docs/STEAM_PAGE.md`: rascunho de pagina Steam.
- `docs/SCREENSHOTS_STEAM.md`: lista de capturas para marketing.
- `docs/QC_DEMO_CHECKLIST.md`: checklist antes de distribuir build.
- `docs/DEMO_PACKAGE.md` e `build/demo/README.txt`: pacote para testers.
- `docs/LAUNCH_PLAN.md`: fases e preco sugerido.
- `docs/TRAILER_INTERNAL.md`: roteiro curto interno.
- `scripts/systems/DemoInfo.cs`: versao da demo.
- `scripts/ui/ScreenshotModeHelper.cs`: modo screenshot com `F9`.
- `scripts/build-demo.ps1`: build/export helper Windows.

## Observacao Sobre C#

O arquivo `.csproj` usa `Godot.NET.Sdk/4.4.1` como ponto de partida. Caso a versao instalada do Godot .NET seja outra, o editor pode atualizar esse valor automaticamente.

## Sprint 14 - Integracao Visual

Arquivos principais:

- `tools/normalize-sprites.ps1`: gera sprites `_game.png` com alpha e frame padronizado.
- `scripts/visual/CharacterSpriteVisual.cs`: controla escala, flip, pulo, locomocao e flash visual.
- `scenes/actors/SideScrollerPlayer.tscn`: usa `caua_idle_game.png` e `caua_walk_sheet_game.png`.
- `scenes/actors/SideScrollerEnemyGrunt.tscn`: usa `grunt_idle_game.png` e `SourceFacesRight = false`.

Regra: referencias em `references/` nao devem ser ligadas direto ao jogo. Primeiro precisam virar assets em `art/` com transparencia, escala e pivot corretos.
