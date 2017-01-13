using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MEFTest.TestCase
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

    public class PartThree
    {
        [Import(RequiredCreationPolicy = CreationPolicy.Shared)]
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
        private CompositionContainer _container;

        public void DoTest()
        {
            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(CreationPoliciesTest).Assembly));
            _container = new CompositionContainer(catalog);
            var class1 = new PartTwo();
            var class2 = new PartThree();
            try
            {
                _container.ComposeParts(class1);
                _container.ComposeParts(class2);
            }
            catch (CompositionException compositionException)
            {
                Console.WriteLine(compositionException.ToString());
            }
        }
    }
}
