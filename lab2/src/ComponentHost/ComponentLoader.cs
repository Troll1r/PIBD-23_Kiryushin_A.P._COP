using ComponentOrientedPlatform;
using System.Reflection;
using System.Text.Json;

namespace ComponentHost;

/// <summary>
/// Загрузка компонентов из каталога, указанного в appsettings.json.
/// </summary>
internal static class ComponentLoader
{
    private sealed class AppSettings
    {
        public string? ComponentDirectory { get; set; }
    }

    public static List<IComponentContract> LoadConfiguredComponents(out List<string> errors)
    {
        errors = [];

        string componentDirectory = ResolveComponentDirectory();

        if (!Directory.Exists(componentDirectory))
        {
            throw new DirectoryNotFoundException(
                $"Каталог компонентов не найден: {componentDirectory}");
        }

        AppDomain.CurrentDomain.AssemblyResolve += (_, args) =>
            ResolveAssembly(args, componentDirectory);

        var components = new List<IComponentContract>();

        foreach (string file in Directory.GetFiles(componentDirectory, "*.dll")
                     .OrderBy(Path.GetFileName))
        {
            try
            {
                Assembly assembly = Assembly.LoadFrom(file);

                foreach (Type type in assembly.GetTypes())
                {
                    if (!typeof(IComponentContract).IsAssignableFrom(type)
                        || type.IsAbstract
                        || type.IsInterface)
                    {
                        continue;
                    }

                    if (type.GetConstructor(Type.EmptyTypes) == null)
                    {
                        errors.Add(
                            $"Тип {type.FullName} не имеет открытого конструктора без параметров.");
                        continue;
                    }

                    if (Activator.CreateInstance(type) is IComponentContract component)
                    {
                        components.Add(component);
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

                errors.Add(
                    $"Не удалось загрузить типы из {Path.GetFileName(file)}: {details}");
            }
            catch (Exception ex)
            {
                errors.Add(
                    $"Не удалось загрузить {Path.GetFileName(file)}: {ex.Message}");
            }
        }

        return components;
    }

    private static string ResolveComponentDirectory()
    {
        string configPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
        string componentDirectory = "Components";

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

                if (!string.IsNullOrWhiteSpace(settings?.ComponentDirectory))
                {
                    componentDirectory = settings.ComponentDirectory;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Не удалось прочитать конфигурацию {configPath}: {ex.Message}",
                    ex);
            }
        }

        return Path.IsPathRooted(componentDirectory)
            ? componentDirectory
            : Path.Combine(AppContext.BaseDirectory, componentDirectory);
    }

    private static Assembly? ResolveAssembly(
        ResolveEventArgs args,
        string componentDirectory)
    {
        string? assemblyName = new AssemblyName(args.Name).Name;

        if (string.IsNullOrWhiteSpace(assemblyName))
        {
            return null;
        }

        string fileName = assemblyName + ".dll";
        string candidate = Path.Combine(componentDirectory, fileName);

        if (File.Exists(candidate))
        {
            return Assembly.LoadFrom(candidate);
        }

        candidate = Path.Combine(AppContext.BaseDirectory, fileName);

        return File.Exists(candidate) ? Assembly.LoadFrom(candidate) : null;
    }
}
