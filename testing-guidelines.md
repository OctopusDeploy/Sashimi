# Sashimi ActionHandler Testing Guide

## Background
One of the benefits of Project-Modularity/Sashimi is that we can build and test each modules’ steps/actions separately. 
The challenge lies in having enough tests to ensure that the system works end-to-end when integrated. 
In short, we want to have the [highest confidence with the least cost](https://octopusdeploy.slack.com/archives/CG46T4HGB/p1595893502235800?thread_ts=1595892061.233200&cid=CG46T4HGB). 

Jump straight to the end for the guidelines.

## Options
There are 3 places where tests can be written: *Calamari*, *Sashimi* and *Server E2E*. The cost/benefits of each are listed below:

#### Calamari 
This runs Calamari in-process directly from the Calamari test project. The test project will run on all frameworks that the Calamari project supports.

##### Summary
- Fast to write and run
- Easy to debug (in process)
- Lowest confidence that this works in real life

#### Sashimi
This runs the action handler (normally run the Server process) from the test project, which then invokes Calamari (either in or out of process). The test project is always NetCoreApp, matching the Octopus Server framework.

What Sashimi tests in addition to pure Calamari is the interaction between the action handler and Calamari command. 
The package and tool acquisition is still done by the test framework, so there’s still a chance that a passing test here still results in a failure on a Server E2E test.

##### Summary
- Fast to write and run
- Easy to debug if Calamari runs NetCore, otherwise runs out of process
- Medium confidence that this works in real life

#### Server End-to-end (E2E)
This creates a real Octopus Server project and deploys it. This gives us high confidence that everything is hooked up correctly (with the exclusion of the UI setting the right properties).

The drawback is that the E2E tests takes at least a minute to spool up the test instance, making it painful to write locally. Also they trigger when someone makes a totally unrelated change to Server.

##### Summary
- Slow to write and run
- Triggers too often for unrelated changes
- Hard to debug (can only attach to Server, needs custom variable to use custom build of Calamari)
- High confidence that this works in real life

## Guidelines
With those trade-off's and consideration in mind, we should do the following for each Calamari flavour/project:

- Write 1 E2E Test that uses package/tools to verify that this Module functions end-to-end (happy path)
- Write remaining tests in Sashimi
  - If the corresponding Calamari project doesn’t run under NetCore and there’s a strong need to debug Calamari, then write the tests in Calamari

