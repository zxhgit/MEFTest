using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InheritanceTest
{
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

    class Program
    {
        static void Main(string[] args)
        {
        }
    }
}
