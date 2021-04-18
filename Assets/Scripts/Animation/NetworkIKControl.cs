using System.Collections.Generic;
using Mirror;
using PropHunt.Utils;
using UnityEngine;

namespace PropHunt.Animation
{
    /// <summary>
    /// Manage IK For humanoid avatar over network
    /// </summary>
    public class NetworkIKControl : NetworkBehaviour
    {
        public static readonly AvatarIKGoal[] avatarIKGoals = {AvatarIKGoal.LeftFoot, AvatarIKGoal.RightFoot,
            AvatarIKGoal.LeftHand, AvatarIKGoal.RightHand};

        public static readonly AvatarIKHint[] avatarIKHints = {AvatarIKHint.LeftElbow, AvatarIKHint.LeftKnee,
            AvatarIKHint.RightElbow, AvatarIKHint.RightKnee};

        /// <summary>
        /// Target for player looking
        /// </summary>
        public Transform ikLookTarget { get; private set; }

        /// <summary>
        /// Targets for various avatar IK Goals
        /// </summary>
        protected Dictionary<AvatarIKGoal, Transform> ikGoalTargets = new Dictionary<AvatarIKGoal, Transform>();

        /// <summary>
        /// Targets for various avatar IK hints
        /// </summary>
        protected Dictionary<AvatarIKHint, Transform> ikHintTargets = new Dictionary<AvatarIKHint, Transform>();

        /// <summary>
        /// Are the Inverse Kinematics controls enabled for this character
        /// </summary>
        public bool ikActive = true;

        /// <summary>
        /// Current look state of the player
        /// </summary>
        [SyncVar(hook = nameof(OnLookStateChange))]
        public bool lookState;

        /// <summary>
        /// Current look weight for the player
        /// </summary>
        [SyncVar(hook = nameof(OnLookWeightChange))]
        public float lookWeight;

        /// <summary>
        /// What is the current avatar IK goal for each state
        /// </summary>
        public SyncDictionary<AvatarIKGoal, bool> avatarIKGoalStates = new SyncDictionary<AvatarIKGoal, bool>();

        /// <summary>
        /// Current weights for IK goal transforms
        /// </summary>
        public SyncDictionary<AvatarIKGoal, float> avatarIKGoalWeights = new SyncDictionary<AvatarIKGoal, float>();

        /// <summary>
        /// What is the current avatar IK hint for each state
        /// </summary>
        public SyncDictionary<AvatarIKHint, bool> avatarIKHintStates = new SyncDictionary<AvatarIKHint, bool>();

        /// <summary>
        /// Current weights for IK hint positions
        /// </summary>
        public SyncDictionary<AvatarIKHint, float> avatarIKHintWeights = new SyncDictionary<AvatarIKHint, float>();

        /// <summary>
        /// Network service for managing player interactions
        /// </summary>
        public INetworkService networkService;

        /// <summary>
        /// Controller for managing current IK targets for animator (abstract the commands)
        /// </summary>
        public IKControl controller;

        public void OnLookStateChange(bool _, bool newState) => this.controller.lookObj = newState ? ikLookTarget : null;
        public void OnLookWeightChange(float _, float newWeight) => this.controller.lookWeight = newWeight;
        public void UpdateIKGoalWeight(AvatarIKGoal goal, float newWeight) => this.controller.SetIKGoalWeight(goal, newWeight);
        public void UpdateIKHintWeight(AvatarIKHint hint, float newWeight) => this.controller.SetIKHintWeight(hint, newWeight);
        public void UpdateIKHintState(AvatarIKHint hint, bool newState) =>
            this.controller.SetIKHintTransform(hint, newState ? ikHintTargets[hint] : null);
        public void UpdateIKGoalState(AvatarIKGoal goal, bool newState) =>
            this.controller.SetIKGoalTransform(goal, newState ? ikGoalTargets[goal] : null);

        public void OnIKGoalStateChange(SyncDictionary<AvatarIKGoal, bool>.Operation operation, AvatarIKGoal goal, bool state)
        {
            if (operation == SyncDictionary<AvatarIKGoal, bool>.Operation.OP_ADD ||
                operation == SyncDictionary<AvatarIKGoal, bool>.Operation.OP_SET)
            {
                UpdateIKGoalState(goal, state);
            }
            else
            {
                UpdateIKGoalState(goal, false);
            }
        }

        public void OnIKHintWeightChange(SyncDictionary<AvatarIKHint, float>.Operation operation, AvatarIKHint hint, float weight)
        {
            if (operation == SyncDictionary<AvatarIKHint, float>.Operation.OP_ADD ||
                operation == SyncDictionary<AvatarIKHint, float>.Operation.OP_SET)
            {
                UpdateIKHintWeight(hint, weight);
            }
            else
            {
                UpdateIKHintWeight(hint, 0.0f);
            }
        }

        public void OnIKGoalWeightChange(SyncDictionary<AvatarIKGoal, float>.Operation operation, AvatarIKGoal goal, float weight)
        {
            if (operation == SyncDictionary<AvatarIKGoal, float>.Operation.OP_ADD ||
                operation == SyncDictionary<AvatarIKGoal, float>.Operation.OP_SET)
            {
                UpdateIKGoalWeight(goal, weight);
            }
            else
            {
                UpdateIKGoalWeight(goal, 0.0f);
            }
        }

        public void SetIKGoalWeight(AvatarIKGoal goal, float newWeight)
        {
            if (!networkService.isServer)
            {
                CmdSetIKGoalWeight(goal, newWeight);
            }
            else
            {
                this.avatarIKGoalWeights[goal] = newWeight;
            }

            if (networkService.isLocalPlayer || networkService.isServer)
            {
                UpdateIKGoalWeight(goal, newWeight);
            }
        }

        public void SetIKGoalState(AvatarIKGoal goal, bool newState)
        {
            if (!networkService.isServer)
            {
                CmdSetIKGoalState(goal, newState);
            }
            else
            {
                this.avatarIKGoalStates[goal] = newState;
            }

            if (networkService.isLocalPlayer || networkService.isServer)
            {
                UpdateIKGoalState(goal, newState);
            }
        }

        public void SetIKHintWeight(AvatarIKHint hint, float newWeight)
        {
            if (!networkService.isServer)
            {
                CmdSetIKHintWeight(hint, newWeight);
            }
            else
            {
                this.avatarIKHintWeights[hint] = newWeight;
            }

            if (networkService.isLocalPlayer || networkService.isServer)
            {
                UpdateIKHintWeight(hint, newWeight);
            }
        }

        public void SetIKHintState(AvatarIKHint hint, bool newState)
        {
            if (!networkService.isServer)
            {
                CmdSetIKHintState(hint, newState);
            }
            else
            {
                this.avatarIKHintStates[hint] = newState;
            }

            if (networkService.isLocalPlayer || networkService.isServer)
            {
                UpdateIKHintState(hint, newState);
            }
        }

        public void Awake()
        {
            networkService = new NetworkService(this);

            // Setup look target
            this.ikLookTarget = new GameObject().transform;
            this.ikLookTarget.position = transform.position;
            this.ikLookTarget.rotation = transform.rotation;
            this.ikLookTarget.parent = transform;
            this.ikLookTarget.name = "Look Target";
            NetworkTransformChild childTransform = gameObject.AddComponent<NetworkTransformChild>();
            childTransform.target = this.ikLookTarget;
            childTransform.clientAuthority = true;

            // Setup and synchronize tarnsform goals
            foreach (AvatarIKGoal goal in avatarIKGoals)
            {
                Transform ikGoalTransform = new GameObject().transform;
                ikGoalTransform.position = transform.position;
                ikGoalTransform.rotation = transform.rotation;
                ikGoalTransform.parent = transform;
                ikGoalTransform.name = goal.ToString();
                NetworkTransformChild ikGoalChildTransform = gameObject.AddComponent<NetworkTransformChild>();
                ikGoalChildTransform.target = ikGoalTransform;
                ikGoalChildTransform.clientAuthority = true;
                this.ikGoalTargets.Add(goal, ikGoalTransform);
            }

            // Setup and synchronize transform hints
            foreach (AvatarIKHint hint in avatarIKHints)
            {
                Transform ikHintTransform = new GameObject().transform;
                ikHintTransform.position = transform.position;
                ikHintTransform.rotation = transform.rotation;
                ikHintTransform.parent = transform;
                ikHintTransform.name = hint.ToString();
                NetworkTransformChild ikGoalChildTransform = gameObject.AddComponent<NetworkTransformChild>();
                ikGoalChildTransform.target = ikHintTransform;
                ikGoalChildTransform.clientAuthority = true;
                this.ikHintTargets.Add(hint, ikHintTransform);
            }
        }

        private void SetLookWeightInternal(float newLookWeight)
        {
            if (networkService.isLocalPlayer || networkService.isServer)
            {
                float previousWeight = this.lookWeight;
                this.lookWeight = newLookWeight;
                OnLookWeightChange(previousWeight, newLookWeight);
            }
        }

        public void SetLookWeight(float newLookWeight)
        {
            if (!networkService.isServer)
            {
                CmdSetLookWeight(newLookWeight);
            }

            if (networkService.isLocalPlayer || networkService.isServer)
            {
                SetLookWeightInternal(newLookWeight);
            }
        }

        public override void OnStartServer()
        {
            foreach (AvatarIKGoal goal in avatarIKGoals)
            {
                this.avatarIKGoalStates[goal] = false;
                this.avatarIKGoalWeights[goal] = 0.0f;
            }
            foreach (AvatarIKHint hint in avatarIKHints)
            {
                this.avatarIKHintStates[hint] = false;
                this.avatarIKHintWeights[hint] = 0.0f;
            }
        }

        public override void OnStartClient()
        {
            this.avatarIKGoalStates.Callback += OnIKGoalStateChange;
            this.avatarIKGoalWeights.Callback += OnIKGoalWeightChange;
            this.avatarIKHintWeights.Callback += OnIKHintWeightChange;
        }

        private void SetLookStateInternal(bool newLookState)
        {
            if (networkService.isLocalPlayer || networkService.isServer)
            {
                bool previousState = this.lookState;
                this.lookState = newLookState;
                OnLookStateChange(previousState, newLookState);
            }
        }

        public void SetLookState(bool newLookState)
        {
            if (!networkService.isServer)
            {
                CmdSetLookState(newLookState);
            }

            if (networkService.isLocalPlayer || networkService.isServer)
            {
                SetLookStateInternal(newLookState);
            }
        }

        [Command]
        public void CmdSetIKHintState(AvatarIKHint goal, bool newState)
        {
            UpdateIKHintState(goal, newState);
        }

        [Command]
        public void CmdSetIKGoalState(AvatarIKGoal goal, bool newState)
        {
            UpdateIKGoalState(goal, newState);
        }

        [Command]
        public void CmdSetIKHintWeight(AvatarIKHint hint, float newWeight)
        {
            UpdateIKHintWeight(hint, newWeight);
        }

        [Command]
        public void CmdSetIKGoalWeight(AvatarIKGoal goal, float newWeight)
        {
            UpdateIKGoalWeight(goal, newWeight);
        }

        [Command]
        public void CmdSetLookWeight(float newLookWeight)
        {
            SetLookWeightInternal(newLookWeight);
        }

        [Command]
        public void CmdSetLookState(bool newLookState)
        {
            SetLookStateInternal(newLookState);
        }
    }
}