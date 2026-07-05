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

Sprint atual: `Sprint 02 - Prototipo Beat 'em Up`.

Direcao atual: migrar do prototipo top-down para uma cena lateral/2.5D beat 'em up, mantendo o top-down como laboratorio de sistemas.

Pilar novo: clima, horario e rua viva fazem parte da identidade do jogo. Nao tratar chuva, noite, vento e tempestade como simples filtro visual.

## Goku - Gameplay

Responsavel por movimento, ataque, esquiva, stamina, camera e sensacao de controle.

Foco atual:

- deixar o combate pesado e responsivo;
- melhorar feedback de acerto;
- ajustar velocidade, dash, stamina e combo;
- garantir que cada mudanca continue jogavel.

Proximas entregas:

- movimento lateral com lanes;
- camera lateral seguindo o jogador;
- adaptar combo para beat 'em up;
- testar dash no formato lateral.

## Vegeta - Inimigos

Responsavel por IA, comportamento agressivo, perseguicao, ataque, dano e variacoes de inimigos.

Foco atual:

- criar inimigo comum melhor;
- criar inimigo rapido;
- criar inimigo bruto;
- adicionar ataques telegrafados.

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
- cuidar de Git e versoes.

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
- definir protagonista, inimigos e mundo.

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
- garantir que a Sprint 02 ja prepare espaco para luzes, chuva e fundo em camadas;
- pensar clima como gameplay e narrativa, nao so decoracao.

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
