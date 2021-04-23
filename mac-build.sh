/Applications/Unity/Hub/Editor/2020.3.2f1/Unity.app/Contents/MacOS/Unity -quit -batchmode -executeMethod ScriptBatch.MacOSBuild
codesign -s - -f --deep Builds/MacOS/PropHunt.app
codesign -s - -f Builds/MacOS/PropHunt.app

