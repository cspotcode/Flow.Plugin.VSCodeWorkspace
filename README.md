CLI tool to list all VSCode workspaces in the "Open recent" list. Might eventually clean it up to a nice CLI
fuzzy-matcher, wire it into `fzf`, something like that.

Companion to:

- Flow Launcher plugin [Flow.Plugin.VSCodeWorkspace](https://www.flowlauncher.com/plugins/vs-code-workspaces/)
- VSCode's own "Open recent" menu: Ctrl+R
- `zoxide` and the like: `cd`-style recent directory lists

# Usage

No installation workflow yet. Clone and build.

```shell
git clone ... # this repo
cd ... # name of clone
just run # to run it
just publish # to build the .exe into redistributable form
```

# Implementation

> [!NOTE]
> This was built off of https://github.com/taooceros/Flow.Plugin.VSCodeWorkspace, which already had all the necesary
> logic for enumerating the "recent" list and launching VSCode.

