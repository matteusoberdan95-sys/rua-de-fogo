# Guia Godot Para Iniciante

Este guia e para trabalhar neste projeto sem precisar dominar Godot inteiro.

## Ideia Principal

Godot funciona com:

- `Node`: um objeto basico.
- `Scene`: um conjunto de nodes salvo em arquivo `.tscn`.
- `Script`: comportamento anexado a um node.

Comparando com Unity:

- Scene do Godot parece uma cena/prefab da Unity.
- Node do Godot parece um GameObject com uma funcao mais especifica.
- Script C# do Godot parece um MonoBehaviour.
- Inspector e onde voce ajusta propriedades.

## O Que Voce Precisa Aprender Primeiro

Nao precisa aprender tudo. Para nosso jogo, comece por estes pontos:

1. Abrir a cena principal.
2. Selecionar nodes na arvore.
3. Mover objetos no viewport 2D.
4. Duplicar objetos.
5. Editar colisao.
6. Adicionar Marker2D para spawn.
7. Salvar a cena.
8. Rodar com F5.

## Cenas Mais Importantes

- `scenes/levels/PrototypeArena.tscn`: arena atual.
- `scenes/actors/Player.tscn`: jogador.
- `scenes/actors/EnemyGrunt.tscn`: inimigo comum.
- `scenes/actors/PlayerProjectile.tscn`: tiro do jogador.

## Como Fazer Blocagem De Fase

Blocagem nao e arte final. E montar o espaco jogavel com formas simples.

No nosso caso:

- ruas sao retangulos escuros;
- paredes sao StaticBody2D com CollisionShape2D;
- pontos de spawn sao Marker2D;
- props podem ser Polygon2D simples;
- sangue pode ser Polygon2D vermelho escuro;
- postes futuramente podem ser Node2D com luz e sprite.

Voce nao precisa desenhar bonito nesta etapa. Precisa testar se o espaco e divertido.

## Tarefa Simples Para Treinar

Na cena `PrototypeArena.tscn`:

1. Selecione `SpawnPoints`.
2. Clique com botao direito em um `SpawnA`, `SpawnB`, etc.
3. Duplique.
4. Mova o novo spawn para outro canto.
5. Salve.
6. Rode o jogo.

Isso muda onde inimigos podem aparecer.

## O Que Nao Mexer Sem Necessidade

Evite mexer no inicio:

- scripts C# se nao souber o efeito;
- collision layers/masks;
- nome dos nodes usados por script;
- `project.godot`;
- arquivos dentro de `.godot`.

Se precisar mudar algum nome de node, avise antes, porque scripts podem depender do caminho.

## Como Saber Se Algo Quebrou

Se o jogo abrir mas nao responder:

- confira se o Godot compilou C#;
- veja o painel `Saida` e `Depurador`;
- feche e abra o projeto;
- rode `dotnet build SangueNoAsfalto.csproj`.

Se uma cena ficou estranha:

- veja se o node ainda esta na arvore;
- confira se o script esta anexado;
- confira se a colisao existe;
- confira se o objeto esta dentro da camera.

## Mentalidade

Godot parece grande no comeco, mas para nosso jogo voce vai usar uma parte pequena:

- scene tree;
- inspector;
- viewport 2D;
- collision shapes;
- sprites/polygons;
- lights;
- tilemap mais tarde.

O resto pode esperar.
