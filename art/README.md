# Arte — Sprint 13 (placeholders)

Estilo: **pintura 2D com pixel aparente** (Krita → PNG → Godot com filtro Nearest).

## Estrutura

| Pasta | Conteudo |
|-------|----------|
| `sprites/player/` | Cauã — idle, walk sheet |
| `sprites/enemies/` | Grunt comum (Quebra-Osso) |
| `backgrounds/vila-esperanca/` | Parallax far / mid / near |

## Substituicao no Krita

1. Canvas ~128–160 px de altura do personagem (escala up no export).
2. Walk cycle: sheet horizontal, 4–6 frames, mesma largura por frame.
3. Pivot nos pes — alinhar com `LaneShadow` na cena.
4. Export PNG; manter nomes de arquivo para nao quebrar `.tscn`.
5. Godot reimporta automaticamente; `texture_filter = Nearest` ja configurado.

Os PNGs atuais sao placeholders gerados para testar pipeline — substituir por arte final desenhada a mao.
