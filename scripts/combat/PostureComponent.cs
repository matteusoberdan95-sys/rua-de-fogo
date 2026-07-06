namespace SangueNoAsfalto.Combat;

/// <summary>
/// Barra de postura estilo Sekiro — enche ao bloquear/parry; quebra abre golpe mortal.
/// </summary>
public partial class PostureComponent : Node
{
    [Signal]
    public delegate void PostureBrokenEventHandler();

    [Signal]
    public delegate void PostureChangedEventHandler(float current, float maximum);

    [Export]
    public float MaxPosture { get; set; } = 100f;

    [Export]
    public float RegenPerSecond { get; set; } = 22f;

    [Export]
    public float BrokenDuration { get; set; } = 3.2f;

    public float CurrentPosture { get; private set; }

    public bool IsBroken => _brokenRemaining > 0f;

    public float BrokenTimeRemaining => _brokenRemaining;

    private float _brokenRemaining;

    public override void _Process(double delta)
    {
        float dt = (float)delta;
        if (_brokenRemaining > 0f)
        {
            _brokenRemaining = Mathf.Max(_brokenRemaining - dt, 0f);
            if (_brokenRemaining <= 0f)
            {
                CurrentPosture = MaxPosture * 0.35f;
                EmitSignal(SignalName.PostureChanged, CurrentPosture, MaxPosture);
            }

            return;
        }

        if (CurrentPosture <= 0f)
        {
            return;
        }

        float before = CurrentPosture;
        CurrentPosture = Mathf.Max(CurrentPosture - RegenPerSecond * dt, 0f);
        if (!Mathf.IsEqualApprox(before, CurrentPosture))
        {
            EmitSignal(SignalName.PostureChanged, CurrentPosture, MaxPosture);
        }
    }

    public void AddPosture(float amount)
    {
        if (amount <= 0f || IsBroken)
        {
            return;
        }

        CurrentPosture = Mathf.Min(CurrentPosture + amount, MaxPosture);
        EmitSignal(SignalName.PostureChanged, CurrentPosture, MaxPosture);

        if (CurrentPosture >= MaxPosture)
        {
            BreakPosture();
        }
    }

    public void BreakPosture()
    {
        if (IsBroken)
        {
            return;
        }

        CurrentPosture = MaxPosture;
        _brokenRemaining = BrokenDuration;
        EmitSignal(SignalName.PostureBroken);
        EmitSignal(SignalName.PostureChanged, CurrentPosture, MaxPosture);
    }

    public void ResetPosture()
    {
        _brokenRemaining = 0f;
        CurrentPosture = 0f;
        EmitSignal(SignalName.PostureChanged, CurrentPosture, MaxPosture);
    }
}
