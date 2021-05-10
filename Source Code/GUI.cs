using System.Collections;
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
            checking_for_bind = false;
        }

        public void OnGUI()
        {
            var gm = GameManager.instance;
            var instance = EasierPantheonPractice.Instance;
            var settings = instance.settings;
            
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
            checking_for_bind = true;
        }
        
        public void Update()
        {
            var settings = EasierPantheonPractice.Instance.settings;
            if (!checking_for_bind) return;
            Event e = Event.current;
            if (e.isKey)
            {
                looking_for_input = e.character.ToString();
                try
                {
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
                }
                catch
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
                    _textObj.text = "The binding was unsuccessful ";
                    GameManager.instance.StartCoroutine(
                        DeleteText("The binding was unsuccessful "));
                }

               
                checking_for_bind = false;

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
            yield return new WaitForSecondsRealtime(3f);
            if (_textObj.text == calling_text) _textObj.text = "";
        }
    }
}