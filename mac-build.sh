/Applications/Unity/Hub/Editor/2020.3.5f1/Unity.app/Contents/MacOS/Unity -quit -batchmode -executeMethod ScriptBatch.MacOSBuild
codesign -s - -f --deep Builds/MacOS/PropHunt.app
