// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.Encodings.Web;
using System.Text.Json;
using Flow.Plugin.VSCodeWorkspaces;
using Flow.Plugin.VSCodeWorkspaces.VSCodeHelper;
using Flow.Plugin.VSCodeWorkspaces.WorkspacesHelper;

namespace Flow.VSCodeWorkspaces.Cli;

internal static class Program
{
    private static int Main(string[] args)
    {
        var jsonOutput = args.Contains("--json");

        VSCodeInstances.LoadVSCodeInstances();

        var workspacesApi = new VSCodeWorkspacesApi();
        var workspaces = workspacesApi.Workspaces
            .Distinct()
            .Select(ToRow)
            .ToList();

        if (jsonOutput)
        {
            var json = JsonSerializer.Serialize(workspaces, new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            });
            Console.WriteLine(json);
        }
        else
        {
            PrintTable(workspaces);
        }

        return 0;
    }

    private static WorkspaceRow ToRow(VsCodeWorkspace ws)
    {
        return new WorkspaceRow(
            Name: ws.Label ?? ws.FolderName,
            Type: ws.WorkspaceType.ToString(),
            Location: ws.WorkspaceTypeToString(),
            Path: SystemPath.RealPath(ws.RelativePath),
            VSCodeVersion: ws.VSCodeInstance.VSCodeVersion.ToString(),
            ExecutablePath: ws.VSCodeInstance.ExecutablePath,
            LaunchCommand: BuildLaunchCommand(ws));
    }

    // Mirrors the launch logic in Main.CreateWorkspaceResult's Action callback.
    private static string BuildLaunchCommand(VsCodeWorkspace ws)
    {
        var flag = ws.WorkspaceType == WorkspaceType.Workspace ? "--file-uri" : "--folder-uri";
        return $"{Quote(ws.VSCodeInstance.ExecutablePath)} {flag} {Quote(ws.Path)}";
    }

    private static string Quote(string value) => $"\"{value}\"";

    private static void PrintTable(List<WorkspaceRow> workspaces)
    {
        if (workspaces.Count == 0)
        {
            Console.WriteLine("No VSCode workspaces found.");
            return;
        }

        string[] headers = { "NAME", "TYPE", "LOCATION", "PATH", "VERSION" };
        var rows = workspaces
            .Select(w => new[] { w.Name, w.Type, w.Location, w.Path, w.VSCodeVersion })
            .ToList();

        var widths = headers
            .Select((h, i) => Math.Max(h.Length, rows.Count == 0 ? 0 : rows.Max(r => r[i].Length)))
            .ToArray();

        void PrintRow(string[] cells)
        {
            Console.WriteLine(string.Join("  ", cells.Select((c, i) => c.PadRight(widths[i]))));
        }

        PrintRow(headers);
        foreach (var row in rows)
        {
            PrintRow(row);
        }
    }

    private record WorkspaceRow(
        string Name,
        string Type,
        string Location,
        string Path,
        string VSCodeVersion,
        string ExecutablePath,
        string LaunchCommand);
}
