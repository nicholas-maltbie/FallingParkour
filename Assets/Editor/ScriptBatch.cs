using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class ScriptBatch : IPostprocessBuildWithReport
{
    public int callbackOrder { get { return 0; } }

    public static string[] GetScenes()
    {
        return new string[] { "Assets/Scenes/BasicHouse.unity" };
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
    }

    [MenuItem("Build/MacOS Build")]
    public static void MacOSBuild()
    {
        // Get filename.
        string path = "Builds/MacOS";
        string[] levels = GetScenes();

        string appFolder = path + "/PropHunt.app";

        // Build player.
        BuildPipeline.BuildPlayer(levels, appFolder, BuildTarget.StandaloneOSX, BuildOptions.Development);
    }

    [MenuItem("Build/Linux Build")]
    public static void LinuxBuild()
    {
        // Get filename.
        string path = "Builds/Linux";
        string[] levels = GetScenes();

        // Build player.
        BuildPipeline.BuildPlayer(levels, path + "/PropHunt.x86_64", BuildTarget.StandaloneLinux64, BuildOptions.Development);
    }

    [MenuItem("Build/Windows64 Build")]
    public static void WindowsBuild()
    {
        // Get filename.
        string path = "Builds/Win64";
        string[] levels = GetScenes();

        // Build player.
        BuildPipeline.BuildPlayer(levels, path + "/PropHunt.exe", BuildTarget.StandaloneWindows64, BuildOptions.Development);
    }

    // [MenuItem("Build/Windows Build")]
    // public static void WindowsBuild ()
    // {
    //     // Get filename.
    //     string path = "Builds/Win";
    //     string[] levels = GetScenes();

    //     // Build player.
    //     BuildPipeline.BuildPlayer(levels, path + "/PropHunt.exe", BuildTarget.StandaloneWindows, BuildOptions.None);
    // }
}
