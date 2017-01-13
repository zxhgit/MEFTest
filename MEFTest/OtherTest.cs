using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MEFTest
{
    public interface IMyAddin
    {
    }


    #region Dynamic Imports

    public class Dynamic_MyClass
    {
        [Import("TheString")]
        public dynamic MyAddin { get; set; }
    }

    [Export("TheString", typeof(IMyAddin))]
    public class Dynamic_MyLogger : IMyAddin
    {

    }

    [Export("TheString")] //运行时名为TheString的Export只能有一个，否则ComposeParts方法会抛异常
    public class Dynamic_MyToolbar
    {

    }

    public class DynamicImportsTest
    {
        private CompositionContainer _container;

        public void DoTest()
        {
            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(DynamicImportsTest).Assembly));
            _container = new CompositionContainer(catalog);
            var myClass = new Dynamic_MyClass();
            try
            {
                _container.ComposeParts(myClass);
            }
            catch (CompositionException compositionException)
            {
                Console.WriteLine(compositionException.ToString());
            }


            var dyn = myClass.MyAddin;
        }
    }

    #endregion

    #region Lazy Imports

    public class Lazy_MyClass
    {
        [Import]
        public Lazy<IMyAddin> MyAddin { get; set; }
    }

    [Export(typeof(IMyAddin))]
    public class Lazy_MyLogger : IMyAddin
    {

    }

    public class LazyImportsTest
    {
        private CompositionContainer _container;

        public void DoTest()
        {
            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(LazyImportsTest).Assembly));
            _container = new CompositionContainer(catalog);
            var myClass = new Lazy_MyClass();
            try
            {
                _container.ComposeParts(myClass);
            }
            catch (CompositionException compositionException)
            {
                Console.WriteLine(compositionException.ToString());
            }


            var dyn = myClass.MyAddin;
        }
    }

    #endregion

    #region Prerequisite Imports

    public interface IMySubAddin : IMyAddin
    {
    }

    [Export(typeof(IMySubAddin))]
    public class SomeSubAddin : IMySubAddin
    {
    }

    [Export(typeof(Prerequisite_MyClass))]
    public class Prerequisite_MyClass
    {
        private IMyAddin _theAddin;
        //Default constructor will NOT be //used because the ImportingConstructor //attribute is present. 


        public Prerequisite_MyClass()
        {
        } //This constructor will be used. //An import with contract type IMyAddin is //declared automatically. 

        //[ImportingConstructor]
        //public Prerequisite_MyClass(IMyAddin MyAddin)
        //{
        //    _theAddin = MyAddin;
        //}

        [ImportingConstructor]
        public Prerequisite_MyClass([Import(typeof(IMySubAddin))] IMyAddin MyAddin)
        {
            _theAddin = MyAddin;
        }
    }

    public class SomeClass
    {
        [Import] public Prerequisite_MyClass Val1;
    }

    public class PrerequisiteImportsTest
    {
        private CompositionContainer _container;

        public void DoTest()
        {
            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(PrerequisiteImportsTest).Assembly));
            _container = new CompositionContainer(catalog);
            var someClass = new SomeClass();
            try
            {
                _container.ComposeParts(someClass);
            }
            catch (CompositionException compositionException)
            {
                Console.WriteLine(compositionException.ToString());
            }



        }
    }

    #endregion

    #region Multiple Imports

    public class MultipleImports_MyClass
    {
        [ImportMany]
        public IEnumerable<IMyAddin> MyAddin { get; set; }
    }

    public class MultipleImports_Lazy_MyClass
    {
        [ImportMany]
        public IEnumerable<Lazy<IMyAddin>> MyAddin { get; set; }
    }

    [Export(typeof(IMyAddin))]
    public class Class1 : IMyAddin
    {
    }

    [Export(typeof(IMyAddin))]
    public class Class2 : IMyAddin
    {
    }

    [Export(typeof(IMyAddin))]
    public class Class3 : IMyAddin
    {
    }

    public class MultipleImportsTest
    {
        private CompositionContainer _container;

        public void DoTest()
        {
            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(MultipleImportsTest).Assembly));
            _container = new CompositionContainer(catalog);
            //var someClass = new MultipleImports_MyClass();
            var someClass = new MultipleImports_Lazy_MyClass();
            try
            {
                _container.ComposeParts(someClass);
                foreach (var lazy in someClass.MyAddin)
                {
                    var val = lazy.Value; //使用时才会真正建立Value
                }
            }
            catch (CompositionException compositionException)
            {
                Console.WriteLine(compositionException.ToString());
            }



        }
    }

    #endregion

    #region Avoiding Discovery

    [Export]
    public class DataOne
    {
        //This part will be discovered 
        //as normal by the catalog. 
    }

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// 抽象类也不会被加入到catalog中
    /// </remarks>
    [Export]
    public abstract class DataTwo
    {
        //This part will not be discovered 
        //by the catalog. 
    }

    [PartNotDiscoverable]
    [Export]
    public class DataThree
    {
        //This part will also not be discovered 
        //by the catalog. 
    }

    public class AvoidingDiscoveryTest
    {
        private CompositionContainer _container;

        public void DoTest()
        {
            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(AvoidingDiscoveryTest).Assembly));
            _container = new CompositionContainer(catalog);
        }
    }

    #endregion

    #region Metadata

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
        [Import] public Lazy<IPlugin, IPluginMetadata> plugin;
    }

    public class User
    {
        [ImportMany] public IEnumerable<Lazy<IPlugin, IPluginMetadata>> plugins;

        public IPlugin InstantiateLogger()
        {
            IPlugin logger = null;
            foreach (Lazy<IPlugin, IPluginMetadata> plugin in plugins)
            {
                if (plugin.Metadata.Name == "Logger")
                    logger = plugin.Value;
            }
            return logger;
        }
    }

    public class MetadataTest
    {
        private CompositionContainer _container;

        public void DoTest()
        {
            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(MetadataTest).Assembly));
            _container = new CompositionContainer(catalog);
            //var someClass1 = new Addin();
            var someClass2 = new User();
            try
            {
                //_container.ComposeParts(someClass1);
                _container.ComposeParts(someClass2);
            }
            catch (CompositionException compositionException)
            {
                Console.WriteLine(compositionException.ToString());
            }

            var logger = someClass2.InstantiateLogger();
        }
    }

    #endregion

    #region Import and Export Inheritance

    public interface IMyData
    {
    }

    [Export("MyData1", typeof(IMyData))]
    public class MyData1 : IMyData
    {
    }

    [Export]
    public class NumOne
    {
        [Import("MyData1")]
        public IMyData MyData { get; set; }
    }

    public class NumTwo : NumOne
    {
        //Imports are always inherited, so NumTwo will 
        //import IMyData. 
        //Ordinary exports are not inherited, so 
        //NumTwo will NOT export anything. As a result it 
        //will not be discovered by the catalog! 
    }

    [InheritedExport]
    public class NumThree
    {
        [Export]
        public IMyData MyData { get; set; }

        //This part provides two exports, one of 
        //contract type NumThree, and one of 
        //contract type IMyData. 
    }

    public class NumFour : NumThree
    {
        //Because NumThree used InheritedExport, 
        //this part has one export with contract 
        //type NumThree. 
        //Member-level exports are never inherited, 
        //so IMyData is not exported. 
    }

    //-----------------------------------------------------------------

    [InheritedExport(typeof(IPlugin)), ExportMetadata("Name", "Logger"), ExportMetadata("Version", 4)]
    public class LoggerInheritance : IPlugin
    {
        //Exports with contract type IPlugin and 
        //metadata "Name" and "Version". 
    }

    public class SuperLogger : LoggerInheritance
    {
        //Exports with contract type IPlugin and 
        //metadata "Name" and "Version", exactly the same 
        //as the Logger class. 
    }

    [InheritedExport(typeof(IPlugin)), ExportMetadata("Status", "Green")] //此处"Green"不再是int，所以不会匹配IPluginMetadata
    public class MegaLogger : LoggerInheritance
    {
        //Exports with contract type IPlugin and 
        //metadata "Status" only. Re-declaring 
        //the attribute replaces all metadata. 
    }

    public class Addin1
    {
        [ImportMany] public IEnumerable<Lazy<IPlugin, IPluginMetadata>> Plugin;
    }

    public class InheritanceTest
    {
        private CompositionContainer _container;

        public void DoTest()
        {
            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(InheritanceTest).Assembly));
            _container = new CompositionContainer(catalog);
            //var someClass1 = new NumOne();
            //var someClass2 = new NumTwo();
            var plugin = new Addin1();
            try
            {
                //_container.ComposeParts(someClass1);
                //_container.ComposeParts(someClass2);
                _container.ComposeParts(plugin);
            }
            catch (CompositionException compositionException)
            {
                Console.WriteLine(compositionException.ToString());
            }
            foreach (var lazy in plugin.Plugin)
            {
                var val = lazy.Value;
                var meta = lazy.Metadata;
            }
        }
    }

    #endregion

    #region Custom Export Attributes

    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class MyAttribute : ExportAttribute
    {
        public MyAttribute(string myMetadata) : base(typeof(IMyAddin))
        {
            MyMetadata = myMetadata;
        }

        public string MyMetadata { get; private set; }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// msdn中的例子有问题
    /// </remarks>
    [Export(typeof(IMyAddin)), ExportMetadata("MyMetadata", "theData")]
    public class MyAddin1 : IMyAddin
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// 与MyAddin1所用的Export、ExportMetadata两个标签等效
    /// </remarks>
    [MyAttribute("theData")]
    public class MyAddin2 : IMyAddin
    {
    }


    #endregion

}
