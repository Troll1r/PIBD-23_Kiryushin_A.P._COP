using PluginsContracts;
using System.Reflection;
using System.Text.Json;

namespace ComponentHost;

internal sealed class PluginOption
{
    public PluginOption(string format, object instance)
    {
        Format = format;
        Instance = instance;
    }

    public string Format { get; }

    public object Instance { get; }

    public override string ToString()
    {
        return Format;
    }
}

internal sealed class PluginLoadResult
{
    public List<PluginOption> TextPlugins { get; } = [];

    public List<PluginOption> LineChartPlugins { get; } = [];

    public List<PluginOption> PieChartPlugins { get; } = [];

    public List<string> Errors { get; } = [];
}

internal static class PluginLoader
{
    private sealed class AppSettings
    {
        public string? PluginDirectory { get; set; }
    }

    public static PluginLoadResult LoadConfiguredPlugins()
    {
        var result = new PluginLoadResult();
        string pluginDirectory = ResolvePluginDirectory();

        if (!Directory.Exists(pluginDirectory))
        {
            result.Errors.Add($"Каталог плагинов не найден: {pluginDirectory}");
            return result;
        }

        AppDomain.CurrentDomain.AssemblyResolve += (_, args) =>
            ResolveAssembly(args, pluginDirectory);

        foreach (string file in Directory
                     .GetFiles(pluginDirectory, "ProductCatalog.Plugins.*.dll")
                     .OrderBy(Path.GetFileName))
        {
            try
            {
                Assembly assembly = Assembly.LoadFrom(file);

                foreach (Type type in assembly.GetTypes())
                {
                    if (type.IsAbstract || type.IsInterface)
                    {
                        continue;
                    }

                    bool isText = typeof(IReportDocumentWithContextTextsContract)
                        .IsAssignableFrom(type);
                    bool isLine = typeof(IReportDocumentWithChartLineContract)
                        .IsAssignableFrom(type);
                    bool isPie = typeof(IReportDocumentWithChartPieContract)
                        .IsAssignableFrom(type);

                    if (!isText && !isLine && !isPie)
                    {
                        continue;
                    }

                    if (type.GetConstructor(Type.EmptyTypes) == null)
                    {
                        result.Errors.Add(
                            $"Тип {type.FullName} не имеет открытого конструктора без параметров.");
                        continue;
                    }

                    if (Activator.CreateInstance(type) is not IReportDocumentContract plugin)
                    {
                        result.Errors.Add(
                            $"Тип {type.FullName} не реализует {nameof(IReportDocumentContract)}.");
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(plugin.DocumentFormat))
                    {
                        result.Errors.Add(
                            $"Тип {type.FullName} не указал формат документа.");
                        continue;
                    }

                    var option = new PluginOption(plugin.DocumentFormat, plugin);

                    if (isText)
                    {
                        AddPlugin(
                            result.TextPlugins,
                            option,
                            "текстового документа",
                            result.Errors);
                    }

                    if (isLine)
                    {
                        AddPlugin(
                            result.LineChartPlugins,
                            option,
                            "линейной диаграммы",
                            result.Errors);
                    }

                    if (isPie)
                    {
                        AddPlugin(
                            result.PieChartPlugins,
                            option,
                            "круговой диаграммы",
                            result.Errors);
                    }
                }
            }
            catch (ReflectionTypeLoadException ex)
            {
                string details = string.Join(
                    "; ",
                    ex.LoaderExceptions
                        .Where(loaderException => loaderException != null)
                        .Select(loaderException => loaderException!.Message));

                result.Errors.Add(
                    $"Не удалось загрузить типы из {Path.GetFileName(file)}: {details}");
            }
            catch (Exception ex)
            {
                result.Errors.Add(
                    $"Не удалось загрузить {Path.GetFileName(file)}: {ex.Message}");
            }
        }

        return result;
    }

    private static void AddPlugin(
        List<PluginOption> target,
        PluginOption option,
        string contractName,
        List<string> errors)
    {
        if (target.Any(existing => string.Equals(
                existing.Format,
                option.Format,
                StringComparison.OrdinalIgnoreCase)))
        {
            errors.Add(
                $"Для {contractName} уже есть плагин с форматом «{option.Format}».");
            return;
        }

        target.Add(option);
    }

    private static string ResolvePluginDirectory()
    {
        string configPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
        string pluginDirectory = "Plugins";

        if (File.Exists(configPath))
        {
            try
            {
                string json = File.ReadAllText(configPath);
                AppSettings? settings = JsonSerializer.Deserialize<AppSettings>(
                    json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                if (!string.IsNullOrWhiteSpace(settings?.PluginDirectory))
                {
                    pluginDirectory = settings.PluginDirectory;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Не удалось прочитать конфигурацию {configPath}: {ex.Message}",
                    ex);
            }
        }

        return Path.IsPathRooted(pluginDirectory)
            ? pluginDirectory
            : Path.Combine(AppContext.BaseDirectory, pluginDirectory);
    }

    private static Assembly? ResolveAssembly(
        ResolveEventArgs args,
        string pluginDirectory)
    {
        string? assemblyName = new AssemblyName(args.Name).Name;

        if (string.IsNullOrWhiteSpace(assemblyName))
        {
            return null;
        }

        string fileName = assemblyName + ".dll";
        string candidate = Path.Combine(pluginDirectory, fileName);

        if (File.Exists(candidate))
        {
            return Assembly.LoadFrom(candidate);
        }

        candidate = Path.Combine(AppContext.BaseDirectory, fileName);

        return File.Exists(candidate) ? Assembly.LoadFrom(candidate) : null;
    }
}
