// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.SerializationTarget
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using MsgPack.Serialization.DefaultSerializers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace MsgPack.Serialization
{
  internal class SerializationTarget
  {
    public IList<SerializingMember> Members { get; private set; }

    public ConstructorInfo DeserializationConstructor { get; private set; }

    public bool IsConstructorDeserialization
    {
      get
      {
        return this.DeserializationConstructor != (ConstructorInfo) null && this.DeserializationConstructor.GetParameters().Length > 0;
      }
    }

    private SerializationTarget(IList<SerializingMember> members, ConstructorInfo constructor)
    {
      this.Members = members;
      this.DeserializationConstructor = constructor;
    }

    public string FindCorrespondingMemberName(ParameterInfo parameterInfo)
    {
      return this.Members.Where<SerializingMember>((Func<SerializingMember, bool>) (item => parameterInfo.Name.Equals(item.Contract.Name, StringComparison.OrdinalIgnoreCase) && item.Member.GetMemberValueType() == parameterInfo.ParameterType)).Select<SerializingMember, string>((Func<SerializingMember, string>) (item => item.Contract.Name)).FirstOrDefault<string>();
    }

    public static void VerifyType(Type targetType)
    {
      if (targetType.GetIsInterface() || targetType.GetIsAbstract())
        throw SerializationExceptions.NewNotSupportedBecauseCannotInstanciateAbstractType(targetType);
    }

    public static SerializationTarget Prepare(
      SerializationContext context,
      Type targetType)
    {
      SerializingMember[] array1 = SerializationTarget.GetTargetMembers(targetType).OrderBy<SerializingMember, int>((Func<SerializingMember, int>) (entry => entry.Contract.Id)).ToArray<SerializingMember>();
      if (array1.Length == 0)
        throw new SerializationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot serialize type '{0}' because it does not have any serializable fields nor properties.", (object) targetType));
      SerializingMember[] array2 = ((IEnumerable<SerializingMember>) array1).Where<SerializingMember>((Func<SerializingMember, bool>) (entry => SerializationTarget.CheckTargetEligibility(entry.Member))).ToArray<SerializingMember>();
      if (array2.Length == 0)
      {
        ConstructorInfo deserializationConstructor = SerializationTarget.FindDeserializationConstructor(targetType);
        return new SerializationTarget(SerializationTarget.ComplementMembers((IList<SerializingMember>) array1, context, targetType), deserializationConstructor);
      }
      ConstructorInfo constructor = targetType.GetConstructor(ReflectionAbstractions.EmptyTypes);
      if (constructor == (ConstructorInfo) null && !targetType.GetIsValueType())
        throw SerializationExceptions.NewTargetDoesNotHavePublicDefaultConstructor(targetType);
      return new SerializationTarget(!((IEnumerable<SerializingMember>) array2).All<SerializingMember>((Func<SerializingMember, bool>) (item => item.Contract.Id == -1)) ? SerializationTarget.ComplementMembers((IList<SerializingMember>) array2, context, targetType) : (IList<SerializingMember>) ((IEnumerable<SerializingMember>) array2).OrderBy<SerializingMember, string>((Func<SerializingMember, string>) (item => item.Contract.Name)).ToArray<SerializingMember>(), constructor);
    }

    private static IEnumerable<SerializingMember> GetTargetMembers(
      Type type)
    {
      MemberInfo[] members = type.FindMembers(MemberTypes.Field | MemberTypes.Property, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, (MemberFilter) null, (object) null);
      MemberInfo[] array = ((IEnumerable<MemberInfo>) members).Where<MemberInfo>((Func<MemberInfo, bool>) (item => item.IsDefined(typeof (MessagePackMemberAttribute)))).ToArray<MemberInfo>();
      if (array.Length > 0)
        return SerializationTarget.GetAnnotatedMembersWithDuplicationDetection(type, array);
      return type.GetCustomAttributesData().Any<CustomAttributeData>((Func<CustomAttributeData, bool>) (attr => attr.GetAttributeType().FullName == "System.Runtime.Serialization.DataContractAttribute")) ? SerializationTarget.GetSystemRuntimeSerializationCompatibleMembers(members) : SerializationTarget.GetPublicUnpreventedMembers(members);
    }

    private static IEnumerable<SerializingMember> GetAnnotatedMembersWithDuplicationDetection(
      Type type,
      MemberInfo[] filtered)
    {
      MemberInfo memberInfo = ((IEnumerable<MemberInfo>) filtered).FirstOrDefault<MemberInfo>((Func<MemberInfo, bool>) (member => member.IsDefined(typeof (MessagePackIgnoreAttribute))));
      if (memberInfo != (MemberInfo) null)
        throw new SerializationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "A member '{0}' of type '{1}' is marked with both MessagePackMemberAttribute and MessagePackIgnoreAttribute.", (object) memberInfo.Name, (object) type));
      return ((IEnumerable<MemberInfo>) filtered).Select<MemberInfo, SerializingMember>((Func<MemberInfo, SerializingMember>) (member => new SerializingMember(member, new DataMemberContract(member, member.GetCustomAttribute<MessagePackMemberAttribute>()))));
    }

    private static IEnumerable<SerializingMember> GetSystemRuntimeSerializationCompatibleMembers(
      MemberInfo[] members)
    {
      return ((IEnumerable<MemberInfo>) members).Select(item => new
      {
        member = item,
        data = item.GetCustomAttributesData().FirstOrDefault<CustomAttributeData>((Func<CustomAttributeData, bool>) (data => data.GetAttributeType().FullName == "System.Runtime.Serialization.DataMemberAttribute"))
      }).Where(item => item.data != null).Select(item =>
      {
        string name = item.data.GetNamedArguments().Where<CustomAttributeNamedArgument>((Func<CustomAttributeNamedArgument, bool>) (arg => arg.GetMemberName() == "Name")).Select<CustomAttributeNamedArgument, string>((Func<CustomAttributeNamedArgument, string>) (arg => (string) arg.GetTypedValue().Value)).FirstOrDefault<string>();
        int? id = item.data.GetNamedArguments().Where<CustomAttributeNamedArgument>((Func<CustomAttributeNamedArgument, bool>) (arg => arg.GetMemberName() == "Order")).Select<CustomAttributeNamedArgument, int?>((Func<CustomAttributeNamedArgument, int?>) (arg => (int?) arg.GetTypedValue().Value)).FirstOrDefault<int?>();
        return new SerializingMember(item.member, new DataMemberContract(item.member, name, NilImplication.MemberDefault, id));
      });
    }

    private static IEnumerable<SerializingMember> GetPublicUnpreventedMembers(
      MemberInfo[] members)
    {
      return ((IEnumerable<MemberInfo>) members).Where<MemberInfo>((Func<MemberInfo, bool>) (member => member.GetIsPublic() && !Attribute.IsDefined(member, typeof (NonSerializedAttribute)) && !member.IsDefined(typeof (MessagePackIgnoreAttribute)))).Select<MemberInfo, SerializingMember>((Func<MemberInfo, SerializingMember>) (member => new SerializingMember(member, new DataMemberContract(member))));
    }

    private static ConstructorInfo FindDeserializationConstructor(Type targetType)
    {
      ConstructorInfo[] array1 = ((IEnumerable<ConstructorInfo>) targetType.GetConstructors()).ToArray<ConstructorInfo>();
      if (array1.Length == 0)
        throw SerializationTarget.NewTypeCannotBeSerializedException(targetType);
      ConstructorInfo[] array2 = ((IEnumerable<ConstructorInfo>) array1).Where<ConstructorInfo>((Func<ConstructorInfo, bool>) (ctor => ctor.IsDefined(typeof (MessagePackDeserializationConstructorAttribute)))).ToArray<ConstructorInfo>();
      switch (array2.Length)
      {
        case 0:
          ConstructorInfo[] array3 = ((IEnumerable<ConstructorInfo>) array1).GroupBy<ConstructorInfo, int>((Func<ConstructorInfo, int>) (ctor => ctor.GetParameters().Length)).OrderByDescending<IGrouping<int, ConstructorInfo>, int>((Func<IGrouping<int, ConstructorInfo>, int>) (g => g.Key)).First<IGrouping<int, ConstructorInfo>>().ToArray<ConstructorInfo>();
          if (array3.Length == 1)
          {
            if (array3[0].GetParameters().Length == 0)
              throw SerializationTarget.NewTypeCannotBeSerializedException(targetType);
            return array3[0];
          }
          throw new SerializationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot serialize type '{0}' because it does not have any serializable fields nor properties, and serializer generator failed to determine constructor to deserialize among({1}).", (object) targetType, (object) string.Join(", ", ((IEnumerable<ConstructorInfo>) array3).Select<ConstructorInfo, string>((Func<ConstructorInfo, string>) (ctor => ctor.ToString())).ToArray<string>())));
        case 1:
          return array2[0];
        default:
          throw new SerializationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "There are multiple constructors marked with MessagePackDeserializationConstrutorAttribute in type '{0}'.", (object) targetType));
      }
    }

    private static SerializationException NewTypeCannotBeSerializedException(
      Type targetType)
    {
      return new SerializationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot serialize type '{0}' because it does not have any serializable fields nor properties, and it does not have any public constructors with parameters.", (object) targetType));
    }

    private static bool CheckTargetEligibility(MemberInfo member)
    {
      PropertyInfo propertyInfo = member as PropertyInfo;
      FieldInfo fieldInfo = member as FieldInfo;
      Type source;
      if (propertyInfo != (PropertyInfo) null)
      {
        if (propertyInfo.GetIndexParameters().Length > 0)
          return false;
        if (propertyInfo.GetSetMethod(true) != (MethodInfo) null)
          return true;
        source = propertyInfo.PropertyType;
      }
      else
      {
        if (!(fieldInfo != (FieldInfo) null) || !fieldInfo.IsInitOnly)
          return true;
        source = fieldInfo.FieldType;
      }
      CollectionTraits collectionTraits = source.GetCollectionTraits();
      switch (collectionTraits.CollectionType)
      {
        case CollectionKind.Array:
        case CollectionKind.Map:
          return collectionTraits.AddMethod != (MethodInfo) null;
        default:
          return false;
      }
    }

    private static IList<SerializingMember> ComplementMembers(
      IList<SerializingMember> candidates,
      SerializationContext context,
      Type targetType)
    {
      if (candidates[0].Contract.Id < 0)
        return candidates;
      if (context.CompatibilityOptions.OneBoundDataMemberOrder && candidates[0].Contract.Id == 0)
        throw new NotSupportedException("Cannot specify order value 0 on DataMemberAttribute when SerializationContext.CompatibilityOptions.OneBoundDataMemberOrder is set to true.");
      List<SerializingMember> serializingMemberList = new List<SerializingMember>(candidates.Max<SerializingMember>((Func<SerializingMember, int>) (item => item.Contract.Id)) + 1);
      int index = 0;
      int num = context.CompatibilityOptions.OneBoundDataMemberOrder ? 1 : 0;
      while (index < candidates.Count)
      {
        if (candidates[index].Contract.Id < num)
          throw new SerializationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "The member ID '{0}' is duplicated in the '{1}' elementType.", (object) candidates[index].Contract.Id, (object) targetType));
        for (; candidates[index].Contract.Id > num; ++num)
          serializingMemberList.Add(new SerializingMember());
        serializingMemberList.Add(candidates[index]);
        ++index;
        ++num;
      }
      SerializationTarget.VerifyNilImplication(targetType, (IEnumerable<SerializingMember>) serializingMemberList);
      SerializationTarget.VerifyKeyUniqueness((IList<SerializingMember>) serializingMemberList);
      return (IList<SerializingMember>) serializingMemberList;
    }

    private static void VerifyNilImplication(Type type, IEnumerable<SerializingMember> entries)
    {
      foreach (SerializingMember entry in entries)
      {
        if (entry.Contract.NilImplication == NilImplication.Null)
        {
          Type memberValueType = entry.Member.GetMemberValueType();
          if (memberValueType != typeof (MessagePackObject) && memberValueType.GetIsValueType() && Nullable.GetUnderlyingType(memberValueType) == (Type) null)
            throw SerializationExceptions.NewValueTypeCannotBeNull(entry.Member.ToString(), memberValueType, type);
          FieldInfo member;
          if (!((member = entry.Member as FieldInfo) != (FieldInfo) null) ? (entry.Member as PropertyInfo).GetSetMethod() == (MethodInfo) null : member.IsInitOnly)
            throw SerializationExceptions.NewNullIsProhibited(entry.Member.ToString());
        }
      }
    }

    private static void VerifyKeyUniqueness(IList<SerializingMember> result)
    {
      Dictionary<string, List<MemberInfo>> source = new Dictionary<string, List<MemberInfo>>();
      Dictionary<string, SerializingMember> dictionary = new Dictionary<string, SerializingMember>();
      foreach (SerializingMember serializingMember in (IEnumerable<SerializingMember>) result)
      {
        if (serializingMember.Contract.Name != null)
        {
          try
          {
            dictionary.Add(serializingMember.Contract.Name, serializingMember);
          }
          catch (ArgumentException ex)
          {
            List<MemberInfo> memberInfoList;
            if (source.TryGetValue(serializingMember.Contract.Name, out memberInfoList))
              memberInfoList.Add(serializingMember.Member);
            else
              source.Add(serializingMember.Contract.Name, new List<MemberInfo>()
              {
                dictionary[serializingMember.Contract.Name].Member,
                serializingMember.Member
              });
          }
        }
      }
      if (source.Count > 0)
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Some member keys specified with custom attributes are duplicated. Details: {{{0}}}", (object) string.Join(",", source.Select<KeyValuePair<string, List<MemberInfo>>, string>((Func<KeyValuePair<string, List<MemberInfo>>, string>) (kv => string.Format((IFormatProvider) CultureInfo.CurrentCulture, "\"{0}\":[{1}]", (object) kv.Key, (object) string.Join(",", kv.Value.Select<MemberInfo, string>((Func<MemberInfo, string>) (m => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}({2})", (object) m.DeclaringType, (object) m.Name, (object) (m as FieldInfo) != null ? (object) "Field" : (object) "Property"))).ToArray<string>())))).ToArray<string>())));
    }

    public static bool BuiltInSerializerExists(
      ISerializerGeneratorConfiguration configuration,
      Type type,
      CollectionTraits traits)
    {
      return GenericSerializer.IsSupported(type, traits, configuration.PreferReflectionBasedSerializer) || SerializerRepository.InternalDefault.Contains(type);
    }
  }
}
