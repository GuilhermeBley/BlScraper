using System.Linq.Expressions;
using System.Reflection;

namespace BlScraper.DependencyInjection.Builder.Internal;

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
            if (IsTypeValidQuest(type))
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
    /// <param name="nonObsolete">If true, Don't map type which contains <see cref="ObsoleteAttribute"/></param>
    /// <returns>Type founded or null</returns>
    /// <exception cref="ArgumentException">Conflict</exception>
    public static Type? GetUniqueAssignableFrom(Assembly[] assemblies, Type typeTo, bool onlyClass = true, bool nonAbstract = true, bool nonObsolete = true)
    {
        Type? found = null;

        foreach (var type in GetAssignableFrom(assemblies, typeTo, onlyClass, nonAbstract, nonObsolete))
        {
            if (found is not null)
                throw new ArgumentException($"Duplicate type of same assign in {found.FullName} and {type.FullName}.", typeTo.Name);

            found = type;
        }

        return found;
    }

    
    /// <summary>
    /// Check in all of the assemblies objects which assign the <paramref name="typeTo"/>
    /// </summary>
    /// <param name="assemblies">assemblies to check</param>
    /// <param name="typeTo">type to check assignable from</param>
    /// <param name="onlyClass">Only classes is mapped, true to map only classes</param>
    /// <param name="nonAbstract">Abstract members isn't mapped, true to non map abstract</param>
    /// <param name="nonObsolete">If true, Don't map type which contains <see cref="ObsoleteAttribute"/></param>
    /// <returns>Type founded or null</returns>
    /// <exception cref="ArgumentException">Conflict</exception>
    public static IEnumerable<Type> GetAssignableFrom(Assembly[] assemblies, Type typeTo, bool onlyClass = true, bool nonAbstract = true, bool nonObsolete = true)
    {
        foreach (var assembly in assemblies)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (onlyClass && !type.IsClass)
                    continue;

                if (nonAbstract && type.IsAbstract)
                    continue;

                if (nonObsolete && IsObsolete(type))
                    continue;

                if (!typeTo.IsAssignableFrom(type))
                    continue;

                yield return type;
            }
        }
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

    /// <summary>
    /// Checks if type contains attribute obsolete
    /// </summary>
    /// <param name="type">Type to check</param>
    /// <param name="checkBase">If true, Checks all of the base type</param>
    /// <returns>true : obsolete attribute was find</returns>
    public static bool IsObsolete(Type? type, bool checkBase = true)
    {
        if (type is null)
            return false;

        do
        {
            if (type.GetCustomAttribute<ObsoleteAttribute>() is not null)
                return true;

            type = type.BaseType;
        } while (checkBase && type is not null);

        return false;
    }

    /// <summary>
    /// Checks if method contains attribute obsolete
    /// </summary>
    /// <param name="methodInfo">Method to check</param>
    /// <param name="checkBase">If true, Checks all of the base methods</param>
    /// <returns>true : obsolete attribute was find</returns>
    public static bool IsObsolete(MethodInfo? methodInfo, bool checkBase = true)
    {
        if (methodInfo is null)
            return false;

        do
        {
            if (methodInfo.GetCustomAttribute<ObsoleteAttribute>() is not null)
                return true;

            methodInfo = methodInfo.GetRuntimeBaseDefinition();
        } while (checkBase && methodInfo is not null);

        return false;
    }

    /// <summary>
    /// Check if <paramref name="type"/> is a valid quest to map
    /// </summary>
    /// <param name="type">type to check</param>
    /// <returns>true : is valid, false : isn't valid</returns>
    public static bool IsTypeValidQuest(Type questType)
    {
        if (!TypeUtils.IsSubclassOfRawGeneric(typeof(BlScraper.Model.Quest<>), questType)
            || questType.IsAbstract
            || !questType.IsClass
            || !questType.IsPublic)
            return false;
        return true;
    }

    /// <summary>
    /// Check all parameters of constructor and remove unused
    /// </summary>
    /// <param name="constructorInfo">Constructor to check</param>
    /// <param name="args">parameters of constructor to check</param>
    /// <returns>Parsed parameters, with all unused removed.</returns>
    public static object?[]? TryParseConstructorParameters(ConstructorInfo constructorInfo, params object[] args)
    {
        List<object?> newArgs = new();
        List<object?> oldArgs = new(args);

        foreach (var parameter in constructorInfo.GetParameters())
        {
            bool found = false;
            foreach (var arg in oldArgs)
            {
                if (arg is null)
                    continue;
                
                if (parameter.ParameterType.Equals(arg.GetType()) || 
                    parameter.ParameterType.IsAssignableFrom(arg.GetType()))
                {
                    found = true;
                    newArgs.Add(arg);
                    oldArgs.Remove(arg);
                    break;
                }
            }

            if (!found && parameter.IsOptional)
            {
                found = true;
                newArgs.Add(parameter.DefaultValue);
            }

            if (!found)
                return null;
        }

        return newArgs.ToArray();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="genericType"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    /// <inheritdoc cref="System.Type.MakeGenericType(Type[])" path="/exception"/>
    public static Type SetGenericParameters(Type genericType, params Type[] parameters)
    {
        if (!genericType.ContainsGenericParameters)
            throw new ArgumentException($"'{genericType.FullName}' don't have generic types.");

        return genericType.MakeGenericType(parameters);
    }
}