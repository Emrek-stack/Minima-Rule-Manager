using System;
using System.Threading.Tasks;
using RuleEngine;

namespace RuleEngine
{
    /// <summary>
    /// SQLite olmadan RuleManager kullanım örneği - Console uygulaması
    /// </summary>
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                await SimpleRuleManagerExample.RunExample();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Hata: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }

            Console.WriteLine("\n👆 Yukarıdaki örnek SQLite olmadan RuleManager'ın nasıl kullanılabileceğini gösteriyor.");
            Console.WriteLine("📝 RuleManager sadece in-memory rule'lar ile çalışabilir, veritabanı gerekmez!");
            
            Console.WriteLine("\n🔧 Kullanım alanları:");
            Console.WriteLine("  - Basit business rule'lar");
            Console.WriteLine("  - Validation logic'leri");
            Console.WriteLine("  - Conditional logic'ler");
            Console.WriteLine("  - Test senaryoları");
            
            Console.WriteLine("\n💡 Avantajları:");
            Console.WriteLine("  - Hızlı ve hafif");
            Console.WriteLine("  - Veritabanı bağımlılığı yok");
            Console.WriteLine("  - Kolay test edilebilir");
            Console.WriteLine("  - Roslyn ile C# kodu derlenir");
            
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
