using System;
using System.Threading.Tasks;

namespace Multiyield
{
    class Program
    {
        static async Task Main(string[] args)
        {            
            foreach(var a in Test())
            {
                Console.WriteLine(a);
            }
        }

        static async IConcatEnumerable<string> Test()
        {
            for (var i = 1; i < 5; i++)
            {
                await Yield.Single($"single{i}");
            }

            await Yield.Multiple(new[] { "multiple1", "multiple2", "multiple3" });
            return default;        
        }
    }
}
