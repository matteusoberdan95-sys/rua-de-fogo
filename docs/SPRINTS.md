# Sprints - Sangue no Asfalto

Este documento deve ser atualizado sempre que uma sprint comecar ou terminar.

## Estado Atual

Sprint atual: `Sprint 23 - Progressao Marcial Por XP` (planejada).

Ultima sprint concluida: `Sprint 22 - Combate Plastico E Impacto` (validada no Godot).

Referencia: `docs/COMBAT_DESIGN.md`

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

## Sprint 14 - Integracao Visual De Assets

Status: em andamento / segunda correcao visual aplicada.

Objetivo: transformar as referencias novas em pipeline real de jogo, corrigindo primeiro os problemas que quebram a apresentacao: fundo preto em sprites, escala diferente entre idle/walk e inimigo virando para o lado errado.

Problemas identificados:

- `art/sprites/player/caua_idle.png`, `caua_walk_sheet.png` e `art/sprites/enemies/grunt_idle.png` nao tinham transparencia;
- idle do Caua usava frame 1536x1024, enquanto walk usava frames 384x1024;
- a troca idle/walk fazia o personagem parecer mudar de tamanho;
- o script visual assumia que todo sprite-fonte olhava para a direita;
- o grunt-fonte olhava para a esquerda, causando flip estranho;
- inimigo comum ainda nao tem animacao de walk, entao parecia uma imagem estatica.

Entregas implementadas:

- criada ferramenta `tools/normalize-sprites.ps1`;
- gerados assets normalizados:
  - `art/sprites/player/caua_idle_game.png`;
  - `art/sprites/player/caua_walk_sheet_game.png`;
  - `art/sprites/enemies/grunt_idle_game.png`;
- `SideScrollerPlayer.tscn` agora usa sprites `_game`;
- `SideScrollerEnemyGrunt.tscn` agora usa sprite `_game`;
- `CharacterSpriteVisual` recebeu `SourceFacesRight` para corrigir flip por asset;
- grunt configurado com `SourceFacesRight = false`;
- inimigo sem walk sheet ganhou bob leve ao se mover;
- criada ferramenta `tools/extract-reference-assets.ps1`;
- gerados sprites temporarios consistentes a partir da prancha `references/personagens_ref/`:
  - `art/sprites/player/caua_ref_idle.png`;
  - `art/sprites/player/caua_ref_walk_sheet.png`;
  - `art/sprites/player/caua_ref_attack.png`;
- `SideScrollerPlayer.tscn` agora usa `caua_ref_*` para parado/andando/atacando parecerem o mesmo personagem;
- `VilaEsperancaParallax.tscn` reposicionado para exibir melhor o background pintado;
- blocagem antiga de predios, postes, carro e placas foi escondida/atenuada para nao cobrir a arte;
- bloqueadores invisiveis desses props foram desligados para evitar colisao sem leitura visual;
- `dotnet build SangueNoAsfalto.csproj` validado com 0 erros e 0 avisos.

Referencias novas:

- `references/personagens_ref/`: pranchas de personagens, inimigos, bosses e poses;
- `references/cenarios/`: pranchas de cenarios/fases e atmosferas.

Pendencias de validacao:

- abrir no Godot e testar `F5`;
- confirmar se o fundo preto sumiu;
- confirmar se idle/walk/attack agora parecem o mesmo Caua;
- confirmar se o grunt vira para o lado correto;
- confirmar se o cenario ficou menos blocagem e mais background pintado;
- se o background ficar desalinhado no F5, ajustar `motion_offset`, `position` e `scale` em `VilaEsperancaParallax.tscn`;
- decidir quais referencias serao recortadas/desenhadas primeiro no Krita.

## Marco futuro - Demo Publica / Steam

Status: bloqueado ate a fase jogavel de ~10 min estar pronta.

Pre-requisitos:

- 1 fase com qualidade visual aceitavel vs `references/pillars`;
- duracao real de ~10 minutos;
- tutorial dedicado;
- polimento minimo de bugs e pacing.

Os docs de Steam/export da Sprint 11 permanecem como rascunho, nao como sprint ativa.

## Sprint 15 - Personagem Vivo E Pipeline De Arte Final

Status: implementada / aguardando validacao no Godot.

Objetivo: parar de tratar recorte de prancha como arte do protagonista e preparar o jogo para um Caua com vida: respiracao, peso, cabelo, arma, golpes e leitura corporal.

Problemas identificados no playtest:

- o recorte `caua_ref_*` melhorou coerencia, mas ainda parecia uma imagem colada;
- arte extraida de prancha traz artefatos transparentes e bordas sujas;
- idle/walk/attack precisam comunicar corpo vivo, nao apenas troca de textura;
- o jogo final precisa de sprite sheet propria, desenhada/exportada limpa.

Entregas implementadas:

- `scripts/visual/CharacterSpriteVisual.cs` ganhou modo `UseLayeredPrototype`;
- `SideScrollerPlayer.tscn` ativa `UseLayeredPrototype = true`;
- o Caua agora e desenhado em partes no runtime:
  - pernas separadas;
  - bracos separados;
  - torso/camisa;
  - cabeca;
  - cabelo;
  - faca/machete;
  - pulso visual na camisa;
- idle com respiracao, movimento de cabeca, cabelo e batimento/peito;
- walk com pernas/bracos alternando e torso com peso;
- attack com windup, swing de torso/braco/arma e esticada de alcance;
- dash com inclinacao do corpo e cabelo para tras;
- flash de dano tambem funciona no rig em camadas;
- sprite recortado continua no projeto apenas como fallback/referencia, escondido no Caua;
- `dotnet build SangueNoAsfalto.csproj` validado com 0 erros e 0 avisos.

Pendencias de validacao:

- abrir no Godot e testar `F5`;
- verificar tamanho do Caua na camera;
- verificar se os pes batem com sombra/colisao;
- testar J/K/Espaco andando e parado;
- decidir se o rig em camadas fica como prototipo visual ou se voltamos para sprite sheet assim que a primeira sheet limpa existir.

Proximo passo recomendado:

- Sprint 16 deve focar em **sprite sheet final do Caua**:
  - idle 6-8 frames;
  - walk 8 frames;
  - combo 1/2/3;
  - hurt;
  - dodge;
  - low health;
  - death;
  - export transparente com pivot nos pes.

## Sprint 16 - Rua Viva E Inimigos Em Camadas

Status: concluida (validada no Godot).

Objetivo: aplicar ao cenario e aos inimigos a mesma regra aprendida com o Caua: referencias guiam o visual, mas nao devem ser imagens coladas no gameplay ativo.

Problemas identificados:

- o background pintado melhorou a leitura, mas ainda era uma imagem grande por tras da fase;
- o cenario precisava de partes separadas para animar neon, fios, lixo, agua e clima;
- o inimigo comum ainda dependia de sprite recortado;
- referencias devem ficar em `references/`/`art/` como direcao, nao como runtime final.

Entregas implementadas:

- criado `scripts/world/LayeredStreetPrototype.cs`;
- `scenes/world/VilaEsperancaParallax.tscn` agora usa `Node2D` + `LayeredStreetPrototype`, sem `bg_far/bg_mid/bg_near` ativos;
- rua viva montada por codigo com:
  - ceu e glow distante;
  - morro/favela distante;
  - predios com janelas;
  - fios;
  - boteco;
  - cerca de madeira;
  - placa de vende-se;
  - posters;
  - postes;
  - calcada;
  - asfalto molhado;
  - pocos, sangue, lixo e bueiro;
  - neon e reflexos animados;
- `CharacterSpriteVisual` ganhou `LayeredPrototypePreset`;
- `SideScrollerEnemyGrunt.tscn` agora usa `UseLayeredPrototype = true` e `LayeredPreset = QuebraOsso`;
- sprite antigo do inimigo fica escondido como fallback/referencia;
- `dotnet build SangueNoAsfalto.csproj` validado com 0 erros e 0 avisos.

Pendencias de validacao:

- abrir no Godot e testar `F5`;
- confirmar se o cenario aparece na camera inteira;
- confirmar se a rua em camadas nao ficou vazia em trechos avancados da fase;
- confirmar se o Quebra-Osso tem escala boa contra o Caua;
- ajustar cores/posicoes se o cenario ficar poligonal demais;
- decidir se Sprint 17 sera sprite sheet final do Caua ou refinamento visual da rua.

Validacao:

- escala Caua/inimigo, cobertura da camera, chao e leitura da rua aprovados no playtest;
- proximo foco: dar vida aos personagens (Sprint 17).

## Sprint 17 - Personagens Vivos: Combo, Impacto E Reacao

Status: concluida (validada no playtest).

Objetivo: depois de validar cenario e escala (Sprint 16), fazer personagens parecerem vivos no combate — nao apenas parados/andando, mas reagindo, telegrafando e golpeando em camadas.

Criterio de pronto:

- idle com respiracao visivel + cabelo/roupa com movimento secundario;
- combo 1/2/3 com animacoes distintas no rig em camadas;
- trail de arma no swing e feedback de impacto ligado ao rig;
- inimigo telegrafa ataque no corpo (nao so modulate no root);
- hit reaction com recoil direcional no torso/cabeca/bracos;
- `CombatFeedback` aciona `CharacterSpriteVisual.PlayHitReaction`.

Entregas implementadas:

- `CharacterSpriteVisual` ganhou: `_vestFlap`, `_clothSway`, `_hairTail`, estados `hurt` e `telegraph`;
- combo 0/1/2 com windups e swings diferentes + `SetAttackCombo()`;
- `SpawnSwingTrail()` por golpe;
- `AnimateHurt()` com recoil direcional;
- `AnimateTelegraph()` para Quebra-Osso;
- `SideScrollerPlayerController` passa combo index e hit-stun ao visual;
- `SideScrollerEnemyController` usa telegraph visual no rig (sem flash no root);
- `CombatFeedback` chama `PlayHitReaction` e escala spark em golpes pesados;
- `dotnet build SangueNoAsfalto.csproj` validado com 0 erros e 0 avisos.

Validacao:

- aprovada no playtest; combate e rig com boa leitura de vida.

## Sprint 18 - Combate Desarmado E Dano Cumulativo

Status: concluida (validada no Godot).

Por que agora (decisao agents):

- rig vivo pronto (Sprint 17); proximo passo e trocar corte por soco/chute;
- Krita antes disso repetiria faca errada;
- finishers gore e estilos marciais ficam para Sprints 19–20.

Fora de escopo nesta sprint:

- decapitacao / partir ao meio (Sprint 19);
- Muay Thai, boxe, karate, capoeira, jiu-jitsu por XP (Sprint 20);
- sprite sheet final Krita.

### Fase 18A — Tirar armas fixas

- remover machete/tubo fixo dos rigs Caua e Quebra-Osso;
- impacto de punho/pe no lugar de trail de lamina;
- HUD `Punhos` / `Estilo: Rua`; tutorial atualizado.

### Fase 18B — Golpes desarmados

- combo 1: jab; 2: chute lateral; 3: voadora/cotovelada;
- inimigo: soco, cabecada, gancho telegrafados;
- hitbox no membro, rebalance de dano.

### Fase 18C — Machucado cumulativo

- 3 tiers visuais por HP (intacto / machucado / critico);
- overlays no rig inimigo.

Criterio de pronto: ver checklist em `docs/COMBAT_DESIGN.md`.

Entregas implementadas:

- removida faca/tubo fixos dos rigs Caua e Quebra-Osso;
- combo desarmado: jab / chute lateral / voadora (`AnimateJab`, `AnimateSideKick`, `AnimateFlyingKick`);
- inimigo: soco e cabecada alternados (`AnimateHeadbutt`);
- `SpawnStrikeImpact` substitui trail de lamina;
- `EnemyDamageState.cs` + tiers visual intacto/machucado/critico no inimigo;
- HUD `Estilo: Rua | Punhos`; tutorial atualizado;
- hitbox por golpe rebalanceada no player;
- `dotnet build` validado com 0 erros e 0 avisos.

Validacao:

- aprovada no playtest; combate desarmado e machucado cumulativo no inimigo ok.

## Sprint 19 - Movimento, Spawn E Sidearm

Status: concluida (validada no Godot).

Objetivo: corrigir feel de combate e pacing da fase — voadora no ar, corrida, spawn a frente do jogador, Caua machucando, pistola com animacao e sangramento basico.

Entregas implementadas:

- **Espaco + J** forca voadora no ar;
- **double-tap A/D** ativa corrida (2.4s) com golpes proprios (soco/chute corrida);
- spawn escalonado **a frente** da posicao do jogador (nao mais so nos cantos fixos da fase);
- intro encurtada + trigger ao avancar 140px;
- inimigo grunt **115 HP**; intervalo entre ondas reduzido;
- **Caua machuca** visualmente (tiers HP no rig);
- **Pistola**: animacao de sacar/atirar, projetil pequeno, **7 balas**, HUD `Pistola X/7`;
- **BleedEffect** — tiro causa sangramento (DOT) no inimigo;
- tutorial atualizado;
- `dotnet build` validado.

Validacao:

- aprovada no playtest; corrida, voadora no ar, spawn a frente, pistola e machucado do Caua ok.

## Sprint 20 - Armas Improvisadas E Finishers

Status: concluida (validada no Godot).

Objetivo: armas temporarias no chao com durabilidade, golpes e finishers por tipo, reload da pistola e VFX de sangramento.

Entregas implementadas:

- **3 armas**: vergalhao (6), martelo (4), faca (5) — pickups na fase;
- golpe basico por arma com animacao no rig;
- **finalizador**: J perto de inimigo critico (<33% HP) com arma equipada;
- finishers com hit pause longo, zoom de camera e burst de sangue;
- **facas** causam sangramento (DOT) no golpe basico e finisher;
- **E** recarrega pistola (1.25s); pickup de municao (+4 balas);
- **BleedEffect** com gotas visuais enquanto inimigo sangra;
- tutorial e HUD atualizados;
- `dotnet build` validado.

Validacao:

- aprovada no playtest.

## Sprint 21 - Fase Scroll E Postura/Parry

Status: concluida (validada no Godot).

Objetivo: fase longa estilo Final Fight com spawn conforme avanco; sistema de postura e parry (Sekiro) com golpe mortal.

Entregas implementadas:

- **Fase ~4000px** (-760 ate ~3180); limites de camera e mundo estendidos;
- **spawn por progresso** (`StageScrollSpawns`) — inimigos entram pela borda direita (fade-in) conforme X do jogador;
- sem ondas vazias / backtrack; checkpoint no meio (~480);
- **PostureComponent** (jogador + inimigo grunt);
- **Q = parry** — deflect no timing; enche postura do inimigo;
- postura quebrada = inimigo vulneravel; **J = golpe mortal** brutal;
- barra de postura no HUD; tutorial atualizado;
- `dotnet build` validado.

Validacao:

- aprovada no playtest; fase scroll e parry/postura ok.

## Sprint 22 - Combate Plastico E Impacto

Status: concluida (validada no Godot).

Objetivo: combate menos generico — golpes pesados, dor explicita no inimigo, stamina por golpe, feedback de impacto como diferencial do jogo.

Entregas implementadas:

- golpes com **windup** e duracao maior (jab/chute/voadora);
- **stamina por golpe** (10/15/24); sem stamina nao ataca;
- reacao de **dor** no rig: careta, buckle, hurt empilhado;
- callouts **"ugh!" / "AU!" / "CRACK!"**, onda de impacto, camera shake;
- hit pause e stun **escalam com dano**;
- inimigo reage visualmente ao knockback (`PlayHitReaction`);
- `CombatImpactFeel.cs`; tutorial atualizado;
- `dotnet build` validado.

Validacao:

- aprovada no playtest; combate plastico e dor visivel ok.

## Sprint 23 - Progressao Marcial Por XP (planejada)

Desbloqueio de estilos e deck de golpes por nivel. Ver `docs/COMBAT_DESIGN.md`.

## Backlog Tecnico Permanente

- manter `dotnet build SangueNoAsfalto.csproj` sem erros;
- nao quebrar a cena top-down enquanto a lateral nao substituir oficialmente;
- versionar mudancas importantes;
- atualizar `docs/SPRINTS.md` ao final de cada sprint;
- atualizar `docs/HANDOFF.md` quando o fluxo mudar.
