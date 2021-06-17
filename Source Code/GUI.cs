using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Modding;
using UnityEngine.UI;
using System.Collections.Generic;
using Modding.Menu;
using Modding.Menu.Config;
using UnityEngine;
using UnityEngine.UI;
using Patch = Modding.Patches;


//TODO Add Hide UI option


namespace Easier_Pantheon_Practice
{
    internal class GUIForSettings : MonoBehaviour
    {
        public GUIStyle Default_Label, Default_Button;
        private static bool checking_for_bind;
        private static string looking_for_input, current_setting;
        private const int buffer = 50;
        private const int width = 230, heighta1 = 155, heighta2 = 125, height = 285;
        private const int widtha1 = 150, widtha2 = 30 ,widtha3 = 50 ;
        
        private void Awake()
        {
            checking_for_bind = false;//makes sure it doesnt directly start looking for input when GUI is activated
        }
        
        public void OnGUI()
        {
            var gm = GameManager.instance;
            var instance = EasierPantheonPractice.Instance;
            var settings = EasierPantheonPractice.settings;
            
            //only want it to show in pause menu, in HoG and not when looking for input
            if (!gm.isPaused) return;
            if (gm.GetSceneNameString() != "GG_Workshop") return;
            if (checking_for_bind) return;

            Default_Label = new GUIStyle(GUI.skin.label)
            {
                fontSize = 20,
                alignment = TextAnchor.MiddleCenter,
            };
            Default_Button = new GUIStyle(GUI.skin.button)
            {
                fontSize = 20,
                alignment = TextAnchor.MiddleCenter,
            };

            GUI.contentColor = Color.white;
            GUI.skin.font.name = "TrajanBold";

            if (!settings.ShowUI) return;
            GUILayout.BeginArea(new Rect(Screen.width - (width + buffer), Screen.height - (heighta1 + heighta2 + 20),
                widtha1, heighta1));
            
            GUI.contentColor = Color.gray;
            GUILayout.Label("Settings",Default_Label);
            GUI.contentColor = Color.white;
            
            GUILayout.Label("Remove Health",Default_Label);
            GUILayout.Label("Lifeblood",Default_Label);
            GUILayout.Label("Soul",Default_Label);
            GUILayout.Label("Hitless Practice}",Default_Label);
            
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(Screen.width - (width - widtha1 + buffer), Screen.height - (heighta1+heighta2 + 20), widtha2, heighta1));
            
            GUILayout.Label("",Default_Label);

            GUI.backgroundColor = new Color(249, 249, 249);
            if (GUILayout.Button("-", Default_Button))
            {
                if (settings.remove_health > 0) settings.remove_health--;
            }
            if (GUILayout.Button("-", Default_Button))
            {
                if (settings.lifeblood > 0) settings.lifeblood--;
            }
            if (GUILayout.Button("-", Default_Button))
            {
                if (settings.soul >= 33) settings.soul -= 33;
            }
            if (GUILayout.Button("-", Default_Button))
            {
                settings.hitless_practice = false;
            }
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(Screen.width - (width -widtha1-widtha2 + buffer), Screen.height - (heighta1+heighta2 + 20), widtha3, heighta1));
            
            GUI.contentColor = Color.gray;
            GUILayout.Label("",Default_Label);
            GUI.contentColor = Color.white;
            
            GUILayout.Label(settings.remove_health.ToString(), Default_Label);
            GUILayout.Label(settings.lifeblood.ToString(), Default_Label);
            GUILayout.Label(settings.soul.ToString(), Default_Label);
            GUILayout.Label(settings.hitless_practice.ToString(), Default_Label);
            
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(Screen.width - (width -widtha1-widtha2-widtha3 + buffer), Screen.height - (heighta1+heighta2 + 20), widtha2, heighta1));
            
            GUILayout.Label("",Default_Label);

            if (GUILayout.Button("+", Default_Button))
            {
                if (settings.remove_health < PlayerData.instance.maxHealth - 1) settings.remove_health++;
            }
            if (GUILayout.Button("+", Default_Button))
            {
                if (settings.lifeblood < 10) settings.lifeblood++;
            }
            if (GUILayout.Button("+", Default_Button))
            {
                if (settings.soul < 198) settings.soul += 33;
            }
            if (GUILayout.Button("+", Default_Button))
            {
                settings.hitless_practice = true;
            }
            GUILayout.EndArea();
            
            
            GUILayout.BeginArea(new Rect(Screen.width - (width + buffer), Screen.height - (heighta2 + 15),
                widtha1, heighta2));
            
            GUI.contentColor = Color.gray;
            GUILayout.Label("KeyBinds",Default_Label);
            GUI.contentColor = Color.white;
            
            GUILayout.Label($"Return to HOG", Default_Label);
            GUILayout.Label($"Reload Boss", Default_Label);
            GUILayout.Label($"Teleport in HoG", Default_Label);
            
            GUILayout.EndArea();
            
            GUILayout.BeginArea(new Rect(Screen.width - (width -widtha1 + buffer), Screen.height - (heighta2 + 15), widtha2, heighta2));

            GUILayout.Label("",Default_Label);
            
            GUILayout.Label(PrintSetting(settings.Key_return_to_hog), Default_Label);
            GUILayout.Label(PrintSetting(settings.Key_Reload_Boss), Default_Label);
            GUILayout.Label(PrintSetting(settings.Key_teleport_around_HoG), Default_Label);
            
            GUILayout.EndArea();
            
            
            GUILayout.BeginArea(new Rect(Screen.width - (width -widtha1-widtha2 + buffer), Screen.height - (heighta2 + 15), widtha2 + widtha3, heighta2));

            GUILayout.Label("",Default_Label);

            if (GUILayout.Button("Bind", Default_Button))
            {
                Keybind("Key: Return to HOG");
            }
            if (GUILayout.Button("Bind", Default_Button))
            {
                Keybind("Key: Reload Boss");
            }
            if (GUILayout.Button("Bind", Default_Button))
            {
                Keybind("Key: Teleport Around HOG");
            }
            
            GUILayout.EndArea();
            
        }

        private string PrintSetting(string current_key_bind) => current_key_bind == "" ? "None" : current_key_bind;

        private void Keybind(string the_current_setting)
        {
            CreateCanvas();
            _textObj.text = "Waiting for input ";
            current_setting = the_current_setting;
            checking_for_bind = true;//removes the GUI to avoid confusion
        }
        public void Update()
        {
            var settings = EasierPantheonPractice.settings;
            if (!checking_for_bind) return;

            foreach (KeyCode keypress in Enum.GetValues(typeof(KeyCode)))
            {
               
                if (Input.GetKeyDown(keypress))
                {
                    //make esc key remove a bind (also makes sure pause isnt bound to a key becuase that would cause problem
                    if (keypress == KeyCode.Escape)
                    {
                        if (Input.GetKeyDown(KeyCode.Escape))
                        {
                            looking_for_input = "";
                            switch (current_setting)
                            {
                                case "Key: Return to HOG":
                                    settings.Key_return_to_hog = looking_for_input;
                                    break;
                                case "Key: Reload Boss":
                                    settings.Key_Reload_Boss = looking_for_input;
                                    break;
                                case "Key: Teleport Around HOG":
                                    settings.Key_teleport_around_HoG = looking_for_input;
                                    break;
                            }

                            _textObj.text = "The key was unbound  ";
                            GameManager.instance.StartCoroutine(
                                DeleteText("The key was unbound  "));
                            checking_for_bind = false;
                        }
                    }
                    else
                    {
                        looking_for_input = Enum.GetName(typeof(KeyCode), keypress);

                        switch (current_setting)
                        {
                            case "Key: Return to HOG":
                                settings.Key_return_to_hog = looking_for_input;
                                break;
                            case "Key: Reload Boss":
                                settings.Key_Reload_Boss = looking_for_input;
                                break;
                            case "Key: Teleport Around HOG":
                                settings.Key_teleport_around_HoG = looking_for_input;
                                break;
                        }

                        _textObj.text = $"The binding for {current_setting} is {looking_for_input} ";
                        GameManager.instance.StartCoroutine(
                            DeleteText($"The binding for {current_setting} is {looking_for_input} "));
                        checking_for_bind = false;
                    }
                }
            }
        }

        public static Text _textObj;
        public static GameObject _canvas;

        public void CreateCanvas()
        {
            if (_canvas != null) return;

            CanvasUtil.CreateFonts();
            _canvas = CanvasUtil.CreateCanvas(RenderMode.ScreenSpaceOverlay, new Vector2(1920, 1080));
            UnityEngine.Object.DontDestroyOnLoad(_canvas);

            GameObject canvas = CanvasUtil.CreateTextPanel
            (
                _canvas,
                "",
                20,
                TextAnchor.LowerLeft,
                new CanvasUtil.RectData
                (
                    new Vector2(0, 50),
                    new Vector2(0, 45),
                    new Vector2(0, 0),
                    new Vector2(1, 0),
                    new Vector2(0.5f, 0.5f)
                )
            );

            _textObj = canvas.GetComponent<Text>();
            _textObj.font = CanvasUtil.TrajanBold;
            _textObj.text = "";
            _textObj.fontSize = 30;
        }
        private IEnumerator DeleteText(string calling_text)
        {
            //need to check for calling text or else it causes text to insta-delete if alot of text is put on screen
            yield return new WaitForSecondsRealtime(3f);
            if (_textObj.text == calling_text) _textObj.text = "";
        }

        public void OnDestroy()
        {
            UnityEngine.Object.Destroy(_canvas);
            UnityEngine.Object.Destroy(_textObj);
        }
    }

    internal class ModMenu
    {
        private MenuScreen screen;
    }
}
