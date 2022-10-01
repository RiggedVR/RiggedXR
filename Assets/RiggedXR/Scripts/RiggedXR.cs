using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.ReorderableList;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

namespace RiggedXR
{
    public class RiggedXR : MonoBehaviour
    {
        public static RiggedXR Instance { get; private set; }

        public InputActionAsset riggedXRInputAsset;
        
        [Header("Input Action References")]
        
        public InputActionReference leftTriggerReference = null;
        public InputActionReference leftGripReference = null;
        [Space]
        public InputActionReference rightTriggerReference = null;
        public InputActionReference rightGripReference = null;

        public float leftTriggerValue;
        public float rightTriggerValue;
        public float leftGripValue;
        public float rightGripValue;
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }

            Instance = this;
        }

        private void Start()
        {
            
        }

        private void Update()
        {
            leftTriggerValue = leftTriggerReference.action.ReadValue<float>();
            rightTriggerValue = rightTriggerReference.action.ReadValue<float>();

            leftGripValue = leftGripReference.action.ReadValue<float>();
            rightGripValue = rightGripReference.action.ReadValue<float>();
        }
    }
}
