using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvoidingDiscoveryTest
{
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
        public static void DoTest()
        {
            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(AvoidingDiscoveryTest).Assembly));
            using (var container = new CompositionContainer(catalog))
            {
                foreach (var composablePartDefinition in container.Catalog.Parts)
                {
                    //composablePartDefinition.GetType()
                    var name = composablePartDefinition.ExportDefinitions.ToList()[0].ContractName;
                    Console.WriteLine(name);
                }
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            AvoidingDiscoveryTest.DoTest();
            Console.ReadLine();
        }
    }
}
