// Decompiled with JetBrains decompiler
// Type: MsgPack.TupleItems
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MsgPack
{
  internal static class TupleItems
  {
    public static List<Type> CreateTupleTypeList(IList<Type> itemTypes)
    {
      Stack<List<Type>> typeListStack = new Stack<List<Type>>(itemTypes.Count / 7 + 1);
      for (int index = 0; index < itemTypes.Count / 7; ++index)
        typeListStack.Push(itemTypes.Skip<Type>(index * 7).Take<Type>(7).ToList<Type>());
      if (itemTypes.Count % 7 != 0)
        typeListStack.Push(itemTypes.Skip<Type>(itemTypes.Count / 7 * 7).Take<Type>(itemTypes.Count % 7).ToList<Type>());
      List<Type> source = new List<Type>(typeListStack.Count);
      while (0 < typeListStack.Count)
      {
        List<Type> typeList = typeListStack.Pop();
        if (0 < source.Count)
          typeList.Add(source.Last<Type>());
        Type type = Type.GetType("System.Tuple`" + (object) typeList.Count, true).MakeGenericType(typeList.ToArray());
        source.Add(type);
      }
      source.Reverse();
      return source;
    }

    public static IList<Type> GetTupleItemTypes(Type tupleType)
    {
      Type[] genericArguments = tupleType.GetGenericArguments();
      List<Type> typeList = new List<Type>(tupleType.GetGenericArguments().Length);
      TupleItems.GetTupleItemTypes((IList<Type>) genericArguments, (IList<Type>) typeList);
      return (IList<Type>) typeList;
    }

    private static void GetTupleItemTypes(IList<Type> itemTypes, IList<Type> result)
    {
      int num = itemTypes.Count == 8 ? 7 : itemTypes.Count;
      for (int index = 0; index < num; ++index)
        result.Add(itemTypes[index]);
      if (itemTypes.Count != 8)
        return;
      TupleItems.GetTupleItemTypes((IList<Type>) itemTypes[7].GetGenericArguments(), result);
    }

    public static bool IsTuple(Type type)
    {
      Assembly assembly = type.GetAssembly();
      return (assembly.Equals((object) typeof (object).GetAssembly()) || assembly.Equals((object) typeof (Enumerable).GetAssembly())) && type.GetIsPublic() && type.Name.StartsWith("Tuple`", StringComparison.Ordinal);
    }
  }
}
