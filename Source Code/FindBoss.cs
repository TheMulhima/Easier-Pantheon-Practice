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
        private static int current_blue_masks, damage_to_be_dealt;
        public static string CurrentBoss;                                                               //for bosses to be changed by the class BossNerf 
        public static string CurrentBoss_1, CurrentBoss_2, CurrentBoss_3, CurrentBoss_4, CurrentBoss_5;   //for bosses to be changed by the class HealthNerfOnly
        private static GameObject _TheBoss, _TheBoss_1, _TheBoss_2, _TheBoss_3, _TheBoss_4, _TheBoss_5;             //for bosses to be changed by the class HealthNerfOnly
        public static bool altered_1, altered_2, altered_3, altered_4, altered_5;                       //helps reuse the HealthNerfOnly class for SOB and WK
        public static bool SOB;                                                                
        private static Vector3 OldPosition, PosToMove;
        private static string CurrentScene;
        private static int current_move, WhichNext;
        const int x_value = 0, y_value = 1;
        private static bool loop;
        private static string SceneToLoad;



        Dictionary<int, List<float>> MoveAround = new Dictionary<int, List<float>>
        {
            { 0 , new List<float>{11f,36.4f } },//bench
            { 1 , new List<float>{207f,36.4f } },//nkg
            { 2 , new List<float>{29f,6.4f } },//gruz
            { 3 , new List<float>{207f,6.4f } },//gpz

        };

        Dictionary<string, string> _BossSceneName = new Dictionary<string, string>()
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

        List<string> Exceptions_BossSceneName = new List<string>()
        {
            "GG_Vengefly_V",
            "GG_Nailmasters",
            "GG_Soul_Master",
            "GG_Oblobbles",
            "GG_Mantis_Lords_V",
            "GG_God_Tamer",
            "GG_Watcher_Knights",
            "GG_Soul_Tyrant",
            "GG_Mantis_Lords",//not in P5 but added it for the sake of completion
        };

        
        private void Start()
        {
            ModHooks.Instance.BeforeSceneLoadHook += BeforeSceneChange;
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += SceneManager_sceneLoaded;
            On.BossSceneController.DoDreamReturn += DoDreamReturn;
            ModHooks.Instance.HeroUpdateHook += HotKeys;
        }
        private string BeforeSceneChange(string sceneName)
        {
            CurrentScene = GameManager.instance.sceneName;
            EasierPantheonPractice.Instance.Log($" The Current Scene was {CurrentScene}");
            if (CurrentScene == "GG_Workshop") OldPosition = HeroController.instance.transform.position;
            EasierPantheonPractice.Instance.Log($"old pos is {OldPosition}");
            return sceneName;
        }

        private void DoDreamReturn(On.BossSceneController.orig_DoDreamReturn orig, BossSceneController self)
        {
            loop = false;
            orig(self);
        }



        private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            
            StartCoroutine(WaitPls_SceneLoad());
            EasierPantheonPractice.Instance.Log($"Scene name = {arg0.name}");
            EasierPantheonPractice.Instance.Log($"Boss index {BossSequenceController.BossIndex}");
            altered_1 = altered_2 = altered_3 = altered_4 = altered_5 = false;
            SOB = true;
            
            ReflectionHelper.GetField(typeof(BossSequenceController), "bossIndex", false).SetValue(null, 1);
            
            if (EasierPantheonPractice._unloaded) return;
            EasierPantheonPractice.Instance.Log($"Boss index {BossSequenceController.BossIndex}");
            

            if (Exceptions_BossSceneName.Contains(CurrentScene) || _BossSceneName.ContainsKey(CurrentScene))
            {
                if (!loop)
                {
                    EasierPantheonPractice.Instance.Log("decreasing damage");
                    ModHooks.Instance.TakeHealthHook -= Only1Damage;
                }
            }

            if (Exceptions_BossSceneName.Contains(arg0.name) || _BossSceneName.ContainsKey(arg0.name))
            {
                if (!loop)
                {
                    EasierPantheonPractice.Instance.Log("increasing damage");
                    ModHooks.Instance.TakeHealthHook += Only1Damage;
                }
 
                if (Exceptions_BossSceneName.Contains(arg0.name))
                {
                    if (arg0.name == "GG_Vengefly_V") StartCoroutine(_2BossException("Giant Buzzer Col", "Giant Buzzer Col (1)"));
                    if (arg0.name == "GG_Soul_Master") StartCoroutine(_2BossException("Mage Lord", "Mage Lord Phase2"));
                    if (arg0.name == "GG_Oblobbles") StartCoroutine(_2BossException("Mega Fat Bee", "Mega Fat Bee (1)"));
                    if (arg0.name == "GG_God_Tamer") StartCoroutine(_2BossException("Lancer", "Lobster"));
                    if (arg0.name == "GG_Soul_Tyrant") StartCoroutine(_2BossException("Dream Mage Lord", "Dream Mage Lord Phase2"));
                    if (arg0.name == "GG_Nailmasters") StartCoroutine(_2BossException("Oro", "Mato"));
                    if (arg0.name == "GG_Mantis_Lords_V") StartCoroutine(SistersOfBattle(true));
                    if (arg0.name == "GG_Mantis_Lords") StartCoroutine(SistersOfBattle(false));
                    if (arg0.name == "GG_Watcher_Knights") StartCoroutine(WatcherKnight());
                }
                else if (_BossSceneName.ContainsKey(arg0.name))
                {
                    CurrentBoss = _BossSceneName[arg0.name];
                    StartCoroutine(p5Boss());
                }
            }
        }

        private static IEnumerator WaitPls_SceneLoad()
        {
            yield return null;
        }
        private static IEnumerator WaitPls_ApplySettings()
        {
            yield return new WaitForFinishedEnteringScene();
            if (loop)
            {
                yield return null;
                yield return null;
                yield return null;
            }
            ApplySettings();
        }


        #region Find Bosses
        private static IEnumerator p5Boss()
        {
            _TheBoss = GameObject.Find(CurrentBoss);
            GameManager.instance.StartCoroutine(WaitPls_ApplySettings());
            do
            {
                _TheBoss = GameObject.Find(CurrentBoss);
                yield return new WaitForSeconds(0.5f);
            }
            while (_TheBoss == null);
            EasierPantheonPractice.Instance.Log("Altered The Boss: " + CurrentBoss);
            _TheBoss.AddComponent<BossNerf>();
        }

        private static IEnumerator _2BossException(string Boss_name_1, string Boss_name_2)
        {

            CurrentBoss = Boss_name_1;
            CurrentBoss_1 = Boss_name_2;

            _TheBoss = GameObject.Find(CurrentBoss);
            _TheBoss_1 = GameObject.Find(CurrentBoss_1);
            GameManager.instance.StartCoroutine(WaitPls_ApplySettings());
            do
            {
                _TheBoss = GameObject.Find(CurrentBoss);
                _TheBoss_1 = GameObject.Find(CurrentBoss_1);
                yield return new WaitForSeconds(1.0f);
            }
            while (_TheBoss == null || _TheBoss_1 == null);
            EasierPantheonPractice.Instance.Log("Altered The Bosses: " + CurrentBoss + " and " + CurrentBoss_1);

            _TheBoss.AddComponent<BossNerf>();
            _TheBoss_1.AddComponent<HealthNerfOnly>();

        }
        private IEnumerator SistersOfBattle(bool isSOB)//function for SOB cuz there are 4 bosses in this one
        {
            SOB = isSOB;
            CurrentBoss = "Mantis Lord";
            CurrentBoss_1 = "Mantis Lord S1";
            CurrentBoss_2 = "Mantis Lord S2";
            if (SOB) CurrentBoss_3 = "Mantis Lord S3";

            _TheBoss = GameObject.Find(CurrentBoss);
            GameManager.instance.StartCoroutine(WaitPls_ApplySettings());

            do
            {
                _TheBoss = GameObject.Find(CurrentBoss);
                yield return new WaitForSeconds(0.5f);
            }
            while (_TheBoss == null);

            
            EasierPantheonPractice.Instance.Log("Altered The Boss: " + CurrentBoss);
            _TheBoss.AddComponent<BossNerf>();
            yield return new WaitForSeconds(1f);
            _TheBoss_1 = GameObject.Find(CurrentBoss_1);
            _TheBoss_2 = GameObject.Find(CurrentBoss_2);
            if (SOB) _TheBoss_3 = GameObject.Find(CurrentBoss_3);

            if (SOB)
            {
                while (_TheBoss_1 == null || _TheBoss_2 == null || _TheBoss_3 == null)//The lords dont exist before the 1st one dies
                {
                    _TheBoss_1 = GameObject.Find("Mantis Lord S1");
                    if (_TheBoss_1 != null) _TheBoss_1.AddComponent<HealthNerfOnly>();

                    _TheBoss_2 = GameObject.Find("Mantis Lord S2");
                    if (_TheBoss_2 != null) _TheBoss_2.AddComponent<HealthNerfOnly>();

                    _TheBoss_3 = GameObject.Find("Mantis Lord S3");
                    if (_TheBoss_3 != null) _TheBoss_3.AddComponent<HealthNerfOnly>();

                    yield return new WaitForSeconds(0.5f);
                }
            }
            else if (!SOB)
            {
                while (_TheBoss_1 == null || _TheBoss_2 == null)//The lords dont exist before the 1st one dies
                {
                    _TheBoss_1 = GameObject.Find("Mantis Lord S1");
                    if (_TheBoss_1 != null) _TheBoss_1.AddComponent<HealthNerfOnly>();

                    _TheBoss_2 = GameObject.Find("Mantis Lord S2");
                    if (_TheBoss_2 != null) _TheBoss_2.AddComponent<HealthNerfOnly>();

                    yield return new WaitForSeconds(0.5f);
                }
            }
            yield return new WaitForSeconds(1f);
        }

        private IEnumerator WatcherKnight()
        {
            CurrentBoss = "Black Knight 1";
            CurrentBoss_1 = "Black Knight 2";
            CurrentBoss_2 = "Black Knight 3";
            CurrentBoss_3 = "Black Knight 4";
            CurrentBoss_4 = "Black Knight 5";
            CurrentBoss_5 = "Black Knight 6";
            

            _TheBoss = GameObject.Find(CurrentBoss);
            GameManager.instance.StartCoroutine(WaitPls_ApplySettings());
            ApplySettings();
            do
            {
                _TheBoss = GameObject.Find(CurrentBoss);
                yield return new WaitForSeconds(0.5f);
            }
            while (_TheBoss == null);

            EasierPantheonPractice.Instance.Log("Altered The Boss: " + CurrentBoss);
            _TheBoss.AddComponent<BossNerf>();
            yield return new WaitForSeconds(0.5f);

            //should_check = true;
            _TheBoss_1 = GameObject.Find(CurrentBoss_1);//unlike SOB, the knights do exist before their turn comes
            _TheBoss_2 = GameObject.Find(CurrentBoss_2);
            _TheBoss_3 = GameObject.Find(CurrentBoss_3);
            _TheBoss_4 = GameObject.Find(CurrentBoss_4);
            _TheBoss_5 = GameObject.Find(CurrentBoss_5);
            _TheBoss_1.AddComponent<HealthNerfOnly>();
            _TheBoss_2.AddComponent<HealthNerfOnly>();
            _TheBoss_3.AddComponent<HealthNerfOnly>();
            _TheBoss_4.AddComponent<HealthNerfOnly>();
            _TheBoss_5.AddComponent<HealthNerfOnly>();
            yield return new WaitForSeconds(1f);
        }

        #endregion


        #region In Scene Stuff
        public static int Only1Damage(int damage)
        { 
            if (EasierPantheonPractice.Instance.settings.hitless_practice) return 1000;

            current_blue_masks = PlayerData.instance.healthBlue;

            if (BossSceneController.Instance.BossLevel == 0) damage_to_be_dealt = damage;
            if (BossSceneController.Instance.BossLevel == 1) damage_to_be_dealt = damage / 2;

            if (current_blue_masks > 0)
            {
                if (current_blue_masks == damage_to_be_dealt)
                {
                    PlayerData.instance.healthBlue = damage_to_be_dealt = 0;
                    EventRegister.SendEvent("UPDATE BLUE HEALTH");
                }
            }
            return damage_to_be_dealt;
        }

        private static void ApplySettings()
        {
            if (!EasierPantheonPractice.Instance.settings.hitless_practice)
            {
                EasierPantheonPractice.Instance.Log("Applying Setting");

                if (BossSceneController.Instance.BossLevel == 0) HeroController.instance.TakeHealth(EasierPantheonPractice.Instance.settings.remove_health);
                if (BossSceneController.Instance.BossLevel == 1) HeroController.instance.TakeHealth(2 * EasierPantheonPractice.Instance.settings.remove_health);

                for (int lifeblood_increment = 0; lifeblood_increment < EasierPantheonPractice.Instance.settings.lifeblood; lifeblood_increment++) EventRegister.SendEvent("ADD BLUE HEALTH");

            }

            HeroController.instance.AddMPCharge(EasierPantheonPractice.Instance.settings.soul);
            GameCameras.instance.soulOrbFSM.SendEvent("MP GAIN");

        }
        #endregion


        #region hotkeys
        
        public void HotKeys()
        {
            if (EasierPantheonPractice.Instance.settings.Key_return_to_hog != "")
            {
                if (Input.GetKeyDown(EasierPantheonPractice.Instance.settings.Key_return_to_hog))
                {
                    if (Exceptions_BossSceneName.Contains(GameManager.instance.GetSceneNameString()) || _BossSceneName.ContainsKey(GameManager.instance.GetSceneNameString()))
                    {
                        StartCoroutine(LoadWorkshop());
                    }
                }
            }
            if (EasierPantheonPractice.Instance.settings.Key_teleport_around_HoG != "")
            {
                if (Input.GetKeyDown(EasierPantheonPractice.Instance.settings.Key_teleport_around_HoG))
                {
                    FindNextStop();
                    PosToMove.Set(MoveAround[current_move][x_value], MoveAround[current_move][y_value], 0f);
                    HeroController.instance.transform.position = PosToMove;
                }
            }

            if (Input.GetKeyUp(KeyCode.N))
            {
                if (Exceptions_BossSceneName.Contains(GameManager.instance.GetSceneNameString()) || _BossSceneName.ContainsKey(GameManager.instance.GetSceneNameString()))
                {
                    LoadBossInLoop();
                }
            }
            
        }


        public void LoadBossScene()
        {
            PlayerData.instance.dreamReturnScene = "GG_Workshop";
            PlayMakerFSM.BroadcastEvent("BOX DOWN DREAM");
            PlayMakerFSM.BroadcastEvent("CONVO CANCEL");
            HutongGames.PlayMaker.FsmGameObject Transition = FsmUtil
                .GetAction<CreateObject>(EasierPantheonPractice.PreloadedObjects["Inspect"].LocateMyFSM("GG Boss UI"),
                    "Transition", 0).gameObject;

            foreach (var FSMObject in Transition.Value.GetComponentsInChildren<PlayMakerFSM>())
            {
                FSMObject.SendEvent("GG TRANSITION OUT");
            }

            HeroController.instance.ClearMPSendEvents();

            GameManager.instance.TimePasses();
            GameManager.instance.ResetSemiPersistentItems();
            HeroController.instance.enterWithoutInput = true;
            HeroController.instance.AcceptInput();


            GameManager.instance.BeginSceneTransition(new GameManager.SceneLoadInfo
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
            yield return new WaitForSecondsRealtime(0.5f);
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
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
            On.BossSceneController.DoDreamReturn -= DoDreamReturn;
            ModHooks.Instance.HeroUpdateHook -= HotKeys;
        }


    }
}
