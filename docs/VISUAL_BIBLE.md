# Biblia Visual - Sangue no Asfalto

## Direcao Geral

`Sangue no Asfalto` deve parecer um jogo 2D sombrio dos anos 90 reimaginado com tecnologia atual. A leitura visual vem de silhuetas fortes, poses exageradas, cenarios laterais/top-down dramaticos, violencia estilizada e contraste alto. A identidade precisa ser brasileira, urbana e sobrenatural.

As referencias de jogos como hack and slash gotico, run and gun arcade e classicos dos anos 90 servem como linguagem: impacto visual, ritmo, exagero, leitura imediata e atmosfera. O jogo final nao deve copiar personagens, iconografia, composicoes, HUDs, sprites, nomes, faccoes, bosses ou artes reconheciveis de outras obras.

## Frase Visual

Um suburbio brasileiro em noite de apagao, onde postes amarelos, asfalto molhado, muros pichados e sangue escuro revelam uma praga sobrenatural que transforma gente comum em monstros violentos.

## Estilo

**Decisao travada (Sprint 13):** pintura 2D com pixel aparente — brush visivel, silhuetas fortes, export PNG do Krita com filtro `Nearest` no Godot. Nao e pixel art pura de grid fixo.

- 2D com camera lateral/2.5D em estilo beat 'em up para a direcao final.
- O prototipo top-down atual fica como laboratorio de combate, IA e sistemas.
- Pixel art de alta resolucao ou pintura 2D com pixels aparentes.
- Animacoes expressivas, com poucos frames bem marcados no inicio.
- Luz moderna: halos de poste, sombras duras, rim light em personagens e brilho sobrenatural.
- Sangue estilizado, escuro e legivel, sem realismo fotografico.

## Decisao De Camera

Direcao recomendada: side-scrolling beat 'em up 2.5D.

Motivos:

- valoriza muito mais personagens, roupas, armas e poses;
- combina melhor com a energia arcade dos anos 90;
- permite fases urbanas cinematograficas com profundidade, postes, fios, barracos, bares e vielas;
- deixa o combate sangrento mais legivel e impactante;
- e mais proximo das referencias visuais atuais do projeto.

Modelo de gameplay:

- camera lateral;
- movimento horizontal pela fase;
- movimento vertical curto para trocar de "lane" ou profundidade;
- combate corpo a corpo frontal;
- armas improvisadas pegaveis;
- inimigos entrando pelas laterais, fundos e plataformas;
- fases com camadas: rua, calcada, telhado, viela e escadaria.

O top-down ainda pode ser usado para prototipos internos, mas a vertical slice visual deve migrar para beat 'em up lateral.

## Paleta Base

- Preto asfalto: `#0B0D0E`
- Concreto frio: `#2B3033`
- Cinza fumaca: `#5B6265`
- Luz de poste: `#D6A13A`
- Vermelho sangue escuro: `#5A0508`
- Vermelho impacto: `#B81414`
- Branco arma/osso: `#D8D1B7`
- Verde doente/sobrenatural: `#6A8F3A`

Regra: o jogo deve ser escuro, mas nao ilegivel. Personagem, inimigo, ataque e perigo precisam ser lidos em menos de meio segundo.

## Cenario

Elementos principais:

- rua suburbana molhada;
- calcada quebrada;
- muros com pichacao original;
- boteco fechado;
- mercadinho com porta de aco;
- poste falhando;
- fiacao baixa;
- sacos de lixo;
- santinho rasgado, vela apagada, altar improvisado;
- valas, bueiros e marcas de sangue;
- carros velhos parcialmente destruidos.

O cenario deve parecer brasileiro sem virar caricatura. O foco e rua, periferia, concreto, improviso, religiosidade popular e decadencia sobrenatural.

## Clima E Tempo

O ciclo de horario e clima e um pilar visual do jogo.

Estados desejados:

- madrugada com pouca luz e baixa movimentacao;
- amanhecer com neblina, rua molhada e tensao silenciosa;
- manha com comercio abrindo, civis e rotas mais visiveis;
- tarde quente com poeira, calor e inimigos mais expostos;
- por do sol com luz vermelha, sombras longas e escalada de perigo;
- noite com neon, bares, emboscadas e violencia maior;
- temporal noturno com raios, alagamento, queda de luz e caos.

Cada fase deve ter uma relacao clara com horario e clima. O objetivo nao e trocar filtro de cor, e fazer a rua parecer viva.

Regras:

- clima deve reforcar boss, inimigos e historia da fase;
- chuva deve revelar reflexos, pocas e sangue escorrendo;
- vento deve mexer lixo, faixas, roupas e particulas;
- raio e queda de luz devem ser dramatizados, mas legiveis;
- luz natural e artificial devem guiar o olhar do jogador.

Documento detalhado: `docs/WEATHER_TIME_SYSTEM.md`.

## Protagonista

Silhueta:

- corpo humano normal, nada heroico demais;
- jaqueta escura ou camisa rasgada;
- bandagem, pano vermelho ou detalhe claro para leitura;
- arma branca improvisada: facao grande, vergalhao, machete, foice curta ou lamina quebrada;
- postura cansada, agressiva e resiliente.

Pilar atual:

- Caua como protagonista base: jovem trabalhador dos arredores, visual de rua, corpo machucado, roupa rasgada, raiva contida e armas improvisadas.
- Ele precisa parecer humano e vulneravel, mas perigoso quando entra em combate.

Direcao:

- Caua nao pode parecer uma imagem colada ou recorte de concept sheet.
- Idle precisa mostrar respiracao, cansaco, tensao e algum micro movimento no cabelo/roupa.
- Caminhada precisa ter peso, ombro e arma atrasando o corpo.
- Golpes precisam ter antecipacao, impacto e follow-through.
- Low health deve mudar postura, nao apenas barra de vida.
- Sangue/machucados devem reforcar drama sem destruir leitura da silhueta.

- nao parecer cavaleiro medieval;
- nao parecer anime famoso;
- nao parecer soldado tatico;
- deve parecer alguem da rua empurrado para um horror impossivel.

## Inimigos

Inimigo comum:

- humano corrompido;
- pele escura/vermelha;
- olhos amarelos ou verdes;
- movimentos quebrados;
- ataque simples com garra, faca ou mordida.

Inimigo rapido:

- magro, baixo, quase animalizado;
- salta ou avanca em linha reta;
- pouca vida, alto perigo.

Inimigo bruto:

- grande, lento;
- usa pedaco de concreto, placa de rua ou portao como arma;
- ataque telegrafado.

Mini-chefe:

- ex-morador/figura de bairro corrompida;
- mistura de corpo humano, fios, ferragens e simbolos religiosos quebrados;
- ataques de area e invocacao de inimigos menores.

## Combate

O combate deve combinar peso moderno com clareza arcade:

- golpe corpo a corpo com impacto forte;
- combo curto de 3 ataques;
- esquiva com risco e tempo de recarga por stamina;
- tiro/energia como recurso secundario, nao arma principal;
- inimigos telegrafam ataques com luz, pose ou som;
- sangue e knockback confirmam acerto;
- morte do jogador deve parecer justa, nao confusa.

## Iluminacao

Principios:

- luz de poste amarela cria areas seguras/visiveis;
- cantos escuros escondem spawns e detalhes;
- inimigos podem ter olhos/luzes pequenas;
- ataques do jogador brilham em branco quente ou vermelho;
- bosses usam luz sobrenatural propria.

No Godot, usar futuramente:

- `PointLight2D` para postes;
- sombras 2D em paredes/obstaculos;
- materiais simples com emissao para olhos, sangue e magia;
- particulas para fagulhas, poeira e sangue.

## HUD

O HUD deve ser limpo e agressivo:

- vida em vermelho escuro;
- stamina em amarelo sujo;
- contador de onda pequeno;
- mensagens curtas;
- nada de menu grande durante combate.

Visual: metal gasto, concreto, vermelho seco, linhas retas, pouca ornamentacao.

## Audio

Direcao sonora:

- grave industrial baixo;
- zunido eletrico de poste;
- distante de sirene, cachorro, chuva, transformador;
- golpes secos e pesados;
- inimigos com respiracao rasgada;
- musica com percussao lenta, baixo e texturas sombrias.

## Regras De Originalidade

- Nao copiar sprites, bosses, personagens, interfaces ou composicoes de jogos existentes.
- Nao usar nomes de animes/jogos dentro do produto final.
- Referencias sao para linguagem visual e sensacao, nao para reproduzir arte.
- Toda iconografia religiosa/sobrenatural deve ser criada para o universo do jogo.

## Primeira Meta Visual

Substituir os placeholders atuais por uma arena com:

- asfalto com textura escura;
- tres postes com luz;
- paredes/muros com colisao;
- boteco fechado como landmark;
- protagonista com sprite simples mas reconhecivel;
- inimigo comum com silhueta clara;
- sangue e efeitos de hit.

## Referencias Pilar Salvas

As referencias principais estao em `references/pillars/`:

- `01-character-lineup-caua-enemies-boss.png`
- `02-stage-vila-esperanca-side-scroller.png`
- `03-stage-vila-santana-layered.png`
- `04-hud-combat-ui-weapons.png`
- `05-supporting-cast-allies.png`
- `06-bosses-corruption-network.png`
- `07-weather-time-system.png`
- `08-dynamic-world-progression.png`
- `09-scenario-boss-variants.png`
