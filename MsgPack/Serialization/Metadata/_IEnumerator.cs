// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.Metadata._IEnumerator
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace MsgPack.Serialization.Metadata
{
  internal static class _IEnumerator
  {
    private static readonly Type[] EmptyTypes = new Type[0];
    public static readonly MethodInfo MoveNext = FromExpression.ToMethod<IEnumerator, bool>((Expression<Func<IEnumerator, bool>>) (enumerator => enumerator.MoveNext()));
    public static readonly PropertyInfo Current = FromExpression.ToProperty<IEnumerator, object>((Expression<Func<IEnumerator, object>>) (enumerator => enumerator.Current));

    public static PropertyInfo FindEnumeratorCurrentProperty(
      Type enumeratorType,
      CollectionTraits traits)
    {
      PropertyInfo propertyInfo = traits.GetEnumeratorMethod.ReturnType.GetProperty("Current");
      if (propertyInfo == (PropertyInfo) null)
      {
        if (enumeratorType == typeof (IDictionaryEnumerator))
          propertyInfo = _IDictionaryEnumerator.Entry;
        else if (enumeratorType.GetIsInterface())
        {
          if (enumeratorType.GetIsGenericType() && enumeratorType.GetGenericTypeDefinition() == typeof (IEnumerator<>))
            propertyInfo = typeof (IEnumerator<>).MakeGenericType(traits.ElementType).GetProperty("Current");
          else
            propertyInfo = _IEnumerator.Current;
        }
      }
      return propertyInfo;
    }

    public static MethodInfo FindEnumeratorMoveNextMethod(Type enumeratorType)
    {
      MethodInfo methodInfo = enumeratorType.GetMethod("MoveNext", _IEnumerator.EmptyTypes);
      if (methodInfo == (MethodInfo) null)
        methodInfo = _IEnumerator.MoveNext;
      return methodInfo;
    }
  }
}
