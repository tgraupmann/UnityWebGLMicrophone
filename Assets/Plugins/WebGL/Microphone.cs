#if UNITY_WEBGL && !UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace UnityEngine
{
    public class Microphone
    {
        [DllImport("__Internal")]
        public static extern void Init();

        [DllImport("__Internal")]
        public static extern void QueryAudioInput();

        [DllImport("__Internal")]
        private static extern int GetNumberOfMicrophones();

        [DllImport("__Internal")]
        private static extern string GetMicrophoneDeviceName(int index);

        [DllImport("__Internal")]
        private static extern float GetMicrophoneVolume(int index);

        private static List<Action> _sActions = new List<Action>();

        public static void Update()
        {
            for (int i = 0; i < _sActions.Count; ++i)
            {
                Action action = _sActions[i];
                action.Invoke();
            }
        }

        public static string[] devices
        {
            get
            {
                List<string> list = new List<string>();
                int size = GetNumberOfMicrophones();
                for (int index = 0; index < size; ++index)
                {
                    string deviceName = GetMicrophoneDeviceName(index);
                    list.Add(deviceName);
                }
                return list.ToArray();
            }
        }

        public static float[] volumes
        {
            get
            {
                List<float> list = new List<float>();
                int size = GetNumberOfMicrophones();
                for (int index = 0; index < size; ++index)
                {
                    float volume = GetMicrophoneVolume(index);
                    list.Add(volume);
                }
                return list.ToArray();
            }
        }

        public static bool IsRecording(string deviceName)
        {
            return false;
        }

        public static void GetDeviceCaps(string deviceName, out int minFreq, out int maxFreq)
        {
            minFreq = 0;
            maxFreq = 0;
        }

        public static void End(string deviceName)
        {
        }
    }
}

#endif