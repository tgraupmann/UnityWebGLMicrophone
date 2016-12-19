using UnityEngine;

namespace UnityWebGLMicrophone
{
    public class DisplayMics : MonoBehaviour
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        void Awake()
        {
            Microphone.Init();
            Microphone.QueryAudioInput();
        }
#endif

#if UNITY_WEBGL && !UNITY_EDITOR
        void Update()
        {
            Microphone.Update();
        }
#endif

        void OnGUI()
        {
            GUILayout.BeginVertical(GUILayout.Height(Screen.height));
            GUILayout.FlexibleSpace();

            string[] devices = Microphone.devices;

            GUILayout.BeginHorizontal(GUILayout.Width(Screen.width));
            GUILayout.FlexibleSpace();
            GUILayout.Label(string.Format("Microphone count={0}", devices.Length));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            for (int index = 0; index < devices.Length; ++index)
            {
                string deviceName = devices[index];
                if (deviceName == null)
                {
                    deviceName = string.Empty;
                }

                GUILayout.BeginHorizontal(GUILayout.Width(Screen.width));
                GUILayout.FlexibleSpace();
                GUILayout.Label(string.Format("Device Name={0}", deviceName));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
        }
    }
}
