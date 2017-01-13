using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrerequisiteImportsTest
{
    public interface IMyAddin
    {
    }

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
        //Default constructor will NOT be 
        //used because the ImportingConstructor 
        //attribute is present. 


        public Prerequisite_MyClass()
        {
        } 
        //This constructor will be used. 
        //An import with contract type IMyAddin is 
        //declared automatically. 

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

        public IMyAddin GetAddin()
        {
            return _theAddin;
        }
    }

    public class SomeClass
    {
        [Import]
        public Prerequisite_MyClass Val1;
    }

    public class PrerequisiteImportsTest
    {
        public static void DoTest()
        {
            var someClass = new SomeClass();
            ComposeObjects(someClass);
            var typeInfo = someClass.Val1.GetAddin().GetType();
            Console.WriteLine(typeInfo);
        }

        private static void ComposeObjects(params object[] objects)
        {
            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(PrerequisiteImportsTest).Assembly));
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
            PrerequisiteImportsTest.DoTest();
            Console.ReadLine();
        }
    }
}
