using Godot;

namespace SangueNoAsfalto.Core;

public partial class InputBootstrap : Node
{
    public override void _Ready()
    {
        ConfigureKeyAction("move_up", Key.W, Key.Up);
        ConfigureKeyAction("move_down", Key.S, Key.Down);
        ConfigureKeyAction("move_left", Key.A, Key.Left);
        ConfigureKeyAction("move_right", Key.D, Key.Right);
        ConfigureKeyAction("dash", Key.K, Key.Space);
        ConfigureKeyAction("attack", Key.J);
        ConfigureKeyAction("shoot", Key.L);
        ConfigureKeyAction("restart", Key.R);
        AddMouseButton("attack", MouseButton.Left);
        AddMouseButton("shoot", MouseButton.Right);
    }

    private static void ConfigureKeyAction(string action, params Key[] keys)
    {
        EnsureAction(action);

        foreach (Key key in keys)
        {
            if (HasKey(action, key))
            {
                continue;
            }

            InputMap.ActionAddEvent(action, new InputEventKey
            {
                Keycode = key,
                PhysicalKeycode = key,
            });
        }
    }

    private static void AddMouseButton(string action, MouseButton button)
    {
        EnsureAction(action);

        foreach (InputEvent inputEvent in InputMap.ActionGetEvents(action))
        {
            if (inputEvent is InputEventMouseButton mouseButton && mouseButton.ButtonIndex == button)
            {
                return;
            }
        }

        InputMap.ActionAddEvent(action, new InputEventMouseButton
        {
            ButtonIndex = button,
        });
    }

    private static bool HasKey(string action, Key key)
    {
        foreach (InputEvent inputEvent in InputMap.ActionGetEvents(action))
        {
            if (inputEvent is InputEventKey eventKey &&
                (eventKey.Keycode == key || eventKey.PhysicalKeycode == key))
            {
                return true;
            }
        }

        return false;
    }

    private static void EnsureAction(string action)
    {
        if (!InputMap.HasAction(action))
        {
            InputMap.AddAction(action);
        }
    }
}
