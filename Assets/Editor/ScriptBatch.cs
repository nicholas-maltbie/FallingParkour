using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.PackageManager;
using UnityEngine;

public class ScriptBatch : IPostprocessBuildWithReport
{
    public int callbackOrder { get { return 0; } }

    public static string[] GetScenes()
    {
        return new string[]
        {
            "Assets/Scenes/MainMenu.unity",
            "Assets/Scenes/LobbyScene.unity",
            "Assets/Scenes/BasicHouse.unity"
        };
    }

    [MenuItem("Build/Build All")]
    public static void BuildAll()
    {
        MacOSBuild();
        LinuxBuild();
        WindowsBuild();
    }

    public void OnPostprocessBuild(BuildReport report)
    {
#if UNITY_STANDALONE_OSX
        UnityEngine.Debug.Log("Signing files for MacOS Build");
        UnityEditor.OSXStandalone.MacOSCodeSigning.CodeSignAppBundle(report.summary.outputPath + "/Contents/PlugIns/phonon.bundle");
        UnityEditor.OSXStandalone.MacOSCodeSigning.CodeSignAppBundle(report.summary.outputPath + "/Contents/PlugIns/audioplugin_phonon.bundle");
        UnityEditor.OSXStandalone.MacOSCodeSigning.CodeSignAppBundle(report.summary.outputPath + "/Contents/PlugIns/steam_api.bundle");
        UnityEditor.OSXStandalone.MacOSCodeSigning.CodeSignAppBundle(report.summary.outputPath); 
#endif
        UnityEngine.Debug.Log("MyCustomBuildProcessor.OnPostprocessBuild for target " + report.summary.platform + " at path " + report.summary.outputPath);

        UnityEngine.Debug.Log("Setting Scripting Backend back to IL2CPP");
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Standalone, ScriptingImplementation.IL2CPP);
    }

    [MenuItem("Build/MacOS Build")]
    public static void MacOSBuild()
    {
        UnityEngine.Debug.Log("Setting Scripting Backend to Mono");
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Standalone, ScriptingImplementation.Mono2x);
        // Get filename.
        string path = "Builds/MacOS";
        string[] levels = GetScenes();

        string appFolder = path + "/FallingParkour.app";

        // Build player.
        BuildPipeline.BuildPlayer(levels, appFolder, BuildTarget.StandaloneOSX, BuildOptions.Development);
    }

    [MenuItem("Build/Linux Build")]
    public static void LinuxBuild()
    {
        UnityEngine.Debug.Log("Setting Scripting Backend to Mono");
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Standalone, ScriptingImplementation.Mono2x);
        // Get filename.
        string path = "Builds/Linux";
        string[] levels = GetScenes();

        // Build player.
        BuildPipeline.BuildPlayer(levels, path + "/FallingParkour.x86_64", BuildTarget.StandaloneLinux64, BuildOptions.Development);
    }

    [MenuItem("Build/Windows64 Build")]
    public static void WindowsBuild()
    {
        UnityEngine.Debug.Log("Setting Scripting Backend to IL2CPP");
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Standalone, ScriptingImplementation.IL2CPP);
        // BuildUtilities.RegisterShouldIncludeInBuildCallback(new UnityEditor.PackageManager.IShouldIncludeInBuildCallback("Code Coverage"));
        BuildPlayerOptions options = new BuildPlayerOptions
        {
            scenes = GetScenes(),
            locationPathName = "Builds/Win64/FallingParkour.exe",
            targetGroup = BuildTargetGroup.Standalone,
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.Development
        };

        // Build player.
        BuildPipeline.BuildPlayer(options);
    }

    [MenuItem("Build/Test Build")]
    public static void TestBuild()
    {
        UnityEngine.Debug.Log("Setting Scripting Backend to Mono");
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Standalone, ScriptingImplementation.Mono2x);

        // BulidUtilities.RegisterShouldIncludeInBuildCallback(PackageManager.IShouldIncludeBUildCallback("Code Coverage"));
        BuildPlayerOptions options = new BuildPlayerOptions
        {
            scenes = GetScenes(),
            locationPathName = "Builds/Test-Win64/FallingParkour.exe",
            targetGroup = BuildTargetGroup.Standalone,
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.Development
        };

        // Build player.
        BuildPipeline.BuildPlayer(options);
    }

    // [MenuItem("Build/Windows Build")]
    // public static void WindowsBuild ()
    // {
    //     // Get filename.
    //     string path = "Builds/Win";
    //     string[] levels = GetScenes();

    //     // Build player.
    //     BuildPipeline.BuildPlayer(levels, path + "/FallingParkour.exe", BuildTarget.StandaloneWindows, BuildOptions.None);
    // }
}
