using System.Collections;
using NUnit.Framework;
using PropHunt.UI;
using PropHunt.UI.Actions;
using UnityEngine;
using UnityEngine.UI;

namespace Tests.EditMode.UI.Actions
{
    [TestFixture]
    public class ChangeScreenActionsTests
    {
        ChangeScreenActions changeScreen;
        Dropdown windowedDropdown;
        Dropdown resolutionDropdown;
        Dropdown displayDropdown;

        [SetUp]
        public void SetUp()
        {
            GameObject go = new GameObject();
            ScreenLoading.setupDisplay = false;
            changeScreen = go.AddComponent<ChangeScreenActions>();

            // Setup dropdowns
            changeScreen.displayDropdown = new GameObject().AddComponent<Dropdown>();
            changeScreen.windowedDropdown = new GameObject().AddComponent<Dropdown>();
            changeScreen.resolutionDropdown = new GameObject().AddComponent<Dropdown>();
            changeScreen.vsyncToggle = new GameObject().AddComponent<Toggle>();

            // Attach dropdons to objects
            changeScreen.displayDropdown.transform.parent = changeScreen.transform;
            changeScreen.windowedDropdown.transform.parent = changeScreen.transform;
            changeScreen.resolutionDropdown.transform.parent = changeScreen.transform;
            changeScreen.vsyncToggle.transform.parent = changeScreen.transform;

            // Attach confirm dialog
            GameObject confirmDialog = new GameObject();
            GameObject settingsPanel = new GameObject();
            confirmDialog.transform.parent = go.transform;
            settingsPanel.transform.parent = go.transform;

            changeScreen.settingsPage = settingsPanel.AddComponent<CanvasGroup>();
            changeScreen.confirmPanel = confirmDialog.AddComponent<CanvasGroup>();
            changeScreen.confirmDialogYes = new GameObject().AddComponent<Button>();
            changeScreen.confirmDialogNo = new GameObject().AddComponent<Button>();
            changeScreen.settingsMenuController = settingsPanel.AddComponent<MenuController>();
            changeScreen.confirmDialogYes.gameObject.transform.parent = confirmDialog.transform;
            changeScreen.confirmDialogNo.gameObject.transform.parent = confirmDialog.transform;
            changeScreen.confirmDialogText = confirmDialog.AddComponent<Text>();

            // Setup change screne object
            changeScreen.Awake();
        }

        [TearDown]
        public void TearDown()
        {
            GameObject.DestroyImmediate(changeScreen.gameObject);
        }

        [Test]
        public void ModifyScreenSettings()
        {
            // Simulate selecting an option from each control
            changeScreen.windowedDropdown.onValueChanged?.Invoke(0);
            changeScreen.displayDropdown.onValueChanged?.Invoke(0);
            changeScreen.resolutionDropdown.onValueChanged?.Invoke(0);
            changeScreen.vsyncToggle.onValueChanged?.Invoke(false);
        }

        [Test]
        public void TestFullScreenLookup()
        {
            // Simulate translations of each setting
            ChangeScreenActions.GetFullScreenModeDropdownIndex(FullScreenMode.ExclusiveFullScreen);
            ChangeScreenActions.GetFullScreenModeDropdownIndex(FullScreenMode.FullScreenWindow);
            ChangeScreenActions.GetFullScreenModeDropdownIndex(FullScreenMode.MaximizedWindow);
            ChangeScreenActions.GetFullScreenModeDropdownIndex(FullScreenMode.Windowed);

            Assert.IsTrue(ChangeScreenActions.GetFullScreenMode(ScreenLoading.fullScreenModeName)
                == FullScreenMode.ExclusiveFullScreen);
            Assert.IsTrue(ChangeScreenActions.GetFullScreenMode(ScreenLoading.windowedModeName)
                == FullScreenMode.Windowed);
            Assert.IsTrue(ChangeScreenActions.GetFullScreenMode(ScreenLoading.borderlessWindowModeName)
                == FullScreenMode.FullScreenWindow);
            Assert.IsTrue(ChangeScreenActions.GetFullScreenMode("other")
                == FullScreenMode.ExclusiveFullScreen);
        }

        [Test]
        public void ConfirmDialogSettings()
        {
            // Simulate waiting for a timeout
            // Create a confirm dialog
            IEnumerator confirm = this.changeScreen.OpenConfirmChangesDialog();
            // Wait for timeout and confirm revert
            while (confirm.MoveNext()) ;

            // Wait for timeout but interrupt early by clicking the yes button
            confirm = this.changeScreen.OpenConfirmChangesDialog();
            confirm.MoveNext();
            confirm.MoveNext();
            confirm.MoveNext();
            // end early by cancel
            this.changeScreen.confirmDialogYes.onClick?.Invoke();
            // Finish the co-routine
            while (confirm.MoveNext()) ;

            // Wait for timeout but interrupt early by clicking the no button
            confirm = this.changeScreen.OpenConfirmChangesDialog();
            confirm.MoveNext();
            confirm.MoveNext();
            confirm.MoveNext();
            // end early by cancel
            this.changeScreen.confirmDialogNo.onClick?.Invoke();
            // Finish the co-routine
            while (confirm.MoveNext()) ;
        }
    }
}