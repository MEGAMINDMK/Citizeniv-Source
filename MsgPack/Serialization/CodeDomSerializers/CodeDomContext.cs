// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.CodeDomSerializers.CodeDomContext
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using MsgPack.Serialization.AbstractSerializers;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security;
using System.Text;

namespace MsgPack.Serialization.CodeDomSerializers
{
  internal class CodeDomContext : SerializerGenerationContext<CodeDomConstruct>, ISerializerCodeGenerationContext
  {
    public static readonly CodeCatchClause[] EmptyCatches = new CodeCatchClause[0];
    private readonly Dictionary<SerializerFieldKey, string> _dependentSerializers = new Dictionary<SerializerFieldKey, string>();
    private readonly Dictionary<RuntimeFieldHandle, CodeDomContext.CachedFieldInfo> _cachedTargetFields = new Dictionary<RuntimeFieldHandle, CodeDomContext.CachedFieldInfo>();
    private readonly Dictionary<RuntimeMethodHandle, CodeDomContext.CachedMethodBase> _cachedPropertyAccessors = new Dictionary<RuntimeMethodHandle, CodeDomContext.CachedMethodBase>();
    private readonly Dictionary<Type, CodeTypeDeclaration> _declaringTypes = new Dictionary<Type, CodeTypeDeclaration>();
    private readonly Dictionary<string, int> _uniqueVariableSuffixes = new Dictionary<string, int>();
    public const string ConditionalExpressionHelperMethodName = "__Conditional";
    public const string ConditionalExpressionHelperConditionParameterName = "condition";
    public const string ConditionalExpressionHelperWhenTrueParameterName = "whenTrue";
    public const string ConditionalExpressionHelperWhenFalseParameterName = "whenFalse";
    private readonly SerializerCodeGenerationConfiguration _configuration;
    private CodeTypeDeclaration _buildingType;

    public CodeTypeDeclaration DeclaringType
    {
      get
      {
        return this._buildingType;
      }
    }

    public CodeDomContext(
      SerializationContext context,
      SerializerCodeGenerationConfiguration configuration)
      : base(context)
    {
      this._configuration = configuration;
    }

    public string GetSerializerFieldName(
      Type targetType,
      EnumMemberSerializationMethod enumSerializationMethod,
      DateTimeMemberConversionMethod dateTimeConversionMethod,
      PolymorphismSchema polymorphismSchema)
    {
      SerializerFieldKey key = new SerializerFieldKey(targetType, enumSerializationMethod, dateTimeConversionMethod, polymorphismSchema);
      string str;
      if (!this._dependentSerializers.TryGetValue(key, out str))
      {
        str = "_serializer" + this._dependentSerializers.Count.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        this._dependentSerializers.Add(key, str);
      }
      return str;
    }

    public Dictionary<SerializerFieldKey, string> GetDependentSerializers()
    {
      return this._dependentSerializers;
    }

    public string GetCachedFieldInfoName(FieldInfo field)
    {
      RuntimeFieldHandle fieldHandle = field.FieldHandle;
      CodeDomContext.CachedFieldInfo cachedFieldInfo;
      if (!this._cachedTargetFields.TryGetValue(fieldHandle, out cachedFieldInfo))
      {
        cachedFieldInfo = new CodeDomContext.CachedFieldInfo(field, "_field" + field.DeclaringType.Name.Replace('`', '_') + "_" + field.Name + this._cachedTargetFields.Count.ToString((IFormatProvider) CultureInfo.InvariantCulture));
        this._cachedTargetFields.Add(fieldHandle, cachedFieldInfo);
      }
      return cachedFieldInfo.StorageFieldName;
    }

    public Dictionary<RuntimeFieldHandle, CodeDomContext.CachedFieldInfo> GetCachedFieldInfos()
    {
      return this._cachedTargetFields;
    }

    public string GetCachedMethodBaseName(MethodBase method)
    {
      RuntimeMethodHandle methodHandle = method.MethodHandle;
      CodeDomContext.CachedMethodBase cachedMethodBase;
      if (!this._cachedPropertyAccessors.TryGetValue(methodHandle, out cachedMethodBase))
      {
        cachedMethodBase = new CodeDomContext.CachedMethodBase(method, "_methodBase" + method.DeclaringType.Name.Replace('`', '_') + "_" + method.Name + this._cachedPropertyAccessors.Count.ToString((IFormatProvider) CultureInfo.InvariantCulture));
        this._cachedPropertyAccessors.Add(methodHandle, cachedMethodBase);
      }
      return cachedMethodBase.StorageFieldName;
    }

    public Dictionary<RuntimeMethodHandle, CodeDomContext.CachedMethodBase> GetCachedMethodBases()
    {
      return this._cachedPropertyAccessors;
    }

    public override string GetUniqueVariableName(string prefix)
    {
      int num;
      if (!this._uniqueVariableSuffixes.TryGetValue(prefix, out num))
      {
        this._uniqueVariableSuffixes.Add(prefix, 0);
        return prefix;
      }
      this._uniqueVariableSuffixes[prefix] = num + 1;
      return prefix + num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
    }

    public bool IsInternalToMsgPackLibrary
    {
      get
      {
        return this._configuration.IsInternalToMsgPackLibrary;
      }
    }

    protected override void ResetCore(Type targetType, Type baseClass)
    {
      CodeTypeDeclaration codeTypeDeclaration = new CodeTypeDeclaration(IdentifierUtility.EscapeTypeName(targetType) + "Serializer");
      codeTypeDeclaration.BaseTypes.Add(baseClass);
      codeTypeDeclaration.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof (GeneratedCodeAttribute)), new CodeAttributeArgument[2]
      {
        new CodeAttributeArgument((CodeExpression) new CodePrimitiveExpression((object) "MsgPack.Serialization.CodeDomSerializers.CodeDomSerializerBuilder")),
        new CodeAttributeArgument((CodeExpression) new CodePrimitiveExpression((object) this.GetType().Assembly.GetName().Version.ToString()))
      }));
      codeTypeDeclaration.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof (DebuggerNonUserCodeAttribute))));
      this._declaringTypes.Add(targetType, codeTypeDeclaration);
      this._dependentSerializers.Clear();
      this._cachedTargetFields.Clear();
      this._cachedPropertyAccessors.Clear();
      this._buildingType = codeTypeDeclaration;
      this.Packer = (CodeDomConstruct) CodeDomConstruct.Parameter(typeof (Packer), "packer");
      this.PackToTarget = (CodeDomConstruct) CodeDomConstruct.Parameter(targetType, "objectTree");
      this.Unpacker = (CodeDomConstruct) CodeDomConstruct.Parameter(typeof (Unpacker), "unpacker");
      this.UnpackToTarget = (CodeDomConstruct) CodeDomConstruct.Parameter(targetType, "collection");
      CollectionTraits collectionTraits = targetType.GetCollectionTraits();
      if (!(collectionTraits.ElementType != (Type) null))
        return;
      this.CollectionToBeAdded = (CodeDomConstruct) CodeDomConstruct.Parameter(targetType, "collection");
      this.ItemToAdd = (CodeDomConstruct) CodeDomConstruct.Parameter(collectionTraits.ElementType, "item");
      if (collectionTraits.DetailedCollectionType == CollectionDetailedKind.GenericDictionary || collectionTraits.DetailedCollectionType == CollectionDetailedKind.GenericReadOnlyDictionary)
      {
        this.KeyToAdd = (CodeDomConstruct) CodeDomConstruct.Parameter(collectionTraits.ElementType.GetGenericArguments()[0], "key");
        this.ValueToAdd = (CodeDomConstruct) CodeDomConstruct.Parameter(collectionTraits.ElementType.GetGenericArguments()[1], "value");
      }
      this.InitialCapacity = (CodeDomConstruct) CodeDomConstruct.Parameter(typeof (int), "initialCapacity");
    }

    public void ResetMethodContext()
    {
      this._uniqueVariableSuffixes.Clear();
    }

    [SecuritySafeCritical]
    public IEnumerable<SerializerCodeGenerationResult> Generate()
    {
      using (CodeDomProvider provider = CodeDomProvider.CreateProvider(this._configuration.Language))
      {
        CodeGeneratorOptions options = new CodeGeneratorOptions()
        {
          BlankLinesBetweenMembers = true,
          ElseOnClosing = false,
          IndentString = this._configuration.CodeIndentString,
          VerbatimOrder = false
        };
        string str1 = Path.Combine(this._configuration.OutputDirectory, this._configuration.Namespace.Replace(Type.Delimiter, Path.DirectorySeparatorChar));
        Directory.CreateDirectory(str1);
        List<SerializerCodeGenerationResult> generationResultList = new List<SerializerCodeGenerationResult>(this._declaringTypes.Count);
        foreach (KeyValuePair<Type, CodeTypeDeclaration> declaringType in this._declaringTypes)
        {
          string str2 = declaringType.Value.Name;
          if (declaringType.Value.TypeParameters.Count > 0)
            str2 = str2 + "`" + declaringType.Value.TypeParameters.Count.ToString((IFormatProvider) CultureInfo.InvariantCulture);
          string path2 = str2 + "." + provider.FileExtension;
          CodeNamespace codeNamespace = new CodeNamespace(this._configuration.Namespace);
          codeNamespace.Types.Add(declaringType.Value);
          CodeCompileUnit compileUnit = new CodeCompileUnit();
          compileUnit.Namespaces.Add(codeNamespace);
          string str3 = Path.Combine(str1, path2);
          generationResultList.Add(new SerializerCodeGenerationResult(declaringType.Key, str3, string.IsNullOrEmpty(codeNamespace.Name) ? declaringType.Value.Name : codeNamespace.Name + "." + declaringType.Value.Name, codeNamespace.Name, declaringType.Value.Name));
          using (StreamWriter streamWriter = new StreamWriter(str3, false, Encoding.UTF8))
            provider.GenerateCodeFromCompileUnit(compileUnit, (TextWriter) streamWriter, options);
        }
        return (IEnumerable<SerializerCodeGenerationResult>) generationResultList;
      }
    }

    public CodeCompileUnit CreateCodeCompileUnit()
    {
      return new CodeCompileUnit()
      {
        Namespaces = {
          new CodeNamespace(this._configuration.Namespace)
          {
            Types = {
              this._buildingType
            }
          }
        }
      };
    }

    public struct CachedFieldInfo
    {
      public readonly string StorageFieldName;
      public readonly FieldInfo Target;

      public CachedFieldInfo(FieldInfo target, string storageFieldName)
      {
        this.Target = target;
        this.StorageFieldName = storageFieldName;
      }
    }

    public struct CachedMethodBase
    {
      public readonly string StorageFieldName;
      public readonly MethodBase Target;

      public CachedMethodBase(MethodBase target, string storageFieldName)
      {
        this.Target = target;
        this.StorageFieldName = storageFieldName;
      }
    }
  }
}
