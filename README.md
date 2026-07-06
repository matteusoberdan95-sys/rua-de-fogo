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

**Sprint atual:** `Sprint 29 — Polimento Visual da Fase 1` (implementada — aguardando validacao no Godot).

**Proxima:** validar a Sprint 29 no Godot e decidir se a Sprint 30 foca em HUD/camera ou sprites finais.

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

### Regra visual

Imagens em `art/` e `references/` sao **referencia**. O runtime usa **rig 2D em camadas** (`UseLayeredPrototype = true`).

### Regra de sprint

Toda sprint so e concluida apos: `dotnet build` sem erros, validacao F5 no Godot, docs atualizados, commit e push.

## Documentos importantes

| Documento | Conteudo |
|-----------|----------|
| `docs/SPRINTS.md` | Planejamento e sprint atual |
| `docs/HANDOFF.md` | Continuar em outro PC / Codex / CLI |
| `docs/STAGE_01_VILA_ESPERANCA.md` | Plano da Fase 1 |
| `docs/COMBAT_DESIGN.md` | Combate, armas, progressao marcial |
| `docs/VISUAL_BIBLE.md` | Direcao visual |
| `docs/ARCHITECTURE.md` | Arquitetura de codigo |
| `docs/BACKLOG.md` | Marcos e checklist |
| `docs/AGENTS.md` | Responsabilidades dos agentes |
