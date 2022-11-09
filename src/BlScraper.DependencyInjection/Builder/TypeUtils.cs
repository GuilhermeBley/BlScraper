using System.Linq.Expressions;
using System.Reflection;

namespace BlScraper.DependencyInjection.Builder;

/// <summary>
/// Useful 
/// </summary>
internal static class TypeUtils
{
    
    /// <summary>
    /// Checks if type is assignable from a generic
    /// </summary>
    /// <param name="generic">Generic type</param>
    /// <param name="toCheck">Type to check</param>
    /// <returns>true : is assignable from generic type, false : don't is</returns>
    public static bool IsSubclassOfRawGeneric(Type generic, Type? toCheck)
    {
        return IsSubclassOfRawGeneric(generic, toCheck, out Type? assignableToGenericFound);
    }

    /// <summary>
    /// Checks if type is assignable from a generic
    /// </summary>
    /// <param name="generic">Generic type</param>
    /// <param name="toCheck">Type to check</param>
    /// <param name="assignableToGenericFound">Assingnable to found</param>
    /// <returns>true : is assignable from generic type, false : don't is</returns>
    public static bool IsSubclassOfRawGeneric(Type generic, Type? toCheck, out Type? assignableToGenericFound)
    {
        assignableToGenericFound = null;
        while (toCheck != null && toCheck != typeof(object))
        {
            var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
            if (generic == cur)
            {
                assignableToGenericFound = toCheck;
                return true;
            }
            toCheck = toCheck.BaseType!;
        }
        return false;
    }

    /// <summary>
    /// Map all classes which is a instance of generic type <paramref name="typeGenericToMap"/>
    /// </summary>
    /// <remarks>
    ///     <para>Map only non abstract and public class</para>
    /// </remarks>
    /// <param name="assembly">assembly types</param>
    /// <returns>List witch is assignable from <paramref name="typeGenericToMap"/></returns>
    /// <exception cref="ArgumentException"/>
    public static IEnumerable<Type> MapClassFromAssemblie(System.Reflection.Assembly assembly, Type typeGenericToMap)
    {
        if (!typeGenericToMap.IsGenericType)
            return Enumerable.Empty<Type>();

        List<Type> listTypes = new();

        foreach (Type type in assembly.GetTypes())
        {
            if (type.IsClass &&
                !type.IsAbstract &&
                type.IsPublic &&
                TypeUtils.IsSubclassOfRawGeneric(typeGenericToMap, type))
            {
                var normalizedName = type.Name.ToUpper();
                listTypes.Add(type);
            }
        }

        return listTypes;
    }

    /// <summary>
    /// Check in all of the assemblies objects which assign the <paramref name="typeTo"/>
    /// </summary>
    /// <param name="assemblies">assemblies to check</param>
    /// <param name="typeTo">type to check assignable from</param>
    /// <param name="onlyClass">Only classes is mapped, true to map only classes</param>
    /// <param name="nonAbstract">Abstract members isn't mapped, true to non map abstract</param>
    /// <returns>Type founded or null</returns>
    /// <exception cref="ArgumentException">Conflict</exception>
    public static Type? TryGetUniqueAssignableFrom(Assembly[] assemblies, Type typeTo, bool onlyClass = true, bool nonAbstract = true)
    {
        Type? finded = null;

        foreach (var assembly in assemblies)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (!type.IsClass && onlyClass)
                    continue;

                if (type.IsAbstract && nonAbstract)
                    continue;

                if (!typeTo.IsAssignableFrom(type))
                    continue;

                if (finded != null)
                    throw new ArgumentException($"Duplicate type of same assign in {finded.FullName} and {type.FullName}.");

                finded = type;
            }
        }

        return finded;
    }

    /// <summary>
    /// Create delegate by methodinfo in target
    /// </summary>
    /// <param name="method">method info</param>
    /// <param name="target">A instance of the object which contains the method where will be execute</param>
    /// <returns>delegate or null</returns>
    public static Delegate? CreateDelegateWithTarget(MethodInfo? method, object? target)
    {
        if (method is null ||
            target is null)
            return null;

        if (method.IsStatic)
            return null;

        if (method.IsGenericMethod)
            return null;

        return method.CreateDelegate(Expression.GetDelegateType(
            (from parameter in method.GetParameters() select parameter.ParameterType)
            .Concat(new[] { method.ReturnType })
            .ToArray()), target);
    }
}