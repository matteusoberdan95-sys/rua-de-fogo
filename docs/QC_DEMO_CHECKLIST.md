# Checklist QC - Vertical Slice Windows

Use antes de enviar build para testers. Detalhes em `docs/VERTICAL_SLICE_QC.md`.

## Build

- [ ] `dotnet build SangueNoAsfalto.csproj` com 0 erros
- [ ] Export Windows (`build/windows/SangueNoAsfalto.exe`) ou `scripts/build-demo.ps1`
- [ ] Executavel abre sem editor
- [ ] Menu mostra `Vertical Slice v1.1 - Vila Esperanca`

## Fluxo

- [ ] Menu → tutorial → fase lateral
- [ ] Checkpoint persiste (save local)
- [ ] `R` reinicia apos morte (checkpoint ou inicio)
- [ ] `M` volta ao menu (morte/vitoria)
- [ ] Vitoria ao derrotar chefes e avancar ao portao SAIDA

## Combate

- [ ] A/D + W/S, corrida, dash, pulo
- [ ] Combo 4 golpes, block (Q), parry (toque Q)
- [ ] Pistola + recarga, postura, estilos por XP
- [ ] Pickups cura/arma/continue
- [ ] `F2` controles alternativos | `F4` limpar save

## Cenario e clima

- [ ] Landmarks por ato, chuva/temporal, pocas lama/eletrica
- [ ] Sem crash ao morrer, vencer ou voltar ao menu
- [ ] `F9` modo screenshot (HUD oculto)

## Pacote

- [ ] `build/demo/README.txt` no zip
- [ ] Canal de feedback definido
