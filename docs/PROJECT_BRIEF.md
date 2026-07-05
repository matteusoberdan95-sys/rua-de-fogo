# Project Brief - Sangue no Asfalto

## O Que Estamos Construindo

`Sangue no Asfalto` e um jogo 2D/2.5D de acao dark fantasy, beat 'em up/hack and slash, sangrento e urbano, ambientado em um suburbio brasileiro tomado por uma praga sobrenatural.

O jogo mistura:

- energia de jogos 2D dos anos 90;
- combate moderno com peso, dash, stamina e feedback forte;
- atmosfera dark, violenta e opressiva;
- identidade brasileira suburbana;
- tecnologia atual de luz, sombra, particulas e som;
- clima e ciclo de tempo como parte da jogabilidade e da narrativa.

## Referencias De Sensacao

Estas referencias ajudam a guiar clima e linguagem, mas nao devem ser copiadas:

- jogos 2D de acao dos anos 90;
- jogos com arte religiosa/gotica e violencia estilizada;
- run and gun arcade com leitura clara de tela;
- soulslike como referencia de tensao, risco e peso.

Regra importante: referencia nao e copia. O jogo precisa ter identidade propria.

## Tecnologia

Engine:

- Godot 4.7 .NET

Linguagem principal:

- C#

Plataforma alvo inicial:

- Windows
- Steam futuramente

Arte:

- PureRef para moodboard e referencias;
- Krita para concept art e pintura 2D;
- Aseprite se decidirmos seguir pixel art/sprite sheets;
- Godot para montar cenas, luzes, colisao, HUD e efeitos.

Mundo dinamico:

- horarios como madrugada, amanhecer, manha, tarde, por do sol e noite;
- clima como garoa, chuva forte, vento, neblina, calor seco e tempestade eletrica;
- fases, inimigos e bosses combinando com o estado da rua;
- sangue, agua, lama, luz e sombra reagindo visualmente em camadas.

Backend:

- fora do escopo inicial;
- possivel uso futuro de .NET, Postgres e API propria se houver ranking, contas, telemetria, eventos ou servicos externos.

## Escopo Atual

O prototipo atual tem:

- jogador com movimento;
- ataque corpo a corpo;
- combo simples;
- tiro basico;
- stamina;
- dash com invulnerabilidade;
- inimigos perseguidores;
- ondas de inimigos;
- HUD;
- reinicio rapido;
- arena suburbana placeholder.

Observacao: o prototipo atual usa camera top-down/arena para validar sistemas. A direcao final recomendada e migrar para camera lateral/2.5D beat 'em up.

## Proxima Meta

Transformar o prototipo funcional em uma vertical slice pequena:

- nova arena lateral/2.5D visualmente mais bonita;
- luz 2D com postes;
- props urbanos;
- primeira passada simples de clima/horario visual;
- inimigos mais legiveis;
- feedback de hit;
- sangue e particulas;
- checkpoint funcional;
- mini-chefe simples;
- menu inicial;
- build Windows.

## Filosofia Do Projeto

Primeiro fazer o jogo funcionar.

Depois fazer o jogo ficar bom.

Depois fazer o jogo ficar bonito.

Depois fazer o jogo ficar vendavel.

Nao vamos tentar construir o jogo completo de uma vez. Cada sprint precisa deixar algo jogavel.
