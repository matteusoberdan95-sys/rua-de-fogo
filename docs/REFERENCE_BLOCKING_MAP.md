# Mapa Referencia → Blocking 2D

Pranchas em `references/` guiam **silhueta, cor e pose**. Runtime = `Polygon2D` + codigo no Godot. **Zero PNG no gameplay.**

## Personagens (`references/personagens_ref/` prancha 1)

| Preset | Referencia | Silhueta em bloco |
|--------|------------|-------------------|
| `Caua` | Caua | Regata vermelha, cargo preto, tenis vermelho, buzz cut, faca caseira |
| `QuebraOsso` | Quebra-Osso | Careca, regata oliva, camo, tubo de ferro no ombro |
| `Fast` | Variante menor | Mesmo blocking, escala 0.84 |
| `Infected` | Acougueiro | Avental branco manchado, facao |
| `Brute` | Grande | Camisa onca amarela, calca clara, soqueira |
| `MiniBoss` | Capitao Sangrento | Terno escuro, faixa verde-amarela, bastao/revolver |

## Golpes (pose da prancha)

| Personagem | Ataque padrao |
|------------|---------------|
| Caua (estilo Rua) | Corte horizontal com faca |
| Quebra-Osso / Fast | Tubo overhead |
| Acougueiro | Facada com facao |
| Grande | Soco com impacto |
| Capitao | Bastonada / tiro |

## Cenario (`references/pillars/02-stage-vila-esperanca-side-scroller.png`)

| Marco | Blocking |
|-------|----------|
| Boteco do Ze | `StageAssetLibrary.BuildBotecoDoZe` + mesas + cadeiras vermelhas + neon SKOL |
| Ponto de onibus | `BuildBusShelter` — "PONTO FINAL VILA ESPERANCA" + poster RAFAEL |
| Carro estacionado | `BuildParkedCar` — Gol velho na rua |
| Cachorro | `BuildStrayDog` — hazard ambiente |
| Favela ao fundo | `LayeredStreetPrototype.AddFarFavela` + sirene policial |
| Poste quente | `BuildStreetPole` — cone de luz ambar no asfalto |
