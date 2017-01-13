using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportExportBasicsTest
{
    public interface IMyAddin
    {
    }

    public class MyClass
    {
        [Import]
        public IMyAddin MyAddin { get; set; }
    }

    [Export(typeof(IMyAddin))]
    public class MyLogger : IMyAddin
    {

    }

    [Export] //WILL NOT match the previous import! 
    public class MyLogger1 : IMyAddin
    {

    }

    public class MyClass1
    {
        [Import("MajorRevision")]
        public int MajorRevision { get; set; }

    }

    public class MyExportClass
    {
        [Export("MajorRevision")] //This one will match. 
        public int MajorRevision = 4;

        [Export("MinorRevision")]
        public int MinorRevision = 16;
    }


    public class MyAddin
    {
        //Explicitly specifying a generic type. 
        [Export(typeof(Func<int, string>))]
        public string DoSomething(int TheParam)
        {
            return "MyAddin.DoSomething(" + TheParam + ")";
        }
    }

    public class MyClass2
    {
        [Import] //Contract name must match! 
        public Func<int, string> DoSomething { get; set; }
    }


    public class ImportExportBasicsTest
    {
       

        public static void DoTest()
        {
            var myclass1 = new MyClass1();
            ComposeObjects(myclass1);
            Console.WriteLine("myclass1.MajorRevision:" + myclass1.MajorRevision);

            var myclass2 = new MyClass2();
            ComposeObjects(myclass2);
            var doSomethingVal = myclass2.DoSomething(123);
            Console.WriteLine("doSomethingVal:" + doSomethingVal);
        }

        private static void ComposeObjects(params object[] objects)
        {
            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(ImportExportBasicsTest).Assembly));
            using (var container = new CompositionContainer(catalog))
            {
                try
                {
                    //container中的Catalog.Parts的类型是System.ComponentModel.Composition.ReflectionModel.ReflectionComposablePartDefinition
                    //其抽象基类是ComposablePartDefinition，该类的CreatePart方法返回的类型是ComposablePart
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
            ImportExportBasicsTest.DoTest();
            Console.ReadLine();
        }
    }
}
