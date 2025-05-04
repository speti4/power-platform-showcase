# Power Platform CLI
## Prerequisites
* [Node.js LTS](https://nodejs.org/)
* [Power Platform Tools extension](https://marketplace.visualstudio.com/items?itemName=microsoft-IsvExpTools.powerplatform-vscode)
* [C# Dev Kit extension](https://marketplace.visualstudio.com/items/?itemName=ms-dotnettools.csdevkit)

## Terminal commands
[CLI reference](https://learn.microsoft.com/en-us/power-platform/developer/cli/reference/)

**install/update power platform cli**
```console
pac install latest
```

**establish a connection from the CLI**
```console
pac auth create --name Lab --url https://spetidev.crm4.dynamics.com/
```

**lsit connections**
```console
pac auth list
```

**select a connection**
```console
pac auth select --index 1
```

**environment check**
```console
pac org who
```

**list environments**
```console
pac env list
```

**select environment**
```console
pac env select -env 2d207bddf0a34bcda23e06b0d145a3
```

**list solutions in environment**
```console
pac solution list
```
**clone solution to the current directory**
```console
pac solution clone --name sampleSolutionName
```
**unpack solution zip**
```console
pac solution unpack --zipfile C:\SampleSolution.zip --folder .\SampleSolutionUnpacked\.
```
**pack/build solution zip**
```console
pac solution pack --zipfile C:\SampleSolution.zip --folder .\SampleSolutionUnpacked\.
```
**unpack the .msapp file of a canvas app**
```console
pac canvas unpack --msapp "SampleApp1.msapp" --sources src
```
**pack/build canvas app files to .msapp**
```console
pac canvas pack --msapp "SampleApp1.msapp" --sources src
```


### [Create a Power Apps Component Framework (PCF) component](https://learn.microsoft.com/en-gb/training/modules/developer-tools-extend/exercise)
**initialize the component project**
```console
pac pcf init --namespace lab --name FirstControl --template field
```

**pull npm repo dependencies**
```console
npm install
```

**test with modified code**
```console
npm start
```

**push control to the environment - ⚠️ [works with PP Tools 2.0.13 or older](https://dianabirkelbach.wordpress.com/2024/01/16/how-to-fix-pac-pcf-push-issues-with-pac-cli-newer-than-1-28-3-publisher-error-or-solution-deleted/)**


```console
pac pcf push --publisher-prefix lab
```

**Launch developer tools**

Configuration Migration Tool
```console
pac tool cmt
```
Plugin registration tool
```console
pac tool prt
```
Package Deployer
```console
pac tool pd
```

**build the code component**
```console
npm run build
```

### [Create a solution file with Power Platform CLI](https://learn.microsoft.com/en-gb/training/modules/build-power-apps-component/package-code-component)
**create solution directory in the project folder**
```console
md Solution
cd Solution
```

**Initialize your Dataverse solution project**
```console
pac solution init --publisher-name mslearn --publisher-prefix msl
```

**Inform your solution project its components will be added during the build**
```console
pac solution add-reference --path ..
```

**To generate your solution's zip file, use Microsoft Build Engine, or msbuild for short. You'll only need to use the /restore flag the first time the solution project is built. In every subsequent build, you'll need to run msbuild only.**
```console
"C:\Program Files (x86)\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin\MSBuild.exe\" /t:build /restore

msbuild /t:build /restore
```


### [Create custom API](https://learn.microsoft.com/en-gb/training/modules/introduction-power-platform-extend/exercise)

**initialize a new Dataverse plugin class library - genreates snk strong name key file**
```console
pac plugin init
```
**VSCode does not rely on project or solution files in the same way as Visual Studio. To enable project-like features (like build, add nuget package, .NET commands, etc.) in the Solution Explorer tab, you have to create a solution, in the project folder:**
```console
code .
```

**Install nuget package**

[nuget.org](https://www.nuget.org/packages/system.text.regularexpressions/)
```console
dotnet add package System.Text.RegularExpressions --version 4.3.1
```

**Build / clean / rebuild project**
```console
dotnet build
dotnet clean
dotnet rebuild
```

### [Create Azure Function](https://learn.microsoft.com/en-gb/training/modules/develop-azure-functions/5-create-function-visual-studio-code)
**Prerequisites**
* The [Azure Functions Core Tools](https://github.com/Azure/azure-functions-core-tools#installing)  version 4.x.
* [.NET 8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) is the target framework.
* The [C# extension](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp) for Visual Studio Code.
* The [Azure Functions extension](https://marketplace.visualstudio.com/items?itemName=ms-azuretools.vscode-azurefunctions) for Visual Studio Code.

### [Custom connector management - paconn](https://learn.microsoft.com/en-gb/training/modules/policy-templates-custom-connectors/6-exercise)

**Prerequisites**

* Python runtime

**Install paconn**
```console
pip install paconn
```
**login - To sign in, use a web browser to open the page https://microsoft.com/devicelogin and enter the code XXXXXXXXX to authenticate.**
```console
paconn login
```
**Download connector or connector policy**
```console
paconn download
```

**Update connector or connector policy**
```console
paconn update --api-def apiDefinition.swagger.json --api-prop apiProperties.json --icon icon.png
```

### Power Platform Admin powershell

**Install module**
```console
Install-Module -Name Microsoft.PowerApps.Administration.PowerShell
```

**Get environment regions**
```console
$env = Get-AdminPowerAppEnvironment

$env | Format-Table -Property DisplayName, @{n="Region";e={$_.Internal.Properties.azureRegionHint}}
```