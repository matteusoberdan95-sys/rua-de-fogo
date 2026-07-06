# Sangue no Asfalto

Prototipo alpha de beat 'em up / hack and slash dark fantasy em cenario suburbano brasileiro, feito em **Godot 4.7 .NET** com **C#**.

## Como abrir

1. Instale a versao **.NET** do Godot 4.7.
2. Abra esta pasta pelo Godot.
3. Se o editor pedir para atualizar o `Godot.NET.Sdk`, aceite ou ajuste `SangueNoAsfalto.csproj`.
4. Execute com **F5**. A cena principal e `res://scenes/ui/MainMenu.tscn` (menu → tutorial → fase lateral).

Validacao rapida no terminal:

```powershell
dotnet build SangueNoAsfalto.csproj
```

## Controles (fase lateral — `SideScrollerPrototype`)

| Tecla | Acao |
|-------|------|
| `A/D` ou setas | Mover pela rua |
| `W/S` ou setas cima/baixo | Trocar lane / profundidade |
| Duplo-tap `A` ou `D` | Correr |
| `J` ou botao esquerdo | Combo de ataque (4 golpes por estilo marcial) |
| `L` ou botao direito | Pistola (municao limitada, sangramento) |
| `E` | Recarregar pistola |
| `K` | Esquiva / dash |
| `Espaco` | Pulo (voadora com `J` no ar) |
| **Segurar `Q`** | Defender (chip de dano, empurra para tras, enche postura) |
| **Toque rapido `Q`** | Parry no timing ou no `! PARRY !` do inimigo |
| `R` | Reiniciar |
| `M` | Voltar ao menu (morte/vitoria) |
| `F1` | HUD debug |
| `F9` | Modo screenshot |

## Estado atual (Jul/2026)

**Sprint atual:** Sprint 40 - Art Pipeline Real (iniciada / build validado).

**Nota de continuidade:** Sprint 39 confirmou o limite do rig procedural como visual final. O rig em `Polygon2D` continua como laboratorio de gameplay, mas o caminho vendavel agora e `art/production/`: personagens e cenarios reais, importados gradualmente.

**Ultima concluida:** Sprint 34 — vertical slice v1.1 e QC validados no Godot.

**Marco atual:** demo compartilhavel Vila Esperanca (~10 min) — sprints 29–34 fechadas.

### O que ja funciona

- **Fase lateral ~10 min** na Vila Esperanca: spawn por progresso, chefes, portao SAIDA.
- **Rig 2D em camadas** (`CharacterSpriteVisual`, `EnemyLayeredVisual`) — personagens animados por codigo, sem sprites colados no gameplay.
- **Anatomia segmentada:** pernas com joelho, bracos com cotovelo, walk/run com passada.
- **Dano progressivo:** olho roxo, sangue no nariz, rasgo na roupa.
- **Combate arcade:** combo 4 golpes, buffer/cancel, corrida+J, dash+J.
- **Estilos marciais por XP:** Rua, Boxe, Muay Thai, Karate, Capoeira, Jiu-Jitsu (`MoveCatalog`, `CombatStyleCatalog`).
- **Armas improvisadas** no chao (vergalhao, martelo, faca) + finishers gore.
- **Postura / parry estilo Sekiro:** segurar Q bloqueia; toque Q no telegraph = parry + contra brutal.
- **Defesa com peso:** bloqueio empurra o jogador, postura quebra em ~3 hits fortes, camera shake no impacto.
- **Props quebraveis** (`BreakableStageProp`): caixote, cerca, lixo, placa.
- **Clima e horario** dinamicos, chuva, rua em camadas, HUD arcade.
- **Sprint 29:** primeira passada de producao visual na Vila Esperanca: lojas, lixeira, fios, roupas no varal, caixa eletrica, pichacoes, asfalto remendado, buracos, lixo, cacos, marcas de rua e atmosfera no trecho inteiro.
- **Correcao visual Sprint 29:** camadas antigas `Road`/`LaneBand`/`NightVignette` foram escondidas e a rua nova teve `ZIndex` corrigido para aparecer no gameplay.
- **Sprint 30 concluida:** HUD compacto, camera mais aberta, chuva/vinheta menos invasivas e contorno sutil de leitura nos personagens, validada no Godot.
- **Sprint 31 concluida:** personagens expressivos, telegraph/morte legiveis, `CombatPacing` (combate mais lento e com peso), validada no Godot.
- **Sprint 32 concluida:** `StageAssetLibrary`, landmarks por ato e pipeline de assets, validada no Godot.
- **Sprint 33 concluida:** clima por ato, pocas, vento, apagao nos chefes, validada no Godot.
- **Sprint 39 implementada:** proporcoes menos cabecudas, membros mais longos, caminhada com passada maior e jab/cross/side kick preservando extensao real do braco/perna.
- **Sprint 40 iniciada:** pipeline de arte real, pasta `art/production/`, regra visual revisada e plano para trocar Caua/inimigos/cenario por assets reais.
- **Sprint 34 concluida:** vertical slice v1.1, QC, tutorial/HUD/F9, pacote testers — validada no Godot.

### Regra visual (obrigatoria)

**`references/` NAO entra no gameplay.** E moodboard. **`art/production/` e o caminho oficial para assets reais** a partir da Sprint 40. Detalhes: **`docs/VISUAL_RULE.md`**.

Runtime atual: `UseLayeredPrototype = true` em todos os atores, ate a entrada gradual de production art com fallback procedural.

### Regra de sprint

Toda sprint so e concluida apos: `dotnet build` sem erros, validacao F5 no Godot, docs atualizados, commit e push.

## Documentos importantes

| Documento | Conteudo |
|-----------|----------|
| `docs/VISUAL_RULE.md` | **Regra: referencias NAO viram PNG no jogo** |
| `docs/SPRINTS.md` | Planejamento e sprint atual |
| `docs/HANDOFF.md` | Continuar em outro PC / Codex / CLI |
| `docs/STAGE_01_VILA_ESPERANCA.md` | Plano da Fase 1 |
| `docs/STAGE_ASSET_PIPELINE.md` | Pipeline Krita → Godot para cenario |
| `docs/VERTICAL_SLICE_QC.md` | Checklist QC da vertical slice |
| `docs/COMBAT_DESIGN.md` | Combate, armas, progressao marcial |
| `docs/VISUAL_BIBLE.md` | Direcao visual |
| `docs/ARCHITECTURE.md` | Arquitetura de codigo |
| `docs/CHARACTER_RIG_PLAN.md` | Rig procedural vs arte final dos personagens |
| `docs/SPRINT_39_ANATOMIA_POSES.md` | Correcao visual de anatomia, membros e poses |
| `docs/SPRINT_40_ART_PIPELINE_REAL.md` | Pipeline real de arte para personagem/cenario vendavel |
| `art/production/README.md` | Estrutura oficial para assets reais |
| `docs/BACKLOG.md` | Marcos e checklist |
| `docs/AGENTS.md` | Responsabilidades dos agentes |
