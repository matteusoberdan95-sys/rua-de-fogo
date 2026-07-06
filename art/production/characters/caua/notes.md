# Caua - Production Art Notes

Versao atual: **v1** (sprites jogaveis na fase lateral).

## Pacote minimo entregue

| Animacao | Frames | Arquivos |
|----------|--------|----------|
| idle | 4 | `caua_idle_01.png` … `04` |
| walk | 6 | `caua_walk_01.png` … `06` |
| run | 4 | `caua_run_01.png` … `04` |
| jab | 3 | `caua_jab_01.png` … `03` |
| cross | 3 | `caua_cross_01.png` … `03` |
| kick | 3 | `caua_kick_01.png` … `03` |
| hurt | 2 | `caua_hurt_01.png` … `02` |
| death | 1 | `caua_death_01.png` |

## Regenerar placeholders v1

```powershell
powershell -File tools/generate-caua-production-v1.ps1
```

## Substituir por arte pintada

1. Exporte PNGs com **mesmos nomes** e baseline no pe (Y=132, canvas 112x144).
2. Coloque em `sprites/`.
3. Ajuste `manifest.json` se mudar fps ou contagem de frames.
4. F5 na fase ou `scenes/tests/CauaProductionArtTest.tscn`.

## Runtime

- `SideScrollerPlayer`: `UseProductionArt = true`
- Fallback: rig procedural se pasta/sprites sumirem
