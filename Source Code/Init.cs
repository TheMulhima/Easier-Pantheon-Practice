using Modding;
using UnityEngine;
using USceneManager = UnityEngine.SceneManagement.SceneManager;
using Modding.Menu;
using Modding.Menu.Config;

namespace Easier_Pantheon_Practice
{
    public class EasierPantheonPractice : Mod,ITogglableMod, IGlobalSettings<GlobalSettings>,ICustomMenuMod
    {
        public EasierPantheonPractice() : base("Easier Pantheon Practice") { }
        public override string GetVersion() => "1.0.7";

         public MenuScreen GetMenuScreen(MenuScreen modListMenu) 
         {
             string[] maskvalues = new string[11];
             for (int i = 0; i < 10; i++)
             {
                 maskvalues[i] = i.ToString();
             }
             string[] soulvalues = new string[199];
             for (int i = 0; i < 199; i++)
             {
                 soulvalues[i] = i.ToString();
             }

             string[] boolvalues = {"False", "True"};
             
                return new MenuBuilder(UIManager.instance.UICanvas.gameObject, "Menu")
                    .CreateTitle("Easier Pantheon Practice Settings", MenuTitleStyle.vanillaStyle)
                    .CreateContentPane(RectTransformData.FromSizeAndPos(
                        new RelVector2(new Vector2(1920f, 903f)),
                        new AnchoredPosition(
                            new Vector2(0.5f, 0.5f),
                            new Vector2(0.5f, 0.5f),
                            new Vector2(0f, -60f)
                        )
                    ))
                    .CreateControlPane(RectTransformData.FromSizeAndPos(
                        new RelVector2(new Vector2(1920f, 259f)),
                        new AnchoredPosition(
                            new Vector2(0.5f, 0.5f),
                            new Vector2(0.5f, 0.5f),
                            new Vector2(0f, -502f)
                        )
                    ))
                    .SetDefaultNavGraph(new ChainedNavGraph())
                    .AddContent(
                        RegularGridLayout.CreateVerticalLayout(105f),
                        c =>
                        {
                            c.AddHorizontalOption(
                                "RemoveHealth",
                                new HorizontalOptionConfig
                                {
                                    Label = "Remove Health",
                                    Options = maskvalues,
                                    ApplySetting = (_, i) =>
                                    {
                                        settings.remove_health = i;
                                    },
                                    RefreshSetting = (s, _) => s.optionList.SetOptionTo(settings.remove_health),
                                    CancelAction = _ => UIManager.instance.UIGoToDynamicMenu(modListMenu),
                                    Style = HorizontalOptionStyle.VanillaStyle
                                }).AddHorizontalOption(
                                "Lifeblood",
                                new HorizontalOptionConfig
                                {
                                    Label = "Lifeblood",
                                    Options = maskvalues,
                                    ApplySetting = (_, i) =>
                                    {
                                        settings.lifeblood = i;
                                    },
                                    RefreshSetting = (s, _) => s.optionList.SetOptionTo(settings.lifeblood),
                                    CancelAction = _ => UIManager.instance.UIGoToDynamicMenu(modListMenu),
                                    Style = HorizontalOptionStyle.VanillaStyle
                                }).AddHorizontalOption(
                                "Soul",
                                new HorizontalOptionConfig
                                {
                                    Label = "Soul",
                                    Options = soulvalues,
                                    ApplySetting = (_, i) =>
                                    {
                                        settings.soul = i;
                                    },
                                    RefreshSetting = (s, _) => s.optionList.SetOptionTo(settings.soul),
                                    CancelAction = _ => UIManager.instance.UIGoToDynamicMenu(modListMenu),
                                    Style = HorizontalOptionStyle.VanillaStyle
                                }).AddHorizontalOption(
                                "HitlessPractice",
                                new HorizontalOptionConfig
                                {
                                    Label = "Hitless Practice",
                                    Options = boolvalues,
                                    ApplySetting = (_, i) =>
                                    {
                                        settings.hitless_practice = i != 0;
                                    },
                                    RefreshSetting = (s, _) => s.optionList.SetOptionTo(settings.hitless_practice ? 1:0),
                                    CancelAction = _ => UIManager.instance.UIGoToDynamicMenu(modListMenu),
                                    Style = HorizontalOptionStyle.VanillaStyle
                                },
                                out var MainOptions);
                            c.AddKeybind(
                                "ReloadBossBind",
                                settings.keybinds.Key_Reload_Boss,
                                new KeybindConfig
                                {
                                    Label = "Reload Boss",
                                    CancelAction = _ => UIManager.instance.UIGoToDynamicMenu(modListMenu)
                                }
                            ).AddKeybind(
                                "ReturnToHoGBind",
                                settings.keybinds.Key_return_to_hog,
                                new KeybindConfig
                                {
                                    Label = "Return To HoG",
                                    CancelAction = _ => UIManager.instance.UIGoToDynamicMenu(modListMenu)
                                }
                            ).AddKeybind(
                                "TeleportAroundHoGBind",
                                settings.keybinds.Key_teleport_around_HoG,
                                new KeybindConfig
                                {
                                    Label = "Teleport Around HoG",
                                    CancelAction = _ => UIManager.instance.UIGoToDynamicMenu(modListMenu)
                                }
                            );
                        }
                    )
                    .AddControls(
                        new SingleContentLayout(new AnchoredPosition(
                            new Vector2(0.5f, 0.5f),
                            new Vector2(0.5f, 0.5f),
                            new Vector2(0f, -64f)
                        )),
                        c => c.AddMenuButton(
                            "BackButton",
                            new MenuButtonConfig {
                                Label = "Back",
                                CancelAction = _ => UIManager.instance.UIGoToDynamicMenu(modListMenu),
                                SubmitAction = _ => UIManager.instance.UIGoToDynamicMenu(modListMenu),
                                Style = MenuButtonStyle.VanillaStyle,
                                Proceed = true
                            }
                        )
                    )
                    .Build();
        }
         
         public static GlobalSettings settings { get; set; } = new GlobalSettings();
         public void OnLoadGlobal(GlobalSettings s) => EasierPantheonPractice.settings = s;
         public GlobalSettings OnSaveGlobal() => EasierPantheonPractice.settings;

        
        internal static EasierPantheonPractice Instance;
        
        public override void Initialize()
        {
            Instance = this;
            Log("Trying to load mod");

            Load_Mod();
            
            ModHooks.SavegameLoadHook += Load_Save;
            ModHooks.NewGameHook += Load_Mod;
            ModHooks.LanguageGetHook += BossDesc;
        }

        private bool BossDesc(string key, string sheettitle, string orig, string current, out string res)
        {
            switch (key)
            {
                case "CHALLENGE_UI_LEVEL2":
                    res = "P5 PRACTICE";
                    return true;
                case "CHALLENGE_UI_LEVEL1":
                    res = "P1-4 PRACTICE";
                    return true;
            }

            #region for the trolls

            if (settings.funny_descriptions)
            {

                if (key == "NAME_MEGA_MOSS_CHARGER") res = "UNKILLABLE MOSS CHARGER";
                else if (key == "GG_S_MEGAMOSS")
                    res = "The True Champion of the Gods. Try as hard as you want, you can not kill it";
                else if (key == "MEGA_MOSS_SUPER") res = "UNKILLABLE";
                else if (key == "MEGA_MOSS_SUB") res = "THE TRUE CHAMPION OF THE GODS";

                else if (key == "GG_S_GRUZ") res = "My head hurts. Please dont make me slam my head again";
                else if (key == "GG_S_BIGBUZZ") res = "Vicious God of running away";
                else if (key == "GG_S_FLUKEMUM") res = "Alluring God of standing still";
                else if (key == "GG_S_BIGBEES") res = "Gods of RNG";
                else if (key == "GG_S_NOSK_HORNET") res = "Vicious God of running away, but worse";
                else if (key == "GG_S_COLLECTOR") res = "The boss that gives nightmares to All Binding players";
                else if (key == "GG_S_MIGHTYZOTE") res = "I like giving ear aches";
                else if (key == "KNIGHT_STATUE_1"||key == "KNIGHT_STATUE_2"|| key == "KNIGHT_STATUE_3")
                            res = "Did you really just spend all these hours grinding just to get this??";
                else if (key == "GG_S_RADIANCE")
                    res = "I'm the god of light and you insult me by referring to me as a tiny moth??";
                else if (key == "GG_S_SLY") res = "Bug Yoda";
                else if (key == "GG_S_GHOST_HU") res = "I love PANCAKES";
                else if (key == "GG_S_GHOST_GORB") res = "Ascend with Gorb";
                else if (key == "GG_S_SOULMASTER") res = "Teleporting freak";
                else if (key == "GG_S_SOUL_TYRANT") res = "Teleporting freak v2";
                else if (key == "GG_S_MAGEKNIGHT") res = "Am i really a boss?";
                else if (key == "CHARM_NAME_2") res = "OP Compass";
                else if (key == "CHARM_DESC_2")
                    res = "Its the most OP charm in the game.<br><br>Wear this charm to get good";
                else
                {
                    res = current;
                }

                return current == orig;
            }
            #endregion
            
            
            res = current;
            return true;
        }

        private void Load_Mod()
        {
            var MainComponent = GameManager.instance.gameObject.GetComponent<FindBoss>();
            if (MainComponent == null) GameManager.instance.gameObject.AddComponent<FindBoss>();
        }
        private void Load_Save(int obj)
        {
            Load_Mod();
        }
        public void Unload()
        {
            ModHooks.LanguageGetHook -= BossDesc;
            
            var MainComponent = GameManager.instance?.gameObject.GetComponent<FindBoss>();
            if (MainComponent != null) UnityEngine.Object.Destroy(MainComponent);
            
        }
    }
}