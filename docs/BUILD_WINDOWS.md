# Build Windows

Este projeto usa Godot 4 .NET. O preset inicial de exportacao fica em `export_presets.cfg`.

## Pelo Editor

1. Abra o projeto no Godot .NET.
2. Confirme que os templates de exportacao estao instalados.
3. Acesse `Project > Export`.
4. Selecione `Windows Desktop`.
5. Exporte para `build/windows/SangueNoAsfalto.exe`.

## Pelo CLI

Quando o executavel do Godot estiver disponivel no `PATH`, rode na raiz do projeto:

```bash
godot --headless --export-release "Windows Desktop" "build/windows/SangueNoAsfalto.exe"
```

Antes de exportar, rode:

```bash
dotnet build SangueNoAsfalto.csproj
```

## Pelo script

Na raiz do projeto:

```powershell
./scripts/build-demo.ps1
```

Se o Godot CLI nao estiver no `PATH`, o script ainda valida o build C# e orienta export manual.

## Pacote para testers

Depois do export:

1. Copie o executavel e dependencias para uma pasta limpa.
2. Inclua `build/demo/README.txt`.
3. Siga `docs/DEMO_PACKAGE.md` e `docs/QC_DEMO_CHECKLIST.md`.

Versao atual da demo: `1.0.0-demo` (`Demo v1.0` no menu).
