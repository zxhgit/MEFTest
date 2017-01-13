using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultipleImportsTest
{
    public interface IMyAddin
    {
    }

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
        public static void DoTest()
        {
            var someClass = new MultipleImports_Lazy_MyClass();
            ComposeObjects(someClass);
            foreach (var lazy in someClass.MyAddin)
            {
                Console.WriteLine(lazy.GetType().ToString());             
            }

            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(MultipleImportsTest).Assembly));
            var container = new CompositionContainer(catalog);
            var someClass1=new MultipleImports_Lazy_MyClass();
            container.ComposeParts(someClass1);
            foreach (var lazy in someClass1.MyAddin)
            {
                Console.WriteLine(lazy.Value.GetType().ToString());
            }
        }

        private static void ComposeObjects(params object[] objects)
        {
            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(MultipleImportsTest).Assembly));
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
            MultipleImportsTest.DoTest();
            Console.ReadLine();
        }
    }
}
