using System;
using Modding;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using USceneManager = UnityEngine.SceneManagement.SceneManager;
using UnityEngine;
using System.Collections;
using UObject = UnityEngine.Object;



namespace Easier_Pantheon_Practice
{
    public class FindBoss : MonoBehaviour
    {
        private const int x_value = 0, y_value = 1; //to make the move position function more readable
        private static int damage_to_be_dealt, current_move;
        public static bool altered, SOB; //to allow some functions to be readable
        private static bool loop;
        private static string PreviousScene, SceneToLoad;
        public static string CurrentBoss, CurrentBoss_1;
        private static Vector3 OldPosition, PosToMove;

        private static readonly Dictionary<int, List<float>> MoveAround = new Dictionary<int, List<float>>
        {
            {0, new List<float> {11f, 36.4f}}, //bench
            {1, new List<float> {207f, 6.4f}}, //gpz
        };


        //Dict for bosses that have only 1 boss in them
        private static readonly Dictionary<string, string> _BossSceneName = new Dictionary<string, string>()
        {
            {"GG_Gruz_Mother_V", "Giant Fly"},
            {"GG_False_Knight", "False Knight New"},
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
            {"GG_Sly", "Sly Boss"},
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
            {"GG_Vengefly", "Giant Buzzer Col"},
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


        //Dict for bosses that have only 2 boss in them
        private static readonly Dictionary<string, List<string>> SemiExceptions_BossSceneName =
            new Dictionary<string, List<string>>()
            {
                {"GG_Vengefly_V", new List<string>() {"Giant Buzzer Col", "Giant Buzzer Col (1)"}},
                {"GG_Soul_Master", new List<string>() {"Mage Lord", "Mage Lord Phase2"}},
                {"GG_Oblobbles", new List<string>() {"Mega Fat Bee", "Mega Fat Bee (1)"}},
                {"GG_Soul_Tyrant", new List<string>() {"Dream Mage Lord", "Dream Mage Lord Phase2"}},
                {"GG_Nailmasters", new List<string>() {"Oro", "Mato"}},
                {"GG_God_Tamer", new List<string>() {"Lancer", "Lobster"}},
            };

        //Dict for bosses that have more than 2 boss in them
        private static readonly List<string> Exceptions_BossSceneName = new List<string>()
        {
            "GG_Mantis_Lords_V",
            "GG_Watcher_Knights",
            "GG_Mantis_Lords",
        };

        public void Awake()
        {
            EasierPantheonPractice.Instance.Log("Component added");
        }

        private void Start()
        {
            ModHooks.BeforeSceneLoadHook += BeforeSceneChange;
            USceneManager.sceneLoaded += SceneManager_sceneLoaded;
            On.BossSceneController.DoDreamReturn += DoDreamReturn;
            ModHooks.HeroUpdateHook += HeroUpdateFunction;
            ModHooks.AfterTakeDamageHook += Only1Damage;
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


            if (DoesDictContain(arg0.name))
            {
                StartCoroutine(ApplySettings());

                if (EasierPantheonPractice.settings.only_apply_settings) return;

                SceneToLoad = arg0.name;
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

        #region Find and alter bosses

        private IEnumerator ChangeBoss(string BossName, bool wait = true)
        {
            //Thank you to redfrog for this non-cursed code (before it was a while loop which didnt make sense)
            if (wait) yield return new WaitUntil(() => GameObject.Find(BossName));
            GameObject.Find(BossName).AddComponent<BossNerf>();


        }

        private void p5Boss()
        {
            StartCoroutine(ChangeBoss(CurrentBoss));

            if (CurrentBoss_1 != "") StartCoroutine(ChangeBoss(CurrentBoss_1));
        }

        private IEnumerator SistersOfBattle(bool isSOB)
        {
            SOB = isSOB;
            CurrentBoss = "Mantis Lord";
            CurrentBoss_1 = "Mantis Lord S";

            StartCoroutine(ChangeBoss(CurrentBoss));

            yield return new WaitUntil(() => GameObject.Find(CurrentBoss_1 + "1")); //Waits for phase 2

            for (int i = 1; i <= (SOB ? 3 : 2); i++)
            {
                StartCoroutine(ChangeBoss(CurrentBoss_1 + i.ToString(), false));
            }
        }

        private IEnumerator WatcherKnight()
        {
            CurrentBoss = CurrentBoss_1 = "Black Knight ";

            yield return new WaitUntil(() => GameObject.Find(CurrentBoss + "1"));
            for (int i = 1; i <= 6; i++)
            {
                StartCoroutine(ChangeBoss(CurrentBoss + i.ToString(), false));
            }
        }

        #endregion

        #region Setup Player

        private IEnumerator ApplySettings()
        {
            //waiting
            yield return new WaitForFinishedEnteringScene();
            var settings = EasierPantheonPractice.settings;
            var BSC = BossSceneController.Instance;
            var HC = HeroController.instance;


            //remove health and add lifeblood
            if (!(settings.hitless_practice || BSC.BossLevel == 2)) //checks for hitless practice or radiant fights
            {
                HC.TakeHealth(settings.remove_health);

                for (int lifeblood_increment = 0; lifeblood_increment < settings.lifeblood; lifeblood_increment++)
                    EventRegister.SendEvent("ADD BLUE HEALTH");
            }

            //adds soul
            HC.AddMPCharge(settings.soul);

            //makes sure the HUD updates
            yield return null;
            HeroController.instance.AddMPCharge(1);
            HeroController.instance.AddMPCharge(-1);
        }

        private static int Only1Damage(int hazardType, int damage)
        {
            if (EasierPantheonPractice.settings.only_apply_settings) return damage;
            if (!(loop || (DoesDictContain(GameManager.instance.GetSceneNameString()) &&
                           PreviousScene == "GG_Workshop"))) return damage;
            damage_to_be_dealt = BossSceneController.Instance.BossLevel == 1 ? (damage / 2) : damage;

            if (EasierPantheonPractice.settings.hitless_practice) damage_to_be_dealt = 1000;

            return damage_to_be_dealt;
        }

        #endregion

        #region hotkeys

        public void HeroUpdateFunction()
        {
            if (!EasierPantheonPractice.settings.allow_reloads_in_loads) Hotkeys();
        }
        public void Update()
        {
            if (EasierPantheonPractice.settings.allow_reloads_in_loads) Hotkeys();
        }

        public static void Hotkeys()
        {
            var settings = EasierPantheonPractice.settings;
            var HC = HeroController.instance;


            string theCurrentScene = GameManager.instance.GetSceneNameString();

            if (settings.keybinds.Key_return_to_hog.WasPressed)
            {
                if (HC.acceptingInput)
                {
                    if (loop || (DoesDictContain(theCurrentScene) && PreviousScene == "GG_Workshop"))
                    {
                        GameManager.instance.StartCoroutine(LoadWorkshop());
                    }
                }
            }

            if (settings.keybinds.Key_teleport_around_HoG.WasPressed)
            {
                if (theCurrentScene == "GG_Workshop")
                {
                    current_move++;
                    PosToMove.Set(MoveAround[current_move % 2][x_value], MoveAround[current_move % 2][y_value], 0f);
                    HC.transform.position = PosToMove;
                }
            }

            if (settings.keybinds.Key_Reload_Boss.WasPressed)
            {
                if (loop || (DoesDictContain(theCurrentScene) && PreviousScene == "GG_Workshop"))
                {
                    LoadBossInLoop();
                }
            }
        }

        public static void LoadBossScene()
        {
            var HC = HeroController.instance;
            var GM = GameManager.instance;

            //Copy paste of the FSM that loads a boss from HoG
            PlayerData.instance.dreamReturnScene = "GG_Workshop";
            PlayMakerFSM.BroadcastEvent("BOX DOWN DREAM");
            PlayMakerFSM.BroadcastEvent("CONVO CANCEL");

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
            GameManager.instance.StartCoroutine(FixSoul());
        }

        private static IEnumerator FixSoul()
        {
            yield return new WaitForFinishedEnteringScene();
            yield return null;
            yield return new WaitForSeconds(1f); //this line differenciates this function from ApplySettings
            HeroController.instance.AddMPCharge(1);
            HeroController.instance.AddMPCharge(-1);
        }

        private static IEnumerator LoadWorkshop()
        {
            loop = false;
            GameManager.instance.BeginSceneTransition(new GameManager.SceneLoadInfo
            {
                SceneName = "GG_Workshop",
                EntryDelay = 0,
                PreventCameraFadeOut = true,
                EntryGateName = "left1",
                Visualization = GameManager.SceneLoadVisualizations.GodsAndGlory,
            });
            yield return new WaitForFinishedEnteringScene();
            yield return new WaitForSceneLoadFinish();
            yield return new WaitUntil(() => HeroController.instance.acceptingInput);
            HeroController.instance.transform.position = OldPosition;
        }

        #endregion

        public static void LoadBossInLoop()
        {
            loop = true;
            LoadBossScene();
        }

        #region Misc Functions


        private string BeforeSceneChange(string sceneName)
        {
            PreviousScene = GameManager.instance.sceneName;
            if (PreviousScene == "GG_Workshop")
            {
                OldPosition = HeroController.instance.transform.position;
            }

            return sceneName;
        }

        private void DoDreamReturn(On.BossSceneController.orig_DoDreamReturn orig, BossSceneController self)
        {
            //this comes to play when the player dies or dreamgates
            loop = false;
            orig(self);
        }

        private static bool DoesDictContain(string KeyToSearch)
        {
            return Exceptions_BossSceneName.Contains(KeyToSearch) || _BossSceneName.ContainsKey(KeyToSearch) ||
                   SemiExceptions_BossSceneName.ContainsKey(KeyToSearch);
        }

        private IEnumerator SceneLoaded()
        {
            yield return null;
        }


        #endregion

        private void OnDestroy()
        {
            ModHooks.BeforeSceneLoadHook -= BeforeSceneChange;
            USceneManager.sceneLoaded -= SceneManager_sceneLoaded;
            On.BossSceneController.DoDreamReturn -= DoDreamReturn;
            ModHooks.AfterTakeDamageHook -= Only1Damage;
            ModHooks.HeroUpdateHook -= HeroUpdateFunction;
        }
    }
}
