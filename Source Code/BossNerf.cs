using System.Collections.Generic;
using UnityEngine;
using ModCommon.Util;
using HutongGames.PlayMaker.Actions;

namespace Easier_Ascended
{
    internal class BossNerf : MonoBehaviour
    {
        private HealthManager health;
        private PlayMakerFSM _control;

        Dictionary<string, int> _BossHealth = new Dictionary<string, int>()//dict for boss healths
        { { "Giant Buzzer Col", 190 },
            { "Giant Fly", 650 },
            { "False Knight New", 260 },
            { "Mega Moss Charger", 9999 },//actual hp is 480. i did it for the trolls
            { "Hornet Boss 1", 900 },
            { "Ghost Warrior Slug", 650 },
            { "Dung Defender", 800 },
            { "Mage Knight", 750 },
            { "Mawlek Body", 750 },
            { "Oro", 500 },
            { "Ghost Warrior Xero", 650 },
            { "Mega Zombie Beam Miner (1)", 650 },
            { "Mage Lord", 600 },
            {" Mega Fat Bee",450 },
            { "Mantis Lord", 500 },
            { "Ghost Warrior Marmu", 416 },
            { "Fluke Mother", 500 },
            { "Infected Knight", 700 },
            { "Ghost Warrior Galien", 650 },
            { "Sheo Boss", 950 },
            { "Hive Knight", 850 },
            { "Ghost Warrior Hu", 600 },
            { "Jar Collector", 900 },
            { "Lancer", 750 },
            { "Grimm Boss", 1000 },
            { "Black Knight 1", 350 },
            { "Mega Jellyfish GG", 350 },
            { "Hornet Nosk", 750 },
            { "Sly Boss", 800 },
            { "Hornet Boss 2", 800 },
            { "Zombie Beam Miner Rematch", 650 },
            { "Lost Kin", 1200 },
            { "Ghost Warrior No Eyes", 570 },
            { "Mantis Traitor Lord", 800 },
            { "White Defender", 1600 },
            { "Dream Mage Lord", 900 },
            { "Ghost Warrior Markoth", 650 },
            { "Grey Prince", 1400 },
            {"False Knight Dream",360 },
            { "Nightmare Grimm Boss", 1250 },
            { "HK Prime", 1600 },
            { "Absolute Radiance", 3000 },
        };

        private void Awake()
        {
            health = gameObject.GetComponent<HealthManager>();
        }
        private void Start()
        {
            ApplySettings();

            //changes the bosses health
            health.hp = _BossHealth[FindBoss.CurrentBoss];
            Modding.Logger.Log("[Easier-Ascended]: Health of \"" + FindBoss.CurrentBoss + "\" changed to: " + health.hp);

            ChangeFSM();//some bosses' phase change/health doesnt occur correctly hence requies changing them by editting FSMs
        }

        private void ApplySettings()
        {
            Modding.Logger.Log("[Easier-Ascended]: Applying Damage Setting");
            if (EasierAscended.DoDamage > 1) HeroController.instance.TakeHealth(EasierAscended.DoDamage); ;//the removing health allows the player to move during boss scream. so its best not to do it if its not required

            Modding.Logger.Log("[Easier-Ascended]: Applying Soul Setting");
            PlayerData.instance.AddMPCharge(EasierAscended.AddSoul);

            Modding.Logger.Log("[Easier-Ascended]: Applying Lifeblood Setting");
            for (int lifeblood_increment = 0; lifeblood_increment < EasierAscended.AddBlueMasks; lifeblood_increment++) EventRegister.SendEvent("ADD BLUE HEALTH");//playerdata.instance.bluehealth doesnt update lifeblood on HUD
        }

        private void ChangeFSM()
        {
            if (FindBoss.CurrentBoss == "Grimm Boss" || FindBoss.CurrentBoss == "Nightmare Grimm Boss") Grimms();
            else if (FindBoss.CurrentBoss == "Jar Collector") Collector();
            else if (FindBoss.CurrentBoss == "HK Prime") PV();
            else if (FindBoss.CurrentBoss == "Oro") NailMasters();
            else if (FindBoss.CurrentBoss == "Dung Defender") DungDefender();
            else if (FindBoss.CurrentBoss == "Sly Boss") Sly();
            else if (FindBoss.CurrentBoss == "False Knight Dream") FailedChampion();
            else if (FindBoss.CurrentBoss == "False Knight New") FalseKnight();
            else if (FindBoss.CurrentBoss == "Mega Moss Charger") MMC();
        }
        private void PV()
        {
            Modding.Logger.Log("[Easier-Ascended]: Chaning FSM for" + FindBoss.CurrentBoss);
            _control = gameObject.LocateMyFSM("Control");
            _control.Fsm.GetFsmInt("Half HP").Value = health.hp * 2/3;//WHY IS THIS NAMED HALF HP???
            _control.Fsm.GetFsmInt("Quarter HP").Value = health.hp * 1 / 3;//WHY IS THIS NAMED QUATER HP???
        }
        private void Collector()
        {
            Modding.Logger.Log("[Easier-Ascended]: Chaning FSM for" + FindBoss.CurrentBoss);
            _control = gameObject.LocateMyFSM("Phase Control");
            _control.Fsm.GetFsmInt("Phase 2 HP").Value = 300;
        }
        private void Grimms()
        {
            Modding.Logger.Log("[Easier-Ascended]: Chaning FSM for" + FindBoss.CurrentBoss);
            _control = gameObject.LocateMyFSM("Control");
            _control.Fsm.GetFsmInt("Rage HP 1").Value = health.hp * 3 / 4;
            _control.Fsm.GetFsmInt("Rage HP 2").Value = health.hp * 2 / 4;
            _control.Fsm.GetFsmInt("Rage HP 3").Value = health.hp * 1 / 4;
        }
        private void NailMasters()
        {
            Modding.Logger.Log("[Easier-Ascended]: Chaning FSM for" + FindBoss.CurrentBoss);
            _control = gameObject.LocateMyFSM("nailmaster");
            _control.Fsm.GetFsmInt("P2 HP").Value = 600;
        }
        private void DungDefender()
        {
            Modding.Logger.Log("[Easier-Ascended]: Chaning FSM for" + FindBoss.CurrentBoss);
            _control = gameObject.LocateMyFSM("Dung Defender");
            _control.Fsm.GetFsmInt("Rage HP").Value = 350;
        }
        private void Sly()
        {
            Modding.Logger.Log("[Easier-Ascended]: Chaning FSM for" + FindBoss.CurrentBoss);
            _control = gameObject.LocateMyFSM("Control");
            _control.Fsm.GetFsmInt("Ascended HP").Value = 250;//WHY IS THIS NAMED ASCENDED HP IT LITERALLY MAKES NO SENSE!!
        }
        private void FailedChampion()
        {
            Modding.Logger.Log("[Easier-Ascended]: Chaning FSM for" + FindBoss.CurrentBoss);
            _control = gameObject.LocateMyFSM("FalseyControl");
            _control.Fsm.GetFsmInt("Recover HP").Value = 360;
        }
        private void FalseKnight()
        {
            Modding.Logger.Log("[Easier-Ascended]: Chaning FSM for" + FindBoss.CurrentBoss);
            _control = gameObject.LocateMyFSM("Check Health");//WHY DOES FK AND FC HAVE THEIR RECOVER HEALTH IN DIFFERENT FSMs
            _control.Fsm.GetFsmInt("Recover HP").Value = 260;
        }

        private void MMC()//for the trolls
        {
            PlayMakerFSM mmc  = gameObject.LocateMyFSM("Mossy Control");
            mmc.Fsm.GetFsmFloat("Charge Speed").Value *= 10;
            mmc.Fsm.GetFsmFloat("Accel").Value *= 10;
            mmc.Fsm.GetFsmFloat("Velocity").Value *= 10;
        }
    }


    internal class HealthNerfOnly : MonoBehaviour
    {
        private HealthManager health;
        

        Dictionary<string, int> Exceptions_BossHealth = new Dictionary<string, int>()
        {
            { "Giant Buzzer Col (1)", 450 },
            { "Dream Mage Lord Phase2", 350 },
            { "Lobster", 750 },
            { "Mega Fat Bee (1)", 450 },
            { "Mage Lord Phase2", 350 },
            { "Mato", 1000 },
            { "Mantis Lord S1", 750 },
            { "Mantis Lord S2", 750 },
            { "Mantis Lord S3", 750 },
            { "Black Knight 2", 350 },
            { "Black Knight 3", 350 },
            { "Black Knight 4", 350 },
            { "Black Knight 5", 350 },
            { "Black Knight 6", 350 }
        };

        private void Awake()
        {
            health = gameObject.GetComponent<HealthManager>();
        }
        private void Start()
        {
            //Modding.Logger.Log(FindBoss.CurrentBoss_1 +": "+ health.hp);
            if (FindBoss.altered_1 == false)//this if condition helps reuse this class for SOB and WK
            {
                health.hp = Exceptions_BossHealth[FindBoss.CurrentBoss_1];
                FindBoss.altered_1 = true;
                Modding.Logger.Log("[Easier-Ascended]: Health of \"" + FindBoss.CurrentBoss_1 + "\" changed to: " + health.hp);
            }
            else if (FindBoss.altered_2 == false)
            {
                health.hp = Exceptions_BossHealth[FindBoss.CurrentBoss_2];
                FindBoss.altered_2 = true;
                Modding.Logger.Log("[Easier-Ascended]: Health of \"" + FindBoss.CurrentBoss_2 + "\" changed to: " + health.hp);
            }
            else if (FindBoss.altered_3 == false)
            {
                health.hp = Exceptions_BossHealth[FindBoss.CurrentBoss_3];
                FindBoss.altered_3 = true;
                Modding.Logger.Log("[Easier-Ascended]: Health of \"" + FindBoss.CurrentBoss_3 + "\" changed to: " + health.hp);
            }
            else if (FindBoss.altered_4 == false)
            {
                health.hp = Exceptions_BossHealth[FindBoss.CurrentBoss_2];
                FindBoss.altered_4 = true;
                Modding.Logger.Log("[Easier-Ascended]: Health of \"" + FindBoss.CurrentBoss_4 + "\" changed to: " + health.hp);
            }
            else if (FindBoss.altered_5 == false)
            {
                health.hp = Exceptions_BossHealth[FindBoss.CurrentBoss_5];
                FindBoss.altered_5 = true;
                Modding.Logger.Log("[Easier-Ascended]: Health of \"" + FindBoss.CurrentBoss_5 + "\" changed to: " + health.hp);
            }
        }
    }
}