using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ProspectingPlus.ModSystem;
using ProspectingPlus.Shared.Extensions;
using ProspectingPlus.Shared.Packets;
using Vintagestory.API.Client;
using Vintagestory.API.Config;
using Vintagestory.API.Util;
using Vintagestory.GameContent;

namespace ProspectingPlus.Client.GUI
{
    public class ProspectingPlusGuiDialog : GuiDialog
    {
        private static readonly MethodInfo _composeDropDown =
            typeof(GuiElementDropDown).GetMethod("ComposeCurrentValue", BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly FieldInfo _switchesField =
            typeof(GuiElementListMenu).GetField("switches", BindingFlags.Instance | BindingFlags.NonPublic);

        private readonly ProspectingOverlayLayer _overlayLayer;
        private readonly ProspectingPlusClient _client;

        private bool _isSetup;
        private List<string> _oreList;

        public override string ToggleKeyCombinationCode => null;

        // TODO: Implement toggling regular chat printout for the propick.
        // TODO: Implement alpha slider for the overlay.

        public ProspectingPlusGuiDialog(ICoreClientAPI capi) : base(capi)
        {
            _overlayLayer = (ProspectingOverlayLayer) capi.ModLoader
                .GetModSystem<WorldMapManager>()
                .MapLayers
                .FirstOrDefault(x => x is ProspectingOverlayLayer);
            _client = capi.ModLoader.GetModSystem<ProspectingPlusSystem>().Client;
        }

        private void SetupDialog()
        {
            _isSetup = true;

            var font = CairoFont.WhiteSmallText();

            var dialogBounds = ElementStdBounds.AutosizedMainDialog.WithAlignment(EnumDialogArea.RightTop)
                .WithFixedAlignmentOffset(-GuiStyle.DialogToScreenPadding, GuiStyle.DialogToScreenPadding);

            var textBounds = ElementBounds.Fixed(0, 1, 175, 25);
            var switchBounds = ElementBounds.Fixed(180, 0, 20, 25);

            var bgBounds = ElementBounds.Fill.WithFixedPadding(GuiStyle.ElementToDialogPadding);
            bgBounds.BothSizing = ElementSizing.FitToChildren;

            var composer = capi.Gui.CreateCompo("prospectingPlusConfigGui", dialogBounds)
                .AddShadedDialogBG(bgBounds, false)
                .BeginChildElements(bgBounds);
            composer.AddStaticText("Enable Chat Reports?", font, textBounds);
            composer.AddSwitch(
                value => { _client.ClientState.TextReportsEnabled = value; }, 
                switchBounds, 
                "enableChatReports", 
                25);
            textBounds = textBounds.BelowCopy(fixedDeltaY: 10);
            switchBounds = switchBounds.BelowCopy(fixedDeltaY: 10);
            composer.AddStaticText(
                "Overlay Opacity (%)", 
                font, 
                textBounds);
            composer.AddSlider(
                value =>
                {
                    _client.ClientState.OverlayOpacityPercent = value;
                    _overlayLayer.ModifyAlpha(value);
                    return true;
                },
                switchBounds.FlatCopy().WithFixedWidth(170),
                "opacitySlider");
            var buttonBounds = textBounds.BelowCopy(fixedDeltaY: 10).WithFixedWidth(350);
            composer.AddSmallButton("Toggle All", () => ToggleAllOres(true), buttonBounds);
            buttonBounds = buttonBounds.BelowCopy(fixedDeltaY: 5);
            composer.AddSmallButton("Toggle None", () => ToggleAllOres(false), buttonBounds);
            buttonBounds = buttonBounds.BelowCopy(fixedDeltaY: 10);
            composer.AddMultiSelectDropDown(
                _oreList.ToArray(),
                _oreList.Select(x => Lang.Get(x)).ToArray(),
                1,
                (code, selected) => { UpdateMapFilter(); },
                buttonBounds,
                "oreList");
            buttonBounds = buttonBounds.BelowCopy(fixedDeltaY: 10);
            var densityDict = OreDensityExtensions.GetOreDensityStrings();
            composer.AddDropDown(
                densityDict.Select(x => x.Key).ToArray(),
                densityDict.Select(x => x.Value).ToArray(),
                densityDict.Select(x => x.Key).ToArray().IndexOf(_client.ClientState.SelectedMinimumDensity.ToString()),
                (code, selected) => UpdateMapFilter(),
                buttonBounds,
                "densityList");
            SingleComposer = composer.EndChildElements().Compose();
            SingleComposer.GetSwitch("enableChatReports")
                .SetValue(_client.ClientState.TextReportsEnabled);
            SingleComposer.GetSlider("opacitySlider")
                .SetValues(_client.ClientState.OverlayOpacityPercent, 0, 100, 1, "%");
            ToggleSavedOres();
        }

        public override bool TryOpen()
        {
            if (!_isSetup)
                SetupDialog();
            return base.TryOpen();
        }

        private void UpdateMapFilter()
        {
            var elem = SingleComposer.GetDropDown("oreList");
            var density = SingleComposer.GetDropDown("densityList").SelectedValue;
            var switches = (GuiElementSwitch[]) _switchesField?.GetValue(elem.listMenu);
            var filterList = new List<string>();
            for (var i = 0; i < switches.Length; i++)
                if (switches[i].On)
                    filterList.Add(elem.listMenu.Values[i]);
            _overlayLayer.FilterUpdated(filterList, density.ToDensityEnum());
        }

        private void ToggleSavedOres()
        {
            if (_client.ClientState.EnabledOreKeys is null)
            {
                ToggleAllOres(true);
                return;
            }
            var elem = SingleComposer.GetDropDown("oreList");
            elem.listMenu.SelectedIndices = new int[_client.ClientState.EnabledOreKeys.Count];
            var switches = (GuiElementSwitch[]) _switchesField?.GetValue(elem.listMenu);
            for (var i = 0; i < elem.listMenu.SelectedIndices.Length; i++)
            {
                var index = i;
                elem.listMenu.SelectedIndices[i] =
                    elem.listMenu.Values.IndexOf(x => x == _client.ClientState.EnabledOreKeys[index]);
                switches[i].On = true;
            }

            _composeDropDown?.Invoke(elem, null);
        }

        private bool ToggleAllOres(bool toggleOn)
        {
            var elem = SingleComposer.GetDropDown("oreList");
            elem.listMenu.SelectedIndices = toggleOn
                ? Enumerable.Range(0, _oreList.Count).ToArray()
                : new int[0];
            _composeDropDown?.Invoke(elem, null);
            var switches = (GuiElementSwitch[]) _switchesField?.GetValue(elem.listMenu);
            foreach (var switchElem in switches)
                switchElem.On = toggleOn;
            UpdateMapFilter();
            return true;
        }

        public void SendOreList(OreList oreList)
        {
            _oreList = oreList.OreCodes;
        }
    }
}