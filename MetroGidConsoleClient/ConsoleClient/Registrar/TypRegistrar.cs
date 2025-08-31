using System.Collections.Concurrent;
using Spectre.Console.Cli;

namespace ConsoleClient.Registrar;

public sealed class TypeRegistrar : ITypeRegistrar
{
    private readonly ConcurrentDictionary<Type, Func<ITypeResolver, object>> _factories = new();

    public ITypeResolver Build() => new TypeResolver(_factories);

    public void Register<IService, IImplementation>() 
        where IImplementation : class, IService 
        where IService : class
    {
        Register(typeof(IService), typeof(IImplementation));
    }

    public void Register(Type service, Type implementation)
    {
        _factories[service] = resolver => CreateInstance(implementation, resolver);
    }

    public void RegisterInstance<IService>(IService instance) 
        where IService : class
    {
        RegisterInstance(typeof(IService), instance);
    }

    public void RegisterInstance(Type service, object instance)
    {
        _factories[service] = _ => instance;
    }

    public void RegisterLazy(Type service, Func<object> factory)
    {
        var lazy = new Lazy<object>(factory);
        _factories[service] = _ => lazy.Value;
    }

    private static object CreateInstance(Type type, ITypeResolver resolver)
    {
        var ctor = type.GetConstructors()[0];
        var parameters = ctor.GetParameters();
        var args = new object[parameters.Length];

        for (int i = 0; i < parameters.Length; i++)
        {
            var paramType = parameters[i].ParameterType;

            // Специальная обработка: IEnumerable<T>
            if (paramType.IsGenericType && paramType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                var serviceType = paramType.GetGenericArguments()[0];
                var implementations = GetAllRegisteredImplementations(serviceType, resolver);
                var array = Array.CreateInstance(serviceType, implementations.Count);
                for (int j = 0; j < implementations.Count; j++)
                    array.SetValue(implementations[j], j);

                args[i] = array;
                continue;
            }

            var service = resolver.Resolve(paramType);
            if (service == null)
                throw new InvalidOperationException($"Не удалось разрешить зависимость: {paramType}");

            args[i] = service;
        }

        return Activator.CreateInstance(type, args)!;
    }

    // Возвращает все зарегистрированные реализации типа T
    private static List<object> GetAllRegisteredImplementations(Type serviceType, ITypeResolver resolver)
    {
        var result = new List<object>();
        var queue = new Queue<Type>();
        queue.Enqueue(serviceType);

        // Мы не можем перечислить все зарегистрированные типы напрямую,
        // поэтому в реальном случае нужно хранить их в отдельной коллекции.
        // Но для Spectre.Console.Cli нам нужны только его внутренние IHelpProvider.

        // Пока добавим заглушку: если запрашивают IEnumerable<IHelpProvider> — вернём пустой массив
        if (serviceType.Name == "IHelpProvider" && serviceType.Namespace == "Spectre.Console.Cli.Help")
        {
            return new List<object>();
        }

        return result;
    }

    private sealed class TypeResolver : ITypeResolver
    {
        private readonly ConcurrentDictionary<Type, Func<ITypeResolver, object>> _factories;

        public TypeResolver(ConcurrentDictionary<Type, Func<ITypeResolver, object>> factories)
        {
            _factories = factories;
        }

        public object? Resolve(Type? type)
        {
            if (type == null) return null;
            if (!_factories.TryGetValue(type, out var factory)) return null;
            return factory(this);
        }
    }
}