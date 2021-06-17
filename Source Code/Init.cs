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
                                        GlobalSaveData.remove_health = i;
                                    },
                                    RefreshSetting = (s, _) => s.optionList.SetOptionTo(GlobalSaveData.remove_health),
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
                                        GlobalSaveData.lifeblood = i;
                                    },
                                    RefreshSetting = (s, _) => s.optionList.SetOptionTo(GlobalSaveData.lifeblood),
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
                                        GlobalSaveData.soul = i;
                                    },
                                    RefreshSetting = (s, _) => s.optionList.SetOptionTo(GlobalSaveData.soul),
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
                                        GlobalSaveData.hitless_practice = i != 0;
                                    },
                                    RefreshSetting = (s, _) => s.optionList.SetOptionTo(GlobalSaveData.hitless_practice ? 1:0),
                                    CancelAction = _ => UIManager.instance.UIGoToDynamicMenu(modListMenu),
                                    Style = HorizontalOptionStyle.VanillaStyle
                                },
                                out var MainOptions);
                            c.AddKeybind(
                                "ReloadBossBind",
                                GlobalSaveData.keybinds.Key_Reload_Boss,
                                new KeybindConfig
                                {
                                    Label = "Reload Boss",
                                    CancelAction = _ => UIManager.instance.UIGoToDynamicMenu(modListMenu)
                                }
                            ).AddKeybind(
                                "ReturnToHoGBind",
                                GlobalSaveData.keybinds.Key_return_to_hog,
                                new KeybindConfig
                                {
                                    Label = "Return To HoG",
                                    CancelAction = _ => UIManager.instance.UIGoToDynamicMenu(modListMenu)
                                }
                            ).AddKeybind(
                                "TeleportAroundHoGBind",
                                GlobalSaveData.keybinds.Key_teleport_around_HoG,
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
         
         public static GlobalSettings GlobalSaveData { get; set; } = new GlobalSettings();
         public void OnLoadGlobal(GlobalSettings s) => EasierPantheonPractice.GlobalSaveData = s;
         public GlobalSettings OnSaveGlobal() => EasierPantheonPractice.GlobalSaveData;

        
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
                    res =  "P1-4 PRACTICE";
                    return true;
            }
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
