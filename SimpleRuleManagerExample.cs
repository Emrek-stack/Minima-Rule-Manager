using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RuleEngine.Core.Rule;
using RuleEngine.Core.Models;

namespace RuleEngine
{
    /// <summary>
    /// SQLite olmadan RuleManager kullanım örneği
    /// </summary>
    public class SimpleRuleManagerExample
    {
        public static async Task RunExample()
        {
            Console.WriteLine("=== RuleManager SQLite Olmadan Kullanım Örneği ===\n");

            // 1. RuleCompiler oluştur
            var compiler = new RuleCompiler<TestInput, bool>();

            // 2. Basit bir rule string'i tanımla
            string ruleString = "Input.Age > 18 && Input.HasLicense == true";

            // 3. Rule syntax'ını kontrol et
            var syntaxErrors = compiler.CheckSyntax(ruleString);
            if (syntaxErrors.Count > 0)
            {
                Console.WriteLine("❌ Syntax hataları:");
                foreach (var error in syntaxErrors)
                {
                    Console.WriteLine($"  - {error.Title}: {error.Description}");
                }
                return;
            }

            Console.WriteLine("✅ Rule syntax'ı geçerli!");

            // 4. Rule'ı derle
            try
            {
                var compiledRule = await compiler.CompileAsync("age-check-rule", ruleString);
                Console.WriteLine("✅ Rule başarıyla derlendi!");

                // 5. Test verileri oluştur
                var testCases = new[]
                {
                    new TestInput { Age = 20, HasLicense = true, Name = "Ahmet" },
                    new TestInput { Age = 17, HasLicense = true, Name = "Mehmet" },
                    new TestInput { Age = 25, HasLicense = false, Name = "Ayşe" },
                    new TestInput { Age = 30, HasLicense = true, Name = "Fatma" }
                };

                // 6. Rule'ları test et
                Console.WriteLine("\n🧪 Rule testleri:");
                foreach (var testCase in testCases)
                {
                    var result = compiledRule.Invoke(testCase);
                    var status = result ? "✅ GEÇER" : "❌ GEÇERSİZ";
                    Console.WriteLine($"  {testCase.Name} (Yaş: {testCase.Age}, Ehliyet: {testCase.HasLicense}) -> {status}");
                }

                // 7. RuleSet oluştur (daha gelişmiş kullanım)
                Console.WriteLine("\n🔧 RuleSet örneği:");
                var predicateRule = "Input.Age >= 21";
                var resultRule = "Output.CanDrink = true; Output.Message = \"İçki içebilir\";";
                
                var ruleSet = await RuleSet.CreateAsync<TestInput, TestOutput>("drinking-rule", predicateRule, resultRule, 1);
                Console.WriteLine("✅ RuleSet oluşturuldu!");

                // 8. RuleSet'i test et
                var testInput = new TestInput { Age = 22, HasLicense = true, Name = "Test" };
                var ruleSetResult = await ruleSet.ExecuteAsync(testInput);
                
                if (ruleSetResult.Success)
                {
                    Console.WriteLine($"✅ RuleSet sonucu: {ruleSetResult.Result}");
                }
                else
                {
                    Console.WriteLine($"❌ RuleSet hatası: {ruleSetResult.ErrorMessage}");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Rule derleme hatası: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Test input modeli
    /// </summary>
    public class TestInput : RuleInputModel
    {
        public int Age { get; set; }
        public bool HasLicense { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    /// <summary>
    /// Test output modeli
    /// </summary>
    public class TestOutput
    {
        public bool CanDrink { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}




