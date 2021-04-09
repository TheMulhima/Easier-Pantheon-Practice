using Modding;
using System;
using System.Reflection;

namespace Easier_Ascended
{
    public class EasierAscended : Mod, ITogglableMod
    {
        //stores them like this so it can be acessed by the other classes
        public static int DoDamage, AddBlueMasks, AddSoul;
        public static bool radiant,_unloaded;

        internal static EasierAscended Instance;

        public EasierAscended() : base("Easier P5 Practice") { }//Mod name on top left
        public override string GetVersion() => Assembly.GetExecutingAssembly().GetName().Version.ToString();//cuz im lazy to increment it myself
        public GlobalModSettings _globalSettings = new GlobalModSettings(); //get the settings from settings file

        public override ModSettings GlobalSettings
        {
            get => _globalSettings;
            set => _globalSettings = (GlobalModSettings) value;
        }
        public override void Initialize()
        {

            Instance = this;

            Log("Easier Ascended Mod Initialized");

            //intilises the variables so it can be used by other classes
            _unloaded = false;//to make sure the mod is toggleable

            DoDamage = 2 * _globalSettings.remove_health; //x2 becuase the function is called after Only1Damage is called
            AddBlueMasks = _globalSettings.lifeblood;
            AddSoul = _globalSettings.soul;
            radiant = _globalSettings.hitless_practice;

            ModHooks.Instance.AfterSavegameLoadHook += Load_Easier_Ascended;//loads mod when save game is loaded
            ModHooks.Instance.NewGameHook += New_Game_Easier_Ascended; 
            ModHooks.Instance.LanguageGetHook += BossDesc;
        }

        public string BossDesc(string key, string sheet)
        {
            switch (key)
            {
                case "CHALLENGE_UI_LEVEL2": return "P5 PRACTICE";

                #region for the trolls
                case "NAME_MEGA_MOSS_CHARGER": return "UNKILLABLE MOSS CHARGER";
                case "GG_S_MEGAMOSS": return "The True Champion of the Gods. Try as hard as you want, you can not kill it";
                case "MEGA_MOSS_SUPER": return "UNKILLABLE";
                case "MEGA_MOSS_SUB": return "THE TRUE CHAMPION OF THE GODS";

                case "GG_S_GRUZ": return "My head hurts. Please dont make me slam my head again";
                case "GG_S_BIGBUZZ": return "Vicious God of running away";
                case "GG_S_FLUKEMUM": return "Alluring God of standing still";
                case "GG_S_BIGBEES": return "Gods of RNG";
                case "GG_S_NOSK_HORNET": return "Vicious God of running away, but worse";
                case "GG_S_COLLECTOR": return "The boss that gives nightmares to All Binding players";
                case "GG_S_MIGHTYZOTE": return "I like giving ear aches";
                case "KNIGHT_STATUE_1": return "Did you really just spend all these hours grinding just to get this??";
                case "KNIGHT_STATUE_2": return "Did you really just spend all these hours grinding just to get this??";
                case "KNIGHT_STATUE_3": return "Did you really just spend all these hours grinding just to get this??";
                case "GG_S_RADIANCE": return "I'm the god of light and you insult me by refering to me as a tiny moth??";
                case "GG_S_SLY": return "Bug Yoda";
                case "GG_S_GHOST_HU": return "I love PANCAKES";
                case "GG_S_GHOST_GORB": return "Ascend with Gorb";
                case "GG_S_SOULMASTER": return "Teleporting freak";
                case "GG_S_SOUL_TYRANT": return "Teleporting freak v2";
                case "GG_S_MAGEKNIGHT": return "Am i really a boss?";
                case "UI_CHALLENGE_DESC_5": return "After all this practice, are you finally ready?";
                case "UI_BEGIN": return "I'm Ready";
                case "CHARM_NAME_2": return "OP Compass";
                case "CHARM_DESC_2": return "Its the most OP charm in the game.<br><br>Wear this charm to get good";
                #endregion
        }
            

            return Language.Language.GetInternal(key, sheet);
        }

        private void New_Game_Easier_Ascended()
        {
            GameManager.instance.gameObject.AddComponent<FindBoss>();
        }
        private void Load_Easier_Ascended(SaveGameData data)
        {
            GameManager.instance.gameObject.AddComponent<FindBoss>();
        }

        public void Unload() //To allow it to toggle between on and off in the mods option in settings
        {
            ModHooks.Instance.AfterSavegameLoadHook -= Load_Easier_Ascended;
            ModHooks.Instance.LanguageGetHook -= BossDesc;
            _unloaded = true;
        }
    }
}
