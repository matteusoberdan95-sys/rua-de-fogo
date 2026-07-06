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

## Contrato Do Caua Final

O recorte de prancha nao e arte final. A Sprint 15 adicionou um rig em camadas para provar vida no personagem, mas a versao comercial do Caua precisa ser uma sprite sheet limpa.

Estados minimos:

- `idle`: 6 a 8 frames, respiracao visivel, cabelo/roupa com micro movimento;
- `walk`: 8 frames, peso nos passos, arma com atraso leve;
- `attack_1`: golpe rapido;
- `attack_2`: golpe medio, mais aberto;
- `attack_3`: golpe pesado com antecipacao e follow-through;
- `hurt`: 2 a 4 frames;
- `dodge`: 4 a 6 frames;
- `jump`: 4 a 6 frames ou pose unica com squash/stretch por codigo;
- `low_health`: idle cansado/machucado;
- `death`: queda/execucao curta.

Padrao tecnico:

- PNG transparente;
- sem texto, grade, moldura ou fundo;
- pivot sempre nos pes;
- personagem cabendo no frame `384x1024` enquanto estivermos nesse prototipo;
- todos os frames alinhados pelo chao;
- filtro `Nearest` no Godot;
- uma sheet por familia quando ficar grande: `caua_idle_walk.png`, `caua_combat.png`, `caua_damage.png`.

Camadas recomendadas no Krita/Aseprite:

- corpo/base;
- camisa/jaqueta;
- cabeca;
- cabelo;
- bracos;
- pernas;
- arma;
- sangue/machucados;
- sombra separada ou controlada no Godot.

O rig `UseLayeredPrototype` em `CharacterSpriteVisual` serve como guia de movimento: respiracao, cabelo, torso, bracos, pernas e faca devem continuar existindo visualmente quando a sprite sheet final substituir o prototipo.

## Contrato Do Cenario Final

As imagens de referencia de cenario nao devem ser usadas como tela chapada no jogo final. A Sprint 16 criou `LayeredStreetPrototype` para provar o caminho certo: a rua precisa ser feita de partes.

Camadas minimas por fase:

- ceu/luz de horario;
- favela/morro distante;
- predios e janelas;
- fios;
- paredes/muros;
- lojas/boteco/placas;
- calcada;
- asfalto;
- poças/reflexos;
- lixo/props;
- sangue/manchas;
- luzes/neon/postes;
- efeitos de clima por cima.

Cada camada deve poder:

- receber animacao leve;
- reagir a horario/clima;
- ser substituida por arte final sem mexer no gameplay;
- ter leitura separada de colisao.

Regra: uma imagem grande pode existir como referencia ou mockup, mas a cena ativa deve preferir nodes, tiles, sprites recortados limpos ou layers exportadas separadamente.

## Sprint 29 - Ponte Para Arte Final Da Fase 1

`LayeredStreetPrototype` agora funciona como blocagem visual rica da Vila Esperanca: lojas, pichacoes, fios, varal, lixo, buracos, poças e marcas de rua. Cada elemento deve ser tratado como alvo futuro de arte final:

- trocar `ColorRect`/`Polygon2D` por sprite/tile limpo quando houver asset;
- manter escala, posicao e camada se o gameplay estiver bom;
- nao substituir a rua por uma unica imagem pintada;
- validar primeiro no Godot se a leitura melhorou antes de desenhar asset definitivo.
