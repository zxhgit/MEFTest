using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetadataTest
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// 元数据视图接口（Metadata View Interface）只能包含properties
    /// 必须要有get访问器
    /// </remarks>
    public interface IPluginMetadata
    {
        string Name { get; }

        [DefaultValue(1)]
        int Version { get; }
    }

    public interface IPlugin
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Each piece of metadata is composed of a name/value pair. 
    /// The name portion of the metadata must match the name of the appropriate property in the metadata view, 
    /// and the value will be assigned to that property.
    /// Metadata是对Export部件（parts）的描述，采用键值对形式，一个部件可以使用多个Metadata
    /// </remarks>
    [Export(typeof(IPlugin)), ExportMetadata("Name", "Logger"), ExportMetadata("Version", 4)]
    public class Logger : IPlugin
    {

    }

    [Export(typeof(IPlugin)), ExportMetadata("Name", "Disk Writer")]
    //Version is not required because of the DefaultValue 
    public class DWriter : IPlugin
    {

    }

    public class Addin
    {
        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// It is the importer that specifies what metadata view, if any, will be in use. 
        /// An import with metadata is declared as a lazy import, with the metadata interface as the second type parameter to Lazy<T,T>. 
        /// The following class imports the previous part with metadata.
        /// </remarks>
        [Import]
        public Lazy<IPlugin, IPluginMetadata> plugin;
    }

    public class User
    {
        [ImportMany]
        public IEnumerable<Lazy<IPlugin, IPluginMetadata>> plugins;

        public IPlugin InstantiateLogger()
        {
            IPlugin logger = null;
            foreach (Lazy<IPlugin, IPluginMetadata> plugin in plugins)//如果提前释放CompositionContainer，plugins会为null
            {
                if (plugin.Metadata.Name == "Logger")
                    logger = plugin.Value;
            }
            return logger;
        }
    }

    public class MetadataTest
    {
        public static void DoTest()
        {
            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(MetadataTest).Assembly));            
            //var someClass1 = new Addin();
            var someClass2 = new User();
            using (var container = new CompositionContainer(catalog))
            {
                //container.ComposeParts(someClass1, someClass2);
                container.ComposeParts(someClass2);
                //var someClass1MetaDataName = someClass1.plugin.Metadata.Name;
                //var someClass1MetaDataVersion = someClass1.plugin.Metadata.Version;
                //var someClass1Type = someClass1.plugin.Value.GetType().ToString();
                //Console.WriteLine("MetaDataName:" + someClass1MetaDataName);
                //Console.WriteLine("MetaDataVersion" + someClass1MetaDataVersion);
                //Console.WriteLine("Type" + someClass1Type);

                var logger = someClass2.InstantiateLogger();
                var loggerType = logger.GetType();
                Console.WriteLine("Type" + loggerType);
            }
            
            
        }

        private static void ComposeObjects(params object[] objects)
        {
            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(MetadataTest).Assembly));
            using (var container = new CompositionContainer(catalog))
            {
                try
                {
                    container.ComposeParts(objects);
                }
                catch (CompositionException compositionException)
                {
                    Console.WriteLine(compositionException.ToString());
                }
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            MetadataTest.DoTest();
            Console.ReadLine();
        }
    }
}
