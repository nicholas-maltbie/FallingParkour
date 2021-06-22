using NUnit.Framework;
using PropHunt.Character;
using PropHunt.UI.Actions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace Tests.EditMode.UI.Actions
{
    [TestFixture]
    public class ChangeMouseSensitivityTests
    {
        [Test]
        public void TestChangeAudioLevel()
        {
            // Setup the object
            var mouseSettings = new GameObject().AddComponent<ChangeMouseSensitivity>();
            mouseSettings.slider = mouseSettings.gameObject.AddComponent<Slider>();

            // Test setup
            mouseSettings.Awake();

            // Test setting slider level
            mouseSettings.slider.onValueChanged?.Invoke(0.45f);
            // Test setting slider level to minimum
            mouseSettings.slider.onValueChanged?.Invoke(0.0f);
            // Test setting slider level to maximum
            mouseSettings.slider.onValueChanged?.Invoke(1.0f);

            // Test reading slider level below zero
            ChangeMouseSensitivity.GetSliderValue(PlayerInputManager.minimumMouseSensitivity - 10.0f);
            // Test reading slider level above max
            ChangeMouseSensitivity.GetSliderValue(PlayerInputManager.maximumMouseSensitivity + 10.0f);

            // Cleanup
            GameObject.DestroyImmediate(mouseSettings.gameObject);
        }
    }
}