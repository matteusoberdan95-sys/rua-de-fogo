# Checklist QC - Demo Windows

Use este checklist antes de enviar build para testers ou subir material Steam.

## Build

- [ ] `dotnet build SangueNoAsfalto.csproj` com 0 erros
- [ ] Export Windows concluido (`build/windows/SangueNoAsfalto.exe`)
- [ ] Executavel abre sem depender do editor
- [ ] Versao exibida no menu: `Demo v1.0`

## Fluxo principal

- [ ] Menu inicial abre com `F5`/executavel
- [ ] `Enter` ou botao iniciam a demo
- [ ] Tutorial aparece sem pausar o jogo
- [ ] Checkpoint persiste apos fechar e reabrir
- [ ] `R` reinicia corretamente apos morte
- [ ] `M` volta ao menu nos estados finais
- [ ] Vitoria da demo aparece ao derrotar o chefe alpha

## Combate e sistemas

- [ ] Movimento A/D + W/S
- [ ] Ataque, tiro, esquiva e pulo
- [ ] Combo callout aparece com sequencia longa
- [ ] Barra de furia sobe ao acertar inimigos
- [ ] Pickups de cura, arma e continue funcionam
- [ ] Continue revive uma vez
- [ ] `F2` alterna controles no menu e no jogo
- [ ] `F4` limpa save

## Visual e performance

- [ ] Chuva e relampago aparecem
- [ ] Boteco, ponto final e props da Vila Esperanca visiveis
- [ ] Sem slow motion preso apos combates
- [ ] Sem crash ao morrer, vencer ou voltar ao menu

## Pacote para testers

- [ ] `build/demo/README.txt` incluido no zip
- [ ] Controles documentados
- [ ] Feedback channel definido (Discord, formulario, e-mail)
