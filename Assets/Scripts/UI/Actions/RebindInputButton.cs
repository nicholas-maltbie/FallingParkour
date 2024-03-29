using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace PropHunt.UI.Actions
{
    /// <summary>
    /// Rebind an individual button input action
    /// </summary>
    public class RebindInputButton : MonoBehaviour
    {
        /// <summary>
        /// Prefix for input mapping for saving to player preferences
        /// </summary>
        public const string inputMappingPlayerPrefPrefix = "Input Mapping";

        /// <summary>
        /// Input action being modified
        /// </summary>
        public InputActionReference inputAction = null;
        /// <summary>
        /// Binding display name for showing the control button description
        /// </summary>
        public Text bindingDisplayNameText = null;
        /// <summary>
        /// Button to start rebinding for the given input action
        /// </summary>
        public Button startRebinding = null;
        /// <summary>
        /// Text to display when waiting for the player to press a new input action
        /// </summary>
        public GameObject waitingForInputObject = null;
        /// <summary>
        /// Menu controller related to this selected object
        /// </summary>
        public MenuController menuController;

        /// <summary>
        /// Rebinding operation action waiting for player command to change button bindings
        /// </summary>
        public InputActionRebindingExtensions.RebindingOperation rebindingOperation { get; private set; }

        /// <summary>
        /// Get a readable display name for a binding index
        /// </summary>
        /// <returns>Human readable information of button for this binding index</returns>
        private string GetKeyReadableName() =>
            InputControlPath.ToHumanReadableString(
                inputAction.action.bindings[0].effectivePath,
                InputControlPath.HumanReadableStringOptions.OmitDevice);

        /// <summary>
        /// Get the input mapping player preference key from a given index
        /// </summary>
        public string InputMappingKey => $"{inputMappingPlayerPrefPrefix} {inputAction.action.name}";

        public void Awake()
        {
            // Load the default mapping saved to the file
            string inputMapping = PlayerPrefs.GetString(InputMappingKey, string.Empty);
            if (!string.IsNullOrEmpty(inputMapping))
            {
                inputAction.action.ApplyBindingOverride(inputMapping);
            }
        }

        public void Start()
        {
            startRebinding.onClick.AddListener(() => StartRebinding());
            bindingDisplayNameText.text = GetKeyReadableName();
        }

        /// <summary>
        /// Start the rebinding process for a given component of this composite axis.
        /// </summary>
        public void StartRebinding()
        {
            startRebinding.gameObject.SetActive(false);
            waitingForInputObject.SetActive(true);
            menuController.allowInputChanges = false;

            inputAction.action.Disable();
            inputAction.action.actionMap.Disable();
            rebindingOperation = inputAction.action.PerformInteractiveRebinding(0)
                .WithControlsExcluding("<Pointer>/position") // Don't bind to mouse position
                .WithControlsExcluding("<Pointer>/delta")    // To avoid accidental input from mouse motion
                .WithCancelingThrough("<Keyboard>/escape")
                .OnMatchWaitForAnother(0.1f)
                .WithTimeout(5.0f)
                .OnComplete(operation => RebindComplete())
                .Start();
        }

        /// <summary>
        /// Finish the rebinding process for a given component of this composite axis.
        /// </summary>
        public void RebindComplete()
        {
            string overridePath = inputAction.action.bindings[0].overridePath;
            foreach (PlayerInput input in GameObject.FindObjectsOfType<PlayerInput>())
            {
                InputAction action = input.actions.FindAction(inputAction.name);
                if (action != null) action.ApplyBindingOverride(0, overridePath);
            }

            bindingDisplayNameText.text = GetKeyReadableName();
            rebindingOperation.Dispose();

            PlayerPrefs.SetString(InputMappingKey, inputAction.action.bindings[0].overridePath);

            startRebinding.gameObject.SetActive(true);
            waitingForInputObject.SetActive(false);
            menuController.allowInputChanges = true;
            inputAction.action.Enable();
            inputAction.action.actionMap.Enable();
        }
    }
}