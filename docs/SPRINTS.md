# Sprints - Sangue no Asfalto

Este documento deve ser atualizado sempre que uma sprint comecar ou terminar.

## Estado Atual

Sprint atual: `Sprint 02 - Prototipo Beat 'em Up`

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

Status: implementada / em validacao jogavel.

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

Pendencias de validacao:

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

Status: futura.

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

Agentes principais:

- Goku: sensacao de golpe.
- Vegeta: ataque inimigo.
- Trunks: feedback visual.
- Freeza: balanceamento.

## Sprint 04 - Identidade Visual Da Primeira Rua

Status: futura.

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

Agentes principais:

- Gohan: direcao visual.
- Trunks: leitura de tela e HUD.
- Bulma: pipeline de arte.
- Piccolo: integracao com cenas.
- Shenlong: horario, chuva leve e luz ambiente.

## Sprint 05 - Prototipo De Clima E Tempo

Status: futura.

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

Agentes principais:

- Shenlong: sistema de clima e tempo.
- Piccolo: arquitetura.
- Trunks: leitura visual/HUD se necessario.
- Gohan: clima narrativo da fase.

## Sprint 06 - Vertical Slice Pequena

Status: futura.

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

Agentes principais:

- Goku: gameplay.
- Vegeta: encontros.
- Freeza: mini-chefe.
- Trunks: menu, morte, vitoria.
- Bulma: build.

## Sprint 07 - Sistemas De Jogo

Status: futura.

Objetivo: comecar sistemas que sustentam um jogo maior.

Entregas planejadas:

- save local;
- selecao/estado de arma;
- durabilidade de arma;
- pickups;
- vida extra ou continues;
- configuracoes basicas;
- remapeamento de controles se necessario.

Agentes principais:

- Piccolo: arquitetura.
- Goku: armas e pickups.
- Trunks: telas.
- Bulma: persistencia local.

## Sprint 08 - Conteudo Alpha

Status: futura.

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
