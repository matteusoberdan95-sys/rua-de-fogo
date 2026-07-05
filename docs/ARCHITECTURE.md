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

## Observacao Sobre C#

O arquivo `.csproj` usa `Godot.NET.Sdk/4.4.1` como ponto de partida. Caso a versao instalada do Godot .NET seja outra, o editor pode atualizar esse valor automaticamente.
