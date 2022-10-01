using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Experimental.XR.Interaction;
using UnityEngine.XR.Management;

namespace RiggedXR
{
    public class RiggedXRSetup : EditorWindow
    {

        public bool hasOpenXR;
        public bool hasOpenXRInitialized;
        public bool hasXRToolkit;

        private bool busy;
        
        [MenuItem("RiggedXR/Setup")]
        static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(RiggedXRSetup), true, "RiggedXR Setup");
        }
        
        private void OnGUI()
        {
            GUILayout.Label("RiggedXR Setup Window", EditorStyles.boldLabel);

            hasOpenXR = IsPackageInstalled("com.unity.xr.openxr");
            hasOpenXRInitialized = IsOpenXRInitialized();
            hasXRToolkit = IsPackageInstalled("com.unity.xr.interaction.toolkit");
            
            if (hasOpenXR)
            {
                EditorGUILayout.Toggle("OpenXR Plugin Installed", true);
            }
            else
            {
                EditorGUILayout.Toggle("OpenXR Plugin Installed", false);
            }

            if (hasOpenXRInitialized)
            {
                EditorGUILayout.Toggle("OpenXR Loader Initialized", true);
            }
            else
            {
                EditorGUILayout.Toggle("OpenXR Loader Initialized", false);
            }
            
            if (hasXRToolkit)
            {
                EditorGUILayout.Toggle("XR Interaction Toolkit Installed", true);
            }
            else
            {
                EditorGUILayout.Toggle("XR Interaction Toolkit Installed", false);
            }
            
            if (DetectIssues())
            {
                if (!busy)
                {
                    if (GUILayout.Button("Fix Issues"))
                    {
                        FixIssues();
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("Currently Busy! Please Wait!", MessageType.Warning, true);
                }
                
                
                EditorGUILayout.HelpBox("Issues Detected!", MessageType.Error, true);
            }
            else
            {
                if (GUILayout.Button("Setup RiggedXR Scene"))
                {
                    SetupScene();
                }
            }
        }

        private void SetupScene()
        {
            
            
            RiggedXRSettings settings = (RiggedXRSettings)Resources.Load("RiggedXRSettings");
            if (settings)
            {
                if (!FindObjectOfType<RiggedXR>())
                {
                    PrefabUtility.InstantiatePrefab(settings.RiggedXRPrefab);
                }
                else
                {
                    Debug.LogWarning("Already have a RiggedXR Object in the scene, adding multiple could cause issues unless you've modified it yourself!");
                }

                if (!FindObjectOfType<RiggedXRPlayer>())
                {
                    PrefabUtility.InstantiatePrefab(settings.RiggedPlayerPrefab);
                }
                else
                {
                    Debug.LogWarning("Already have a RiggedXR Player Object in the scene, adding multiple could cause issues unless you've modified it yourself!");
                }
            }
            else
            {
                Debug.LogError("Missing Rigged XR Settings in Resources Folder!");
            }
        }

        private bool IsOpenXRInitialized()
        {
            foreach (XRLoader loader in XRGeneralSettings.Instance.Manager.activeLoaders)
            {
                if (loader.GetType().Name == "OpenXRLoader")
                {
                    return true;
                }
            }

            return false;
        }

        private static AddRequest Request;
        private void FixIssues()
        {
            busy = true;
            
            if (!hasOpenXR) // Install OpenXR Plugin if We don't have it!
            {
                Request = Client.Add("com.unity.xr.openxr");
                EditorApplication.update += Progress;
                
                return;
            }

            if (!hasOpenXRInitialized)
            {
                Debug.LogError("Please Initialize OpenXR under (Edit -> Project Settings -> XR Plugin Management)");
                busy = false;
                
                return;
            }

            if (!hasXRToolkit)
            {
                Request = Client.Add("com.unity.xr.interaction.toolkit");
                EditorApplication.update += Progress;
                
                return;
            }
        }

        private void Progress()
        {
            if (Request.IsCompleted)
            {
                if(Request.Status == StatusCode.Success)
                    Debug.Log("Installed: " + Request.Result.packageId);
                else if(Request.Status >= StatusCode.Failure)
                    Debug.Log(Request.Error.message);

                busy = false;
                EditorApplication.update -= Progress;
            }
        }

        public bool DetectIssues()
        {
            if (!hasOpenXR)
            {
                return true;
            }

            if (!hasOpenXRInitialized)
            {
                return true;
            }

            if (!hasXRToolkit)
            {
                return true;
            }

            return false;
        }
        
        public static bool IsPackageInstalled(string packageId)
        {
            if ( !File.Exists("Packages/manifest.json") )
                return false;
 
            string jsonText = File.ReadAllText("Packages/manifest.json");
            return jsonText.Contains( packageId );
        }
    }
}
