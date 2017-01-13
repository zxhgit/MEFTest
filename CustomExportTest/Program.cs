using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomExportTest
{
    public interface IMyAddin
    {
    }

    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class MyAttribute : ExportAttribute
    {
        public MyAttribute(string myMetadata) : base(typeof(IMyAddin))
        {
            MyMetadata = myMetadata;
        }

        public string MyMetadata { get; private set; }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// msdn中的例子有问题
    /// </remarks>
    [Export(typeof(IMyAddin)), ExportMetadata("MyMetadata", "theData")]
    public class MyAddin1 : IMyAddin
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// 与MyAddin1所用的Export、ExportMetadata两个标签等效
    /// </remarks>
    [MyAttribute("theData")]
    public class MyAddin2 : IMyAddin
    {
    }

    class Program
    {
        static void Main(string[] args)
        {
        }
    }
}
