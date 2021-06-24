using System.Collections.Generic;
using NUnit.Framework;
using PropHunt.Game.Flow;
using PropHunt.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;
using static PropHunt.UI.GameTimerDisplay;

namespace Tests.EditMode.UI
{
    [TestFixture]
    public class GameTimerDisplayTests
    {
        GameObject timerGo;
        GameObject displayGo;
        GameTimerDisplay timerDisplay;
        GameTimer timer;

        [SetUp]
        public void SetUp()
        {
            // Setup timer object
            timerGo = new GameObject(); ;
            timer = timerGo.AddComponent<GameTimer>();
            timerGo.tag = GameTimerDisplay.gamePhaseTimerTag;

            // Setup timer display object
            displayGo = new GameObject();
            timerDisplay = displayGo.AddComponent<GameTimerDisplay>();
            timerDisplay.defaultColor = Color.black;
            timerDisplay.timerText = displayGo.AddComponent<Text>();
            timerDisplay.thresholds = new List<TimerColorChangeThreshold>();
            timerDisplay.thresholds.Add(new TimerColorChangeThreshold { time = 4.0f, textColor = Color.red });
            // Test setup behaviour
            timerDisplay.Start();
            timerDisplay.OnScreenLoaded();
        }

        [TearDown]
        public void TearDown()
        {
            timerDisplay.OnScreenUnloaded();
            // Cleanup objects
            GameObject.DestroyImmediate(timerGo);
            GameObject.DestroyImmediate(displayGo);
        }

        [Test]
        public void TestChangeThreshold()
        {
            timer.StartTimer(timerDisplay.thresholds[0].time * 2);
            this.timerDisplay.Update();
            Assert.That(timerDisplay.timerText.color, Is.EqualTo(timerDisplay.defaultColor));
            timer.StartTimer(timerDisplay.thresholds[0].time);
            this.timerDisplay.Update();
            Assert.That(timerDisplay.timerText.color, Is.EqualTo(timerDisplay.thresholds[0].textColor));
        }

        [Test]
        public void TestDisplayTimerProperly()
        {
            timer.StartTimer(10.0f);
            this.timerDisplay.Update();
            Assert.That(timerDisplay.timerText.text, Is.EqualTo(timer.GetTime()));
        }

        [Test]
        public void TestDisplayNoTimer()
        {
            timerGo.SetActive(false);
            this.timerDisplay.Update();
            Assert.That(timerDisplay.timerText.text, Is.EqualTo(""));
            timerGo.SetActive(true);
        }
    }
}