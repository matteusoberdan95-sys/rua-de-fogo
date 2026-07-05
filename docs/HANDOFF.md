# Handoff - Continuar Em Codex, CLI Ou Outro PC

Este documento existe para manter o projeto continuavel mesmo se trocar de computador, abrir pelo Codex Desktop, Codex CLI, Cursor/Cursor CLI ou outro fluxo.

## Onde Estamos

Projeto: `Sangue no Asfalto`

Engine: Godot 4.7 .NET

Linguagem: C#

Direcao atual: prototipo lateral/2.5D beat 'em up implementado para validacao, com o top-down mantido como laboratorio antigo.

Sprint atual: ver `docs/SPRINTS.md`.

## Arquivos Que Devem Ser Lidos Primeiro

Ao abrir o projeto em outro ambiente, leia nesta ordem:

1. `README.md`
2. `docs/PROJECT_BRIEF.md`
3. `docs/SPRINTS.md`
4. `docs/VISUAL_BIBLE.md`
5. `docs/AGENTS.md`
6. `docs/WEATHER_TIME_SYSTEM.md`
7. `docs/ARCHITECTURE.md`
8. `docs/BACKLOG.md`

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
- `J` ou botao esquerdo: combo/ataque
- `L` ou botao direito: tiro
- `K` ou `Espaco`: esquiva
- `R`: reiniciar

## Cenas Atuais

- `scenes/levels/PrototypeArena.tscn`: prototipo top-down/arena.
- `scenes/levels/SideScrollerPrototype.tscn`: prototipo lateral/2.5D atual e cena principal do `F5`.
- `scenes/actors/Player.tscn`: jogador atual.
- `scenes/actors/EnemyGrunt.tscn`: inimigo comum.
- `scenes/actors/SideScrollerPlayer.tscn`: jogador lateral.
- `scenes/actors/SideScrollerEnemyGrunt.tscn`: inimigo lateral.
- `scenes/actors/PlayerProjectile.tscn`: tiro do jogador.

## Cena Principal Atual

Cena que roda com `F5`:

```text
scenes/levels/SideScrollerPrototype.tscn
```

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

O projeto deve ser mantido em Git para permitir continuidade entre maquinas.

Rotina recomendada:

```powershell
git status
git add .
git commit -m "Describe the sprint change"
```

Ainda falta conectar este repositorio local a um remoto, por exemplo GitHub, GitLab, Azure DevOps ou outro repositorio privado.

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
