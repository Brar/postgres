using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace UpdateClrConfig
{
    class Program
    {
        private const string RuntimeJsonUrl =
            "https://raw.githubusercontent.com/dotnet/runtime/master/src/libraries/pkg/Microsoft.NETCore.Platforms/runtime.json";
        private const string RuntimeJsonCachePath = "./runtime.json";
        private static readonly string[] PublishedRuntimes =
        {
            "linux-arm",
            "linux-arm64",
            "linux-musl-x64",
            "linux-x64",
            "osx-x64",
            "rhel.6-x64",
            "win-arm",
            "win-arm64",
            "win-x64",
            "win-x86",
        };

        static async Task<int> Main(string[] args)
        {
            try
            {
                await UpdateClrConfigAsync(args);
                return 0;
            }
            catch (Exception e)
            {
                await Console.Error.WriteLineAsync(e.Message);
                return 1;
            }
        }

        private static async Task UpdateClrConfigAsync(string[] args)
        {
            using var jsonDocument = await GetJsonDocumentAsync(args);
            var graph = GetRuntimeGraph(jsonDocument);
            var runtimeCompatibilityList = GetRuntimeCompatibilityList(graph);
            using StreamWriter w = new StreamWriter(File.OpenWrite("./output.txt"));
//            TextWriter w = Console.Out;

            await w.WriteLineAsync(@"AC_MSG_CHECKING([.NET Core nethost directory])
case $clr_runtime_identifier in");
            foreach (var list in runtimeCompatibilityList.Where(e => !e.Key.Contains("win")))
            {
                await w.WriteLineAsync($"  {list.Key})");
                await w.WriteAsync(
                    @"    clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$clr_runtime_identifier/$dotnet_runtime_version/runtimes/$clr_runtime_identifier/native/
    if test -d $clr_nethostdir
    then
      AC_MSG_RESULT([$clr_nethostdir])
    else
      for compatible_runtime_identifier in ");
                await w.WriteLineAsync(string.Join(' ', list.Value));
                await w.WriteLineAsync(@"      do
        clr_nethostdir=${clr_basedir}packs/Microsoft.NETCore.App.Host.$compatible_runtime_identifier/$dotnet_runtime_version/runtimes/$compatible_runtime_identifier/native/
        if test -d $clr_nethostdir
        then
            break
        fi
      done
    fi
    ;;");
            }
            await w.WriteLineAsync("esac");
        }

        private static Dictionary<string, string[]> GetRuntimeCompatibilityList(RuntimeGraph graph)
        {
            var list = new Dictionary<string, string[]>();
            foreach (var runtimeIdentifier in graph.RuntimeIdentifiers.Values)
            {
                var compatibles = new List<string>();
                AddCompatibleRuntimeIdentifiers(runtimeIdentifier, compatibles);

                list.Add(runtimeIdentifier.Name, compatibles.Distinct().OrderByDescending(e=> e.Length).ToArray());
            }

            return list;
        }

        private static void AddCompatibleRuntimeIdentifiers(RuntimeIdentifier runtimeIdentifier, List<string> compatibles)
        {
            compatibles.AddRange(runtimeIdentifier.CompatibleIdentifiers!.Select(i => i.Name));
            //compatibles.AddRange(runtimeIdentifier.CompatibleIdentifiers!.Where(i => PublishedRuntimes.Any(pr => pr == i.Name)).Select(i => i.Name));

            foreach (var innerIdentifier in runtimeIdentifier.CompatibleIdentifiers!)
            {
                AddCompatibleRuntimeIdentifiers(innerIdentifier, compatibles);
            }
        }

        private static RuntimeGraph GetRuntimeGraph(JsonDocument jsonDocument)
        {
            var runtimesObject = jsonDocument.RootElement.EnumerateObject()
                .FirstOrDefault(e => e.Name == "runtimes");

            if (runtimesObject.Value.ValueKind != JsonValueKind.Object)
                throw new Exception(
                    "Invalid runtime.json file. Expecting {JsonValueKind.Object} named 'runtimes' as root element but got " +
                    runtimesObject.Value.ValueKind +
                    (runtimesObject.Value.ValueKind == JsonValueKind.Undefined
                        ? ""
                        : $" named '{runtimesObject.Name}'"));

            var graph = new RuntimeGraph();
            foreach (var property in runtimesObject.Value.EnumerateObject())
            {
                if (property.Value.ValueKind != JsonValueKind.Object)
                    throw new Exception(
                        $"Invalid runtime.json file. Expecting {JsonValueKind.Object} but got {runtimesObject.Value.ValueKind}.");

                if (property.Value.EnumerateObject().Any(e => e.Name != "#import") ||
                    property.Value.EnumerateObject().Count() != 1 ||
                    property.Value.EnumerateObject().First().Value.ValueKind != JsonValueKind.Array)
                    throw new Exception(
                        $"Invalid runtime.json file. Expecting single child element named '#import' with an array value but got {property.Value}.");

                var runtimeIdentifier = new RuntimeIdentifier(property.Name);
                graph.RuntimeIdentifiers[property.Name] = runtimeIdentifier;
                if (!property.Value.EnumerateObject().First().Value.EnumerateArray().Any())
                {
                    runtimeIdentifier.CompatibleIdentifiers = Array.Empty<RuntimeIdentifier>();
                }
            }

            while (graph.RuntimeIdentifiers.Values.Any(e => e.CompatibleIdentifiers == null))
            {
                foreach (var property in runtimesObject.Value.EnumerateObject())
                {
                    if (property.Value.EnumerateObject().First().Value.EnumerateArray().Any(e =>
                        graph.RuntimeIdentifiers[e.GetString()].CompatibleIdentifiers == null)) continue;

                    graph.RuntimeIdentifiers[property.Name].CompatibleIdentifiers = property.Value.EnumerateObject()
                        .First().Value.EnumerateArray().Select(e => graph.RuntimeIdentifiers[e.GetString()]).ToArray();
                }
            }

            return graph;
        }

        private static async Task<JsonDocument> GetJsonDocumentAsync(string[] args)
        {
            if (args.Length == 0)
            {
                if (!File.Exists(RuntimeJsonCachePath))
                    await DownloadRuntimeJsonAsync();

                return await JsonDocument.ParseAsync(File.OpenRead(RuntimeJsonCachePath));
            }
            if (args.Length == 1 && File.Exists(args[0]))
                return await JsonDocument.ParseAsync(File.OpenRead(args[0]));

            throw new ArgumentException($"Invalid argument: '{args[0]}'", nameof(args));
        }

        static async Task DownloadRuntimeJsonAsync()
        {
            using var client = new HttpClient();
            using var result = await client.GetAsync(RuntimeJsonUrl);
            result.EnsureSuccessStatusCode();
            await using var localRuntimeJsonCacheFile = File.OpenWrite(RuntimeJsonCachePath);
            await result.Content.CopyToAsync(localRuntimeJsonCacheFile);
        }
    }
}
