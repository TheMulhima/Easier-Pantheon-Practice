using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Modding;
using UnityEngine.UI;

namespace Easier_Pantheon_Practice
{
    internal class GUIForSettings : MonoBehaviour
    {
        public GUIStyle Default_Label, Default_Button;
        private static bool checking_for_bind;
        private static string looking_for_input, current_setting;
        private const int width = 375, height = 375;
        
        private void Awake()
        {
            checking_for_bind = false;//makes sure it doesnt directly start looking for input when GUI is activated
        }

        public void OnGUI()
        {
            var gm = GameManager.instance;
            var instance = EasierPantheonPractice.Instance;
            var settings = instance.settings;
            
            //only want it to show in pause menu, in HoG and not when looking for input
            if (!gm.isPaused) return;
            if (gm.GetSceneNameString() != "GG_Workshop") return;
            if (checking_for_bind) return;

            Default_Label = new GUIStyle(GUI.skin.label)
            {
                fontSize = 24,
                alignment = TextAnchor.MiddleCenter,
            };
            Default_Button = new GUIStyle(GUI.skin.button)
            {
                fontSize = 24,
                alignment = TextAnchor.MiddleCenter,
            };

            GUI.contentColor = Color.white;
            GUI.skin.font.name = "TrajanBold";
            
            GUILayout.BeginArea(new Rect(Screen.width - (width + 20), Screen.height - (Screen.height/2 > height ? Screen.height/2: height + 20), width, height));
            
            GUILayout.Label("Easier Pantheon Practice",Default_Label);
            GUI.contentColor = Color.gray;
            GUILayout.Label("Settings",Default_Label);
            GUI.contentColor = Color.white;
            GUI.skin.button.onHover.textColor = Color.grey;
            if (GUILayout.Button($"Remove Health: {settings.remove_health}", Default_Button))
            {
                settings.remove_health++;
                if (settings.remove_health >= PlayerData.instance.maxHealth) settings.remove_health = 0;
            }
            if (GUILayout.Button($"Lifeblood: {settings.lifeblood}", Default_Button))
            {
                settings.lifeblood++;
                if (settings.lifeblood > 10) settings.lifeblood = 0;
            }
            if (GUILayout.Button($"Soul: {settings.soul}", Default_Button))
            {
                settings.soul += 33;
                if (settings.soul > 198) settings.soul = 0;
            }
            if (GUILayout.Button($"Hitless Practice: {settings.hitless_practice}", Default_Button))
            {
                settings.hitless_practice = !settings.hitless_practice;
            }
            GUI.contentColor = Color.gray;
            GUILayout.Label("KeyBinds",Default_Label);
            GUI.contentColor = Color.white;
            if (GUILayout.Button($"Key: Return to HOG: {PrintSetting(settings.Key_return_to_hog)}", Default_Button))
            {
                Keybind("Key: Return to HOG");
            }
            if (GUILayout.Button($"Key: Reload Boss: {PrintSetting(settings.Key_Reload_Boss)}", Default_Button))
            {
                Keybind("Key: Reload Boss");
            }
            if (GUILayout.Button($"Key: Teleport Around HOG: {PrintSetting(settings.Key_teleport_around_HoG)}", Default_Button))
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
        
        //list of keys that can be inputted, its not a full list but i hope its enough
        private List<string> Keys = new List<string>()
        {
            "backspace",
            "delete",
            "tab",
            "clear",
            "return",
            "pause",
            "space",
            "up",
            "down",
            "right",
            "left",
            "insert",
            "home",
            "end",
            "page up",
            "page down",
            "f1",
            "f2",
            "f3",
            "f4",
            "f5",
            "f6",
            "f7",
            "f8",
            "f9",
            "f10",
            "f11",
            "f12",
            "f13",
            "f14",
            "f15",
            "0",
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "!",
            "\"",
            "#",
            "$",
            "&",
            "'",
            "(",
            ")",
            "*",
            "+",
            ",",
            "-",
            ".",
            "/",
            ":",
            ";",
            "<",
            "=",
            ">",
            "?",
            "@",
            "[",
            "\\",
            "]",
            "^",
            "_",
            "`",
            "a",
            "b",
            "c",
            "d",
            "e",
            "f",
            "g",
            "h",
            "i",
            "j",
            "k",
            "l",
            "m",
            "n",
            "o",
            "p",
            "q",
            "r",
            "s",
            "t",
            "u",
            "v",
            "w",
            "x",
            "y",
            "z",
            "numlock",
            "caps lock",
            "scroll lock",
            "right shift",
            "left shift",
            "right ctrl",
            "left ctrl",
            "right alt",
            "left alt",
            "[0]",
            "[1]",
            "[2]",
            "[3]",
            "[4]",
            "[5]",
            "[6]",
            "[7]",
            "[8]",
            "[9]",
            "[+]",
            "[-]",
            "[*]",
            "[/]",
            "[.]",
            "mouse 0",
            "mouse 1",
            "mouse 2",
        };

        public void Update()
        {
            var settings = EasierPantheonPractice.Instance.settings;
            if (!checking_for_bind) return;

            //make esc key remove a bind (also makes sure pause isnt bound to a key becuase that would cause problems
            if (Input.GetKeyDown("escape"))
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

            foreach (string keypress in Keys)
            {
                if (Input.GetKeyDown(keypress))
                {
                    looking_for_input = keypress;

                    Input.GetKeyDown(looking_for_input);
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
                30,
                TextAnchor.LowerRight,
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
    }
}