![alt text](https://i.imgur.com/QysizI7.png "BuildNotifications - For the neccessary summary")
![GitHub Workflow Status (branch)](https://img.shields.io/github/workflow/status/grollmus/BuildNotifications/CI%20Build/master)
[![Codacy Badge](https://app.codacy.com/project/badge/Grade/0c8a9c1f0e9f4ed1ab12e9c7204682ba)](https://www.codacy.com/gh/grollmus/BuildNotifications/dashboard?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=grollmus/BuildNotifications&amp;utm_campaign=Badge_Grade)
[![Codacy Badge](https://app.codacy.com/project/badge/Coverage/0c8a9c1f0e9f4ed1ab12e9c7204682ba)](https://www.codacy.com/gh/grollmus/BuildNotifications/dashboard?utm_source=github.com&utm_medium=referral&utm_content=grollmus/BuildNotifications&utm_campaign=Badge_Coverage)
![GitHub commit activity](https://img.shields.io/github/commit-activity/m/grollmus/BuildNotifications)
![GitHub All Releases](https://img.shields.io/github/downloads/grollmus/BuildNotifications/total)
![GitHub](https://img.shields.io/github/license/grollmus/BuildNotifications)

Build Notifications displays all your builds and notifies you about issues of your code.
Whenever you want to get
- notified only about certain branches,
- only when you broke a CI,
- when someone cancelled your manual build,
- you have a different platforms for source control and builds or
- have multiple projects to take care of,

BuildNotifications got you covered.

# Installation

You can either [download the latest release here](https://github.com/grollmus/BuildNotifications/releases)
or see [build requirements](#build). Note that you need to set your OS language to German on Windows 11
to configure BuildNotifications.
Beta! Right now only Azure DevOps is supported! More plugins are on their way. See [issues](https://github.com/grollmus/BuildNotifications/issues) for more details.

### Group
![alt text](https://i.imgur.com/rgNxwP8.gif "Group by whatever you want")
Group with up to 4 levels deep. By source/defintion/branch to exactly suit your needs.

### React
![alt text](https://i.imgur.com/h8Gbj7M.gif "Highlight failed builds")
Sort your builds by status ascending, descending or by name. Immedietaly see and react when a specific branch/definition/project is not building and minimize failures. Use powerful settings to specify exactly for what you want to be notified for.

### Customize
![alt text](https://i.imgur.com/oXAbwIr.gif "Customize your experience to cater your needs")
Write or use plugins for whatever platform you use. GitHub for sources, Jenkins for builds? Azure DevOps for both? Something completely different? It's all possible and configurable. Want to be notified for only certain branches? Only when manually built? Prefer dark mode?

# Build
* Visual Studio 2022 (ReSharper recommended) or Rider required
* .NET 6.0 SDK required
* Open BuildNotifications.sln from root folder
* Build solution
