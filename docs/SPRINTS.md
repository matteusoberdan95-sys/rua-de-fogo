# Sprints - Sangue no Asfalto

Este documento deve ser atualizado sempre que uma sprint comecar ou terminar.

## Estado Atual

Sprint atual: `Sprint 08 - Conteudo Alpha`

Direcao oficial atual:

- jogo 2D/2.5D lateral;
- beat 'em up/hack and slash;
- suburbio brasileiro dark;
- personagens fortes como pilar visual;
- clima, horario e rua viva como identidade do jogo;
- prototipo top-down mantido como laboratorio de sistemas.

## Definicao De Pronto

Uma tarefa so conta como pronta quando:

- compila sem erros;
- roda no Godot;
- foi testada jogando;
- esta documentada quando muda fluxo, controle, cena ou arquitetura;
- nao quebra o prototipo anterior sem motivo claro.

## Regra Obrigatoria De Fechamento De Sprint

Uma sprint so pode ser marcada como `concluida` depois de:

- rodar `dotnet build SangueNoAsfalto.csproj` com 0 erros;
- validar no Godot com `F5`;
- atualizar `README.md` e documentos relevantes em `docs/`;
- commitar as alteracoes;
- fazer push para o remoto configurado.

Se ainda nao houve validacao no Godot, a sprint deve ficar como `implementada / aguardando validacao`.

## Sprint 00 - Fundacao

Status: concluida.

Objetivo: criar o projeto Godot .NET e validar que C# funciona.

Entregas:

- projeto Godot .NET;
- estrutura de pastas;
- cena `PrototypeArena.tscn`;
- scripts de vida, dano, jogador, inimigo e HUD;
- build C# validada.

## Sprint 01 - Arena Jogavel

Status: concluida.

Objetivo: validar um loop basico de combate.

Entregas:

- movimento do jogador;
- ataque corpo a corpo;
- dash;
- stamina;
- inimigo perseguidor;
- ondas de inimigos;
- tiro basico;
- reinicio com `R`;
- HUD com vida, stamina, onda e status.

## Sprint 02 - Prototipo Beat 'em Up

Status: concluida como base jogavel / ainda pede ajuste fino no Godot.

Objetivo: criar uma segunda cena lateral/2.5D para testar se a direcao visual e de gameplay funciona melhor que o top-down.

Entregas implementadas:

- criada `scenes/levels/SideScrollerPrototype.tscn`;
- criados `SideScrollerPlayer.tscn` e `SideScrollerEnemyGrunt.tscn`;
- criado controle lateral com lanes;
- mantidos `WASD`, `J`, `L`, `K/Espaco`, `R`;
- camera lateral seguindo o jogador;
- rua simples estilo Vila Esperanca com boteco, muro, carro, postes, pocos e sangue;
- cena preparada para receber luzes, chuva e transicoes futuras;
- inimigos entram pelos lados e alinham na lane antes de atacar;
- combo lateral em formato beat 'em up;
- prototipo top-down mantido intacto em `PrototypeArena.tscn`;
- `dotnet build SangueNoAsfalto.csproj` validado com 0 erros e 0 avisos.

Pendencias levadas para ajuste fino:

- testar no Godot com `F5`;
- ajustar velocidade, alcance, camera e ritmo apos jogar;
- decidir se a cena lateral substitui oficialmente o top-down como laboratorio principal.

Agentes principais:

- Goku: movimento, combate e camera.
- Vegeta: inimigos laterais.
- Piccolo: arquitetura da nova cena.
- Gohan: clima da fase.
- Shenlong: preparacao do sistema de clima/tempo.

## Sprint 03 - Feedback De Combate

Status: concluida.

Objetivo: fazer cada golpe parecer pesado e sangrento.

Entregas planejadas:

- flash de dano em inimigos;
- knockback melhor;
- hit pause curto;
- particulas de sangue;
- efeito visual de slash;
- som placeholder de impacto;
- indicador de invulnerabilidade mais claro;
- ataque inimigo telegrafado.

Entregas implementadas:

- feedback centralizado em `scripts/core/CombatFeedback.cs`;
- flash vermelho no alvo quando dano entra;
- sangue, mancha no chao e impacto visual placeholder gerados por codigo;
- hit pause curto ao acertar;
- knockback com hit-stun via `ICombatKnockbackReceiver`;
- slash visual placeholder no combo lateral do jogador;
- telegraph do inimigo lateral com pulso de cor mais forte;
- som placeholder de impacto gerado por codigo;
- indicador de invulnerabilidade do jogador com pulso visual azul/ciano.

Validado jogando:

- combate lateral esta funcionando e ficando mais pesado;
- sangue foi aumentado apos teste;
- bug de slow motion persistente apos matar inimigos foi corrigido.
- `dotnet build SangueNoAsfalto.csproj` validado com 0 erros e 0 avisos ao encerrar.

Recomendacao:

- iniciar a Sprint 04 para melhorar a identidade visual da primeira rua;
- nao iniciar clima/tempo dinamico antes da rua base ter leitura visual melhor.

Agentes principais:

- Goku: sensacao de golpe.
- Vegeta: ataque inimigo.
- Trunks: feedback visual.
- Freeza: balanceamento.

## Sprint 04 - Identidade Visual Da Primeira Rua

Status: concluida.

Objetivo: trocar blocagem feia por uma fase ainda simples, mas com cara do jogo.

Entregas planejadas:

- rua lateral com asfalto molhado;
- boteco fechado;
- poste com luz 2D;
- muros pichados;
- props urbanos;
- parallax simples no fundo;
- primeira variacao visual de horario/clima;
- paleta oficial aplicada;
- primeiro sprite melhor do protagonista;
- primeiro sprite melhor do inimigo comum.

Entregas implementadas:

- asfalto mais escuro com brilho molhado, reflexos e rachaduras;
- camadas simples de fundo com neblina, casas, caixas d'agua e fios;
- boteco/mercadinho fechado com placa, porta de aco e linhas de metal;
- muros com pichacao, aviso sobrenatural e cartaz rasgado;
- props urbanos: sacos de lixo, bueiro, carro quebrado mais detalhado e altar de rua com vela;
- postes com cones de luz desenhados e pools de luz no asfalto;
- terceiro poste falhando como landmark central;
- paleta aproximada da biblia visual aplicada na rua;
- protagonista com jaqueta, bandagem, cabelo, bracos e lamina mais reconheciveis;
- inimigo comum com olhos fortes, mandibula quebrada, costelas/brilho e silhueta mais agressiva.

Validado jogando:

- validada no Godot com `F5`;
- leitura visual aprovada;
- props, reflexos e silhuetas ficaram bons para seguir para clima/tempo.

Agentes principais:

- Gohan: direcao visual.
- Trunks: leitura de tela e HUD.
- Bulma: pipeline de arte.
- Piccolo: integracao com cenas.
- Shenlong: horario, chuva leve e luz ambiente.

## Sprint 05 - Prototipo De Clima E Tempo

Status: concluida.

Objetivo: provar que horario e clima mudam a sensacao da fase sem quebrar o gameplay.

Entregas planejadas:

- `TimeOfDayController` simples;
- `WeatherController` simples;
- transicao visual entre manha, tarde e noite;
- chuva ou garoa placeholder;
- relampago placeholder;
- poste/luz reagindo ao horario;
- uma zona simples de lama ou poca;
- documentar limites do sistema para a vertical slice.

Entregas implementadas:

- criado `scripts/world/TimeOfDayController.cs`;
- criado `scripts/world/WeatherController.cs`;
- `SideScrollerPrototype.tscn` agora tem `TimeOfDayController` e `WeatherController`;
- ciclo visual simples entre amanhecer, manha, tarde, por do sol e noite;
- luzes/pools de poste reagem ao horario;
- camada visual de garoa/chuva forte/tempestade com gotas `Line2D` geradas por codigo;
- neblina e lama/poca placeholder com intensidade por clima;
- relampago placeholder em tempestade;
- build C# validada com 0 erros e 0 avisos;
- chuva ajustada para aparecer com gotas `Line2D` geradas por codigo.

Validado jogando:

- validada no Godot com `F5`;
- ceu muda de cor corretamente;
- relampagos aparecem;
- chuva ficou visivel apos ajuste das gotas;
- resultado visual aprovado para seguir.

Agentes principais:

- Shenlong: sistema de clima e tempo.
- Piccolo: arquitetura.
- Trunks: leitura visual/HUD se necessario.
- Gohan: clima narrativo da fase.

## Sprint 06 - Vertical Slice Pequena

Status: concluida.

Objetivo: criar uma fase curta com inicio, meio e fim.

Entregas planejadas:

- primeira fase jogavel curta;
- 3 a 5 encontros de combate;
- checkpoint;
- mini-chefe;
- tela de morte;
- tela de vitoria;
- menu inicial simples;
- build Windows.

Entregas implementadas:

- `SideScrollerDirector` agora controla fluxo curto de fase;
- sequencia atual: entrada da rua, checkpoint, segundo encontro, mini-chefe e vitoria;
- checkpoint simples em memoria da cena;
- ao morrer depois do checkpoint, `R` volta para o checkpoint;
- ao morrer antes do checkpoint, `R` volta para o inicio;
- criada cena `scenes/actors/SideScrollerMiniBoss.tscn`;
- mini-chefe placeholder usa base do inimigo lateral, com vida maior, corpo maior e ataque mais pesado;
- HUD mostra etapa, objetivo, checkpoint, morte e vitoria;
- `dotnet build SangueNoAsfalto.csproj` validado com 0 erros e 0 avisos.

Validado jogando:

- validada no Godot com `F5`;
- fluxo da vertical slice aprovado;
- checkpoint, mini-chefe e mensagens de morte/vitoria aprovados para seguir;
- `dotnet build SangueNoAsfalto.csproj` validado com 0 erros e 0 avisos.

Pendencias levadas para sprints futuras:

- menu inicial simples;
- build Windows;
- checkpoint persistente em save local, se fizer sentido.

Agentes principais:

- Goku: gameplay.
- Vegeta: encontros.
- Freeza: mini-chefe.
- Trunks: menu, morte, vitoria.
- Bulma: build.

## Sprint 07 - Sistemas De Jogo

Status: concluida.

Objetivo: comecar sistemas que sustentam um jogo maior.

Entregas planejadas:

- save local;
- selecao/estado de arma;
- durabilidade de arma;
- pickups;
- vida extra ou continues;
- configuracoes basicas;
- remapeamento de controles se necessario.

Entregas implementadas:

- criado `scripts/systems/GameSave.cs`;
- criado `scripts/systems/SaveManager.cs`;
- save local em `user://save_game.json`;
- checkpoint da vertical slice agora persiste no save local;
- estado de arma improvisada com durabilidade simples;
- pickups de cura, arma improvisada e continue;
- continue simples, limitado a 1, com revive parcial;
- HUD mostra arma, durabilidade e continue;
- configuracao persistente `ShowDebugHud`, alternada com `F1`;
- remapeamento minimo persistente com controles alternativos, alternado com `F2`;
- `F4` limpa o save local e reinicia a cena;
- `GlobalUsings.cs` criado para organizar imports globais por camadas;
- scripts limpos para depender dos global usings;
- `dotnet build SangueNoAsfalto.csproj` validado com 0 erros e 0 avisos.

Validado jogando:

- validada no Godot com `F5`;
- save local, checkpoint persistente, pickups, arma, continue e atalhos aprovados;
- build C# validada com 0 erros e 0 avisos antes do commit.

Agentes principais:

- Piccolo: arquitetura.
- Goku: armas e pickups.
- Trunks: telas.
- Bulma: persistencia local.

## Sprint 08 - Conteudo Alpha

Status: proxima / pronta para iniciar.

Objetivo: expandir o jogo para uma versao alpha.

Entregas planejadas:

- 2 ou 3 fases;
- 4 tipos de inimigos;
- 2 mini-chefes;
- 1 chefe principal;
- trilha e efeitos temporarios;
- primeira rodada de balanceamento.

## Sprint 09 - Demo Publica

Status: futura.

Objetivo: preparar uma demo jogavel para feedback externo.

Entregas planejadas:

- uma fase completa polida;
- tutorial discreto;
- menu;
- configuracoes;
- build Windows;
- trailer curto interno;
- pacote para testers.

## Sprint 10 - Producao Steam

Status: futura.

Objetivo: transformar a demo em produto comercial.

Entregas planejadas:

- pagina Steam;
- screenshots;
- trailer;
- build demo;
- achievements se fizer sentido;
- controle de qualidade;
- plano de preco e lancamento.

## Backlog Tecnico Permanente

- manter `dotnet build SangueNoAsfalto.csproj` sem erros;
- nao quebrar a cena top-down enquanto a lateral nao substituir oficialmente;
- versionar mudancas importantes;
- atualizar `docs/SPRINTS.md` ao final de cada sprint;
- atualizar `docs/HANDOFF.md` quando o fluxo mudar.
