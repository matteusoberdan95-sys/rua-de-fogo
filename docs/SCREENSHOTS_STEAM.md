# Screenshots Steam

Resolucao recomendada: `1920x1080` ou `1280x720`.

Antes de capturar:

1. Rode `dotnet build SangueNoAsfalto.csproj`.
2. Abra o projeto no Godot e pressione `F5`.
3. Ative o modo screenshot com `F9` para esconder tutorial, controles e debug.
4. Use captura nativa do Windows ou ferramenta externa.

Salvar arquivos em:

```text
marketing/screenshots/steam/
```

## Lista obrigatoria (demo atual)

| # | Cena | Objetivo | Referencia pilar |
|---|---|---|---|
| 1 | Menu inicial | Mostrar titulo e demo publica | identidade geral |
| 2 | Entrada da Vila Esperanca | Boteco, rua molhada, HUD arcade | `02-stage-vila-esperanca-side-scroller.png` |
| 3 | Combate com combo alto | Callout `BRUTAL`/`VICIOUS` visivel | `04-hud-combat-ui-weapons.png` |
| 4 | Chuva/tempestade | Gotas, neblina, reflexos no asfalto | `07-weather-time-system.png` |
| 5 | Checkpoint do altar | Progressao e respawn persistente | vertical slice |
| 6 | Mini-chefe ou chefe alpha | Escala, telegraph e tensao | `06-bosses-corruption-network.png` |
| 7 | Tela de vitoria da demo | Fim do trecho alpha | demo publica |

## Dicas de enquadramento

- Priorize contraste entre Caua, inimigos e fundo.
- Evite HUD debug (`F1`) nas capturas finais.
- Prefira noite/chuva para 3 das 7 imagens; use manha/tarde para variar.
- Mantenha sangue e impacto visiveis, mas legiveis.
- Centralize o banner `VILA ESPERANCA` quando fizer sentido.

## Nomes sugeridos

- `01-menu.png`
- `02-vila-esperanca-rua.png`
- `03-combo-brutal.png`
- `04-chuva-noturna.png`
- `05-checkpoint-altar.png`
- `06-chefe-alpha.png`
- `07-vitoria-demo.png`

## Trailer interno

O roteiro curto de trailer fica em `docs/TRAILER_INTERNAL.md`.
