using Content.Shared._Afterlight.Generator;
using JetBrains.Annotations;
using Robust.Client.GameObjects;

namespace Content.Client._Afterlight.Generator;

[UsedImplicitly]
public sealed class SolidFuelGeneratorBoundUserInterface : BoundUserInterface
{
    private GeneratorWindow? _window;

    public SolidFuelGeneratorBoundUserInterface(ClientUserInterfaceComponent owner, Enum uiKey) : base(owner, uiKey)
    {
    }

    protected override void Open()
    {
        base.Open();
        _window = new GeneratorWindow(this, Owner.Owner);

        _window.OpenCenteredLeft();
        _window.OnClose += Close;
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        _window?.Update((SolidFuelGeneratorComponentBuiState)state);
    }

    protected override void Dispose(bool disposing)
    {
       _window?.Dispose();
    }

    public void SetTargetPower(int target)
    {
        SendMessage(new SetTargetPowerMessage(target));
    }
}
