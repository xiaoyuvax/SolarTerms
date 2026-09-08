using Py2Cs;
using SolarTerms24Net;

namespace SolarTermsCmd
{
    class Program
    {
        static void Main(string[] args)
        {
            // 等价于 Python 的 years = init("input.ini")
            List<string> years = ["2001"];

            string outPath = Path.Combine(Util.ProjectRoot, "output", "output.txt");
            Directory.CreateDirectory(Path.GetDirectoryName(outPath));
            using (StreamWriter f = new(outPath))
            {
                foreach (var y in years)
                {
                                        
                    f.WriteLine(SolarTerms.PaiYue(y));
                }
            }
            Console.WriteLine("==============finished!============");
        }
    }
}