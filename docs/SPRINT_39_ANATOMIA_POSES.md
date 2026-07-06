# Sprint 39 - Anatomia e Poses Anti-South-Park

Status: implementada / build validado / aguardando validacao F5 no Godot.

Data: 06/07/2026.

## Motivo

Playtest da Sprint 38 mostrou que o projeto evoluiu em sistemas e cenario, mas os personagens ainda estavam com leitura visual fraca: cabeca grande, membros curtos, caminhada travada e golpes sem extensao clara. O feedback do usuario foi que parecia boneco rigido e ainda longe das referencias de `references/`.

## Direcao

Continuamos obedecendo `docs/VISUAL_RULE.md`: referencias nao viram PNG no gameplay. O runtime segue em rig 2D procedural (`Polygon2D` + codigo), mas com silhueta mais adulta e poses que vendem melhor o impacto.

## Entregas

- presets revisados com corpos mais altos, cabecas menores e rig menos achatado;
- torso, coxa, canela, braco e antebraco alongados;
- tenis/botas reposicionados para acompanhar pernas mais longas;
- caminhada/corrida com passada maior, lift mais claro, joelho dobrando mais e balanco de bracos menos travado;
- guarda do Caua separada do braco que esta atacando para nao sobrescrever golpes;
- jab e cross com lunge maior e extensao mais clara de ombro/cotovelo;
- side kick com perna mais esticada, camara mais legivel e impacto mais distante.

## Validacao No Godot

Build: `dotnet build SangueNoAsfalto.csproj` validado com 0 erros e 0 avisos.

Testar em F5:

- Caua parado: deve parecer mais alto/atlético e menos cabecudo;
- caminhada: deve ter passada mais clara, sem virar boneco deslizante;
- `J` no estilo Rua/Boxe: jab/cross devem esticar o braco;
- chute lateral/teep/chutes: perna deve alcançar mais longe e dobrar no preparo;
- inimigos: devem manter identidade, mas menos bloco empilhado.

## Proxima Sprint Recomendada

Sprint 40 - Personagem Vivo e Cidade Viva:

- animacao secundaria de cabelo, regata, faixa/roupa e respiracao;
- microposes de recoil, stun, postura quebrada e recuperacao;
- mais props narrativos na Vila Esperanca com variação por ato;
- revisar HUD em combate cheio para nao cobrir personagens;
- primeiro passe de arte final real: desenhar 1 pose de Caua por cima do rig para guiar proporcoes futuras.
