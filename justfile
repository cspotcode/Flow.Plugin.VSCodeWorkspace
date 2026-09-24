set dotenv-load
set positional-arguments

cli_project := "Flow.VSCodeWorkspaces.Cli/Flow.VSCodeWorkspaces.Cli.csproj"
cli_exe := "Flow.VSCodeWorkspaces.Cli/bin/Debug/net8.0-windows/vscode-workspaces.exe"

@default:
  just --list --unsorted

# Build the CLI.
build:
  dotnet build {{cli_project}} -c Debug

# Build and run the CLI, forwarding extra args, e.g. `just run --json`
# Invokes the built exe directly (not `dotnet run`) so stdout is only ever the
# program's own output and can be piped straight into `jq` etc; `dotnet build`
# and `dotnet run` both print MSBuild output to stdout, so the build step's
# stdout is redirected to stderr here rather than left to mix with the exe's.
run *args:
  @dotnet build {{cli_project}} -c Debug 1>&2
  @{{cli_exe}} {{args}}

# Publish a small, framework-dependent win-x64 build of the CLI (needs the .NET runtime installed).
# Plain `dotnet build` copies native deps (e.g. sqlite) for every RID (~25MB dead weight);
# publishing with -r win-x64 trims that down to just what's needed.
publish out="publish":
  dotnet publish {{cli_project}} -c Release -r win-x64 --self-contained false -o {{out}}

# Remove build output for both the plugin and the CLI project.
clean:
  rm -rf obj bin Flow.VSCodeWorkspaces.Cli/obj Flow.VSCodeWorkspaces.Cli/bin
