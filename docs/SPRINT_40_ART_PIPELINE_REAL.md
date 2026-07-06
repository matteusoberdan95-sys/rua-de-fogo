# Sprint 40 - Art Pipeline Real

Status: base tecnica implementada / build validado / aguardando F5 e sprite sheet real.

Data: 06/07/2026.

## Decisao De Direcao

O rig procedural em `Polygon2D` fica como laboratorio de gameplay, prototipagem e hitbox. Ele nao e mais tratado como caminho de arte final vendavel.

A partir desta sprint, a meta visual do jogo passa a ser:

- personagens com arte real, desenhados em camadas ou sprite sheets;
- cenarios pintados/modulares em camadas;
- luz, sombra, chuva, reflexo e particulas no Godot por cima da arte;
- referencias em `references/` continuam como moodboard, mas a pasta `art/production/` passa a guardar os assets de producao.

## Problema Validado

Playtest da Sprint 39:

- melhorou pouco;
- personagens continuam com visual rigido e blocado;
- animacoes ainda nao vendem vida, peso ou qualidade comercial;
- cenario tem composicao, mas ainda parece prototipo em varios trechos.

Conclusao: insistir em polir `CharacterSpriteVisual` por codigo tera retorno baixo. O proximo salto precisa vir de pipeline de arte.

## Objetivo Da Sprint

Criar a primeira trilha real de producao visual sem quebrar a demo:

1. manter `CharacterSpriteVisual` como fallback jogavel;
2. definir formato de personagem final;
3. definir formato de cenario final;
4. preparar pastas, guias e checklist para importar assets reais no Godot;
5. planejar a troca gradual: primeiro Caua, depois 1 inimigo, depois 1 trecho da Vila Esperanca.

## Escolha Recomendada

Para o nivel visual desejado, seguir uma destas duas rotas:

### Rota A - Sprite Sheet Frame A Frame

Melhor para visual classico 2D, mais proximo de beat 'em up dos anos 90 com pintura moderna.

Entrega minima:

- `idle`, `walk`, `run`, `jab`, `cross`, `kick`, `hurt`, `death`;
- sprites com fundo transparente;
- pivot no pe/chao;
- hitbox continua vindo do controlador atual.

### Rota B - Rig 2D Com Partes Desenhadas

Melhor para animacao fluida com menos frames desenhados.

Entrega minima:

- personagem separado em cabeca, tronco, braco, antebraco, mao, coxa, canela, pe, cabelo/roupa;
- animado via `Skeleton2D`, `Bone2D`, `AnimationPlayer` ou um rig proprio em nodes;
- permite cabelo/roupa reagindo a movimento.

## Decisao Para Agora

Comecar pela Rota A para validar qualidade visual rapido:

- 1 sprite sheet do Caua em baixa quantidade de frames;
- 1 sprite sheet de inimigo basico;
- 1 trecho curto de cenario pintado em camadas.

Depois, se a animacao frame a frame ficar cara demais, migramos personagens para Rota B.

## Estrutura De Pastas

- `art/production/characters/caua/`
- `art/production/characters/enemies/quebra-osso/`
- `art/production/stages/vila-esperanca/`
- `art/production/props/`
- `art/production/fx/`
- `art/production/_exports/`

## Regras De Asset

Personagem:

- PNG transparente;
- escala consistente;
- pe sempre no mesmo baseline;
- olhar/facing para direita por padrao;
- nome: `caua_idle_0001.png`, `caua_walk_0001.png`, etc.;
- cada animacao deve ter um arquivo `notes.md` com fps, pivot e observacoes.

Cenario:

- separar em camadas: `sky`, `far`, `mid`, `street`, `foreground`, `lights`, `weather_masks`;
- exportar sem HUD;
- manter largura modular para repetir/encaixar;
- sombras pintadas na arte, luz dinamica no Godot por cima.

## Implementacao Tecnica (Sprint 40 — base entregue)

- `ArtCharacterVisual.cs` — `AnimatedSprite2D` + `SpriteFrames` de `art/production/`;
- `ProductionArtCatalog.cs` — resolve caminhos por personagem;
- `IActorVisual.cs` + `ActorVisualResolver.cs` — contrato compartilhado e fallback;
- flag `UseProductionArt` em player/inimigos;
- fallback automatico para `CharacterSpriteVisual` se pacote real nao existir;
- Caua v0 placeholder: `art/production/characters/caua/caua_v0_frames.tres`;
- teste: `scenes/tests/CauaProductionArtTest.tscn`.

## Implementacao Tecnica Planejada (proximas tarefas)

- import preset Godot para pixel/painted 2D sem blur;
- substituir placeholder v0 por sprite sheet real do Caua;
- ativar `UseProductionArt = true` na fase principal apos validacao visual.

## Criterio De Sucesso

A Sprint 40 so deve ser considerada concluida quando:

- houver ao menos uma imagem real de Caua importada e visivel no Godot;
- a demo ainda rodar com o rig antigo se o asset real falhar;
- README e docs indicarem como continuar no Cursor CLI;
- `dotnet build SangueNoAsfalto.csproj` passar;
- commit e push feitos.

## Proximas Sprints

- Sprint 41 - Caua Production Art v0: primeiro `AnimatedSprite2D` jogavel.
- Sprint 42 - Inimigo Basico Production Art v0: Quebra-Osso com idle/walk/attack/hurt.
- Sprint 43 - Vila Esperanca Painted Blockout: cenario real em camadas.
- Sprint 44 - Lighting Pass: sombras, neon, chuva, reflexo e pos-processamento.
- Sprint 45 - Vertical Slice Visual: 1 minuto bonito, jogavel e apresentavel.
