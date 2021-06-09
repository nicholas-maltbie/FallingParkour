using NUnit.Framework;
using PropHunt.Environment.Sound;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

namespace Tests.EditMode.UI.Actions
{
    [TestFixture]
    public class LoadAudioMixerLevelsTests
    {
        [Test]
        public void TestLoadAudioLevel()
        {
            // Setup the object
            var loadAudioLevels = new GameObject().AddComponent<LoadAudioMixerLevels>();
            AudioMixer audioMixer = AssetDatabase.LoadAssetAtPath<AudioMixer>("Assets/Sound/Settings/AudioMixer.mixer");
            loadAudioLevels.mixerGroups = new AudioMixerGroup[1] { audioMixer.FindMatchingGroups("Master")[0] };

            // Test setup
            loadAudioLevels.Start();

            // Cleanup
            GameObject.DestroyImmediate(loadAudioLevels.gameObject);
        }
    }
}