# Fase 1 — Vila Esperança (plano)

## Objetivo

Primeira fase jogável com **rua viva**, props que quebram, leitura de periferia brasileira e progressão até o portão SAIDA (~3090).

Regra: cenario em **camadas nativas Godot** (poligonos/nodes), nunca bitmap colado.

## Atos da fase

| Trecho X | Nome | Clima sugerido | Conteudo |
|----------|------|----------------|----------|
| 0–480 | Entrada da vila | Madrugada chuvosa | Boteco do Zé, lixo, 2 grunts, crate quebravel |
| 480–1180 | Barraco do Martins | Chuva | Altar checkpoint, ponto onibus, mini horda |
| 1180–2050 | Rua central | Noite | Viatura, carro quebrado, placa, Brute + Fast |
| 2050–2780 | Viela estreita | Temporal | Cerca quebravel, infectados, pickup armas |
| 2780–3090 | Portão SAIDA | Neblina | Mini-boss + portao neon |

## Interatividade (Sprint 26+)

- [x] Props quebraveis com golpes (`BreakableStageProp`)
- [ ] Barril/pneu empurravel (knockback em StaticBody2D leve)
- [ ] Poste cai após N hits (bloqueia lane)
- [ ] Vidro do boteco estilhaça (particulas + som)
- [ ] Cerca abre caminho alternativo se quebrada
- [ ] Sangue persistente no asfalto onde cai combate

## Camadas visuais

1. **Far** — morros, casas, fios, janelas acesas
2. **Mid** — muros, boteco, placas, postes, calcada
3. **Near** — asfalto molhado, pocas, sangue, props gameplay
4. **FG** — lixo, saco, grades quebraveis (Z alto)

## Props por trecho (referencia)

## Sprint 29 - Passada Visual Implementada

- Lojas/comercios fechados extras: `BAR DO ZE`, `MERCADINHO`, `OFICINA`, `ACAI E LANCHES`.
- Rua com mais leitura de fase: calcada em placas, remendos de asfalto, buracos, marcas pintadas e pocas.
- Sujeira urbana: lixeira, sacos de lixo, papeis soltos, cacos de garrafa, sangue e marcas no chao.
- Identidade brasileira: pichacoes originais, varal, fios baixos, caixas eletricas e postes.
- Tudo segue em layers/nodes editaveis dentro de `LayeredStreetPrototype`, pronto para virar sprites/tiles finais depois.

## Sprint 32 - Landmarks e Pipeline de Assets

- **`StageAssetLibrary`** — boteco do Ze, mercadinho, poste amarelo + pool de luz, lixo, cerca, pocas.
- **`StageActLandmarks`** — 5 atos com composicao unica (entrada, Martins, central, viela, portao).
- Chao: meio-fio, faixa central tracejada, marcas de pneu, calcada tileada.
- Pipeline documentado em `docs/STAGE_ASSET_PIPELINE.md`; pasta `art/stage/vila-esperanca/`.

- Entrada: caixote, saco lixo, grade oxidada
- Central: garrafa boteco, placa VENDE-SE, mesa boteco
- Viela: placa rua, pneu, caixa de luz quebrada
- Boss: altar maior, portao SAIDA, graffiti boss

## Sprint 33 - Clima como Gameplay

- **`StageClimateDirector`** — garoa → chuva → temporal por ato; neblina no portao.
- **Pocas:** lama reduz velocidade; agua eletrica + raio = dano (evitar na tempestade).
- **Vento** empurra papeis/roupas; **apagao** nos chefes Rain Boss e Alpha.
- **SFX** procedural de trovao e chuva (`WeatherAmbience`).

## Audio / clima

- Chuva + neon pulsando no boteco
- Trovao a cada ~45s apos X=1180
- SFX madeira/vidro ao quebrar props

## Validacao

- Andar a fase inteira sem "fundo vazio"
- Quebrar pelo menos 3 props diferentes em um run
- Combate legivel com postes/luz guiando o olhar
