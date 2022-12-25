﻿using System.Linq;
using Content.Client._Afterlight;
using Content.Client.Maps;
using Content.Client.UserInterface;
using Content.Client.UserInterface.Controls;
using Content.Shared._Afterlight.Worldgen;
using Robust.Client.AutoGenerated;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.XAML;
using Robust.Shared.Console;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Client._Afterlight.Latejoin;

[GenerateTypedNameReferences]
public sealed partial class NewVesselGui : FancyWindow
{
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly IConsoleHost _consoleHost = default!;
    [Dependency] private readonly IResourceCache _resourceManager = default!;
    [Dependency] private readonly IEntityManager _entity = default!;

    public NewVesselGui()
    {
        IoCManager.InjectDependencies(this);
        RobustXamlLoader.Load(this);
        foreach (var map in _prototypeManager.EnumeratePrototypes<GameMapPrototype>())
        {
            if (!map.ValidShip)
                continue;

            ShuttleList.Add(new ItemList.Item(ShuttleList)
            {
                Text = map.MapName,
                Metadata = map
            });
        }

        VesselDescription.SetMessage("Select a vessel.");

        ShuttleList.OnItemSelected += ShuttleListOnOnItemSelected;
        PurchaseShipButton.OnPressed += PurchaseShipButtonOnOnPressed;
    }

    private void PurchaseShipButtonOnOnPressed(BaseButton.ButtonEventArgs obj)
    {
        var id = ((GameMapPrototype) ShuttleList.GetSelected().First().Metadata!).ID;
        _entity.EntityNetManager!.SendSystemNetworkMessage(new RequestShipSpawnEvent(id));
        Close();
    }

    private void ShuttleListOnOnItemSelected(ItemList.ItemListSelectedEventArgs obj)
    {
        PurchaseShipButton.Disabled = false;
        var map = (GameMapPrototype)ShuttleList.GetSelected().FirstOrDefault()!.Metadata!;
        if (!string.IsNullOrEmpty(map.Description))
            VesselDescription.SetMessage(FormattedMessage.FromMarkup(map.Description));
        else
            VesselDescription.SetMessage("No description...");
    }
}
