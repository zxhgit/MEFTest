using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MEFTest.TestCase;

namespace MEFTest
{
    class Program
    {
        private CompositionContainer _container;

        [Import(typeof(ICalculator))]//typeof(ICalculator)可省略，MEF可根据下面calculator的类型ICalculator来自动推断（msdn）
        public ICalculator calculator;//当line34——this._container.ComposeParts(this);运行时才会被装配


        private Program()
        {
            //An aggregate catalog that combines multiple catalogs
            var catalog = new AggregateCatalog();
            //Adds all the parts found in the same assembly as the Program class
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(Program).Assembly));
            //catalog.Catalogs.Add(new DirectoryCatalog("C:\\Users\\SomeUser\\Documents\\Visual Studio 2010\\Projects\\SimpleCalculator3\\SimpleCalculator3\\Extensions"));


            //Create the CompositionContainer with the parts in the catalog
            _container = new CompositionContainer(catalog);

            //Fill the imports of this object
            try
            {
                this._container.ComposeParts(this);
            }
            catch (CompositionException compositionException)
            {
                Console.WriteLine(compositionException.ToString());
            }
        }


        static void Main(string[] args)
        {
            Program p = new Program(); //Composition is performed in the constructor
            String s;
            Console.WriteLine("Enter Command:");
            //while (true)
            //{
            //    s = Console.ReadLine();
            //    Console.WriteLine(p.calculator.Calculate(s));
            //}


            //OtherTest
            //(new DynamicImportsTest()).DoTest();
            //(new LazyImportsTest()).DoTest();
            //(new PrerequisiteImportsTest()).DoTest();
            //(new MultipleImportsTest()).DoTest();
            //(new AvoidingDiscoveryTest()).DoTest();
            //(new MetadataTest()).DoTest();
            //(new InheritanceTest()).DoTest();

            (new CreationPoliciesTest()).DoTest();
        }
    }
}
