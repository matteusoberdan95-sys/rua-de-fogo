# Sprints - Sangue no Asfalto

Este documento deve ser atualizado sempre que uma sprint comecar ou terminar.

## Estado Atual

Sprint atual: `Sprint 09 - Demo Publica`

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

Status: concluida.

Objetivo: expandir o jogo para uma versao alpha.

Entregas planejadas:

- 2 ou 3 fases;
- 4 tipos de inimigos;
- 2 mini-chefes;
- 1 chefe principal;
- trilha e efeitos temporarios;
- primeira rodada de balanceamento.

Entregas implementadas:

- criadas variacoes `SideScrollerEnemyFast.tscn`, `SideScrollerEnemyBrute.tscn` e `SideScrollerEnemyInfected.tscn`;
- criado segundo mini-chefe `SideScrollerRainMiniBoss.tscn`;
- criado chefe placeholder alpha `SideScrollerAlphaBoss.tscn`;
- `SideScrollerDirector` agora usa composicoes de encontro por tipo de inimigo;
- fluxo alpha atual: comuns, checkpoint, comuns + rapido + infectado, bruto + comuns, mini-chefe, mini-chefe de chuva, chefe alpha e vitoria;
- primeira rodada de balanceamento aplicada em vida, dano, velocidade, cooldown e quantidades;
- `Espaco` agora faz o pulo visual do jogador e `K` fica dedicado a esquiva;
- `SideScrollerPrototype.tscn` conectado aos novos inimigos e chefes;
- `dotnet build SangueNoAsfalto.csproj` validado com 0 erros e 0 avisos.

Validado jogando:

- validada no Godot com `F5`;
- novos inimigos e fluxo alpha aprovados;
- chefe alpha placeholder aprovado para seguir;
- `Espaco` ajustado para pulo visual do jogador;
- `dotnet build SangueNoAsfalto.csproj` validado com 0 erros e 0 avisos.

## Sprint 09 - Demo Publica

Status: concluida.

Objetivo: preparar uma demo jogavel para feedback externo.

Entregas planejadas:

- uma fase completa polida;
- tutorial discreto;
- menu;
- configuracoes;
- build Windows;
- trailer curto interno;
- pacote para testers.

Entregas implementadas:

- criada cena `scenes/ui/MainMenu.tscn` como cena principal da aplicacao;
- criado `scripts/ui/MainMenu.cs` com iniciar demo, limpar save, alternar controles e sair fora do editor;
- menu mostra painel simples de configuracoes, controles, checkpoint salvo e atalhos `F1`, `F2`, `F4`;
- HUD da vertical slice ganhou tutorial discreto por etapa sem pausar o jogo;
- overlays de morte e fim da demo mostram instrucoes claras de `R` para tentar de novo e `M` para voltar ao menu;
- `SideScrollerDirector` agora permite voltar ao menu nos estados finais com `M`;
- adicionado `export_presets.cfg` com preset inicial `Windows Desktop`;
- criado `docs/BUILD_WINDOWS.md` com passos de export pelo editor e pelo CLI;
- `dotnet build SangueNoAsfalto.csproj` validado com 0 erros e 0 avisos.

Validado jogando:

- validada no Godot com `F5`;
- menu inicial, iniciar demo, limpar save, alternar controles, tutorial, morte, vitoria, `R` e `M` aprovados;
- `dotnet build SangueNoAsfalto.csproj` validado com 0 erros e 0 avisos.

Pendencias levadas para sprints futuras:

- trailer curto interno;
- pacote final para testers;
- aproximar arte/HUD do visual das referencias em `references/pillars`.

## Sprint 10 - Identidade Visual Pillars

Status: concluida.

Objetivo: aproximar a demo das referencias em `references/pillars`, priorizando o que mais aparece em jogo.

Prioridade dos pilares:

1. `04-hud-combat-ui-weapons.png`: HUD com barras, combo, furia, slot de arma e banner de etapa.
2. `02-stage-vila-esperanca-side-scroller.png`: boteco, ponto final, props urbanos e leitura da rua.
3. `01-character-lineup-caua-enemies-boss.png`: silhueta/cores do Caua mais proximas da referencia.
4. `07-weather-time-system.png`: refinamentos futuros de clima/horario.
5. `03`, `05`, `06`, `08`, `09`: conteudo e producao comercial para sprints seguintes.

Entregas implementadas:

- `BeatEmUpHud` reorganizado com banner de etapa, retrato placeholder, barras de vida/stamina/furia, combo callout e faixa de arma;
- `SideScrollerPlayerController` agora rastreia combo de hits, melhor combo e furia por impacto;
- `Hurtbox` notifica o jogador quando acerta inimigos;
- `SideScrollerDirector` expoe titulo e tagline da etapa `VILA ESPERANCA`;
- `SideScrollerPrototype.tscn` recebeu boteco do Ze, placa SKOL, mesas, ponto final, cachorro e pichacao da referencia;
- `SideScrollerPlayer.tscn` recebeu colete vermelho e regata branca mais proximos do Caua;
- `dotnet build SangueNoAsfalto.csproj` validado com 0 erros e 0 avisos.

Validado jogando:

- validada no Godot com `F5`;
- HUD, combo, furia, banner, boteco, ponto final e silhueta do Caua aprovados;
- `dotnet build SangueNoAsfalto.csproj` validado com 0 erros e 0 avisos.

Pendencias levadas para sprints futuras:

- sprites finais do Caua e inimigos;
- HUD final com tipografia/arte propria;
- props interativos (boteco curando, objetos quebraveis, cachorro como hazard);
- pagina/material Steam.

## Sprint 11 - Producao Steam

Status: concluida.

Objetivo: transformar a demo em produto comercial inicial, com material Steam, build demo e QC.

Entregas planejadas:

- pagina Steam;
- screenshots alinhadas com `references/pillars`;
- trailer;
- build demo;
- achievements se fizer sentido;
- controle de qualidade;
- plano de preco e lancamento.

Entregas implementadas:

- criado `docs/STEAM_PAGE.md` com rascunho de pagina, tags, requisitos e achievements futuros;
- criado `docs/SCREENSHOTS_STEAM.md` com lista de capturas alinhadas aos pilares;
- criado `docs/QC_DEMO_CHECKLIST.md` para validacao antes de distribuir build;
- criado `docs/LAUNCH_PLAN.md` com fases, preco sugerido e metricas;
- criado `docs/DEMO_PACKAGE.md` e `build/demo/README.txt` para testers;
- criado `docs/TRAILER_INTERNAL.md` com roteiro curto interno;
- criado `scripts/systems/DemoInfo.cs` com versao `Demo v1.0`;
- menu mostra versao da demo;
- criado `scripts/ui/ScreenshotModeHelper.cs` com `F9` para capturas limpas;
- criado `scripts/build-demo.ps1` e pasta `marketing/screenshots/steam/`;
- `export_presets.cfg` atualizado para versao `1.0.0.0`;
- HUD simplificado apos playtest: apenas HP, stamina, XP/nivel, arma e habilidades;
- sistema placeholder de XP/nivel adicionado ao jogador;
- tutorial in-game removido; tela dedicada fica para sprint futura;
- `dotnet build SangueNoAsfalto.csproj` validado com 0 erros e 0 avisos.

Validado jogando:

- validada no Godot com `F5`;
- versao `Demo v1.0`, `F9`, fluxo demo e HUD simplificado aprovados;
- `dotnet build SangueNoAsfalto.csproj` validado com 0 erros e 0 avisos.

## Sprint 12 - Fase Jogavel Vila Esperanca

Status: implementada / aguardando validacao no Godot.

Objetivo: entregar **uma fase completa e jogavel por cerca de 10 minutos**, com leitura visual proxima de `references/pillars`. Este e o gate real antes de demo publica ou Steam.

Criterio de pronto:

- jogador consegue jogar ~10 minutos sem sentir que acabou em 2 minutos;
- cenario, personagem e inimigos parecem proximos das referencias (mesmo com placeholders);
- fluxo claro: inicio -> exploracao/combate -> checkpoint -> escalada -> chefe -> fim;
- tutorial fica em tela dedicada, nao espalhado no HUD;
- clima e horario reforcam a fase, nao so existem como efeito solto.

Entregas planejadas:

- repacing do `SideScrollerDirector` para duracao ~10 min;
- expansao do layout da Vila Esperanca (camadas, props, landmarks da ref `02`);
- passada visual do Caua e inimigos comuns (ref `01`);
- encontros variados com tempo de respiracao entre ondas;
- mini-eventos ambientais simples (props, clima, transicao de horario);
- tela de tutorial dedicada no menu ou antes da fase;
- primeira rodada de balanceamento para sessao de 10 min.

Fora de escopo nesta sprint:

- demo publica para testers externos;
- pagina Steam, screenshots de marketing, trailer;
- build de distribuicao como prioridade;
- segunda fase (Vila Santana).

Entregas implementadas:

- `SideScrollerDirector` repacingado para ~10 min: intro, 9 encontros, 4 respiros, 3 chefes;
- clima e horario controlados por ato da fase (noite + chuva/tempestade progressiva);
- criada `scenes/ui/TutorialScreen.tscn` com controles e objetivo da fase;
- menu agora abre tutorial antes da fase (`Jogar` -> tutorial -> fase);
- cenario ganhou carro de policia, neon do boteco e props extras;
- inimigo comum aproximado do Quebra-Osso (regata, short roxo, bone);
- `dotnet build SangueNoAsfalto.csproj` validado com 0 erros e 0 avisos.

Validacao pendente:

- jogar sessao completa no Godot e estimar duracao (~10 min);
- validar tutorial dedicado, pacing, checkpoint e chefes;
- apos aprovacao, marcar sprint como concluida, commitar e fazer push.

## Sprint 13 - Producao Visual Fase 1

Status: implementada / aguardando validacao no Godot.

Objetivo: sair do visual generico (poligonos + UI default) e comecar a parecer jogo, alinhado a `references/pillars/`, com estilo **pintura 2D + pixel aparente**.

Criterio de pronto:

- Caua e grunt comum usam `AnimatedSprite2D` (nao poligonos);
- fundo da Vila Esperanca usa parallax em 3 camadas pintadas;
- HUD com painel escuro, borda vermelha e barras custom;
- filtro `Nearest` nos sprites para leitura pixel aparente;
- placeholders claros mas com silhueta/paleta das refs, prontos para substituicao no Krita.

Entregas implementadas:

- estilo travado em `docs/VISUAL_BIBLE.md` e `docs/ART_PIPELINE.md`;
- assets em `art/sprites/` e `art/backgrounds/vila-esperanca/`;
- `CharacterSpriteVisual.cs` — flip, pulo, idle/walk/attack;
- `SideScrollerPlayer.tscn` e `SideScrollerEnemyGrunt.tscn` migrados para sprites;
- `VilaEsperancaParallax.tscn` com 3 camadas + vignette;
- `GameUiTheme.cs` + skin aplicada no `BeatEmUpHud`;
- poligonos de skyline antigos ocultos na fase (rua/props mantidos);
- `dotnet build SangueNoAsfalto.csproj` validado com 0 erros e 0 avisos.

Validacao pendente:

- F5 no Godot: alinhamento pes/sombra dos sprites vs colisao;
- parallax vs camera e limites da fase;
- legibilidade do HUD em combate;
- comparar com refs `01`, `02`, `04`.

Proximo passo apos validacao:

- redesenhar sprites no Krita (resolucao alvo ~128px altura, sheet horizontal);
- animacao de ataque/hit/morte do Caua;
- SFX ambiente (rua/chuva) + 5-8 efeitos de combate;
- skin nos demais inimigos e menu principal.

## Marco futuro - Demo Publica / Steam

Status: bloqueado ate a fase jogavel de ~10 min estar pronta.

Pre-requisitos:

- 1 fase com qualidade visual aceitavel vs `references/pillars`;
- duracao real de ~10 minutos;
- tutorial dedicado;
- polimento minimo de bugs e pacing.

Os docs de Steam/export da Sprint 11 permanecem como rascunho, nao como sprint ativa.

## Backlog Tecnico Permanente

- manter `dotnet build SangueNoAsfalto.csproj` sem erros;
- nao quebrar a cena top-down enquanto a lateral nao substituir oficialmente;
- versionar mudancas importantes;
- atualizar `docs/SPRINTS.md` ao final de cada sprint;
- atualizar `docs/HANDOFF.md` quando o fluxo mudar.
