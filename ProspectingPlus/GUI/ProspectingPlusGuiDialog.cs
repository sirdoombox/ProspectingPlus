using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ProspectingPlus.Client;
using ProspectingPlus.Shared.Packets;
using Vintagestory.API.Client;
using Vintagestory.API.Config;
using Vintagestory.GameContent;

namespace ProspectingPlus.GUI
{
    public class ProspectingPlusGuiDialog : GuiDialog
    {
        private static readonly MethodInfo _composeDropDown =
            typeof(GuiElementDropDown).GetMethod("ComposeCurrentValue", BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly FieldInfo _switchesField =
            typeof(GuiElementListMenu).GetField("switches", BindingFlags.Instance | BindingFlags.NonPublic);

        private bool _isSetup;

        private List<string> _oreList;
        private readonly ProspectingOverlayLayer _overlayLayer;

        public override string ToggleKeyCombinationCode { get; }


        public ProspectingPlusGuiDialog(ICoreClientAPI capi) : base(capi)
        {
            _overlayLayer = (ProspectingOverlayLayer) capi.ModLoader
                .GetModSystem<WorldMapManager>()
                .MapLayers
                .FirstOrDefault(x => x is ProspectingOverlayLayer);
        }

        private void SetupDialog()
        {
            _isSetup = true;

            var dialogBounds = ElementStdBounds.AutosizedMainDialog.WithAlignment(EnumDialogArea.RightTop)
                .WithFixedAlignmentOffset(-GuiStyle.DialogToScreenPadding, GuiStyle.DialogToScreenPadding);
            var font = CairoFont.WhiteSmallText();

            var buttonBounds = ElementBounds.Fixed(0, GuiStyle.TitleBarHeight, 350, 25);

            var bgBounds = ElementBounds.Fill.WithFixedPadding(GuiStyle.ElementToDialogPadding);
            bgBounds.BothSizing = ElementSizing.FitToChildren;

            var composer = capi.Gui.CreateCompo("prospectingPlusConfigGui", dialogBounds)
                .AddShadedDialogBG(bgBounds)
                .AddDialogTitleBar("Prospecting Plus", () => { })
                .BeginChildElements(bgBounds);
            composer.AddSmallButton(
                "Toggle All",
                () =>
                {
                    var elem = SingleComposer.GetDropDown("oreList");
                    elem.listMenu.SelectedIndices = Enumerable.Range(0, _oreList.Count).ToArray();
                    _composeDropDown?.Invoke(elem, null);
                    var switches = (GuiElementSwitch[]) _switchesField?.GetValue(elem.listMenu);
                    foreach (var switchElem in switches)
                        switchElem.On = true;
                    UpdateMapFilter();
                    return true;
                },
                buttonBounds);
            buttonBounds = buttonBounds.BelowCopy(fixedDeltaY: 5);
            composer.AddSmallButton(
                "Toggle None",
                () =>
                {
                    var elem = SingleComposer.GetDropDown("oreList");
                    elem.listMenu.SelectedIndices = new int[0];
                    _composeDropDown?.Invoke(elem, null);
                    var switches = (GuiElementSwitch[]) _switchesField?.GetValue(elem.listMenu);
                    foreach (var switchElem in switches)
                        switchElem.On = false;
                    UpdateMapFilter();
                    return true;
                },
                buttonBounds);
            buttonBounds = buttonBounds.BelowCopy(fixedDeltaY: 10);
            composer.AddMultiSelectDropDown(
                _oreList.ToArray(),
                _oreList.Select(x => Lang.Get(x)).ToArray(),
                1,
                (code, selected) => { UpdateMapFilter(); },
                buttonBounds,
                "oreList");
            SingleComposer = composer.EndChildElements().Compose();
        }

        public override bool TryOpen()
        {
            //if (!_isSetup)
            SetupDialog();
            return base.TryOpen();
        }

        private void UpdateMapFilter()
        {
            var elem = SingleComposer.GetDropDown("oreList");
            var switches = (GuiElementSwitch[]) _switchesField?.GetValue(elem.listMenu);
            var filterList = new List<string>();
            for (var i = 0; i < switches.Length; i++)
                if (switches[i].On)
                    filterList.Add(elem.listMenu.Values[i]);
            _overlayLayer.FilterUpdated(filterList);
        }

        public void SendOreList(OreList oreList)
        {
            _oreList = oreList.OreCodes;
        }
    }
}