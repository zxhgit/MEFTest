using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CatalogTest
{
    class Program
    {
        #region ..\Simple.Data.Ado\ProviderHelper.cs
        private static Object GetCustomProviderExport<T>(Assembly assembly)
        {
            using (var assemblyCatalog = new AssemblyCatalog(assembly))
            {
                using (var container = new CompositionContainer(assemblyCatalog))
                {
                    return container.GetExportedValueOrDefault<T>();
                }
            }
        }

        //与GetAdjacentComponent<T>(Type knownSiblingType)重复
        //private static T GetCustomProviderExport<T>(ISchemaProvider schemaProvider)
        //{
        //    using (var assemblyCatalog = new AssemblyCatalog(schemaProvider.GetType().Assembly))
        //    {
        //        using (var container = new CompositionContainer(assemblyCatalog))
        //        {
        //            return container.GetExportedValueOrDefault<T>();
        //        }
        //    }
        //}
        #endregion

        #region ..\SimpleDataTest\Simple.Data\MefHelper.cs
        public static T GetAdjacentComponent<T>(Type knownSiblingType)
        {
            using (var assemblyCatalog = new AssemblyCatalog(knownSiblingType.Assembly))
            {
                using (var container = new CompositionContainer(assemblyCatalog))
                {
                    return container.GetExportedValueOrDefault<T>();
                }
            }
        }

        public T Compose<T>()
        {
            using (var container = CreateAppDomainContainer())
            {
                var exports = container.GetExports<T>().ToList();
                if (exports.Count == 1)
                {
                    return exports.Single().Value;
                }
            }
            using (var container = CreateFolderContainer())
            {
                var exports = container.GetExports<T>().ToList();
                if (exports.Count == 0) throw new Exception("No ADO Provider found.");
                if (exports.Count > 1) throw new Exception("Multiple ADO Providers found; specify provider name or remove unwanted assemblies.");
                return exports.Single().Value;
            }
        }

        public T Compose<T>(string contractName)
        {
            try
            {
                using (var container = CreateAppDomainContainer())
                {
                    var exports = container.GetExports<T>(contractName).ToList();
                    if (exports.Count == 1)
                    {
                        return exports.Single().Value;
                    }
                }
                using (var container = CreateFolderContainer())
                {
                    var exports = container.GetExports<T>(contractName).ToList();
                    if (exports.Count == 0) throw new Exception(string.Format("No {0} Provider found.", contractName));
                    if (exports.Count > 1) throw new Exception("Multiple ADO Providers found; specify provider name or remove unwanted assemblies.");
                    return exports.Single().Value;
                }
            }
            catch (ReflectionTypeLoadException ex)
            {
                throw;
            }
        }

        private static CompositionContainer CreateFolderContainer()
        {
            var path = GetSimpleDataAssemblyPath();

            var assemblyCatalog = new AssemblyCatalog(ThisAssembly);
            var aggregateCatalog = new AggregateCatalog(assemblyCatalog);
            foreach (string file in System.IO.Directory.GetFiles(path, "Simple.Data.*.dll"))
            {
                var catalog = new AssemblyCatalog(file);
                aggregateCatalog.Catalogs.Add(catalog);
            }
            return new CompositionContainer(aggregateCatalog);
        }

        private static CompositionContainer CreateAppDomainContainer()
        {
            var aggregateCatalog = new AggregateCatalog();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies().Where(IsSimpleDataAssembly))
            {
                aggregateCatalog.Catalogs.Add(new AssemblyCatalog(assembly));
            }
            return new CompositionContainer(aggregateCatalog);
        }
        #endregion

        #region private
        private static readonly Assembly ThisAssembly = typeof(Program).Assembly;
        private static bool IsSimpleDataAssembly(Assembly assembly)
        {
            return true;
        }
        private static string GetSimpleDataAssemblyPath()
        {
            var path = ThisAssembly.CodeBase.Replace("file:///", "").Replace("file://", "//");
            path = Path.GetDirectoryName(path);
            if (path == null) throw new ArgumentException("Unrecognised assemblyFile.");
            if (!Path.IsPathRooted(path))
            {
                path = Path.DirectorySeparatorChar + path;
            }
            return path;
        }
        #endregion

        static void Main(string[] args)
        {
        }
    }
}
