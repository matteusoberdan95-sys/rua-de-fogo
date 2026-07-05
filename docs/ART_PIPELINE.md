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
- `art/sprites/`: personagens, inimigos, projeteis e efeitos.
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

Tiles:

- PNG em grade regular.
- Tamanho inicial sugerido: `32x32`, `48x48` ou `64x64`.

Efeitos:

- PNG separado ou particulas no Godot.
- Sangue deve ter variacoes pequenas para nao repetir demais.
