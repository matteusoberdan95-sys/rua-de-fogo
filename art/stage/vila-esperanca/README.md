# Assets de cenario — Vila Esperanca

Pasta alvo para sprites/tiles finais da Fase 1 (Sprint 32+).

## Estrutura

```
art/stage/vila-esperanca/
  props/       — boteco, poste, lixo, viatura, portao…
  tiles/       — calcada, asfalto, muro
  landmarks/   — composicoes por ato (opcional, se exportar pronto)
```

## Contrato

- PNG transparente, filtro **Nearest** no Godot
- Pivot no chao (props) ou canto superior esquerdo (tiles)
- Ver lista de prioridade em `docs/STAGE_ASSET_PIPELINE.md`

## Substituicao

Enquanto nao houver arte final, o jogo usa `StageAssetLibrary` (procedural).
Substituir um arquivo por vez e testar F5 antes do proximo.
