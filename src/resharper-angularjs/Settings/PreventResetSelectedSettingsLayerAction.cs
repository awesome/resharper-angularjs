using System.Linq;
using JetBrains.ActionManagement;
using JetBrains.Application.DataContext;
using JetBrains.Application.Settings.UserInterface;
using JetBrains.Util;
using DataConstants = JetBrains.UI.Settings.DataConstants;

namespace JetBrains.ReSharper.Plugins.AngularJS.Settings
{
    public class PreventResetSelectedSettingsLayerAction : IActionHandler
    {
        public bool Update(IDataContext context, ActionPresentation presentation, DelegateUpdate nextUpdate)
        {
            var layers = context.GetData(DataConstants.SelectedUserFriendlySettingsLayers);
            if (layers == null || layers.IsEmpty())
                return false;

            // Action is disabled if *any* layers have reset blocked
            return layers.All(CanReset) && nextUpdate();
        }

        public void Execute(IDataContext context, DelegateExecute nextExecute)
        {
            // TODO: We could get cocky and recreate the file from a resource...
            // Safely call next, we won't get called if any layers have reset blocked
            nextExecute();
        }

        private static bool CanReset(UserFriendlySettingsLayer.Identity layer)
        {
            if (layer.CharacteristicMount == null)
                return false;

            // Look for our metadata id to see if we should prevent reset
            var metadata = layer.CharacteristicMount.Metadata;
            return !metadata.TryGet(TemplatesLoader.PreventReset);
        }
    }
}