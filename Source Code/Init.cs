using System;
using Modding;
using UnityEngine;
using USceneManager = UnityEngine.SceneManagement.SceneManager;
using Modding.Menu;
using Modding.Menu.Config;

namespace Easier_Pantheon_Practice
{
    public class EasierPantheonPractice : Mod, ITogglableMod, IGlobalSettings<GlobalSettings>, ICustomMenuMod
    {
        public EasierPantheonPractice() : base("Easier Pantheon Practice") {}
        public override string GetVersion() => "v1.0.7";

        public bool ToggleButtonInsideMenu => true;

        private bool isEnabled;
        private bool isCurrentlyEnabled;

        private static MenuScreen ExtraSettings;
        public static MenuScreen MainMenu;

        public MenuScreen GetMenuScreen(MenuScreen modListMenu, ModToggleDelegates? toggleDelegates)
        {

            string[] maskvalues = new string[11];
            for (int i = 0; i < 10; i++)
            {
                maskvalues[i] = i.ToString();
            }

            string[] soulvalues = new string[7];
            for (int i = 0; i * 33 <= 198; i++)
            {
                soulvalues[i] = (i * 33).ToString();
            }
            
            string[] boolvalues = {"False", "True"};

            MainMenu = new MenuBuilder(UIManager.instance.UICanvas.gameObject, "EPPMenu")
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
                            "Toggle Mod",
                            new HorizontalOptionConfig
                            {
                                Label = "Toggle Mod",
                                Options = new []{"On","Off"},
                                ApplySetting = (_, i) => { isEnabled = i == 0;},
                                RefreshSetting = (s, _) => s.optionList.SetOptionTo(settings.remove_health),
                                CancelAction = _ => UIManager.instance.UIGoToDynamicMenu(modListMenu),
                                Style = HorizontalOptionStyle.VanillaStyle
                            })
                            .AddHorizontalOption(
                            "RemoveHealth",
                            new HorizontalOptionConfig
                            {
                                Label = "Remove Health",
                                Options = maskvalues,
                                ApplySetting = (_, i) => { settings.remove_health = i; },
                                RefreshSetting = (s, _) => s.optionList.SetOptionTo(settings.remove_health),
                                CancelAction = _ => UIManager.instance.UIGoToDynamicMenu(modListMenu),
                                Style = HorizontalOptionStyle.VanillaStyle
                            }).AddHorizontalOption(
                            "Lifeblood",
                            new HorizontalOptionConfig
                            {
                                Label = "Lifeblood",
                                Options = maskvalues,
                                ApplySetting = (_, i) => { settings.lifeblood = i; },
                                RefreshSetting = (s, _) => s.optionList.SetOptionTo(settings.lifeblood),
                                CancelAction = _ => UIManager.instance.UIGoToDynamicMenu(modListMenu),
                                Style = HorizontalOptionStyle.VanillaStyle
                            }).AddHorizontalOption(
                            "Soul",
                            new HorizontalOptionConfig
                            {
                                Label = "Soul",
                                Options = soulvalues,
                                ApplySetting = (_, i) => { settings.soul = Int32.Parse(soulvalues[i]); },
                                RefreshSetting = (s, _) => s.optionList.SetOptionTo(settings.soul),
                                CancelAction = _ => UIManager.instance.UIGoToDynamicMenu(modListMenu),
                                Style = HorizontalOptionStyle.VanillaStyle
                            }).AddHorizontalOption(
                            "HitlessPractice",
                            new HorizontalOptionConfig
                            {
                                Label = "Hitless Practice",
                                Options = boolvalues,
                                ApplySetting = (_, i) => { settings.hitless_practice = i != 0; },
                                RefreshSetting = (s, _) => s.optionList.SetOptionTo(settings.hitless_practice ? 1 : 0),
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
                        ).AddMenuButton(
                            "ExtraSettings",
                            new MenuButtonConfig
                            {
                                Label = "Additional Settings",
                                SubmitAction = _ => UIManager.instance.UIGoToDynamicMenu(ExtraSettings),
                                CancelAction = _ => UIManager.instance.UIGoToDynamicMenu(modListMenu),
                                Style = MenuButtonStyle.VanillaStyle,
                                Proceed = true,
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
                        new MenuButtonConfig
                        {
                            Label = "Back",
                            CancelAction = _ =>
                            {
                                switch (isEnabled)
                                {
                                    case true when !isCurrentlyEnabled:
                                        Initialize();
                                        break;
                                    case false when isCurrentlyEnabled:
                                        Unload();
                                        break;
                                }
                                UIManager.instance.UIGoToDynamicMenu(modListMenu);
                            },
                            SubmitAction = _ => UIManager.instance.UIGoToDynamicMenu(modListMenu),
                            Style = MenuButtonStyle.VanillaStyle,
                            Proceed = true
                        }
                    )
                )
                .Build();



            ExtraSettings = new MenuBuilder("Additional Settings")
                .CreateTitle("Additional Settings", MenuTitleStyle.vanillaStyle)
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
                            "funny_descriptions",
                            new HorizontalOptionConfig
                            {
                                Label = "Funny Descriptions",
                                Options = new [] {"True", "False"},
                                ApplySetting = (_, i) => { settings.funny_descriptions = i == 0; },
                                RefreshSetting =
                                    (s, _) => s.optionList.SetOptionTo(settings.funny_descriptions ? 0 : 1),
                                CancelAction = _ => UIManager.instance.UIGoToDynamicMenu(modListMenu),
                                Style = HorizontalOptionStyle.VanillaStyle
                            }).AddHorizontalOption(
                            "only_apply_settings",
                            new HorizontalOptionConfig
                            {
                                Label = "Only Apply Settings",
                                Options = boolvalues,
                                ApplySetting = (_, i) => { settings.only_apply_settings = i != 0; },
                                RefreshSetting = (s, _) =>
                                    s.optionList.SetOptionTo(settings.only_apply_settings ? 1 : 0),
                                CancelAction = _ => UIManager.instance.UIGoToDynamicMenu(modListMenu),
                                Style = HorizontalOptionStyle.VanillaStyle
                            }).AddHorizontalOption(
                            "allow_reloads_in_loads",
                            new HorizontalOptionConfig
                            {
                                Label = "Can Reloads Boss in Loads",
                                Options = boolvalues,
                                ApplySetting = (_, i) => { settings.allow_reloads_in_loads = i != 0; },
                                RefreshSetting = (s, _) =>
                                    s.optionList.SetOptionTo(settings.allow_reloads_in_loads ? 1 : 0),
                                CancelAction = _ => UIManager.instance.UIGoToDynamicMenu(modListMenu),
                                Style = HorizontalOptionStyle.VanillaStyle
                            }).AddKeybind(
                            "TeleportAroundHoGBind",
                            settings.keybinds.Key_teleport_around_HoG,
                            new KeybindConfig
                            {
                                Label = "Move Around HoG",
                                CancelAction = _ => UIManager.instance.UIGoToDynamicMenu(modListMenu)
                            }, out var MainOptions);
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
                        new MenuButtonConfig
                        {
                            Label = "Back",
                            CancelAction = _ => UIManager.instance.UIGoToDynamicMenu(MainMenu),
                            SubmitAction = _ => UIManager.instance.UIGoToDynamicMenu(MainMenu),
                            Style = MenuButtonStyle.VanillaStyle,
                            Proceed = true
                        }
                    )
                )
                .Build();


            return MainMenu;
        }

        public static GlobalSettings settings { get; set; } = new GlobalSettings();
        public void OnLoadGlobal(GlobalSettings s) => EasierPantheonPractice.settings = s;
        public GlobalSettings OnSaveGlobal() => EasierPantheonPractice.settings;


        internal static EasierPantheonPractice Instance;

        public override void Initialize()
        {
            Instance = this;

            isCurrentlyEnabled = true;
            
            Log("Trying to load mod");

            Load_Mod();

            ModHooks.SavegameLoadHook += Load_Save;
            ModHooks.NewGameHook += Load_Mod;
            ModHooks.LanguageGetHook += BossDesc;
        }

        private string BossDesc(string key, string sheettitle, string orig)
        {
            switch (key)
            {
                case "CHALLENGE_UI_LEVEL2":
                    return "P5 PRACTICE";
                case "CHALLENGE_UI_LEVEL1":
                    return "P1-4 PRACTICE";
            }

            #region for the trolls

            if (settings.funny_descriptions)
            {
                switch (key)
                {
                    case "NAME_MEGA_MOSS_CHARGER":
                        return "UNKILLABLE MOSS CHARGER";
                        
                    case "GG_S_MEGAMOSS":
                        return "The True Champion of the Gods. Try as hard as you want, you can not kill it";
                        
                    case "MEGA_MOSS_SUPER":
                        return "UNKILLABLE";
                        
                    case "MEGA_MOSS_SUB":
                        return "THE TRUE CHAMPION OF THE GODS";
                        
                    case "GG_S_GRUZ":
                        return "My head hurts. Please dont make me slam my head again";
                        
                    case "GG_S_BIGBUZZ":
                        return "Vicious God of running away";
                        
                    case "GG_S_FLUKEMUM":
                        return "Alluring God of standing still";
                        
                    case "GG_S_BIGBEES":
                        return "Gods of RNG";
                        
                    case "GG_S_NOSK_HORNET":
                        return "Vicious God of running away, but worse";
                        
                    case "GG_S_COLLECTOR":
                        return "The boss that gives nightmares to All Binding players";
                        
                    case "GG_S_MIGHTYZOTE":
                        return "I like giving ear aches";
                        
                    case "KNIGHT_STATUE_1":
                    case "KNIGHT_STATUE_2":
                    case "KNIGHT_STATUE_3":
                        return "Did you really just spend all these hours grinding just to get this??";
                        
                    case "GG_S_RADIANCE":
                        return "I'm the god of light and you insult me by referring to me as a tiny moth??";
                        
                    case "GG_S_SLY":
                        return "Bug Yoda";
                        
                    case "GG_S_GHOST_HU":
                        return "I love PANCAKES";
                        
                    case "GG_S_GHOST_GORB":
                        return "Ascend with Gorb";
                        
                    case "GG_S_SOULMASTER":
                        return "Teleporting freak";
                        
                    case "GG_S_SOUL_TYRANT":
                        return "Teleporting freak v2";
                        
                    case "GG_S_MAGEKNIGHT":
                        return "Am i really a boss?";
                        
                    case "CHARM_NAME_2":
                        return "OP Compass";
                        
                    case "CHARM_DESC_2":
                        return "Its the most OP charm in the game.<br><br>Wear this charm to get good";
                        
                    default:
                        return orig;
                }
            }

            #endregion

            return orig;
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
            isCurrentlyEnabled = false;
            ModHooks.LanguageGetHook -= BossDesc;
            ModHooks.SavegameLoadHook -= Load_Save;
            ModHooks.NewGameHook -= Load_Mod;
            var MainComponent = GameManager.instance?.gameObject.GetComponent<FindBoss>();
            if (MainComponent != null) UnityEngine.Object.Destroy(MainComponent);

        }
    }
}
