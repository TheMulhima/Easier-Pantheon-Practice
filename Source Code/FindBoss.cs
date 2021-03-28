using System.Collections.Generic;
using Modding;
using UnityEngine.SceneManagement;
using USceneManager = UnityEngine.SceneManagement.SceneManager;
using UnityEngine;
using System.Collections;

namespace Easier_Ascended
{
    internal class FindBoss : MonoBehaviour
    {
        public static int current_blue_masks, damage_to_be_dealt;
        public static string CurrentBoss;                                                               //for bosses to be changed by the class BossNerf 
        public static string CurrentBoss_1, CurrentBoss_2, CurrentBoss_3,CurrentBoss_4,CurrentBoss_5;   //for bosses to be changed by the class HealthNerfOnly
        public static GameObject _TheBoss;                                                               //for bosses to be changed by the class BossNerf 
        public static GameObject _TheBoss_1, _TheBoss_2, _TheBoss_3, _TheBoss_4,_TheBoss_5;             //for bosses to be changed by the class HealthNerfOnly
        public static bool altered_1, altered_2, altered_3, altered_4, altered_5;                       //helps reuse the HealthNerfOnly class for SOB and WK


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
            {"GG_Failed_Champion", "False Knight Dream" },              
            {"GG_Radiance", "Absolute Radiance"},
            {"GG_Hollow_Knight", "HK Prime"},
            {"GG_Grimm_Nightmare", "Nightmare Grimm Boss"}
        };//dict for boss scenes and boss names

        List<string> Exceptions_BossSceneName = new List<string>()//list for all the exceptions
        {
            "GG_Vengefly_V",
            "GG_Nailmasters",
            "GG_Soul_Master", 
            "GG_Oblobbles",
            "GG_Mantis_Lords_V",
            "GG_God_Tamer",
            "GG_Watcher_Knights",
            "GG_Soul_Tyrant",
        };

        private void Start()
        {
            USceneManager.activeSceneChanged += SceneChanged;
        }

        private void SceneChanged(Scene arg0, Scene arg1)
        {
            //Modding.Logger.Log("[Easier-Ascended]: Pervious scene is " + arg0.name + " and next scene is " + arg1.name);
            if (EasierAscended._unloaded) return;//helps keep mod toggleable

            if (arg0.name == "GG_Workshop")//makes sure the mod isnt triggered when playing pantheons
            {
                if (Exceptions_BossSceneName.Contains(arg1.name) || _BossSceneName.ContainsKey(arg1.name))
                {
                    ModHooks.Instance.TakeHealthHook += Only1Damage;
                    altered_1 = altered_2 = altered_3 = altered_4 = altered_5 = false;
                }
            }
            if ((Exceptions_BossSceneName.Contains(arg0.name) || _BossSceneName.ContainsKey(arg0.name))) ModHooks.Instance.TakeHealthHook -= Only1Damage;//removes the Only1Damage to make sure the half damage doesnt stack
            
            if (Exceptions_BossSceneName.Contains(arg1.name))
            {
                if (arg1.name == "GG_Vengefly_V") StartCoroutine(_2BossException("Giant Buzzer Col", "Giant Buzzer Col (1)"));     
                if (arg1.name == "GG_Soul_Master") StartCoroutine(_2BossException("Mage Lord", "Mage Lord Phase2"));                        
                if (arg1.name == "GG_Oblobbles") StartCoroutine(_2BossException("Mega Fat Bee", "Mega Fat Bee (1)"));                        
                if (arg1.name == "GG_God_Tamer") StartCoroutine(_2BossException("Lancer","Lobster"));                        
                if (arg1.name == "GG_Soul_Tyrant") StartCoroutine(_2BossException("Dream Mage Lord", "Dream Mage Lord Phase2"));                        
                if (arg1.name == "GG_Nailmasters") StartCoroutine(_2BossException("Oro", "Mato"));
                if (arg1.name == "GG_Mantis_Lords_V") StartCoroutine(SistersOfBattle());                        
                if (arg1.name == "GG_Watcher_Knights") StartCoroutine(WatcherKnight());
            }
            else if (_BossSceneName.ContainsKey(arg1.name))
            {
                CurrentBoss = _BossSceneName[arg1.name];
                StartCoroutine(p5Boss());
            }
            else return;
        }

        private static IEnumerator p5Boss()
        {
            //Modding.Logger.Log("[Easier-Ascended]: Altering Boss");
            _TheBoss = GameObject.Find(CurrentBoss);
            yield return new WaitForSeconds(0.5f);

            do//makes sure to keep checking for the boss until its found. the time taken is different for each boss
            {
                _TheBoss = GameObject.Find(CurrentBoss);
                //Modding.Logger.Log("[Easier-Ascended]: Waiting for: " + CurrentBoss);
                yield return new WaitForSeconds(0.5f);
            }
            while (_TheBoss == null);
            Modding.Logger.Log("[Easier-Ascended]: Altered The Boss: " + CurrentBoss);
            _TheBoss.AddComponent<BossNerf>();//reduces health and applys the settings

        }
        private static IEnumerator _2BossException(string Boss_name_1, string Boss_name_2)//for fights that have 2 bosses
        {

            CurrentBoss = Boss_name_1;
            CurrentBoss_1 = Boss_name_2;


            //Modding.Logger.Log("[Easier-Ascended]: Altering Boss " + CurrentBoss + " and " + CurrentBoss_1);
            _TheBoss = GameObject.Find(CurrentBoss);
            _TheBoss_1 = GameObject.Find(CurrentBoss_1);
            yield return new WaitForSeconds(0.5f);

            do
            {
                //Modding.Logger.Log("[Easier-Ascended]: Waiting for: " + CurrentBoss + " and " + CurrentBoss_1);
                _TheBoss = GameObject.Find(CurrentBoss);
                _TheBoss_1 = GameObject.Find(CurrentBoss_1);
                yield return new WaitForSeconds(1.0f);
            }
            while (_TheBoss == null || _TheBoss_1 == null);
            Modding.Logger.Log("[Easier-Ascended]: Altered The Bosses: " + CurrentBoss + " and " + CurrentBoss_1);

            _TheBoss.AddComponent<BossNerf>();
            _TheBoss_1.AddComponent<HealthNerfOnly>();//only nerfs health
        }

        private IEnumerator SistersOfBattle()//function for SOB cuz there are 4 bosses in this one
        {
            
            CurrentBoss = "Mantis Lord";
            CurrentBoss_1 = "Mantis Lord S1";
            CurrentBoss_2 = "Mantis Lord S2";
            CurrentBoss_3 = "Mantis Lord S3";
            //Modding.Logger.Log("[Easier-Ascended]: Altering Boss");

            _TheBoss = GameObject.Find(CurrentBoss);
            yield return new WaitForSeconds(0.5f);

            do
            {
                _TheBoss = GameObject.Find(CurrentBoss);
                //Modding.Logger.Log("[Easier-Ascended]: Waiting for: " + CurrentBoss);
                yield return new WaitForSeconds(0.5f);
            }
            while (_TheBoss == null);

            Modding.Logger.Log("[Easier-Ascended]: Altered The Boss: " + CurrentBoss);
            _TheBoss.AddComponent<BossNerf>();

            _TheBoss_1 =GameObject.Find(CurrentBoss_1);
            _TheBoss_2 = GameObject.Find(CurrentBoss_2);
            _TheBoss_3 =GameObject.Find(CurrentBoss_3);

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

        private IEnumerator WatcherKnight()
        {
            
            CurrentBoss = "Black Knight 1";
            CurrentBoss_1 = "Black Knight 2";
            CurrentBoss_2 = "Black Knight 3";
            CurrentBoss_3 = "Black Knight 4";
            CurrentBoss_4 = "Black Knight 5";
            CurrentBoss_5 = "Black Knight 6";
            //Modding.Logger.Log("[Easier-Ascended]: Altering Boss");

            _TheBoss = GameObject.Find(CurrentBoss);
            yield return new WaitForSeconds(0.5f);

            do
            {
                _TheBoss = GameObject.Find(CurrentBoss);
                //Modding.Logger.Log("[Easier-Ascended]: The Boss is: " + CurrentBoss);
                //Modding.Logger.Log("[Easier-Ascended]: Waiting");
                yield return new WaitForSeconds(0.5f);
            }
            while (_TheBoss == null);

            Modding.Logger.Log("[Easier-Ascended]: Altered The Boss: " + CurrentBoss);
            _TheBoss.AddComponent<BossNerf>();
            yield return new WaitForSeconds(0.5f);

            //Modding.Logger.Log("[Easier-Ascended]: Starting to attempt");

            _TheBoss_1 = GameObject.Find(CurrentBoss_1);//unlike SOB, the knights do exist before their turn comes
            _TheBoss_2 = GameObject.Find(CurrentBoss_2);
            _TheBoss_3 = GameObject.Find(CurrentBoss_3);
            _TheBoss_4 =GameObject.Find(CurrentBoss_4);
            _TheBoss_5 = GameObject.Find(CurrentBoss_5);
            _TheBoss_1.AddComponent<HealthNerfOnly>();
            _TheBoss_2.AddComponent<HealthNerfOnly>();
            _TheBoss_3.AddComponent<HealthNerfOnly>();
            _TheBoss_4.AddComponent<HealthNerfOnly>();
            _TheBoss_5.AddComponent<HealthNerfOnly>();
        }


        public static int Only1Damage(int damage)//does the p5 damage and fixes lifeblood issue
        {
            if (damage == 1) return 1; //makes it atleast playable in attuned
            current_blue_masks = PlayerData.instance.healthBlue;
            damage_to_be_dealt = damage / 2;
            //Modding.Logger.Log(damage + " became " + damage_to_be_dealt + " damage");

            if (current_blue_masks > 0)//fixes the lifeblood bug in hk 
            {
                if (current_blue_masks == damage_to_be_dealt)
                {
                    PlayerData.instance.healthBlue = 0;
                    EventRegister.SendEvent("UPDATE BLUE HEALTH");
                    return 0;
                }
            }
            return damage_to_be_dealt;
        }
        
        private void OnDestroy()
        {
            USceneManager.activeSceneChanged -= SceneChanged;
            ModHooks.Instance.TakeHealthHook -= Only1Damage;
        }
    }
}
