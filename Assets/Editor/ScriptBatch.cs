using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class ScriptBatch : IPostprocessBuildWithReport
{
    public static string VersionNumber => $"v{Application.version}";

    public static string AppName => $"{Application.productName}";

    public int callbackOrder { get { return 0; } }

    public static string[] GetScenes()
    {
        return new string[]
        {
            "Assets/Scenes/MainMenu.unity",
            "Assets/Scenes/LobbyScene.unity",
            "Assets/Scenes/BigFans.unity"
        };
    }

    public static void BuildSetup(BuildTargetGroup buildTargetGroup = BuildTargetGroup.Standalone,
        ScriptingImplementation scriptingImplementation = ScriptingImplementation.Mono2x)
    {
        UnityEngine.Debug.Log($"Setting Scripting Backend to {scriptingImplementation.ToString()}");
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Standalone, scriptingImplementation);
    }

    public static void BuildCleanup()
    {
        UnityEngine.Debug.Log("Setting Scripting Backend back to Mono");
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Standalone, ScriptingImplementation.Mono2x);
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
        BuildSetup(scriptingImplementation: ScriptingImplementation.Mono2x);
        // Get filename.
        string path = $"Builds/MacOS-{VersionNumber}";
        string[] levels = GetScenes();

        string appFolder = path + $"/{AppName}.app";

        // Build player.
        BuildPipeline.BuildPlayer(levels, appFolder, BuildTarget.StandaloneOSX, BuildOptions.Development);

        BuildCleanup();
    }

    [MenuItem("Build/Linux Build")]
    public static void LinuxBuild()
    {
        BuildSetup(scriptingImplementation: ScriptingImplementation.Mono2x);

        // Get filename.
        string path = $"Builds/Linux-{VersionNumber}";
        string[] levels = GetScenes();

        // Build player.
        BuildPipeline.BuildPlayer(levels, path + $"/{AppName}.x86_64", BuildTarget.StandaloneLinux64, BuildOptions.Development);

        BuildCleanup();
    }

    [MenuItem("Build/Windows64 Build")]
    public static void WindowsBuild()
    {
        BuildSetup(scriptingImplementation: ScriptingImplementation.Mono2x);

        // BuildUtilities.RegisterShouldIncludeInBuildCallback(new UnityEditor.PackageManager.IShouldIncludeInBuildCallback("Code Coverage"));
        BuildPlayerOptions options = new BuildPlayerOptions
        {
            scenes = GetScenes(),
            locationPathName = $"Builds/Win64-{VersionNumber}/{AppName}.exe",
            targetGroup = BuildTargetGroup.Standalone,
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.Development
        };

        // Build player.
        BuildPipeline.BuildPlayer(options);

        BuildCleanup();
    }

    [MenuItem("Build/Test Build")]
    public static void TestBuild()
    {
        BuildSetup(scriptingImplementation: ScriptingImplementation.Mono2x);

        // BulidUtilities.RegisterShouldIncludeInBuildCallback(PackageManager.IShouldIncludeBUildCallback("Code Coverage"));
        BuildPlayerOptions options = new BuildPlayerOptions
        {
            scenes = GetScenes(),
            locationPathName = $"Builds/Test-Win64-{VersionNumber}/{AppName}.exe",
            targetGroup = BuildTargetGroup.Standalone,
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.Development
        };

        // Build player.
        BuildPipeline.BuildPlayer(options);

        BuildCleanup();
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
