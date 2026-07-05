# Pacote Demo Para Testers

## Conteudo do zip

```text
SangueNoAsfalto-Demo-v1.0/
  SangueNoAsfalto.exe
  README.txt
```

Se o export gerar arquivos extras (PCK, DLLs), inclua todos os arquivos produzidos pelo preset `Windows Desktop`.

## Como gerar

1. Rode `dotnet build SangueNoAsfalto.csproj`.
2. Exporte pelo Godot ou use `scripts/build-demo.ps1`.
3. Copie `build/windows/SangueNoAsfalto.exe` e dependencias para uma pasta limpa.
4. Copie `build/demo/README.txt` para a raiz do pacote.
5. Compacte como zip.

## README para testers

O arquivo base fica em `build/demo/README.txt`.

## Perguntas para feedback

- Os controles ficaram claros nos primeiros 2 minutos?
- A dificuldade parece justa antes e depois do checkpoint?
- A rua parece brasileira o suficiente?
- O HUD atrapalha ou ajuda?
- Encontrou bugs, travamentos ou soft lock?
- Você completaria uma versao maior do jogo?

## Canal de retorno

Defina um canal unico antes de distribuir:

- issue no GitHub;
- formulario Google;
- servidor Discord;
- e-mail dedicado.
