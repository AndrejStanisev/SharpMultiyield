using System;
using System.Threading.Tasks;

namespace Multiyield
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var en = Test();

            foreach(var a in en)
            {
                Console.WriteLine(a);
            }

            foreach (var a in en)
            {
                Console.WriteLine(a);
            }
        }

        static async IConcatEnumerable<string> Test()
        {
            for (var i = 0; i < 5; i++)
            {
                await Yield.OneOf(i.ToString());
            }

            await Yield.OneOf("end");

            return "";        
        }
    }
}
