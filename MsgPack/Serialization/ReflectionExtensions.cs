// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.ReflectionExtensions
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using MsgPack.Serialization.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace MsgPack.Serialization
{
  internal static class ReflectionExtensions
  {
    private static readonly Type[] ExceptionConstructorWithInnerParameterTypes = new Type[2]
    {
      typeof (string),
      typeof (Exception)
    };

    public static object InvokePreservingExceptionType(
      this ConstructorInfo source,
      params object[] parameters)
    {
      try
      {
        return source.Invoke(parameters);
      }
      catch (TargetInvocationException ex)
      {
        Exception exception = ReflectionExtensions.HoistUpInnerException(ex);
        if (exception != null)
          throw exception;
        throw;
      }
    }

    public static object InvokePreservingExceptionType(
      this MethodInfo source,
      object instance,
      params object[] parameters)
    {
      try
      {
        return source.Invoke(instance, parameters);
      }
      catch (TargetInvocationException ex)
      {
        Exception exception = ReflectionExtensions.HoistUpInnerException(ex);
        if (exception != null)
          throw exception;
        throw;
      }
    }

    public static T CreateInstancePreservingExceptionType<T>(
      Type instanceType,
      params object[] constructorParameters)
    {
      return (T) ReflectionExtensions.CreateInstancePreservingExceptionType(instanceType, constructorParameters);
    }

    public static object CreateInstancePreservingExceptionType(
      Type type,
      params object[] constructorParameters)
    {
      try
      {
        return Activator.CreateInstance(type, constructorParameters);
      }
      catch (TargetInvocationException ex)
      {
        Exception exception = ReflectionExtensions.HoistUpInnerException(ex);
        if (exception != null)
          throw exception;
        throw;
      }
    }

    private static Exception HoistUpInnerException(
      TargetInvocationException targetInvocationException)
    {
      if (targetInvocationException.InnerException == null)
        return (Exception) null;
      ConstructorInfo constructor = targetInvocationException.InnerException.GetType().GetConstructor(ReflectionExtensions.ExceptionConstructorWithInnerParameterTypes);
      if (constructor == (ConstructorInfo) null)
        return (Exception) null;
      try
      {
        return constructor.Invoke(new object[2]
        {
          (object) targetInvocationException.InnerException.Message,
          (object) targetInvocationException
        }) as Exception;
      }
      catch (Exception ex)
      {
        return (Exception) null;
      }
    }

    public static Type GetMemberValueType(this MemberInfo source)
    {
      if (source == (MemberInfo) null)
        throw new ArgumentNullException(nameof (source));
      PropertyInfo propertyInfo = source as PropertyInfo;
      FieldInfo fieldInfo = source as FieldInfo;
      if (propertyInfo == (PropertyInfo) null && fieldInfo == (FieldInfo) null)
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "'{0}'({1}) is not field nor property.", (object) source, (object) source.GetType()));
      return !(propertyInfo != (PropertyInfo) null) ? fieldInfo.FieldType : propertyInfo.PropertyType;
    }

    public static CollectionTraits GetCollectionTraits(this Type source)
    {
      if (!source.IsAssignableTo(typeof (IEnumerable)))
        return CollectionTraits.NotCollection;
      if (source.IsArray)
        return new CollectionTraits(CollectionDetailedKind.Array, (MethodInfo) null, (MethodInfo) null, (MethodInfo) null, source.GetElementType());
      MethodInfo method = source.GetMethod("GetEnumerator", ReflectionAbstractions.EmptyTypes);
      CollectionTraits result1;
      if (method != (MethodInfo) null && method.ReturnType.IsAssignableTo(typeof (IEnumerator)) && ReflectionExtensions.TryCreateCollectionTraitsForHasGetEnumeratorType(source, method, out result1))
        return result1;
      Type ienumerableT = (Type) null;
      Type icollectionT = (Type) null;
      Type isetT = (Type) null;
      Type ilistT = (Type) null;
      Type idictionaryT = (Type) null;
      Type ireadOnlyCollectionT = (Type) null;
      Type ireadOnlyListT = (Type) null;
      Type ireadOnlyDictionaryT = (Type) null;
      Type ienumerable = (Type) null;
      Type icollection = (Type) null;
      Type ilist = (Type) null;
      Type idictionary = (Type) null;
      Type[] typeArray1 = source.FindInterfaces(new TypeFilter(ReflectionExtensions.FilterCollectionType), (object) null);
      if (source.GetIsInterface() && ReflectionExtensions.FilterCollectionType(source, (object) null))
      {
        Type[] array = ((IEnumerable<Type>) typeArray1).ToArray<Type>();
        Type[] typeArray2 = new Type[array.Length + 1];
        typeArray2[0] = source;
        for (int index = 0; index < array.Length; ++index)
          typeArray2[index + 1] = array[index];
        typeArray1 = typeArray2;
      }
      foreach (Type type in typeArray1)
      {
        CollectionTraits result2;
        if (ReflectionExtensions.TryCreateGenericCollectionTraits(source, type, out result2))
          return result2;
        if (!ReflectionExtensions.DetermineCollectionInterfaces(type, ref idictionaryT, ref ireadOnlyDictionaryT, ref ilistT, ref ireadOnlyListT, ref isetT, ref icollectionT, ref ireadOnlyCollectionT, ref ienumerableT, ref idictionary, ref ilist, ref icollection, ref ienumerable))
          return CollectionTraits.Unserializable;
      }
      if (idictionaryT != (Type) null)
      {
        Type elementType = typeof (KeyValuePair<,>).MakeGenericType(idictionaryT.GetGenericArguments());
        return new CollectionTraits(CollectionDetailedKind.GenericDictionary, ReflectionExtensions.GetAddMethod(source, idictionaryT.GetGenericArguments()[0], idictionaryT.GetGenericArguments()[1]), ReflectionExtensions.GetCountGetterMethod(source, elementType), ReflectionExtensions.FindInterfaceMethod(source, typeof (IEnumerable<>).MakeGenericType(elementType), "GetEnumerator", ReflectionAbstractions.EmptyTypes), elementType);
      }
      if (ireadOnlyDictionaryT != (Type) null)
      {
        Type elementType = typeof (KeyValuePair<,>).MakeGenericType(ireadOnlyDictionaryT.GetGenericArguments());
        return new CollectionTraits(CollectionDetailedKind.GenericReadOnlyDictionary, (MethodInfo) null, ReflectionExtensions.GetCountGetterMethod(source, elementType), ReflectionExtensions.FindInterfaceMethod(source, typeof (IEnumerable<>).MakeGenericType(elementType), "GetEnumerator", ReflectionAbstractions.EmptyTypes), elementType);
      }
      if (ienumerableT != (Type) null)
      {
        Type genericArgument = ienumerableT.GetGenericArguments()[0];
        return new CollectionTraits(ilistT != (Type) null ? CollectionDetailedKind.GenericList : (isetT != (Type) null ? CollectionDetailedKind.GenericSet : (icollectionT != (Type) null ? CollectionDetailedKind.GenericCollection : (ireadOnlyListT != (Type) null ? CollectionDetailedKind.GenericReadOnlyList : (ireadOnlyCollectionT != (Type) null ? CollectionDetailedKind.GenericReadOnlyCollection : CollectionDetailedKind.GenericEnumerable)))), ReflectionExtensions.GetAddMethod(source, genericArgument), ReflectionExtensions.GetCountGetterMethod(source, genericArgument), ReflectionExtensions.FindInterfaceMethod(source, ienumerableT, "GetEnumerator", ReflectionAbstractions.EmptyTypes), genericArgument);
      }
      if (idictionary != (Type) null)
        return new CollectionTraits(CollectionDetailedKind.NonGenericDictionary, ReflectionExtensions.GetAddMethod(source, typeof (object), typeof (object)), ReflectionExtensions.GetCountGetterMethod(source, typeof (object)), ReflectionExtensions.FindInterfaceMethod(source, idictionary, "GetEnumerator", ReflectionAbstractions.EmptyTypes), typeof (object));
      if (ienumerable != (Type) null)
      {
        MethodInfo addMethod = ReflectionExtensions.GetAddMethod(source, typeof (object));
        if (addMethod != (MethodInfo) null)
          return new CollectionTraits(ilist != (Type) null ? CollectionDetailedKind.NonGenericList : (icollection != (Type) null ? CollectionDetailedKind.NonGenericCollection : CollectionDetailedKind.NonGenericEnumerable), addMethod, ReflectionExtensions.GetCountGetterMethod(source, typeof (object)), ReflectionExtensions.FindInterfaceMethod(source, ienumerable, "GetEnumerator", ReflectionAbstractions.EmptyTypes), typeof (object));
      }
      return CollectionTraits.NotCollection;
    }

    private static bool TryCreateCollectionTraitsForHasGetEnumeratorType(
      Type source,
      MethodInfo getEnumerator,
      out CollectionTraits result)
    {
      if (source.Implements(typeof (IDictionary<,>)) || source.Implements(typeof (IReadOnlyDictionary<,>)))
      {
        Type type = ((IEnumerable<Type>) getEnumerator.ReturnType.GetInterfaces()).FirstOrDefault<Type>((Func<Type, bool>) (@interface => @interface.GetIsGenericType() && @interface.GetGenericTypeDefinition() == typeof (IEnumerator<>)));
        if (type != (Type) null)
        {
          Type genericArgument = type.GetGenericArguments()[0];
          result = new CollectionTraits(source.Implements(typeof (IDictionary<,>)) ? CollectionDetailedKind.GenericDictionary : CollectionDetailedKind.GenericReadOnlyDictionary, ReflectionExtensions.GetAddMethod(source, genericArgument.GetGenericArguments()[0], genericArgument.GetGenericArguments()[1]), ReflectionExtensions.GetCountGetterMethod(source, genericArgument), getEnumerator, genericArgument);
          return true;
        }
      }
      if (source.IsAssignableTo(typeof (IDictionary)))
      {
        result = new CollectionTraits(CollectionDetailedKind.NonGenericDictionary, ReflectionExtensions.GetAddMethod(source, typeof (object), typeof (object)), ReflectionExtensions.GetCountGetterMethod(source, typeof (object)), getEnumerator, typeof (DictionaryEntry));
        return true;
      }
      Type type1 = ReflectionExtensions.IsIEnumeratorT(getEnumerator.ReturnType) ? getEnumerator.ReturnType : ((IEnumerable<Type>) getEnumerator.ReturnType.GetInterfaces()).FirstOrDefault<Type>(new Func<Type, bool>(ReflectionExtensions.IsIEnumeratorT));
      if (type1 != (Type) null)
      {
        Type genericArgument = type1.GetGenericArguments()[0];
        result = new CollectionTraits(source.Implements(typeof (IList<>)) ? CollectionDetailedKind.GenericList : (source.Implements(typeof (IReadOnlyList<>)) ? CollectionDetailedKind.GenericReadOnlyList : (source.Implements(typeof (ISet<>)) ? CollectionDetailedKind.GenericSet : (source.Implements(typeof (ICollection<>)) ? CollectionDetailedKind.GenericCollection : (source.Implements(typeof (IReadOnlyCollection<>)) ? CollectionDetailedKind.GenericReadOnlyCollection : CollectionDetailedKind.GenericEnumerable)))), ReflectionExtensions.GetAddMethod(source, genericArgument), ReflectionExtensions.GetCountGetterMethod(source, genericArgument), getEnumerator, genericArgument);
        return true;
      }
      result = (CollectionTraits) null;
      return false;
    }

    private static bool TryCreateGenericCollectionTraits(
      Type source,
      Type type,
      out CollectionTraits result)
    {
      if (type == typeof (IDictionary<MessagePackObject, MessagePackObject>) || type == typeof (IReadOnlyDictionary<MessagePackObject, MessagePackObject>))
      {
        result = new CollectionTraits(source == typeof (IDictionary<MessagePackObject, MessagePackObject>) || source.Implements(typeof (IDictionary<MessagePackObject, MessagePackObject>)) ? CollectionDetailedKind.GenericDictionary : CollectionDetailedKind.GenericReadOnlyDictionary, ReflectionExtensions.GetAddMethod(source, typeof (MessagePackObject), typeof (MessagePackObject)), ReflectionExtensions.GetCountGetterMethod(source, typeof (KeyValuePair<MessagePackObject, MessagePackObject>)), ReflectionExtensions.FindInterfaceMethod(source, typeof (IEnumerable<KeyValuePair<MessagePackObject, MessagePackObject>>), "GetEnumerator", ReflectionAbstractions.EmptyTypes), typeof (KeyValuePair<MessagePackObject, MessagePackObject>));
        return true;
      }
      if (type == typeof (IEnumerable<MessagePackObject>))
      {
        MethodInfo addMethod = ReflectionExtensions.GetAddMethod(source, typeof (MessagePackObject));
        if (addMethod != (MethodInfo) null)
        {
          result = new CollectionTraits(source == typeof (IList<MessagePackObject>) || source.Implements(typeof (IList<MessagePackObject>)) ? CollectionDetailedKind.GenericList : (source == typeof (IReadOnlyList<MessagePackObject>) || source.Implements(typeof (IReadOnlyList<MessagePackObject>)) ? CollectionDetailedKind.GenericReadOnlyList : (source == typeof (ISet<MessagePackObject>) || source.Implements(typeof (ISet<MessagePackObject>)) ? CollectionDetailedKind.GenericSet : (source == typeof (ICollection<MessagePackObject>) || source.Implements(typeof (ICollection<MessagePackObject>)) ? CollectionDetailedKind.GenericCollection : (source == typeof (IReadOnlyCollection<MessagePackObject>) || source.Implements(typeof (IReadOnlyCollection<MessagePackObject>)) ? CollectionDetailedKind.GenericReadOnlyCollection : CollectionDetailedKind.GenericEnumerable)))), addMethod, ReflectionExtensions.GetCountGetterMethod(source, typeof (MessagePackObject)), ReflectionExtensions.FindInterfaceMethod(source, typeof (IEnumerable<MessagePackObject>), "GetEnumerator", ReflectionAbstractions.EmptyTypes), typeof (MessagePackObject));
          return true;
        }
      }
      result = (CollectionTraits) null;
      return false;
    }

    private static bool DetermineCollectionInterfaces(
      Type type,
      ref Type idictionaryT,
      ref Type ireadOnlyDictionaryT,
      ref Type ilistT,
      ref Type ireadOnlyListT,
      ref Type isetT,
      ref Type icollectionT,
      ref Type ireadOnlyCollectionT,
      ref Type ienumerableT,
      ref Type idictionary,
      ref Type ilist,
      ref Type icollection,
      ref Type ienumerable)
    {
      if (type.GetIsGenericType())
      {
        Type genericTypeDefinition = type.GetGenericTypeDefinition();
        if (genericTypeDefinition == typeof (IDictionary<,>))
        {
          if (idictionaryT != (Type) null)
            return false;
          idictionaryT = type;
        }
        else if (genericTypeDefinition == typeof (IReadOnlyDictionary<,>))
        {
          if (ireadOnlyDictionaryT != (Type) null)
            return false;
          ireadOnlyDictionaryT = type;
        }
        else if (genericTypeDefinition == typeof (IList<>))
        {
          if (ilistT != (Type) null)
            return false;
          ilistT = type;
        }
        else if (genericTypeDefinition == typeof (IReadOnlyList<>))
        {
          if (ireadOnlyListT != (Type) null)
            return false;
          ireadOnlyListT = type;
        }
        else if (genericTypeDefinition == typeof (ISet<>))
        {
          if (isetT != (Type) null)
            return false;
          isetT = type;
        }
        else if (genericTypeDefinition == typeof (ICollection<>))
        {
          if (icollectionT != (Type) null)
            return false;
          icollectionT = type;
        }
        else if (genericTypeDefinition == typeof (IReadOnlyCollection<>))
        {
          if (ireadOnlyCollectionT != (Type) null)
            return false;
          ireadOnlyCollectionT = type;
        }
        else if (genericTypeDefinition == typeof (IEnumerable<>))
        {
          if (ienumerableT != (Type) null)
            return false;
          ienumerableT = type;
        }
      }
      else if (type == typeof (IDictionary))
        idictionary = type;
      else if (type == typeof (IList))
        ilist = type;
      else if (type == typeof (ICollection))
        icollection = type;
      else if (type == typeof (IEnumerable))
        ienumerable = type;
      return true;
    }

    private static MethodInfo FindInterfaceMethod(
      Type targetType,
      Type interfaceType,
      string name,
      Type[] parameterTypes)
    {
      if (targetType.GetIsInterface())
        return ((IEnumerable<Type>) targetType.FindInterfaces((TypeFilter) ((type, _) => type == interfaceType), (object) null)).Single<Type>().GetMethod(name, parameterTypes);
      InterfaceMapping interfaceMap = targetType.GetInterfaceMap(interfaceType);
      int index = Array.FindIndex<MethodInfo>(interfaceMap.InterfaceMethods, (Predicate<MethodInfo>) (method => method.Name == name && ((IEnumerable<ParameterInfo>) method.GetParameters()).Select<ParameterInfo, Type>((Func<ParameterInfo, Type>) (p => p.ParameterType)).SequenceEqual<Type>((IEnumerable<Type>) parameterTypes)));
      return index < 0 ? (MethodInfo) null : interfaceMap.TargetMethods[index];
    }

    private static MethodInfo GetAddMethod(Type targetType, Type argumentType)
    {
      Type[] types = new Type[1]{ argumentType };
      MethodInfo method = targetType.GetMethod("Add", types);
      if (method != (MethodInfo) null)
        return method;
      Type target = typeof (ICollection<>).MakeGenericType(argumentType);
      if (targetType.IsAssignableTo(target))
        return target.GetMethod("Add", types);
      if (!targetType.IsAssignableTo(typeof (IList)))
        return (MethodInfo) null;
      return typeof (IList).GetMethod("Add", new Type[1]
      {
        typeof (object)
      });
    }

    private static MethodInfo GetCountGetterMethod(Type targetType, Type elementType)
    {
      PropertyInfo property = targetType.GetProperty("Count");
      if (property != (PropertyInfo) null && property.GetHasPublicGetter())
        return property.GetGetMethod();
      Type target1 = typeof (ICollection<>).MakeGenericType(elementType);
      if (targetType.IsAssignableTo(target1))
        return target1.GetProperty("Count").GetGetMethod();
      Type target2 = typeof (IReadOnlyCollection<>).MakeGenericType(elementType);
      if (targetType.IsAssignableTo(target2))
        return target2.GetProperty("Count").GetGetMethod();
      return targetType.IsAssignableTo(typeof (ICollection)) ? typeof (ICollection).GetProperty("Count").GetGetMethod() : (MethodInfo) null;
    }

    private static MethodInfo GetAddMethod(Type targetType, Type keyType, Type valueType)
    {
      Type[] types = new Type[2]{ keyType, valueType };
      MethodInfo method = targetType.GetMethod("Add", types);
      return method != (MethodInfo) null ? method : typeof (IDictionary<,>).MakeGenericType(types).GetMethod("Add", types);
    }

    private static bool FilterCollectionType(Type type, object filterCriteria)
    {
      if (!type.Assembly.Equals((object) typeof (Array).Assembly))
        return false;
      return type.Namespace == "System.Collections" || type.Namespace == "System.Collections.Generic";
    }

    private static bool IsIEnumeratorT(Type @interface)
    {
      return @interface.GetIsGenericType() && @interface.GetGenericTypeDefinition() == typeof (IEnumerator<>);
    }

    public static bool GetHasPublicGetter(this MemberInfo source)
    {
      PropertyInfo propertyInfo;
      if ((propertyInfo = source as PropertyInfo) != (PropertyInfo) null)
        return propertyInfo.GetGetMethod() != (MethodInfo) null;
      FieldInfo fieldInfo;
      if ((fieldInfo = source as FieldInfo) != (FieldInfo) null)
        return fieldInfo.IsPublic;
      throw new NotSupportedException(source.GetType().ToString() + " is not supported.");
    }

    public static bool GetHasPublicSetter(this MemberInfo source)
    {
      PropertyInfo propertyInfo;
      if ((propertyInfo = source as PropertyInfo) != (PropertyInfo) null)
        return propertyInfo.GetSetMethod() != (MethodInfo) null;
      FieldInfo fieldInfo;
      if (!((fieldInfo = source as FieldInfo) != (FieldInfo) null))
        throw new NotSupportedException(source.GetType().ToString() + " is not supported.");
      return fieldInfo.IsPublic && !fieldInfo.IsInitOnly && !fieldInfo.IsLiteral;
    }

    public static bool GetIsPublic(this MemberInfo source)
    {
      PropertyInfo propertyInfo;
      if ((propertyInfo = source as PropertyInfo) != (PropertyInfo) null)
        return ((IEnumerable<MethodInfo>) propertyInfo.GetAccessors(true)).Where<MethodInfo>((Func<MethodInfo, bool>) (a => a.ReturnType != typeof (void))).All<MethodInfo>((Func<MethodInfo, bool>) (a => a.IsPublic));
      FieldInfo fieldInfo;
      if ((fieldInfo = source as FieldInfo) != (FieldInfo) null)
        return fieldInfo.IsPublic;
      MethodBase methodBase;
      if ((methodBase = source as MethodBase) != (MethodBase) null)
        return methodBase.IsPublic;
      Type type;
      if ((type = source as Type) != (Type) null)
        return type.IsPublic;
      throw new NotSupportedException(source.GetType().ToString() + " is not supported.");
    }

    public static bool GetIsVisible(this Type source)
    {
      return source.IsVisible;
    }
  }
}
