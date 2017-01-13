using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreationPoliciesTest
{
    [Export]
    public class PartOne
    {
        //The default creation policy for an export is Any. 
    }

    public class PartTwo
    {
        [Import]
        public PartOne partOne { get; set; }

        //The default creation policy for an import is Any. 
        //If both policies are Any, the part will be shared. 
    }

    public class PartTwo1
    {
        [Import]
        public PartOne partOne { get; set; }

        //The default creation policy for an import is Any. 
        //If both policies are Any, the part will be shared. 
    }

    public class PartThree
    {
        //[Import]
        //[Import(RequiredCreationPolicy = CreationPolicy.Shared)]
        [Import(RequiredCreationPolicy = CreationPolicy.NonShared)]
        public PartOne partOne { get; set; }

        //The Shared creation policy is explicitly specified. 
        //PartTwo and PartThree will receive references to the 
        //SAME copy of PartOne. 
    }

    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class PartFour
    {
        //The NonShared creation policy is explicitly specified.
    }

    public class PartFive
    {
        [Import]
        public PartFour partFour { get; set; }

        //The default creation policy for an import is Any. 
        //Since the export's creation policy was explictly 
        //defined, the creation policy for this property will 
        //be non-shared. 
    }

    public class PartSix
    {
        [Import(RequiredCreationPolicy = CreationPolicy.NonShared)]
        public PartFour partFour { get; set; }

        //Both import and export specify matching creation 
        //policies.  PartFive and PartSix will each receive 
        //SEPARATE copies of PartFour, each with its own 
        //internal state. 
    }

    public class PartSeven
    {
        [Import(RequiredCreationPolicy = CreationPolicy.Shared)]
        public PartFour partFour { get; set; }

        //A creation policy mismatch.  Because there is no 
        //exported PartFour with a creation policy of Shared, 
        //this import does not match anything and will not be 
        //filled. 
    }

    public class CreationPoliciesTest
    {
        public static void DoTest()
        {
            var class1 = new PartTwo();
            var class1a = new PartTwo1();
            var class2 = new PartThree();
            ComposeObjects(class1, class1a, class2);
            var code1 = class1.partOne.GetHashCode();
            var code1a = class1a.partOne.GetHashCode();
            var code2 = class2.partOne.GetHashCode();
            Console.WriteLine(code1);
            Console.WriteLine(code1a);
            Console.WriteLine(code2);

            var five1 = new PartFive();
            var five2 = new PartFive();
            ComposeObjects(five1, five2);
            var codef1 = five1.partFour.GetHashCode();
            var codef2 = five2.partFour.GetHashCode();
            Console.WriteLine("codef1:" + codef1);
            Console.WriteLine("codef2:" + codef2);

            var six = new PartSix();
            var seven = new PartSeven();
            ComposeObjects(six);
            ComposeObjects(seven);//会抛异常，未找到与约束匹配的导出
        }

        private static void ComposeObjects(params object[] objects)
        {
            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(CreationPoliciesTest).Assembly));
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
            CreationPoliciesTest.DoTest();
            Console.ReadLine();
        }
    }
}
