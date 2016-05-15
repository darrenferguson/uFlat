using log4net;
using Moriyama.UmbracoFlatten.Lib;
using System.Reflection;

namespace Moriyama.UmbracoFlatten
{
    class Program
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        static void Main(string[] args)
        {
            var flattener = new UmbracoContentFlattener("umbracoDbDsn");
            flattener.Flatten();



        }

    }
}
