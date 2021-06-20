using InControl;
using Newtonsoft.Json;
using Modding.Converters;

namespace Easier_Pantheon_Practice
{
    public class GlobalSettings
    {
        public int remove_health = 0;
        public int lifeblood = 0;
        public int soul = 0;
        public bool hitless_practice = false;

        [JsonConverter(typeof(PlayerActionSetConverter))]
        public KeyBinds keybinds = new KeyBinds();

        public bool funny_descriptions = true;
        public bool only_apply_settings = false;
        public bool allow_reloads_in_loads = false;
        public string increment_soul_in = "33";
    }
    public class KeyBinds : PlayerActionSet {
        public PlayerAction Key_return_to_hog;
        public PlayerAction Key_teleport_around_HoG;
        public PlayerAction Key_Reload_Boss;

        public KeyBinds() {
            Key_return_to_hog = CreatePlayerAction("Key_return_to_hog");
            Key_teleport_around_HoG = CreatePlayerAction("Key_teleport_around_HoG");
            Key_Reload_Boss = CreatePlayerAction("Key_Reload_Boss");
        }
    }
}
