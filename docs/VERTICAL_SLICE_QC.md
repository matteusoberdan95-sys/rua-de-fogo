# Vertical Slice — QC Sprint 34

Checklist para validar a demo **Vila Esperanca** antes de compartilhar fora da maquina de dev.

Versao alvo: `Vertical Slice v1.1` (`DemoInfo.VersionLabel`).

## Build e export

- [ ] `dotnet build SangueNoAsfalto.csproj` — 0 erros
- [ ] Godot F5 — menu → tutorial → fase completa sem crash
- [ ] `scripts/build-demo.ps1` ou Export Windows → `build/windows/SangueNoAsfalto.exe`
- [ ] Executavel abre sem editor; versao correta no menu

## Fluxo vertical slice (~10 min)

- [ ] Menu → tutorial → fase lateral
- [ ] Checkpoint no altar (X ~480) persiste no save
- [ ] Morte com checkpoint: `R` volta ao altar
- [ ] Morte sem checkpoint: `R` volta ao inicio
- [ ] Mini-chefe (~2500), Rain Boss (~3020), Alpha Boss (~3120)
- [ ] Vitoria ao limpar area e chegar ao portao SAIDA
- [ ] `M` volta ao menu na morte/vitoria

## Combate e sistemas (baseline Sprints 27–31)

- [ ] Combo 4 golpes (J J J J) por estilo
- [ ] Segurar Q bloqueia; toque Q no `! PARRY !` = parry
- [ ] Corrida (duplo A/D) + J e dash K + J
- [ ] Pistola L, recarga E, sangramento
- [ ] Armas improvisadas + finishers
- [ ] XP desbloqueia estilos (Boxe, Muay Thai…)
- [ ] Continue (1x) ao morrer se houver pickup
- [ ] Sem slow-motion preso apos matar inimigos

## Cenario e clima (Sprints 32–33)

- [ ] Landmarks visiveis nos 5 atos (entrada → portao)
- [ ] Garoa → chuva → temporal conforme progresso
- [ ] Lama reduz velocidade (entrada / pocas)
- [ ] Poca eletrica + raio causa dano na tempestade
- [ ] Vento move lixo/papel na viela
- [ ] Apagao nos chefes Rain/Alpha
- [ ] Combate legivel com neblina/chuva

## HUD e capturas

- [ ] HUD compacto nao cobre combate (Sprint 30)
- [ ] `F9` oculta HUD para screenshot
- [ ] `F1` debug HUD (opcional)
- [ ] Capturas salvas em `marketing/screenshots/steam/` (manual)

## Pacote testers

- [ ] `build/demo/README.txt` atualizado no zip
- [ ] Controles documentados (Q parry, E reload, clima)
- [ ] Canal de feedback definido

## Regressao rapida (5 min smoke test)

1. Iniciar demo, matar 2 grunts, morrer, `R` reinicia.
2. Chegar ao checkpoint, fechar jogo, reabrir — checkpoint mantido.
3. Avancar ate mini-chefe — clima de chefe ativo.
4. `F9` — HUD some; `F9` de novo — HUD volta.
5. `M` no menu apos vitoria ou morte.

## Aprovacao

| Data | Tester | Resultado | Notas |
|------|--------|-----------|-------|
| | | | |

Quando todos os itens criticos estiverem OK, marcar Sprint 34 como **concluida** no Godot.

**Status:** concluida e validada em 06/07/2026.
