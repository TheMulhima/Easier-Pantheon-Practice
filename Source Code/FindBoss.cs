using Modding;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using USceneManager = UnityEngine.SceneManagement.SceneManager;
using UnityEngine;
using System.Collections;
using UObject = UnityEngine.Object;
using HutongGames.PlayMaker.Actions;
using SFCore.Utils;


namespace Easier_Pantheon_Practice
{
    internal class FindBoss : MonoBehaviour
    {
        private static int damage_to_be_dealt, current_move, WhichNext;
        private const int x_value = 0, y_value = 1;
        public static bool altered, SOB;
        private static bool loop;
        private static string PreviousScene, SceneToLoad;
        public static string CurrentBoss, CurrentBoss_1;
        private static Vector3 OldPosition, PosToMove;

        private readonly Dictionary<int, List<float>> MoveAround = new Dictionary<int, List<float>>
        {
            { 0 , new List<float>{11f,36.4f } },//bench
            { 1 , new List<float>{207f,36.4f } },//nkg
            { 2 , new List<float>{29f,6.4f } },//gruz
            { 3 , new List<float>{207f,6.4f } },//gpz

        };

        private readonly Dictionary<string, string> _BossSceneName = new Dictionary<string, string>()
        {
            {"GG_Gruz_Mother_V", "Giant Fly"},
            {"GG_False_Knight","False Knight New" },
            {"GG_Mega_Moss_Charger", "Mega Moss Charger"},
            {"GG_Hornet_1", "Hornet Boss 1"},
            {"GG_Ghost_Gorb_V", "Ghost Warrior Slug"},
            {"GG_Dung_Defender", "Dung Defender"},
            {"GG_Mage_Knight_V", "Mage Knight"},
            {"GG_Brooding_Mawlek_V", "Mawlek Body"},
            {"GG_Ghost_Xero_V", "Ghost Warrior Xero"},
            {"GG_Crystal_Guardian", "Mega Zombie Beam Miner (1)"},
            {"GG_Ghost_Marmu_V", "Ghost Warrior Marmu"},
            {"GG_Flukemarm", "Fluke Mother"},
            {"GG_Broken_Vessel", "Infected Knight"},
            {"GG_Ghost_Galien", "Ghost Warrior Galien"},
            {"GG_Painter", "Sheo Boss"},
            {"GG_Hive_Knight", "Hive Knight"},
            {"GG_Ghost_Hu", "Ghost Warrior Hu"},
            {"GG_Collector_V", "Jar Collector"},
            {"GG_Grimm", "Grimm Boss"},
            {"GG_Uumuu_V", "Mega Jellyfish GG"},
            {"GG_Nosk_Hornet", "Hornet Nosk"},
            {"GG_Sly", "Sly Boss" },
            {"GG_Hornet_2", "Hornet Boss 2"},
            {"GG_Crystal_Guardian_2", "Zombie Beam Miner Rematch"},
            {"GG_Lost_Kin", "Lost Kin"},
            {"GG_Ghost_No_Eyes_V", "Ghost Warrior No Eyes"},
            {"GG_Traitor_Lord", "Mantis Traitor Lord"},
            {"GG_White_Defender", "White Defender"},
            {"GG_Ghost_Markoth_V", "Ghost Warrior Markoth"},
            {"GG_Grey_Prince_Zote", "Grey Prince"},
            {"GG_Failed_Champion", "False Knight Dream"},
            {"GG_Grimm_Nightmare", "Nightmare Grimm Boss"},
            {"GG_Hollow_Knight", "HK Prime"},
            {"GG_Radiance", "Absolute Radiance"},
            {"GG_Nosk", "Nosk"},
            {"GG_Nosk_V", "Nosk"},
            {"GG_Vengefly","Giant Buzzer Col"},
            {"GG_Gruz_Mother", "Giant Fly"},
            {"GG_Ghost_Gorb", "Ghost Warrior Slug"},
            {"GG_Mage_Knight", "Mage Knight"},
            {"GG_Brooding_Mawlek", "Mawlek Body"},
            {"GG_Ghost_Xero", "Ghost Warrior Xero"},
            {"GG_Ghost_Marmu", "Ghost Warrior Marmu"},
            {"GG_Collector", "Jar Collector"},
            {"GG_Uumuu", "Mega Jellyfish GG"},
            {"GG_Ghost_No_Eyes", "Ghost Warrior No Eyes"},
            {"GG_Ghost_Markoth", "Ghost Warrior Markoth"},
        };

        private readonly Dictionary<string, List<string>> SemiExceptions_BossSceneName =
            new Dictionary<string, List<string>>()
            {
                {"GG_Vengefly_V", new List<string>() {"Giant Buzzer Col", "Giant Buzzer Col (1)"}},
                {"GG_Soul_Master", new List<string>() {"Mage Lord", "Mage Lord Phase2"}},
                {"GG_Oblobbles", new List<string>() {"Mega Fat Bee", "Mega Fat Bee (1)"}},
                {"GG_Soul_Tyrant", new List<string>() {"Dream Mage Lord", "Dream Mage Lord Phase2"}},
                {"GG_Nailmasters", new List<string>() {"Oro", "Mato"}},
                {"GG_God_Tamer", new List<string>() {"Lancer", "Lobster"}},
            };
        private readonly List<string> Exceptions_BossSceneName = new List<string>()
        {
            "GG_Mantis_Lords_V",
            "GG_Watcher_Knights",
            "GG_Mantis_Lords",
        };

        
        private void Start()
        {
            ModHooks.Instance.BeforeSceneLoadHook += BeforeSceneChange;
            USceneManager.sceneLoaded += SceneManager_sceneLoaded;
            On.BossSceneController.DoDreamReturn += DoDreamReturn;
            ModHooks.Instance.HeroUpdateHook += HotKeys; 
        }
        private string BeforeSceneChange(string sceneName)
        {
            PreviousScene = GameManager.instance.sceneName;
            if (PreviousScene == "GG_Workshop") OldPosition = HeroController.instance.transform.position;
            return sceneName;
        }

        private static void DoDreamReturn(On.BossSceneController.orig_DoDreamReturn orig, BossSceneController self)
        {
            loop = false;
            orig(self);
        }

        private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            StartCoroutine(SceneLoaded());
            if (!loop)
            {
                if (PreviousScene != "GG_Workshop") return;
            }
            
            altered = false;
            SOB = true;
            CurrentBoss = CurrentBoss_1 = ""; 

            ReflectionHelper.GetField(typeof(BossSequenceController), "bossIndex", false).SetValue(null, 1);

            if (DoesDictContain(PreviousScene))
            {
                if (!loop) ModHooks.Instance.TakeHealthHook -= Only1Damage;

            }

            if (DoesDictContain(arg0.name))
            {
                if (!loop) ModHooks.Instance.TakeHealthHook += Only1Damage;
                
                StartCoroutine(WaitPls_ApplySettings());
                
                if (Exceptions_BossSceneName.Contains(arg0.name))
                {
                    switch (arg0.name)
                    {
                        case "GG_Mantis_Lords_V":
                            StartCoroutine(SistersOfBattle(true));
                            break;
                        case "GG_Mantis_Lords":
                            StartCoroutine(SistersOfBattle(false));
                            break;
                        case "GG_Watcher_Knights":
                            StartCoroutine(WatcherKnight());
                            break;
                    }
                }

                else
                {
                    if (_BossSceneName.ContainsKey(arg0.name)) CurrentBoss = _BossSceneName[arg0.name];
                    else
                    {
                        CurrentBoss = SemiExceptions_BossSceneName[arg0.name][0];
                        CurrentBoss_1 = SemiExceptions_BossSceneName[arg0.name][1];
                    }
                    p5Boss();
                }
            }
        }

        private IEnumerator SceneLoaded()
        {
            yield return null;
           
        }

        private bool DoesDictContain(string KeyToSearch)
        {
            return Exceptions_BossSceneName.Contains(KeyToSearch) || _BossSceneName.ContainsKey(KeyToSearch) ||
                   SemiExceptions_BossSceneName.ContainsKey(KeyToSearch);
        }
    
        #region Find Bosses
        private IEnumerator WaitPls_ApplySettings()
        {
            yield return new WaitForFinishedEnteringScene();
            ApplySettings();
            yield return null;
        }

       

        private IEnumerator AddComponent(string BossName,bool wait = true)
        {
            //Thank you to redfrog6002 for this non-cursed code (before it was a while loop which didnt make sense)
            if (wait) yield return new WaitUntil(() => GameObject.Find(BossName));
            GameObject.Find(BossName).AddComponent<BossNerf>();
        }
        
        private void p5Boss()
        {
            StartCoroutine(AddComponent(CurrentBoss));

            if (CurrentBoss_1 != "") StartCoroutine(AddComponent(CurrentBoss_1));
            
        }

        private IEnumerator SistersOfBattle(bool isSOB)
        {
            SOB = isSOB;
            CurrentBoss = "Mantis Lord";
            CurrentBoss_1 = "Mantis Lord S";

            StartCoroutine(AddComponent(CurrentBoss));
            
            yield return new WaitUntil(() => GameObject.Find(CurrentBoss_1 + "1")); //Waits for phase 2
            
            for (int i = 1; i <= (SOB ? 3 : 2); i++)
            {
                StartCoroutine(AddComponent(CurrentBoss_1 + i.ToString(),false));
            }
        }


        private IEnumerator WatcherKnight()
        {
            CurrentBoss = CurrentBoss_1 = "Black Knight ";

            yield return new WaitUntil(() => GameObject.Find(CurrentBoss + "1"));
            for (int i = 1; i <= 6; i++)
            {
                StartCoroutine(AddComponent(CurrentBoss + i.ToString(),false));
            }
        }

        #endregion

            #region In Scene Stuff

            private static int Only1Damage(int damage)
            {
                damage_to_be_dealt = BossSceneController.Instance.BossLevel == 1 ? damage / 2 : damage;

                if (EasierPantheonPractice.Instance.settings.hitless_practice) damage_to_be_dealt = 1000;

                return damage_to_be_dealt;
            }

            private static void ApplySettings()
            {
                var instance = EasierPantheonPractice.Instance;
                var settings = instance.settings;
                var BSC = BossSceneController.Instance;
                var HC = HeroController.instance;

                if (!(settings.hitless_practice||BSC.BossLevel == 2))
                {
                    HC.TakeHealth(BSC.BossLevel == 0 ? settings.remove_health : 2 * settings.remove_health);

                    for (int lifeblood_increment = 0; lifeblood_increment < settings.lifeblood; lifeblood_increment++)
                        EventRegister.SendEvent("ADD BLUE HEALTH");
                }

                HC.AddMPCharge(EasierPantheonPractice.Instance.settings.soul);

            }

            #endregion


            #region hotkeys

            public void HotKeys()
            {
                var settings = EasierPantheonPractice.Instance.settings;
                var HC = HeroController.instance;
                string theCurrentScene = GameManager.instance.GetSceneNameString();

                if (settings.Key_return_to_hog != "")
                {

                    if (Input.GetKeyDown(settings.Key_return_to_hog))
                    {
                        if (loop||(DoesDictContain(theCurrentScene) && PreviousScene == "GG_Workshop"))
                        {
                            StartCoroutine(LoadWorkshop());
                        }
                    }
                }

                if (settings.Key_teleport_around_HoG != "")
                {
                    if (Input.GetKeyDown(settings.Key_teleport_around_HoG))
                    {
                        if (theCurrentScene == "GG_Workshop")
                        {
                            FindNextStop();
                            PosToMove.Set(MoveAround[current_move][x_value], MoveAround[current_move][y_value], 0f);
                            HC.transform.position = PosToMove;
                        }
                    }
                }

                if (settings.Key_Reload_Boss != "")
                {
                    if (Input.GetKeyUp(settings.Key_Reload_Boss))
                    {
                        if (loop || (DoesDictContain(theCurrentScene) && PreviousScene == "GG_Workshop"))
                        {
                            LoadBossInLoop();
                        }
                    }
                }
            }

            public void LoadBossScene()
            {
                var HC = HeroController.instance;
                var GM = GameManager.instance;
                GameObject Inspect = EasierPantheonPractice.PreloadedObjects["Inspect"];

                PlayerData.instance.dreamReturnScene = "GG_Workshop";
                PlayMakerFSM.BroadcastEvent("BOX DOWN DREAM");
                PlayMakerFSM.BroadcastEvent("CONVO CANCEL");
                var Transition = Inspect.LocateMyFSM("GG Boss UI").GetAction<CreateObject>("Transition", 0).gameObject;

                foreach (var FSMObject in Transition.Value.GetComponentsInChildren<PlayMakerFSM>())
                {
                    FSMObject.SendEvent("GG TRANSITION OUT");
                }

                HC.ClearMPSendEvents();
                GM.TimePasses();
                GM.ResetSemiPersistentItems();
                HC.enterWithoutInput = true;
                HC.AcceptInput();


                GM.BeginSceneTransition(new GameManager.SceneLoadInfo
                {
                    SceneName = SceneToLoad,
                    EntryGateName = "door_dreamEnter",
                    EntryDelay = 0,
                    Visualization = GameManager.SceneLoadVisualizations.GodsAndGlory,
                    PreventCameraFadeOut = true
                });
            }

            private IEnumerator LoadWorkshop()
            {
                if (!(loop || PreviousScene == "GG_Workshop")) StopCoroutine(LoadWorkshop());
                loop = false;
                GameManager.instance.BeginSceneTransition(new GameManager.SceneLoadInfo
                {
                    IsFirstLevelForPlayer = false,
                    SceneName = "GG_Workshop",
                    HeroLeaveDirection = GlobalEnums.GatePosition.unknown,
                    EntryGateName = "left1",
                    PreventCameraFadeOut = false,
                    WaitForSceneTransitionCameraFade = true,
                    Visualization = GameManager.SceneLoadVisualizations.Default,
                    AlwaysUnloadUnusedAssets = false
                });

                yield return new WaitForFinishedEnteringScene();
                yield return new WaitForSceneLoadFinish();
                yield return new WaitUntil(() => HeroController.instance.acceptingInput);
                HeroController.instance.transform.position = OldPosition;
            }


            public void FindNextStop()
            {
                current_move = WhichNext;
                if (WhichNext == 3) WhichNext = 0;
                else WhichNext++;
            }

            #endregion

            public void LoadBossInLoop()
            {
                SceneToLoad = GameManager.instance.GetSceneNameString();
                loop = true;
                LoadBossScene();
            }
            private void OnDestroy()
            {
                ModHooks.Instance.BeforeSceneLoadHook -= BeforeSceneChange;
                USceneManager.sceneLoaded -= SceneManager_sceneLoaded;
                On.BossSceneController.DoDreamReturn -= DoDreamReturn;
                ModHooks.Instance.HeroUpdateHook -= HotKeys;
            }
            
    }
}
