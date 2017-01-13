using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicImportsTest
{
    public interface IMyAddin
    {
    }

    public class Dynamic_MyClass
    {
        [Import("TheString")]
        public dynamic MyAddin { get; set; }
    }

    [Export("TheString", typeof(IMyAddin))]
    public class Dynamic_MyLogger : IMyAddin
    {

    }

    //[Export("TheString")] //运行时名为TheString的Export只能有一个，否则ComposeParts方法会抛异常
    public class Dynamic_MyToolbar
    {

    }

    public class DynamicImportsTest
    {
        public static void DoTest()
        {
            var myClass = new Dynamic_MyClass();
            ComposeObjects(myClass);
            var typeInfo = myClass.MyAddin.GetType().ToString();
            //测试向Dynamic_MyClass的MyAddin属性分别fill Dynamic_MyLogger和Dynamic_MyToolbar，查看MyAddin的类型。
            Console.WriteLine("myClass.MyAddin的Type是：" + typeInfo);
        }

        private static void ComposeObjects(params object[] objects)
        {
            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(DynamicImportsTest).Assembly));
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
            DynamicImportsTest.DoTest();
            Console.ReadLine();
        }
    }
}
