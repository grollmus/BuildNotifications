# Data Sources
Multiple sources with multiple providers

DataSource: Connection (TFS, Jenkins etc.)

Dataprovider: BuildProvider, PullRequestProvider, BranchProvider?

# Builds
- By who, for who, when, status
- Last build of current group? (If so: display no more builds indicator)
- Goto first broken build
- Add new build to build queue

# Pull Requests
- By who, for who, when, status
- Associate with builds?

# Grouping
- By branch (are there scenarios without branches?)
- By definition (are there scenarios without definitions? => Small projects probably won't have different builds)
- None (Plain list of builds in queue)
- Custom?
- Only builds, only PRs, both?

## Levels
1-3 levels including
- Everything in a flat list (1 level)
- Connections split, then grouped by branch then by definition (3 levels)
- Connections split, then by definition, branch doesn't matter (2 levels)

# Search
- Groupings
- Author
- Free text

# Settings
- General
    - Language
    - Update interval
    - Number of elements per group
    - Default grouping
- Data sources
    - Setup (ConnectionCode, Authentication, etc.)
    - Black and whitelists for build definitions, branches, Pull Requests, ...
    - Combination of providers (e.g. builds on Azure TFS, Pull Requests and Branches on Github)

# Deployment
Custom deployments with predefined connections?

# Update
- Squirrel with Github releases
- Silent Updates (maybe a notification)
