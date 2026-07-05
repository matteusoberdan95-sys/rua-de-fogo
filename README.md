# Sangue no Asfalto

Prototipo inicial de um beat 'em up/hack and slash dark fantasy em cenario suburbano brasileiro, feito em Godot 4 .NET com C#.

## Como abrir

1. Instale a versao `.NET` do Godot 4.
2. Abra esta pasta pelo Godot.
3. Se o editor pedir para atualizar a versao do `Godot.NET.Sdk`, aceite ou ajuste o arquivo `SangueNoAsfalto.csproj` para a versao do seu editor.
4. Execute com `F5`. A cena principal atual e `res://scenes/levels/SideScrollerPrototype.tscn`.

## Controles

- `A/D` ou setas esquerda/direita: mover pela rua
- `W/S` ou setas cima/baixo: trocar lane/profundidade
- `J` ou botao esquerdo do mouse: atacar
- `L` ou botao direito do mouse: disparar
- `K` ou `Espaco`: esquivar
- `R`: reiniciar a arena

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

Sprint atual: `Sprint 08 - Conteudo Alpha`.

O prototipo lateral ja tem movimento por lanes, encontros de combate, combo, tiro, esquiva, HUD, ataque inimigo telegrafado, slash placeholder, flash de dano, knockback com hit-stun, hit pause curto, sangue placeholder, som placeholder de impacto e indicador visual de invulnerabilidade.

A Sprint 04 foi validada jogando e adicionou uma primeira passada de identidade visual na rua: asfalto molhado, reflexos, rachaduras, luzes de poste desenhadas, boteco/mercadinho fechado, pichacoes, fios, props urbanos, altar de rua e silhuetas melhores para protagonista e inimigo comum.

A Sprint 05 foi validada jogando e iniciou o sistema visual de clima/tempo com `TimeOfDayController`, `WeatherController`, ciclo simples de horario, chuva/garoa placeholder visivel, neblina, lama/poca visual, relampago e luzes reagindo ao horario.

A Sprint 06 foi validada jogando e implementou a primeira vertical slice pequena: entrada da rua, checkpoint, segundo encontro, mini-chefe placeholder, tela/mensagem de morte e vitoria via HUD.

A Sprint 07 foi validada jogando e iniciou os sistemas de jogo: save local, checkpoint persistente, arma improvisada com durabilidade, pickups de cura/arma/continue, continue simples, configuracao de HUD debug e remapeamento minimo. A organizacao de imports C# agora fica centralizada em `GlobalUsings.cs`.

## Regra obrigatoria de sprint

Toda sprint so pode ser marcada como concluida depois de build C# sem erros, validacao no Godot com `F5`, docs atualizados, commit e push.

## Proximo passo recomendado

Com a Sprint 07 concluida, o recomendado e planejar a Sprint 08:

- expandir conteudo alpha sem quebrar a vertical slice;
- planejar novos inimigos e mini-chefes;
- consolidar mais fases ou variacoes da primeira rua;
- manter build, validacao no Godot, commit e push ao fim da sprint.

## Documentos importantes

- `docs/PROJECT_BRIEF.md`: resumo do projeto, tecnologias e escopo.
- `docs/VISUAL_BIBLE.md`: direcao visual do jogo.
- `docs/WEATHER_TIME_SYSTEM.md`: clima, horario e rua viva.
- `docs/ART_PIPELINE.md`: fluxo de arte e referencias.
- `docs/GODOT_BEGINNER_GUIDE.md`: guia pratico para mexer no Godot.
- `docs/ROADMAP.md`: fases do projeto.
- `docs/SPRINTS.md`: planejamento das sprints e sprint atual.
- `docs/HANDOFF.md`: como continuar em Codex, CLI ou outro PC.
- `docs/AGENTS.md`: responsabilidades dos agentes.
