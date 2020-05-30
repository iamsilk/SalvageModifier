using IAmSilK.SalvageModifier.Configuration;
using Rocket.API;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System.Linq;

namespace IAmSilK.SalvageModifier
{
    public class SalvageModifierPlugin : RocketPlugin<SalvageModifierConfiguration>
    {
        public static SalvageModifierPlugin Instance { get; private set; }

        protected override void Load()
        {
            Instance = this;

            U.Events.OnPlayerConnected += SetPlayerSalvageTime;

            foreach (SteamPlayer steamPlayer in Provider.clients.Where(x => x != null))
            {
                UnturnedPlayer player = UnturnedPlayer.FromSteamPlayer(steamPlayer);

                SetPlayerSalvageTime(player);
            }
        }

        protected override void Unload()
        {
            U.Events.OnPlayerConnected -= SetPlayerSalvageTime;

            Instance = null;
        }

        private const string PermissionPrefix = "salvagetime.";

        public static float GetSalvageTime(UnturnedPlayer player)
        {
            float salvageTime = player.IsAdmin ? 1f : 8f;

            if (Instance.Configuration.Instance.DefaultSalvageTime < salvageTime)
            {
                salvageTime = Instance.Configuration.Instance.DefaultSalvageTime;
            }

            foreach (var permission in player.GetPermissions())
            {
                if (permission.Name.StartsWith(PermissionPrefix))
                {
                    string salvageTimeStr = permission.Name.Substring(PermissionPrefix.Length);

                    float time;
                    if (!float.TryParse(salvageTimeStr, out time))
                    {
                        Logger.Log("Invalid salvage time permission: " + permission.Name);
                        continue;
                    }

                    if (time < salvageTime)
                    {
                        salvageTime = time;
                    }
                }
            }

            return salvageTime;
        }

        public static void SetPlayerSalvageTime(UnturnedPlayer player)
        {
            float salvageTime = GetSalvageTime(player);

            player.Player.interact.sendSalvageTimeOverride(salvageTime);
        }
    }
}
