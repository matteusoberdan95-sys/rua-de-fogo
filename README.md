# Sangue no Asfalto

Prototipo inicial de um beat 'em up/hack and slash dark fantasy em cenario suburbano brasileiro, feito em Godot 4 .NET com C#.

## Como abrir

1. Instale a versao `.NET` do Godot 4.
2. Abra esta pasta pelo Godot.
3. Se o editor pedir para atualizar a versao do `Godot.NET.Sdk`, aceite ou ajuste o arquivo `SangueNoAsfalto.csproj` para a versao do seu editor.
4. Execute com `F5`. A cena principal atual e `res://scenes/ui/MainMenu.tscn`.

## Controles

- `A/D` ou setas esquerda/direita: mover pela rua
- `W/S` ou setas cima/baixo: trocar lane/profundidade
- `J` ou botao esquerdo do mouse: atacar
- `L` ou botao direito do mouse: disparar
- `K`: esquivar
- `Espaco`: pular
- `R`: reiniciar a arena
- `M`: voltar ao menu nos estados de morte/vitoria
- `F1`: alternar HUD debug
- `F2`: alternar controles alternativos
- `F4`: limpar save durante a demo
- `F9`: modo screenshot (esconde tutorial/controles para capturas)

## Primeiro marco

O objetivo do marco `0.1` e validar o loop basico:

- movimentacao responsiva;
- ataque corpo a corpo;
- stamina;
- combo simples;
- disparo de curto alcance;
- ondas de inimigos;
- inimigo perseguidor;
- vida e morte;
- arena pequena com clima suburbano brasileiro.

## Direcao atual

O prototipo top-down continua existindo como laboratorio de sistemas em `PrototypeArena.tscn`. A cena principal atual e lateral/2.5D beat 'em up, com clima, horario, chuva, luz, sangue e tensao urbana como pilares do jogo.

## Estado atual

Sprint atual: `Sprint 14 - Integracao Visual De Assets` (iniciada / correcao tecnica aplicada).

Estamos em **prototipo alpha**. Meta atual: **fase jogavel de ~10 minutos** na Vila Esperanca, com visual proximo de `references/pillars` (pintura 2D + pixel aparente). Demo publica e Steam ficam bloqueadas ate la.

A Sprint 12 esta implementada: fase repacingada (~10 min), tutorial dedicado, clima por ato e props da Vila Esperanca.

A Sprint 13 esta implementada: Caua e grunt com `AnimatedSprite2D`, parallax pintado em 3 camadas, HUD tematizado e estilo visual travado no Krita/pixel aparente. Assets placeholder em `art/` prontos para substituicao.

A Sprint 14 comecou corrigindo o problema visual dos sprites: fundo preto, escala diferente entre idle/walk e flip estranho do grunt. Os assets normalizados ficam em `art/sprites/**/*_game.png` e a ferramenta repetivel fica em `tools/normalize-sprites.ps1`.

O prototipo lateral ja tem movimento por lanes, encontros de combate, combo, tiro, esquiva, pulo visual, HUD, ataque inimigo telegrafado, slash placeholder, flash de dano, knockback com hit-stun, hit pause curto, sangue placeholder, som placeholder de impacto e indicador visual de invulnerabilidade.

A Sprint 04 foi validada jogando e adicionou uma primeira passada de identidade visual na rua: asfalto molhado, reflexos, rachaduras, luzes de poste desenhadas, boteco/mercadinho fechado, pichacoes, fios, props urbanos, altar de rua e silhuetas melhores para protagonista e inimigo comum.

A Sprint 05 foi validada jogando e iniciou o sistema visual de clima/tempo com `TimeOfDayController`, `WeatherController`, ciclo simples de horario, chuva/garoa placeholder visivel, neblina, lama/poca visual, relampago e luzes reagindo ao horario.

A Sprint 06 foi validada jogando e implementou a primeira vertical slice pequena: entrada da rua, checkpoint, segundo encontro, mini-chefe placeholder, tela/mensagem de morte e vitoria via HUD.

A Sprint 07 foi validada jogando e iniciou os sistemas de jogo: save local, checkpoint persistente, arma improvisada com durabilidade, pickups de cura/arma/continue, continue simples, configuracao de HUD debug e remapeamento minimo. A organizacao de imports C# agora fica centralizada em `GlobalUsings.cs`.

A Sprint 08 foi validada jogando e expandiu o conteudo alpha com inimigo rapido, bruto, infectado, segundo mini-chefe, chefe placeholder alpha e encontros por composicao.

O controle atual usa `Espaco` para pulo visual do personagem e `K` para esquiva.

A Sprint 09 foi validada jogando e adiciona menu inicial como cena principal, tutorial discreto no HUD, overlays melhores de morte/vitoria, painel simples de configuracoes/controles no menu e preset inicial de export Windows.

A Sprint 10 foi validada jogando e aproximou a demo das referencias em `references/pillars` com HUD estilo arcade, combo/furia, banner da Vila Esperanca, boteco/ponto final e silhueta mais proxima do Caua.

A Sprint 11 foi validada jogando e preparou material Steam, checklist QC, pacote demo, versao `Demo v1.0`, modo screenshot com `F9` e HUD simplificado com HP, stamina, XP/nivel, arma e habilidades.

## Regra obrigatoria de sprint

Toda sprint so pode ser marcada como concluida depois de build C# sem erros, validacao no Godot com `F5`, docs atualizados, commit e push.

## Proximo passo recomendado

Na outra maquina, validar Sprint 14 no Godot com `F5`:

- menu -> tutorial -> fase;
- conferir se o fundo preto dos personagens sumiu;
- conferir se Caua parado/andando mantem escala coerente;
- conferir se o grunt vira para o lado correto;
- conferir alinhamento de pes/sombra/colisao;
- depois substituir placeholders em `art/` por sprites finais no Krita quando o pipeline estiver ok.

**So depois disso** voltamos a falar de demo publica ou Steam.

## Documentos importantes

- `docs/PROJECT_BRIEF.md`: resumo do projeto, tecnologias e escopo.
- `docs/VISUAL_BIBLE.md`: direcao visual do jogo.
- `docs/WEATHER_TIME_SYSTEM.md`: clima, horario e rua viva.
- `docs/ART_PIPELINE.md`: fluxo de arte e referencias.
- `docs/GODOT_BEGINNER_GUIDE.md`: guia pratico para mexer no Godot.
- `docs/ROADMAP.md`: fases do projeto.
- `docs/SPRINTS.md`: planejamento das sprints e sprint atual.
- `docs/HANDOFF.md`: como continuar em Codex, CLI ou outro PC.
- `docs/BUILD_WINDOWS.md`: passo de exportacao Windows.
- `docs/STEAM_PAGE.md`: rascunho da pagina Steam.
- `docs/SCREENSHOTS_STEAM.md`: lista de screenshots para marketing.
- `docs/QC_DEMO_CHECKLIST.md`: checklist antes de distribuir build.
- `docs/DEMO_PACKAGE.md`: pacote para testers.
- `docs/LAUNCH_PLAN.md`: fases e preco sugerido.
- `docs/AGENTS.md`: responsabilidades dos agentes.
