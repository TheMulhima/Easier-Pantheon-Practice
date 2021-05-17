using Modding;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Easier_Pantheon_Practice
{
    public class EasierPantheonPractice : Mod
    {
        internal static EasierPantheonPractice Instance;

        public EasierPantheonPractice() : base("Easier Pantheon Practice") { }
        public override string GetVersion() => Assembly.GetExecutingAssembly().GetName().Version.ToString();

        public GlobalModSettings settings = new GlobalModSettings();
        public override ModSettings GlobalSettings
        {
            get => settings;
            set => settings = (GlobalModSettings) value;
        }

        public static readonly Dictionary<string, GameObject> PreloadedObjects = new Dictionary<string, GameObject>();
        public override List<(string, string)> GetPreloadNames()
        {
            return new List<(string, string)>
            {
                ("GG_Workshop", "GG_Statue_Hornet/Inspect"),//need this for reload boss feature
            };
        }

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            Instance = this;
            PreloadedObjects.Add("Inspect", preloadedObjects["GG_Workshop"]["GG_Statue_Hornet/Inspect"]);
            
            ModHooks.Instance.AfterSavegameLoadHook += Load_Save;
            ModHooks.Instance.NewGameHook += Load_Easier_Ascended; 
            ModHooks.Instance.LanguageGetHook += BossDesc;
        }

        private string BossDesc(string key, string sheet)
        {
            switch (key)
            {
                case "CHALLENGE_UI_LEVEL2": return "P5 PRACTICE";
                case "CHALLENGE_UI_LEVEL1": return "P1-4 PRACTICE";
            }

            #region for the trolls
            if (settings.funny_descriptions)
            {
                switch (key)
                {
                    case "NAME_MEGA_MOSS_CHARGER": return "UNKILLABLE MOSS CHARGER";
                    case "GG_S_MEGAMOSS":          return "The True Champion of the Gods. Try as hard as you want, you can not kill it";
                    case "MEGA_MOSS_SUPER":        return "UNKILLABLE";
                    case "MEGA_MOSS_SUB":          return "THE TRUE CHAMPION OF THE GODS";

                    case "GG_S_GRUZ":              return "My head hurts. Please dont make me slam my head again";
                    case "GG_S_BIGBUZZ":           return "Vicious God of running away";
                    case "GG_S_FLUKEMUM":          return "Alluring God of standing still";
                    case "GG_S_BIGBEES":           return "Gods of RNG";
                    case "GG_S_NOSK_HORNET":       return "Vicious God of running away, but worse";
                    case "GG_S_COLLECTOR":         return "The boss that gives nightmares to All Binding players";
                    case "GG_S_MIGHTYZOTE":        return "I like giving ear aches";
                    case "KNIGHT_STATUE_1":        
                    case "KNIGHT_STATUE_2":        
                    case "KNIGHT_STATUE_3":        return "Did you really just spend all these hours grinding just to get this??";
                    case "GG_S_RADIANCE":          return "I'm the god of light and you insult me by referring to me as a tiny moth??";
                    case "GG_S_SLY":               return "Bug Yoda";
                    case "GG_S_GHOST_HU":          return "I love PANCAKES";
                    case "GG_S_GHOST_GORB":        return "Ascend with Gorb";
                    case "GG_S_SOULMASTER":        return "Teleporting freak";
                    case "GG_S_SOUL_TYRANT":       return "Teleporting freak v2";
                    case "GG_S_MAGEKNIGHT":        return "Am i really a boss?";
                    case "CHARM_NAME_2":           return "OP Compass";
                    case "CHARM_DESC_2":           return "Its the most OP charm in the game.<br><br>Wear this charm to get good";
                }
                
            }
            #endregion
            
            return Language.Language.GetInternal(key, sheet);
        }

        private void Load_Easier_Ascended()
        {
            GameManager.instance.gameObject.AddComponent<FindBoss>();
            
            //honestly not sure why i added this because GUI > setting file. but its there if someone wants it
            if (!settings.remove_ingame_menu)
            {
                GameManager.instance.gameObject.AddComponent<GUIForSettings>();
            }
        }
        private void Load_Save(SaveGameData data)
        {
            Load_Easier_Ascended();
        }
    }
}
