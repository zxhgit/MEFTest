using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazyImportsTest
{
    public interface IMyAddin
    {
    }

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
        public static void DoTest()
        {
            var myClass = new Lazy_MyClass();
            ComposeObjects(myClass);
            var myAddinTypeInfo = myClass.MyAddin.GetType();
            //var myAddinValTypeInfo = myClass.MyAddin.Value.GetType();//无法访问已释放的对象。\r\n对象名:“System.ComponentModel.Composition.Hosting.CatalogExportProvider”。
            Console.WriteLine("myClass.MyAddin的Type是：" + myAddinTypeInfo);
            //Console.WriteLine("myClass.MyAddin.Value的Type是：" + myAddinValTypeInfo);

            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(LazyImportsTest).Assembly));
            var container = new CompositionContainer(catalog);
            var myClass1= new Lazy_MyClass();
            container.ComposeParts(myClass1);
            var val = myClass1.MyAddin.Value;//此处不会报“无法访问已释放的对象”异常
            //说明释放CompositionContainer导致这个异常，
            //说明对Lazy<T>的求值还是需要CompositionContainer的。★
            var myAddinValTypeInfo = val.GetType();
            Console.WriteLine("myClass.MyAddin.Value的Type是：" + myAddinValTypeInfo);
        }

        private static void ComposeObjects(params object[] objects)
        {
            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(LazyImportsTest).Assembly));
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
            LazyImportsTest.DoTest();
            Console.ReadLine();
        }
    }
}
