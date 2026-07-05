# Sistema De Clima, Tempo E Rua Viva

Este documento define a direcao do sistema dinamico de horario, clima e tensao urbana de `Sangue no Asfalto`.

## Intencao

O clima e o horario nao devem ser apenas fundo bonito. Eles devem mudar a leitura da rua, o comportamento dos inimigos, os perigos do cenario, a presenca de civis/NPCs e a dramaticidade da fase.

Frase guia:

> A rua nao para. O dia muda, a chuva cai, a violencia se adapta.

## Pilares

- Cada fase tem uma identidade de horario e clima.
- A transicao de tempo deve contar historia sem depender de cutscene longa.
- Chuva, lama, vento, calor, noite e tempestade devem afetar o jogo em camadas.
- Sangue, agua, luz e sombra devem reagir visualmente.
- Bosses importantes devem ter clima e horario combinando com sua personalidade.
- O sistema deve nascer simples e crescer por sprints.

## Estados De Tempo Base

Estados principais:

- madrugada;
- amanhecer;
- manha;
- tarde;
- por do sol;
- noite;
- temporal noturno.

Cada estado pode alterar:

- cor global da fase;
- intensidade das luzes;
- visibilidade;
- quantidade de NPCs/civis;
- tipo de inimigo predominante;
- musica e ambiencia;
- nivel de perigo da rua;
- reflexos no asfalto;
- comportamento de patrulhas e emboscadas.

## Modulos De Clima

Modulos iniciais:

- seco;
- garoa;
- chuva forte;
- lama;
- neblina;
- vento forte;
- calor seco;
- tempestade eletrica;
- queda de luz;
- pos-tempestade.

Cada modulo pode ativar efeitos:

- particulas de chuva;
- pocas/reflexos;
- folhas e lixo voando;
- raios no fundo;
- flicker de poste;
- audio de vento, trovoes e transformadores;
- reducao de visibilidade;
- risco ambiental localizado.

## Impacto Na Jogabilidade

O impacto precisa ser legivel e justo.

Exemplos:

- chuva forte reduz visibilidade e aumenta reflexos;
- lama pode reduzir velocidade em trechos especificos;
- noite aumenta emboscadas e luzes de neon/postes;
- vento empurra particulas, lixo e alguns projeteis leves;
- calor aumenta tensao visual, poeira e agressividade de inimigos;
- tempestade eletrica cria riscos temporarios em fios e pocos d'agua;
- queda de luz cria momentos curtos de medo, mas com aviso claro.

Regra: clima nunca deve parecer aleatorio ou injusto. O jogador precisa entender o perigo antes de sofrer dano.

## Sangue, Agua E Superficie

O sangue e parte da identidade do jogo.

Estados visuais:

- sangue fresco no seco: vermelho escuro forte;
- sangue na lama: rastro opaco e sujo;
- sangue em poca: reflexo avermelhado;
- sangue na chuva: dilui e escorre;
- sangue no neon: ganha brilho colorido;
- sangue em tempestade: destacado por flashes de raio.

No prototipo, isso pode comecar com decals/particulas simples. Depois pode evoluir para materiais, shaders e camadas de superficie.

## Relacao Com Inimigos E Bosses

Cada boss deve ter um clima-assinatura.

Exemplos:

- pastor corrupto: templo fechado, noite quente, luz dourada falsa, chuva do lado de fora;
- bicheiro: cassino/bar ilegal, neon, madrugada e fumaca;
- mercenario: porto seco, chuva pesada e refletores;
- madrinha do morro: boate, luz roxa/vermelha, noite e neblina;
- caveira traidor: garagem/oficina, chuva oleosa e queda de luz;
- deputado final: palacio/camara, temporal e alerta maximo.

O cenario deve parecer que pertence ao inimigo, nao apenas que o inimigo foi colocado nele.

## Implementacao Em Camadas

Camada 1 - Visual simples:

- mudar cor de fundo;
- ligar/desligar luzes;
- particulas simples de chuva;
- efeito de relampago;
- audio ambiente placeholder.

Camada 2 - Gameplay leve:

- zonas de lama;
- zonas eletrificadas;
- visibilidade alterada;
- spawns diferentes por horario;
- NPCs/civis aparecendo ou sumindo.

Camada 3 - Sistema real:

- `WeatherState`;
- `TimeOfDayState`;
- transicoes suaves;
- eventos de fase;
- integracao com encontros e bosses;
- dados configuraveis por fase.

Camada 4 - Polimento:

- shaders de agua/reflexo;
- sangue reagindo a superficies;
- vento afetando particulas;
- som adaptativo;
- iluminacao dramatica por evento.

## Godot E C#

Possiveis scripts futuros:

- `scripts/world/TimeOfDayController.cs`
- `scripts/world/WeatherController.cs`
- `scripts/world/StreetTensionController.cs`
- `scripts/world/WeatherZone.cs`
- `scripts/world/EnvironmentalHazard.cs`

Possiveis cenas/nodes:

- `WorldEnvironment2D`;
- `CanvasModulate`;
- `PointLight2D`;
- `GPUParticles2D`;
- zonas `Area2D` para lama, eletricidade e vento;
- parallax para fundo da rua.

## Referencias Salvas

As referencias pilar deste sistema estao em:

- `references/pillars/07-weather-time-system.png`
- `references/pillars/08-dynamic-world-progression.png`
- `references/pillars/09-scenario-boss-variants.png`

## Quando Implementar

Sprint 05 iniciou a camada visual simples do sistema.

Implementado nesta primeira camada:

- `TimeOfDayController` simples;
- `WeatherController` simples;
- transicoes visuais entre horarios;
- chuva/garoa placeholder;
- relampago placeholder;
- neblina e lama/poca visual;
- luzes de poste reagindo ao horario.

O sistema completo de clima entra depois, mas ja deve continuar influenciando o design da cena:

- deixar espaco para postes;
- criar fundo em camadas;
- preparar areas de chuva/reflexo;
- pensar em ruas que possam mudar de horario;
- manter clima e boss conectados desde o planejamento.

Limites atuais:

- clima ainda e visual, sem impacto direto em gameplay;
- chuva ainda e placeholder com gotas `Line2D` geradas por codigo;
- relampago ainda e placeholder desenhado com `Polygon2D`;
- lama/poca ainda nao altera velocidade do jogador;
- audio ambiente de chuva/trovao ainda nao foi implementado;
- estados de clima ainda nao alteram spawns ou comportamento de inimigos.
