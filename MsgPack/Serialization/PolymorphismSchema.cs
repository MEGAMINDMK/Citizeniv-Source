// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.PolymorphismSchema
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using MsgPack.Serialization.Polymorphic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace MsgPack.Serialization
{
  [DebuggerDisplay("{DebugString}")]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class PolymorphismSchema
  {
    private static readonly Dictionary<string, Type> EmptyMap = new Dictionary<string, Type>(0);
    private static readonly PolymorphismSchema[] EmptyChildren = new PolymorphismSchema[0];
    internal static readonly PolymorphismSchema Default = new PolymorphismSchema();
    internal static readonly MethodInfo ForPolymorphicObjectTypeEmbeddingMethod = typeof (PolymorphismSchema).GetMethod("ForPolymorphicObject", new Type[1]
    {
      typeof (Type)
    });
    internal static readonly MethodInfo ForPolymorphicObjectCodeTypeMappingMethod = typeof (PolymorphismSchema).GetMethod("ForPolymorphicObject", new Type[2]
    {
      typeof (Type),
      typeof (IDictionary<string, Type>)
    });
    internal static readonly MethodInfo ForContextSpecifiedCollectionMethod = typeof (PolymorphismSchema).GetMethod("ForContextSpecifiedCollection", new Type[2]
    {
      typeof (Type),
      typeof (PolymorphismSchema)
    });
    internal static readonly MethodInfo ForPolymorphicCollectionTypeEmbeddingMethod = typeof (PolymorphismSchema).GetMethod("ForPolymorphicCollection", new Type[2]
    {
      typeof (Type),
      typeof (PolymorphismSchema)
    });
    internal static readonly MethodInfo ForPolymorphicCollectionCodeTypeMappingMethod = typeof (PolymorphismSchema).GetMethod("ForPolymorphicCollection", new Type[3]
    {
      typeof (Type),
      typeof (IDictionary<string, Type>),
      typeof (PolymorphismSchema)
    });
    internal static readonly MethodInfo ForContextSpecifiedDictionaryMethod = typeof (PolymorphismSchema).GetMethod("ForContextSpecifiedDictionary", new Type[3]
    {
      typeof (Type),
      typeof (PolymorphismSchema),
      typeof (PolymorphismSchema)
    });
    internal static readonly MethodInfo ForPolymorphicDictionaryTypeEmbeddingMethod = typeof (PolymorphismSchema).GetMethod("ForPolymorphicDictionary", new Type[3]
    {
      typeof (Type),
      typeof (PolymorphismSchema),
      typeof (PolymorphismSchema)
    });
    internal static readonly MethodInfo ForPolymorphicDictionaryCodeTypeMappingMethod = typeof (PolymorphismSchema).GetMethod("ForPolymorphicDictionary", new Type[4]
    {
      typeof (Type),
      typeof (IDictionary<string, Type>),
      typeof (PolymorphismSchema),
      typeof (PolymorphismSchema)
    });
    internal static readonly MethodInfo ForPolymorphicTupleMethod = typeof (PolymorphismSchema).GetMethod("ForPolymorphicTuple", new Type[2]
    {
      typeof (Type),
      typeof (PolymorphismSchema[])
    });
    internal static readonly ConstructorInfo CodeTypeMapConstructor = typeof (Dictionary<,>).MakeGenericType(typeof (string), typeof (Type)).GetConstructor(new Type[1]
    {
      typeof (int)
    });
    internal static readonly MethodInfo AddToCodeTypeMapMethod = typeof (Dictionary<,>).MakeGenericType(typeof (string), typeof (Type)).GetMethod("Add", new Type[2]
    {
      typeof (string),
      typeof (Type)
    });
    private readonly ReadOnlyDictionary<string, Type> _codeTypeMapping;
    private readonly ReadOnlyCollection<PolymorphismSchema> _children;

    internal Type TargetType { get; private set; }

    internal PolymorphismType PolymorphismType { get; private set; }

    internal IDictionary<string, Type> CodeTypeMapping
    {
      get
      {
        return (IDictionary<string, Type>) this._codeTypeMapping;
      }
    }

    internal bool UseDefault
    {
      get
      {
        return this.PolymorphismType == PolymorphismType.None;
      }
    }

    internal bool UseTypeEmbedding
    {
      get
      {
        return this.PolymorphismType == PolymorphismType.RuntimeType;
      }
    }

    internal PolymorphismSchemaChildrenType ChildrenType { get; private set; }

    internal IList<PolymorphismSchema> ChildSchemaList
    {
      get
      {
        return (IList<PolymorphismSchema>) this._children;
      }
    }

    internal PolymorphismSchema ItemSchema
    {
      get
      {
        switch (this.ChildrenType)
        {
          case PolymorphismSchemaChildrenType.None:
            return (PolymorphismSchema) null;
          case PolymorphismSchemaChildrenType.CollectionItems:
            return this._children.FirstOrDefault<PolymorphismSchema>();
          case PolymorphismSchemaChildrenType.DictionaryKeyValues:
            return this._children.Skip<PolymorphismSchema>(1).FirstOrDefault<PolymorphismSchema>();
          default:
            throw new NotSupportedException();
        }
      }
    }

    internal PolymorphismSchema KeySchema
    {
      get
      {
        switch (this.ChildrenType)
        {
          case PolymorphismSchemaChildrenType.None:
            return (PolymorphismSchema) null;
          case PolymorphismSchemaChildrenType.DictionaryKeyValues:
            return this._children.FirstOrDefault<PolymorphismSchema>();
          default:
            throw new NotSupportedException();
        }
      }
    }

    private PolymorphismSchema()
    {
      this.TargetType = (Type) null;
      this.PolymorphismType = PolymorphismType.None;
      this.ChildrenType = PolymorphismSchemaChildrenType.None;
      this._codeTypeMapping = new ReadOnlyDictionary<string, Type>((IDictionary<string, Type>) PolymorphismSchema.EmptyMap);
      this._children = new ReadOnlyCollection<PolymorphismSchema>((IList<PolymorphismSchema>) PolymorphismSchema.EmptyChildren);
    }

    private PolymorphismSchema(
      Type targetType,
      PolymorphismType polymorphismType,
      PolymorphismSchemaChildrenType childrenType,
      params PolymorphismSchema[] childItemSchemaList)
      : this(targetType, polymorphismType, new ReadOnlyDictionary<string, Type>((IDictionary<string, Type>) PolymorphismSchema.EmptyMap), childrenType, new ReadOnlyCollection<PolymorphismSchema>((IList<PolymorphismSchema>) ((IEnumerable<PolymorphismSchema>) (childItemSchemaList ?? PolymorphismSchema.EmptyChildren)).Select<PolymorphismSchema, PolymorphismSchema>((Func<PolymorphismSchema, PolymorphismSchema>) (x => x ?? PolymorphismSchema.Default)).ToArray<PolymorphismSchema>()))
    {
    }

    private PolymorphismSchema(
      Type targetType,
      PolymorphismType polymorphismType,
      IDictionary<string, Type> codeTypeMapping,
      PolymorphismSchemaChildrenType childrenType,
      params PolymorphismSchema[] childItemSchemaList)
      : this(targetType, polymorphismType, new ReadOnlyDictionary<string, Type>(codeTypeMapping), childrenType, new ReadOnlyCollection<PolymorphismSchema>((IList<PolymorphismSchema>) ((IEnumerable<PolymorphismSchema>) (childItemSchemaList ?? PolymorphismSchema.EmptyChildren)).Select<PolymorphismSchema, PolymorphismSchema>((Func<PolymorphismSchema, PolymorphismSchema>) (x => x ?? PolymorphismSchema.Default)).ToArray<PolymorphismSchema>()))
    {
    }

    private PolymorphismSchema(
      Type targetType,
      PolymorphismType polymorphismType,
      ReadOnlyDictionary<string, Type> codeTypeMapping,
      PolymorphismSchemaChildrenType childrenType,
      ReadOnlyCollection<PolymorphismSchema> childItemSchemaList)
    {
      if (targetType == (Type) null)
        throw new ArgumentNullException(nameof (targetType));
      this.TargetType = targetType;
      this.PolymorphismType = polymorphismType;
      this._codeTypeMapping = codeTypeMapping;
      this.ChildrenType = childrenType;
      this._children = childItemSchemaList;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static PolymorphismSchema ForPolymorphicObject(Type targetType)
    {
      return new PolymorphismSchema(targetType, PolymorphismType.RuntimeType, PolymorphismSchemaChildrenType.None, new PolymorphismSchema[0]);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static PolymorphismSchema ForPolymorphicObject(
      Type targetType,
      IDictionary<string, Type> codeTypeMapping)
    {
      return new PolymorphismSchema(targetType, PolymorphismType.KnownTypes, codeTypeMapping, PolymorphismSchemaChildrenType.None, new PolymorphismSchema[0]);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static PolymorphismSchema ForContextSpecifiedCollection(
      Type targetType,
      PolymorphismSchema itemSchema)
    {
      return new PolymorphismSchema(targetType, PolymorphismType.None, PolymorphismSchemaChildrenType.CollectionItems, new PolymorphismSchema[1]
      {
        itemSchema
      });
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static PolymorphismSchema ForPolymorphicCollection(
      Type targetType,
      PolymorphismSchema itemSchema)
    {
      return new PolymorphismSchema(targetType, PolymorphismType.RuntimeType, PolymorphismSchemaChildrenType.CollectionItems, new PolymorphismSchema[1]
      {
        itemSchema
      });
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static PolymorphismSchema ForPolymorphicCollection(
      Type targetType,
      IDictionary<string, Type> codeTypeMapping,
      PolymorphismSchema itemSchema)
    {
      return new PolymorphismSchema(targetType, PolymorphismType.KnownTypes, codeTypeMapping, PolymorphismSchemaChildrenType.CollectionItems, new PolymorphismSchema[1]
      {
        itemSchema
      });
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static PolymorphismSchema ForContextSpecifiedDictionary(
      Type targetType,
      PolymorphismSchema keySchema,
      PolymorphismSchema valueSchema)
    {
      return new PolymorphismSchema(targetType, PolymorphismType.None, PolymorphismSchemaChildrenType.DictionaryKeyValues, new PolymorphismSchema[2]
      {
        keySchema,
        valueSchema
      });
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static PolymorphismSchema ForPolymorphicDictionary(
      Type targetType,
      PolymorphismSchema keySchema,
      PolymorphismSchema valueSchema)
    {
      return new PolymorphismSchema(targetType, PolymorphismType.RuntimeType, PolymorphismSchemaChildrenType.DictionaryKeyValues, new PolymorphismSchema[2]
      {
        keySchema,
        valueSchema
      });
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static PolymorphismSchema ForPolymorphicDictionary(
      Type targetType,
      IDictionary<string, Type> codeTypeMapping,
      PolymorphismSchema keySchema,
      PolymorphismSchema valueSchema)
    {
      return new PolymorphismSchema(targetType, PolymorphismType.KnownTypes, codeTypeMapping, PolymorphismSchemaChildrenType.DictionaryKeyValues, new PolymorphismSchema[2]
      {
        keySchema,
        valueSchema
      });
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static PolymorphismSchema ForPolymorphicTuple(
      Type targetType,
      PolymorphismSchema[] itemSchemaList)
    {
      PolymorphismSchema.VerifyArity(targetType, (ICollection<PolymorphismSchema>) itemSchemaList);
      return new PolymorphismSchema(targetType, PolymorphismType.None, PolymorphismSchemaChildrenType.TupleItems, itemSchemaList);
    }

    private static void VerifyArity(Type tupleType, ICollection<PolymorphismSchema> itemSchemaList)
    {
      if (itemSchemaList != null && itemSchemaList.Count != 0 && TupleItems.GetTupleItemTypes(tupleType).Count != itemSchemaList.Count)
        throw new ArgumentException("An arity of itemSchemaList does not match for an arity of the tuple.", nameof (itemSchemaList));
    }

    internal PolymorphismSchema FilterSelf()
    {
      return this == PolymorphismSchema.Default ? this : new PolymorphismSchema(this.TargetType, PolymorphismType.None, this._codeTypeMapping, this.ChildrenType, this._children);
    }

    internal string DebugString
    {
      get
      {
        StringBuilder buffer = new StringBuilder();
        this.ToDebugString(buffer);
        return buffer.ToString();
      }
    }

    private void ToDebugString(StringBuilder buffer)
    {
      buffer.Append("{TargetType:").Append((object) this.TargetType).Append(", SchemaType:").Append((object) this.PolymorphismType);
      switch (this.ChildrenType)
      {
        case PolymorphismSchemaChildrenType.CollectionItems:
          buffer.Append(", CollectionItemsSchema:");
          if (this.ItemSchema == null)
          {
            buffer.Append("null");
            break;
          }
          this.ItemSchema.ToDebugString(buffer);
          break;
        case PolymorphismSchemaChildrenType.DictionaryKeyValues:
          buffer.Append(", DictinoaryKeysSchema:");
          if (this.KeySchema == null)
            buffer.Append("null");
          else
            this.KeySchema.ToDebugString(buffer);
          buffer.Append(", DictinoaryValuesSchema:");
          if (this.ItemSchema == null)
          {
            buffer.Append("null");
            break;
          }
          this.ItemSchema.ToDebugString(buffer);
          break;
        case PolymorphismSchemaChildrenType.TupleItems:
          buffer.Append(", TupleItemsSchema:[");
          bool flag = true;
          using (IEnumerator<PolymorphismSchema> enumerator = this._children.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              PolymorphismSchema current = enumerator.Current;
              if (flag)
                flag = false;
              else
                buffer.Append(", ");
              if (current == null)
                buffer.Append("null");
              else
                current.ToDebugString(buffer);
            }
            break;
          }
      }
      buffer.Append('}');
    }

    internal static PolymorphismSchema Create(
      Type type,
      SerializingMember? memberMayBeNull)
    {
      if (type.GetIsValueType() || !memberMayBeNull.HasValue)
        return PolymorphismSchema.Default;
      SerializingMember serializingMember = memberMayBeNull.Value;
      PolymorphismSchema.TypeTable typeTable = PolymorphismSchema.TypeTable.Create(serializingMember.Member);
      CollectionTraits collectionTraits = serializingMember.Member.GetMemberValueType().GetCollectionTraits();
      switch (collectionTraits.CollectionType)
      {
        case CollectionKind.Array:
          if (!typeTable.Member.Exists && !typeTable.CollectionItem.Exists)
            return PolymorphismSchema.Default;
          return new PolymorphismSchema(serializingMember.Member.GetMemberValueType(), typeTable.Member.PolymorphismType, typeTable.Member.CodeTypeMapping, PolymorphismSchemaChildrenType.CollectionItems, new PolymorphismSchema[1]
          {
            new PolymorphismSchema(collectionTraits.ElementType, typeTable.CollectionItem.PolymorphismType, typeTable.CollectionItem.CodeTypeMapping, PolymorphismSchemaChildrenType.None, new PolymorphismSchema[0])
          });
        case CollectionKind.Map:
          if (!typeTable.Member.Exists && !typeTable.DictionaryKey.Exists && !typeTable.CollectionItem.Exists)
            return PolymorphismSchema.Default;
          return new PolymorphismSchema(serializingMember.Member.GetMemberValueType(), typeTable.Member.PolymorphismType, typeTable.Member.CodeTypeMapping, PolymorphismSchemaChildrenType.DictionaryKeyValues, new PolymorphismSchema[2]
          {
            new PolymorphismSchema(collectionTraits.ElementType.GetGenericArguments()[0], typeTable.DictionaryKey.PolymorphismType, typeTable.DictionaryKey.CodeTypeMapping, PolymorphismSchemaChildrenType.None, new PolymorphismSchema[0]),
            new PolymorphismSchema(collectionTraits.ElementType.GetGenericArguments()[1], typeTable.CollectionItem.PolymorphismType, typeTable.CollectionItem.CodeTypeMapping, PolymorphismSchemaChildrenType.None, new PolymorphismSchema[0])
          });
        default:
          if (TupleItems.IsTuple(serializingMember.Member.GetMemberValueType()))
          {
            if (typeTable.TupleItems.Count == 0)
              return PolymorphismSchema.Default;
            IList<Type> tupleItemTypes = TupleItems.GetTupleItemTypes(serializingMember.Member.GetMemberValueType());
            return new PolymorphismSchema(serializingMember.Member.GetMemberValueType(), PolymorphismType.None, (IDictionary<string, Type>) PolymorphismSchema.EmptyMap, PolymorphismSchemaChildrenType.TupleItems, typeTable.TupleItems.Zip((IEnumerable<Type>) tupleItemTypes, (e, t) => new
            {
              Entry = e,
              ItemType = t
            }).Select(e => new PolymorphismSchema(e.ItemType, e.Entry.PolymorphismType, e.Entry.CodeTypeMapping, PolymorphismSchemaChildrenType.None, new PolymorphismSchema[0])).ToArray<PolymorphismSchema>());
          }
          return !typeTable.Member.Exists ? PolymorphismSchema.Default : new PolymorphismSchema(serializingMember.Member.GetMemberValueType(), typeTable.Member.PolymorphismType, typeTable.Member.CodeTypeMapping, PolymorphismSchemaChildrenType.None, new PolymorphismSchema[0]);
      }
    }

    private struct TypeTable
    {
      public readonly PolymorphismSchema.TypeTableEntry Member;
      public readonly PolymorphismSchema.TypeTableEntry CollectionItem;
      public readonly PolymorphismSchema.TypeTableEntry DictionaryKey;
      public readonly IList<PolymorphismSchema.TypeTableEntry> TupleItems;

      private TypeTable(
        PolymorphismSchema.TypeTableEntry member,
        PolymorphismSchema.TypeTableEntry collectionItem,
        PolymorphismSchema.TypeTableEntry dictionaryKey,
        IList<PolymorphismSchema.TypeTableEntry> tupleItems)
      {
        this.Member = member;
        this.CollectionItem = collectionItem;
        this.DictionaryKey = dictionaryKey;
        this.TupleItems = tupleItems;
      }

      public static PolymorphismSchema.TypeTable Create(MemberInfo member)
      {
        return new PolymorphismSchema.TypeTable(PolymorphismSchema.TypeTableEntry.Create(member, PolymorphismTarget.Member), PolymorphismSchema.TypeTableEntry.Create(member, PolymorphismTarget.CollectionItem), PolymorphismSchema.TypeTableEntry.Create(member, PolymorphismTarget.DictionaryKey), (IList<PolymorphismSchema.TypeTableEntry>) PolymorphismSchema.TypeTableEntry.CreateTupleItems(member));
      }
    }

    private sealed class TypeTableEntry
    {
      private static readonly PolymorphismSchema.TypeTableEntry[] EmptyEntries = new PolymorphismSchema.TypeTableEntry[0];
      private readonly Dictionary<string, Type> _knownTypeMapping = new Dictionary<string, Type>();
      private bool _useTypeEmbedding;

      public IDictionary<string, Type> CodeTypeMapping
      {
        get
        {
          return (IDictionary<string, Type>) this._knownTypeMapping;
        }
      }

      public PolymorphismType PolymorphismType
      {
        get
        {
          if (this._useTypeEmbedding)
            return PolymorphismType.RuntimeType;
          return this._knownTypeMapping.Count <= 0 ? PolymorphismType.None : PolymorphismType.KnownTypes;
        }
      }

      public bool Exists
      {
        get
        {
          return this._useTypeEmbedding || this._knownTypeMapping.Count > 0;
        }
      }

      private TypeTableEntry()
      {
      }

      public static PolymorphismSchema.TypeTableEntry Create(
        MemberInfo member,
        PolymorphismTarget targetType)
      {
        PolymorphismSchema.TypeTableEntry typeTableEntry = new PolymorphismSchema.TypeTableEntry();
        foreach (IPolymorphicHelperAttribute attribute in member.GetCustomAttributes(false).OfType<IPolymorphicHelperAttribute>().Where<IPolymorphicHelperAttribute>((Func<IPolymorphicHelperAttribute, bool>) (a => a.Target == targetType)))
          typeTableEntry.Interpret(attribute, member.ToString(), -1);
        return typeTableEntry;
      }

      public static PolymorphismSchema.TypeTableEntry[] CreateTupleItems(
        MemberInfo member)
      {
        if (!TupleItems.IsTuple(member.GetMemberValueType()))
          return PolymorphismSchema.TypeTableEntry.EmptyEntries;
        PolymorphismSchema.TypeTableEntry[] array = Enumerable.Repeat<object>((object) null, TupleItems.GetTupleItemTypes(member.GetMemberValueType()).Count).Select<object, PolymorphismSchema.TypeTableEntry>((Func<object, PolymorphismSchema.TypeTableEntry>) (_ => new PolymorphismSchema.TypeTableEntry())).ToArray<PolymorphismSchema.TypeTableEntry>();
        foreach (IPolymorphicTupleItemTypeAttribute itemTypeAttribute in (IEnumerable<IPolymorphicTupleItemTypeAttribute>) member.GetCustomAttributes(false).OfType<IPolymorphicTupleItemTypeAttribute>().OrderBy<IPolymorphicTupleItemTypeAttribute, int>((Func<IPolymorphicTupleItemTypeAttribute, int>) (a => a.ItemNumber)))
          array[itemTypeAttribute.ItemNumber - 1].Interpret((IPolymorphicHelperAttribute) itemTypeAttribute, member.ToString(), itemTypeAttribute.ItemNumber);
        return array;
      }

      private void Interpret(
        IPolymorphicHelperAttribute attribute,
        string memberName,
        int tupleItemNumber)
      {
        if (attribute is IPolymorphicKnownTypeAttribute knownTypeAttribute)
        {
          if (this._useTypeEmbedding)
            throw new SerializationException(PolymorphismSchema.TypeTableEntry.GetCannotSpecifyKnownTypeAndRuntimeTypeErrorMessage(attribute, memberName, tupleItemNumber));
          string typeCode = knownTypeAttribute.TypeCode;
          try
          {
            this._knownTypeMapping.Add(typeCode, knownTypeAttribute.BindingType);
          }
          catch (ArgumentException ex)
          {
            throw new SerializationException(PolymorphismSchema.TypeTableEntry.GetCannotDuplicateKnownTypeCodeErrorMessage(attribute, typeCode, memberName, tupleItemNumber));
          }
        }
        else
        {
          if (this._useTypeEmbedding)
            throw new SerializationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot specify multiple '{0}' to #{1} item of tuple type member '{2}'.", (object) typeof (MessagePackRuntimeTupleItemTypeAttribute), (object) tupleItemNumber, (object) memberName));
          if (this._knownTypeMapping.Count > 0)
            throw new SerializationException(PolymorphismSchema.TypeTableEntry.GetCannotSpecifyKnownTypeAndRuntimeTypeErrorMessage(attribute, memberName, tupleItemNumber));
          this._useTypeEmbedding = true;
        }
      }

      private static string GetCannotSpecifyKnownTypeAndRuntimeTypeErrorMessage(
        IPolymorphicHelperAttribute attribute,
        string memberName,
        int tupleItemNumber)
      {
        switch (attribute.Target)
        {
          case PolymorphismTarget.CollectionItem:
            return string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot specify '{0}' and '{1}' simultaneously to collection items of member '{2}' itself.", (object) typeof (MessagePackRuntimeCollectionItemTypeAttribute), (object) typeof (MessagePackKnownCollectionItemTypeAttribute), (object) memberName);
          case PolymorphismTarget.DictionaryKey:
            return string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot specify '{0}' and '{1}' simultaneously to dictionary keys of member '{2}' itself.", (object) typeof (MessagePackRuntimeDictionaryKeyTypeAttribute), (object) typeof (MessagePackKnownDictionaryKeyTypeAttribute), (object) memberName);
          case PolymorphismTarget.TupleItem:
            return string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot specify '{0}' and '{1}' simultaneously to #{2} item of tuple type member '{3}' itself.", (object) typeof (MessagePackRuntimeTupleItemTypeAttribute), (object) typeof (MessagePackKnownTupleItemTypeAttribute), (object) tupleItemNumber, (object) memberName);
          default:
            return string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot specify '{0}' and '{1}' simultaneously to member '{2}' itself.", (object) typeof (MessagePackRuntimeTypeAttribute), (object) typeof (MessagePackKnownTypeAttribute), (object) memberName);
        }
      }

      private static string GetCannotDuplicateKnownTypeCodeErrorMessage(
        IPolymorphicHelperAttribute attribute,
        string typeCode,
        string memberName,
        int tupleItemNumber)
      {
        switch (attribute.Target)
        {
          case PolymorphismTarget.CollectionItem:
            return string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot specify multiple types for ext-type code '{0}' for collection items of member '{1}'.", (object) StringEscape.ForDisplay(typeCode), (object) memberName);
          case PolymorphismTarget.DictionaryKey:
            return string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot specify multiple types for ext-type code '{0}' for dictionary keys of member '{1}'.", (object) StringEscape.ForDisplay(typeCode), (object) memberName);
          case PolymorphismTarget.TupleItem:
            return string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot specify multiple types for ext-type code '{0}' for #{1} item of tuple type member '{2}'.", (object) StringEscape.ForDisplay(typeCode), (object) tupleItemNumber, (object) memberName);
          default:
            return string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot specify multiple types for ext-type code '{0}' for member '{1}'.", (object) StringEscape.ForDisplay(typeCode), (object) memberName);
        }
      }
    }
  }
}
