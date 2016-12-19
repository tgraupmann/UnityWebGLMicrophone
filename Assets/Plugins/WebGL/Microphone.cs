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
        private static extern void InitJavaScriptSharedArray(float[] array, int length);

        [DllImport("__Internal")]
        private static extern int GetMicrophonePosition();

        [DllImport("__Internal")]
        public static extern void QueryAudioInput();

        [DllImport("__Internal")]
        private static extern int GetNumberOfMicrophones();

        [DllImport("__Internal")]
        private static extern string GetMicrophoneDeviceName(int index);

        public static float[] _sSharedArray = null;

        private static List<Action> _sActions = new List<Action>();

        private static Dictionary<string, bool> _sRecording = new Dictionary<string, bool>();

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

        public static bool IsRecording(string deviceName)
        {
            return _sRecording.ContainsKey(deviceName);
        }

        public static int GetPosition(string deviceName)
        {
            return GetMicrophonePosition();
        }

        public static void GetDeviceCaps(string deviceName, out int minFreq, out int maxFreq)
        {
            minFreq = 0;
            maxFreq = 0;
        }

        public static AudioClip Start(string deviceName, bool loop, int lengthSec, int frequency)
        {
            int size = lengthSec * frequency;
            if (null == _sSharedArray)
            {
                _sSharedArray = new float[size];
                InitJavaScriptSharedArray (_sSharedArray, _sSharedArray.Length);
            }
            int channels = 1;
            AudioClip audioClip = AudioClip.Create("MySinusoid", frequency * channels, 1, frequency, false, OnAudioRead, OnAudioSetPosition);
            Action action = () =>
            {
                audioClip.SetData(_sSharedArray, 0);
            };
            action.Invoke();
            _sActions.Add(action);
            _sRecording[deviceName] = true;
            return audioClip;
        }

        public static void End(string deviceName)
        {
            if (_sRecording.ContainsKey(deviceName))
            {
                _sRecording.Remove(deviceName);
            }
        }

        static void OnAudioRead(float[] data)
        {
            Array.Copy(_sSharedArray, data, data.Length);
        }

        static void OnAudioSetPosition(int newPosition)
        {
        }
    }
}

#endif