# Agentes Do Projeto

Os agentes usam nomes inspirados em Dragon Ball Z apenas como apelidos internos de organizacao. Esses nomes nao devem aparecer no produto final, pagina da Steam, arte comercial ou materiais publicos.

## Fonte Da Verdade

Antes de qualquer tarefa grande, todo agente deve consultar:

- `docs/PROJECT_BRIEF.md`
- `docs/SPRINTS.md`
- `docs/VISUAL_BIBLE.md`
- `docs/WEATHER_TIME_SYSTEM.md`
- `docs/ARCHITECTURE.md`
- `docs/HANDOFF.md`

Sprint atual: `Sprint 30 - Legibilidade, Camera e HUD` (implementada / aguardando validacao no Godot).

Direcao de combate: `docs/COMBAT_DESIGN.md` — combo 4 golpes por estilo (`MoveCatalog`), defesa com Q (segurar = block, toque = parry), postura estilo Sekiro.

Direcao visual atual: Sprint 29 validou a rua viva como caminho; Sprint 30 implementou leitura, camera, HUD compacto, chuva menos poluida e contraste dos personagens antes de novos sistemas.

Direcao atual: cena lateral/2.5D beat 'em up como caminho principal, mantendo o top-down como laboratorio antigo de sistemas.

Pilar novo: clima, horario e rua viva fazem parte da identidade do jogo. Nao tratar chuva, noite, vento e tempestade como simples filtro visual.

Regra visual atual: imagens de referencia guiam o resultado, mas nao devem virar imagem chapada no gameplay ativo. Personagens e cenario devem caminhar para rigs/sprite sheets/layers separados.

## Regra Obrigatoria De Sprint

Nenhum agente deve marcar uma sprint como concluida sem:

- `dotnet build SangueNoAsfalto.csproj` com 0 erros;
- validacao no Godot com `F5`;
- docs atualizados;
- commit criado;
- push feito para o remoto.

Se faltar validacao no Godot, manter a sprint como `implementada / aguardando validacao`.

## Goku - Gameplay

Responsavel por movimento, ataque, esquiva, stamina, camera e sensacao de controle.

Foco atual:

- deixar o combate pesado e responsivo;
- melhorar feedback de acerto;
- ajustar velocidade, dash, stamina e combo;
- garantir que cada mudanca continue jogavel.
- validar se a camera aberta da Sprint 30 ainda preserva peso e impacto dos golpes.

Proximas entregas:

- movimento lateral com lanes;
- camera lateral seguindo o jogador;
- adaptar combo para beat 'em up;
- testar dash no formato lateral.

## Vegeta - Inimigos

Responsavel por IA, comportamento agressivo, perseguicao, ataque, dano e variacoes de inimigos.

Foco atual:

- manter inimigos em camadas ou sprite sheets limpas;
- garantir que Quebra-Osso nao pareca recorte;
- preparar variacoes futuras com o mesmo contrato visual;
- preservar ataques telegrafados.

Proximas entregas:

- inimigo entrando pelas laterais;
- perseguicao em lanes;
- ataque frontal com alcance claro;
- variacao simples de inimigo rapido.

## Piccolo - Arquitetura

Responsavel por organizacao de cenas, scripts, padroes de codigo, revisao tecnica e separacao de responsabilidades.

Foco atual:

- manter cenas e scripts separados;
- evitar acoplamento desnecessario;
- garantir que `CharacterSpriteVisual` e `LayeredStreetPrototype` sejam ponte para arte final;
- documentar caminhos importantes;
- revisar mudancas antes de escalar o projeto.

Proximas entregas:

- separar codigo top-down de codigo lateral quando necessario;
- evitar quebrar `PrototypeArena.tscn`;
- definir estrutura da cena `SideScrollerPrototype.tscn`.

## Bulma - Ferramentas

Responsavel por setup, build, exportacao, Git, automacoes e pipeline.

Foco atual:

- manter Godot .NET funcionando;
- organizar pastas de arte e audio;
- preparar build Windows;
- cuidar de Git e versoes;
- manter docs/README/HANDOFF atualizados antes de trocar de maquina.
- garantir que cada sprint visual termine com docs atualizados, build validado e push.

Proximas entregas:

- garantir fluxo Codex Desktop / CLI / outro PC;
- manter `docs/HANDOFF.md` atualizado;
- preparar rotina de build e validacao;
- sugerir commits por sprint.

## Gohan - Design

Responsavel por GDD, lore, missoes, tom narrativo, personagens e mundo.

Foco atual:

- consolidar a identidade do jogo;
- manter a biblia visual;
- transformar referencias em conceitos originais;
- definir protagonista, inimigos e mundo;
- impedir que mockups de referencia sejam tratados como arte final.
- cobrar que a Vila Esperanca conte historia visual, nao seja so fundo escuro.

Proximas entregas:

- consolidar Caua como protagonista base;
- definir Vila Esperanca como primeira fase;
- transformar referencias em descricoes praticas de assets;
- relacionar cada fase, inimigo e boss com horario/clima;
- manter nomes, faccoes e bosses originais.

## Trunks - Interface

Responsavel por HUD, menus, inventario, feedback visual e telas de sistema.

Foco atual:

- melhorar HUD;
- criar tela de morte;
- criar menu inicial;
- mostrar feedback claro sem poluir a tela.
- manter o HUD compacto da Sprint 30 e evitar que debug/tecnicas cubram a acao.

Proximas entregas:

- adaptar HUD para beat 'em up lateral;
- criar estilo de barra de vida/stamina/furia;
- planejar tela de morte;
- planejar menu inicial.

## Freeza - Bosses E Balanceamento

Responsavel por chefes, dificuldade, curvas de progressao, dano, vida, janelas de ataque e ritmo.

Foco atual:

- balancear ondas;
- definir mini-chefe da vertical slice;
- ajustar dano, vida e ritmo de combate.

Proximas entregas:

- balancear Sprint 02;
- definir mini-chefe da primeira rua;
- planejar ataques telegrafados;
- controlar dificuldade sem frustrar cedo demais.

## Shenlong - Clima, Tempo E Rua Viva

Responsavel por ciclo de horario, clima, tensao da rua, eventos ambientais e relacao entre fase, inimigos e boss.

Foco atual:

- manter `docs/WEATHER_TIME_SYSTEM.md` como fonte do sistema;
- garantir que a rua viva tenha luzes, chuva e fundo em camadas;
- pensar clima como gameplay e narrativa, nao so decoracao.
- validar a reducao de chuva/vignette da Sprint 30 sem perder clima dark.

Proximas entregas:

- definir estados base de horario;
- definir modulos de clima;
- planejar `TimeOfDayController` e `WeatherController`;
- criar primeiro prototipo visual de manha/tarde/noite;
- testar chuva, relampago e zonas de lama em uma cena pequena.

## Cell - Backend Futuro

Responsavel por API, Postgres, telemetria, rankings, saves externos, integracoes e servicos auxiliares quando fizer sentido.

Foco atual:

- nao construir backend ainda;
- registrar ideias futuras;
- entrar somente quando houver necessidade real.

Proximas entregas:

- manter backend fora da Sprint 02;
- documentar ideias futuras sem implementar;
- avaliar save local antes de qualquer API.

