# Pipeline De Assets — Fase 1 Vila Esperanca

Documento da **Sprint 32**. Define como sair da blocagem procedural (`LayeredStreetPrototype`) para sprites/tiles finais sem quebrar gameplay.

## Principio

1. **Runtime ativo** = nodes/camadas montados por codigo (`StageAssetLibrary`, `StageActLandmarks`).
2. **Referencia** = `references/pillars/02-stage-vila-esperanca-side-scroller.png` e pranchas em `references/cenarios/`.
3. **Arte final** = PNGs transparentes em `art/stage/vila-esperanca/`, substituindo um prop por vez.
4. **Nunca** colar a prancha inteira como fundo unico.

## Prioridade de export (ordem Krita/Aseprite)

| Prioridade | Prop | Arquivo alvo | Tamanho sugerido | Notas |
|------------|------|--------------|------------------|-------|
| 1 | Boteco do Ze (fachada) | `boteco_ze.png` | ~460×204 | Neon separado em layer |
| 2 | Poste + pool de luz | `poste_luz.png` | ~180×280 | Poste amarelo `#D6A13A` |
| 3 | Calcada tile | `calcada_tile.png` | 86×28 | Tile repetivel |
| 4 | Asfalto patch | `asfalto_patch.png` | 130×90 | Overlay sem tile unico |
| 5 | Poca molhada | `poca_a.png`, `poca_b.png` | ~240×40 | 2 variantes |
| 6 | Lixo/saco | `lixo_cluster.png` | ~90×50 | |
| 7 | Cerca madeira | `cerca_plank.png` | ~24×140 | 1 tábua, repetir no Godot |
| 8 | Mercadinho fechado | `mercadinho_shutter.png` | ~380×176 | |
| 9 | Portao SAIDA | `portao_saida.png` | ~156×228 | Neon verde |
| 10 | Viatura / carro | `viatura.png`, `carro_quebrado.png` | ~200×60 | Props near layer |

## Landmarks por ato (nao repetir visual)

Trechos em `docs/STAGE_01_VILA_ESPERANCA.md` — implementados em `StageActLandmarks.cs`:

| X | Ato | Landmark principal |
|---|-----|-------------------|
| 0–480 | Entrada | Arco VILA ESPERANCA + Boteco do Ze |
| 480–1180 | Barraco Martins | Checkpoint + ponto onibus |
| 1180–2050 | Rua central | Viatura + Oficina + poste piscando |
| 2050–2780 | Viela estreita | Parede comprimida + pneus + cerca quebrada |
| 2780–3090 | Portao SAIDA | Neblina + altar boss + graffiti |

## Integracao no Godot

1. Exportar PNG com fundo transparente, filtro **Nearest**.
2. Criar `Sprite2D` ou `AnimatedSprite2D` na mesma posicao do prop procedural.
3. Desligar/ocultar o node procedural equivalente.
4. Manter colisao separada (`StaticBody2D` / `BreakableStageProp`).
5. Validar com F5: camera Sprint 30, leitura de combate Sprint 31.

## Codigo relacionado

| Arquivo | Funcao |
|---------|--------|
| `scripts/world/LayeredStreetPrototype.cs` | Monta a rua inteira |
| `scripts/world/StageAssetLibrary.cs` | Props reutilizaveis (boteco, poste, lixo…) |
| `scripts/world/StageActLandmarks.cs` | Composicoes por ato |
| `scripts/world/StageAssetContext.cs` | Neon, vento, reflexos, flicker |

## Ferramenta de pastas

```powershell
powershell -ExecutionPolicy Bypass -File tools/setup-stage-art-folders.ps1
```

Cria estrutura em `art/stage/vila-esperanca/` (props, tiles, landmarks).

## Criterio de pronto (Sprint 32)

- [x] Biblioteca de props separada e reutilizavel
- [x] Landmarks distintos nos 5 atos da fase
- [x] Chao com guia, meio-fio e faixa central
- [x] Postes com cor oficial e pool de luz
- [x] Pipeline documentado para export Krita → Godot
- [ ] Validacao F5 no Godot (aguardando playtest)
