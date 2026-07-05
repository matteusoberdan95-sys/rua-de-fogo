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

## Observacoes

- A cena principal da demo e `res://scenes/ui/MainMenu.tscn`.
- O menu inicia a demo em `res://scenes/levels/SideScrollerPrototype.tscn`.
- A pasta `build/` nao deve ser commitada se contiver binarios exportados.
