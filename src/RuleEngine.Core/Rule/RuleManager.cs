using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using RuleEngine.Core.Models;

namespace RuleEngine.Core.Rule
{
    /// <summary>
    /// Kural setlerini provider bazında yönetir.
    /// </summary>
    public static class RuleManager
    {
        //Neden burada sadece Type'a göre tutuyoruz? çünkü providerWorker'lar instance'a göre değil, ruleSet'in tiplerine yani provider'ın tipine göre işlem yapar.
        private static readonly ConcurrentDictionary<Type, ProviderWorker> ProviderWorkers = new ConcurrentDictionary<Type, ProviderWorker>();
        
        private static ProviderWorker<TRuleSet, TInput, TOutput> GetProviderWorker<TRuleSet, TInput, TOutput>(IRuleProvider<TRuleSet, TInput, TOutput> provider)
            where TRuleSet : RuleSet<TInput, TOutput>
            where TInput : RuleInputModel
        {
            var providerWorker = ProviderWorkers.GetOrAdd(provider.GetType(),
                    p =>
                    {
                        var newProviderWorker = new ProviderWorker<TRuleSet, TInput, TOutput>(provider);
                        _ = newProviderWorker.ProcessAsync();
                        return newProviderWorker;
                    })
                as ProviderWorker<TRuleSet, TInput, TOutput>;

            if (providerWorker == null)
                throw new ArgumentException("Unsupported RuleSet type.", nameof(provider));

            providerWorker.WaitInitialization();
            return providerWorker;
        }

        /// <summary>
        /// Rule Provider'ın sağladığı kurallar içerisinde <paramref name="input"/> ile verilen bilgilere seçim kuralını çalıştırır ve uyanları çıktı verir.
        /// </summary>
        /// <typeparam name="TRuleSet"></typeparam>
        /// <typeparam name="TInput"></typeparam>
        /// <typeparam name="TOutput"></typeparam>
        /// <param name="provider"></param>
        /// <param name="input">Seçim kurallarına gönderilecek olan bilgiler.</param>
        /// <returns>Kural kodu ve <typeparamref name="TRuleSet"/> den oluşan dictionary</returns>
        public static IDictionary<string, TRuleSet> PredicateRuleSets<TRuleSet, TInput, TOutput>(
            this IRuleProvider<TRuleSet, TInput, TOutput> provider, TInput input)
            where TRuleSet : RuleSet<TInput, TOutput>
            where TInput : RuleInputModel
        {
            var providerWorker = GetProviderWorker(provider);

            //todo: sayı kontrol edilip eğer belli bir limitin yukarısında ise parallel foreach kullan.
            return providerWorker.RuleSets.Where(rs => rs.Value.Predicate(input))
                .ToDictionary(kv => kv.Key, kv => kv.Value);
        }

        /// <summary>
        /// Rule Provider'ın sağladığı tüm kural setlerini döner.
        /// </summary>
        /// <typeparam name="TRuleSet"></typeparam>
        /// <typeparam name="TInput"></typeparam>
        /// <typeparam name="TOutput"></typeparam>
        /// <param name="provider"></param>
        /// <returns>Kural kodu ve <typeparamref name="TRuleSet"/> den oluşan dictionary</returns>
        public static IDictionary<string, TRuleSet> GetRuleSets<TRuleSet, TInput, TOutput>(
            this IRuleProvider<TRuleSet, TInput, TOutput> provider)
            where TRuleSet : RuleSet<TInput, TOutput>
            where TInput : RuleInputModel
        {
            var providerWorker = GetProviderWorker(provider);

            return providerWorker.RuleSets;
        }

        /// <summary>
        /// Rule Provider'ın sağladığı kurallar içerisinde <paramref name="input"/> ile verilen bilgilere seçim kuralını çalıştırır ve uyanları çıktı verir.
        /// </summary>
        /// <typeparam name="TRuleSet"></typeparam>
        /// <typeparam name="TInput"></typeparam>
        /// <typeparam name="TOutput"></typeparam>
        /// <param name="provider"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static IEnumerable<TOutput> Execute<TRuleSet, TInput, TOutput>(
            IRuleProvider<TRuleSet, TInput, TOutput> provider, TInput input)
            where TRuleSet : RuleSet<TInput, TOutput>
            where TInput : RuleInputModel
        {
            var providerWorker = GetProviderWorker(provider);
            return providerWorker.Execute(input);
        }

        /// <summary>
        /// Rule Provider'ın sağladığı kurallar içerisinde <paramref name="input"/> ile verilen bilgilere seçim kuralını çalıştırır ve uyanları çıktı verir.
        /// </summary>
        /// <typeparam name="TRuleSet"></typeparam>
        /// <typeparam name="TInput"></typeparam>
        /// <typeparam name="TOutput"></typeparam>
        /// <param name="provider"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<TOutput>> ExecuteAsync<TRuleSet, TInput, TOutput>(
            IRuleProvider<TRuleSet, TInput, TOutput> provider, TInput input)
            where TRuleSet : RuleSet<TInput, TOutput>
            where TInput : RuleInputModel
        {
            var providerWorker = GetProviderWorker(provider);
            return await providerWorker.ExecuteAsync(input);
        }

        /// <summary>
        /// Rule Provider'ın sağladığı kurallar içerisinde <paramref name="input"/> ile verilen bilgilere seçim kuralını çalıştırır ve uyanları çıktı verir.
        /// </summary>
        /// <typeparam name="TRuleSet"></typeparam>
        /// <typeparam name="TInput"></typeparam>
        /// <typeparam name="TOutput"></typeparam>
        /// <param name="provider"></param>
        /// <param name="input"></param>
        /// <param name="availableResults"></param>
        /// <returns></returns>
        public static IEnumerable<TOutput> Execute<TRuleSet, TInput, TOutput>(
            IRuleProvider<TRuleSet, TInput, TOutput> provider, TInput input, IEnumerable<TOutput> availableResults)
            where TRuleSet : MultiResultRuleSet<TInput, TOutput>
            where TInput : RuleInputModel
        {
            var providerWorker = GetProviderWorker(provider);
            return providerWorker.Execute(input, availableResults);
        }

        /// <summary>
        /// Rule Provider'ın sağladığı kurallar içerisinde <paramref name="input"/> ile verilen bilgilere seçim kuralını çalıştırır ve uyanları çıktı verir.
        /// </summary>
        /// <typeparam name="TRuleSet"></typeparam>
        /// <typeparam name="TInput"></typeparam>
        /// <typeparam name="TOutput"></typeparam>
        /// <param name="provider"></param>
        /// <param name="input"></param>
        /// <param name="availableResults"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<TOutput>> ExecuteAsync<TRuleSet, TInput, TOutput>(
            IRuleProvider<TRuleSet, TInput, TOutput> provider, TInput input, IEnumerable<TOutput> availableResults)
            where TRuleSet : MultiResultRuleSet<TInput, TOutput>
            where TInput : RuleInputModel
        {
            var providerWorker = GetProviderWorker(provider);
            return await providerWorker.ExecuteAsync(input, availableResults);
        }

        private static readonly MethodInfo _genericGetProviderWorkerMethod = typeof(RuleManager).GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
            .First(m => m.Name == "GetProviderWorker" && m.IsGenericMethod);
        private static readonly ConcurrentDictionary<Type[], MethodInfo> _getProviderWorkerMethods = new ConcurrentDictionary<Type[], MethodInfo>();
        
        private static ProviderWorker GetProviderWorker(IRuleProvider ruleProvider)
        {
            var genericTypes = ruleProvider.GetType().GetInterfaces().First(i => i.IsGenericType).GetGenericArguments();
            var method = _getProviderWorkerMethods.GetOrAdd(genericTypes, types =>
            {
                return _genericGetProviderWorkerMethod.MakeGenericMethod(types);
            });
            return method.Invoke(null, new[] { ruleProvider }) as ProviderWorker ?? throw new InvalidOperationException("Failed to get provider worker");
        }

        public static void WaitInitialization(this IRuleProvider ruleProvider)
        {
            GetProviderWorker(ruleProvider);
        }

        public static void WaitInitialization<TRuleSet, TInput, TOutput>(this IRuleProvider<TRuleSet, TInput, TOutput> provider)
            where TRuleSet : RuleSet<TInput, TOutput>
            where TInput : RuleInputModel
        {
            GetProviderWorker(provider);
        }

        private static readonly CancellationTokenSource CancelToken = new CancellationTokenSource();

        private static readonly Thread BackgroundThread = new Thread(Worker)
        {
            IsBackground = true,
            Name = "RuleManager"
        };

        static RuleManager()
        {
            BackgroundThread.Start();
        }

        private static void Worker()
        {
            //async metodları kullanabilmek için oluşturduk.
            Task.Run(async () =>
                {
                    while (!CancelToken.IsCancellationRequested)
                    {
                        await Task.WhenAll(ProviderWorkers.Values.Select(pw => pw.ProcessAsync()));
                        await Task.Delay(TimeSpan.FromSeconds(30), CancelToken.Token);
                    }
                }, CancelToken.Token)
                .Wait(CancelToken.Token);
        }
    }
}
