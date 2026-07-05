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
