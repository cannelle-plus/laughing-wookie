// --------------------------------------------------------------------------------------
// FAKE build script 
// --------------------------------------------------------------------------------------

#r @"packages/FAKE/tools/FakeLib.dll"
open Fake 
// --------------------------------------------------------------------------------------

let product = "laughing-wookie"

//properties

let mode = getBuildParamOrDefault "mode" "Debug"

let appToDeploy = "src/server/bin/Release"
let deployDir = "./deploy/"



//Targets
Target "Clean" (fun _ -> 
    CleanDir deployDir
)

Target "Build" (fun _ ->
    MSBuildRelease "" "Build" ["laughing-wookie.sln"]
    |> Log "Build-Output:"
)

Target "Deploy" (fun _ ->
    let from = directoryInfo(appToDeploy)
    let towards =directoryInfo(deployDir)
    copyRecursive  from towards  false
    |> Log "Deploy-Output:"
)


// dependencies
"Clean"
    ==> "Build"
    ==> "Deploy"


//start build
RunTargetOrDefault "Deploy"    
