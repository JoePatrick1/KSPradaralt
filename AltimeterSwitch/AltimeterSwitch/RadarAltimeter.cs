using UnityEngine;
using KSP.UI.Screens.Flight;
using System.Text.RegularExpressions;

namespace RadarAltimeter
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class AltimeterSwitch : MonoBehaviour
    {
        public double radarAlt;
        public double alt;
        public AltitudeTumbler altTumbler;
        public bool radarMode;
        public float keyCheck;
        public KeyCode hotkey;
        public KeyCode windowHotkey;
        public bool windowActive = true;
        public Color32 radarColour;
        public bool keyActive;
        public bool windowKeyActive;

        public void Start()
        {
            altTumbler = FindObjectOfType<AltitudeTumbler>();
            radarColour = new Color32(30, 50, 100, 255);
            hotkey = KeyCode.BackQuote;
            windowHotkey = KeyCode.Alpha0;

            if (PlayerPrefs.GetInt("newKey") == 1)
            {
                int id = 0;
                int keyID = PlayerPrefs.GetInt("RAhotkey");
                foreach (KeyCode vKey in System.Enum.GetValues(typeof(KeyCode)))
                {
                    if (id == keyID)
                    {
                        hotkey = vKey;
                        break;
                    }
                    else
                        id++;
                }
            }

            if (PlayerPrefs.GetInt("newKeyWin") == 1)
            {
                int id = 0;
                int keyID = PlayerPrefs.GetInt("RAhotkeyWin");
                foreach (KeyCode vKey in System.Enum.GetValues(typeof(KeyCode)))
                {
                    if (id == keyID)
                    {
                        windowHotkey = vKey;
                        break;
                    }
                    else
                        id++;
                }
            }
        }

        public void Update()
        {
            
            if (Input.GetKeyDown(hotkey) && keyActive)
                radarMode = !radarMode;

            if (Input.GetKeyDown(windowHotkey) && windowKeyActive)
                windowActive = !windowActive;

            alt = FlightGlobals.ActiveVessel.altitude;
            if (alt < 0) radarMode = false;

            if (radarMode)
            {
                radarAlt = FlightGlobals.ActiveVessel.radarAltitude;
                altTumbler.tumbler.SetColor(radarColour);
                altTumbler.tumbler.SetValue(radarAlt);
            }
            else if (alt > 0)
            {
                altTumbler.tumbler.SetColor(Color.black);
            }

            if (hotkeyButton || windowHotkeyButton)
            {
                keyActive = false;
                windowKeyActive = false;
            }
            else
            {
                keyActive = true;
                windowKeyActive = true;
            }
        }

        public void ChangeHotkey(int key, string ppName, string ppnkName)
        {
            int id = 0;
            ScreenMessages.PostScreenMessage("Press the key you want to set as your hotkey", 0.1f, ScreenMessageStyle.UPPER_CENTER);
            foreach (KeyCode vKey in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(vKey) && !Input.GetMouseButton(0))
                {
                    if(key == 0) hotkey = vKey;
                    if (key == 1) windowHotkey = vKey;
                    PlayerPrefs.SetInt(ppName, id);
                    PlayerPrefs.SetInt(ppnkName, 1);
                    PlayerPrefs.Save();
                    hotkeyButton = false;
                    windowHotkeyButton = false;
                }
                id++;
            }
        }

        private Rect mainWindowRect = new Rect(100, 100, 250, 95);
        void OnGUI()
        {
            if (windowActive)
                mainWindowRect = GUILayout.Window(548, mainWindowRect, windowFunc, "Radar Altimeter Settings", HighLogic.Skin.window);
        }

        public bool hotkeyButton;
        public bool windowHotkeyButton;

        void windowFunc(int windowID){
            GUILayout.BeginHorizontal();
            GUILayout.Label("Hotkey: " + hotkey);
            if (hotkeyButton = GUILayout.Toggle(hotkeyButton, "Change", HighLogic.Skin.button))
                ChangeHotkey(0, "RAhotkey","newKey");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Window Hotkey: " + windowHotkey);
            if (windowHotkeyButton = GUILayout.Toggle(windowHotkeyButton, "Change", HighLogic.Skin.button))
                ChangeHotkey(1, "RAhotkeyWin", "newKeyWin");
            GUILayout.EndHorizontal();

            GUI.DragWindow();
        }

    }
}
