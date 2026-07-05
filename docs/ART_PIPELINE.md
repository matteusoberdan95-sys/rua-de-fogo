# Pipeline De Arte

## Fluxo Recomendado

1. Criar referencias no PureRef.
2. Consolidar ideias na `VISUAL_BIBLE.md`.
3. Gerar concept art ou sketch.
4. Criar sprites/tiles finais em Krita ou Aseprite.
5. Exportar PNGs para `art/`.
6. Montar cenas no Godot.
7. Ajustar colisao, luz e feedback jogando.

## Pastas

- `references/`: moodboards, prints e estudos visuais.
- `art/concepts/`: concept art e exploracoes.
- `art/sprites/player/`: Cauã — idle, walk sheet, attack (Krita)
- `art/sprites/enemies/`: grunt e variantes
- `art/backgrounds/vila-esperanca/`: camadas parallax (far, mid, near)
- `art/tiles/`: chao, parede, props e tilesets.
- `art/ui/`: HUD, icones e menus.
- `audio/sfx/`: efeitos sonoros.
- `audio/music/`: trilhas e loops.

## PureRef

Use o PureRef para juntar imagens de referencia. Ele nao cria assets; ele organiza a direcao visual.

Board inicial sugerido:

- ruas suburbanas brasileiras;
- asfalto molhado;
- postes amarelos;
- muros pichados;
- boteco fechado;
- personagens urbanos sombrios;
- monstros deformados;
- HUDs de jogos de acao 2D;
- paletas de vermelho, amarelo, concreto e preto.

## Krita

**Ferramenta principal para este projeto** (estilo pintura 2D + pixel aparente).

Melhor para:

- concept art;
- pintura de cenario;
- retratos;
- splash art;
- estudos de luz;
- sprites desenhados a mao.

## Aseprite

Melhor para:

- pixel art;
- animacoes frame a frame;
- sprite sheets;
- tilesets;
- paletas reduzidas.

## Godot

Melhor para:

- montar cenas;
- posicionar props;
- configurar colisao;
- testar escala;
- adicionar luz 2D;
- organizar spawns;
- criar prefabs/cenas reutilizaveis.

## Padrao De Exportacao

Personagens:

- PNG com fundo transparente.
- Sprite sheet quando houver animacao.
- Pivot consistente nos pes.
- Frame padrao atual para prototipo: `384x1024`, personagem alinhado pelos pes.
- Nome de arquivo de jogo pode usar sufixo `_game.png` quando for asset recortado/normalizado.

Tiles:

- PNG em grade regular.
- Tamanho inicial sugerido: `32x32`, `48x48` ou `64x64`.

Efeitos:

- PNG separado ou particulas no Godot.
- Sangue deve ter variacoes pequenas para nao repetir demais.

## Normalizacao Temporaria De Sprites

A Sprint 14 adicionou uma ferramenta para corrigir assets de referencia que vieram com fundo preto ou enquadramento inconsistente:

```powershell
powershell -ExecutionPolicy Bypass -File tools/normalize-sprites.ps1
```

Ela gera:

- `art/sprites/player/caua_idle_game.png`
- `art/sprites/player/caua_walk_sheet_game.png`
- `art/sprites/enemies/grunt_idle_game.png`

Isso nao substitui o trabalho final no Krita. E uma ponte para o jogo parar de parecer uma colagem com fundo preto enquanto desenhamos sprites finais.

## Extracao Temporaria De Pranchas

Quando uma referencia vier como uma prancha grande com varios personagens/poses, use:

```powershell
powershell -ExecutionPolicy Bypass -File tools/extract-reference-assets.ps1
```

Na Sprint 14 esse script recorta o Caua da primeira prancha em `references/personagens_ref/` e gera:

- `art/sprites/player/caua_ref_idle.png`
- `art/sprites/player/caua_ref_walk_sheet.png`
- `art/sprites/player/caua_ref_attack.png`

Esses arquivos sao placeholders de integracao. A arte final comercial deve ser redesenhada/exportada como sprite sheet limpa, transparente e sem textos/grades da prancha.
