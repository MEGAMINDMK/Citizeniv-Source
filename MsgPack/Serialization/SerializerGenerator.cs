// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.SerializerGenerator
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using MsgPack.Serialization.AbstractSerializers;
using MsgPack.Serialization.CodeDomSerializers;
using MsgPack.Serialization.EmittingSerializers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace MsgPack.Serialization
{
  public class SerializerGenerator
  {
    private readonly HashSet<Type> _targetTypes;
    private readonly AssemblyName _assemblyName;
    private SerializationMethod _method;

    [Obsolete("Use TargetTypes instead.")]
    public Type RootType
    {
      get
      {
        return this._targetTypes.FirstOrDefault<Type>();
      }
    }

    [Obsolete("Use static methods instead.")]
    public ICollection<Type> TargetTypes
    {
      get
      {
        return (ICollection<Type>) this._targetTypes;
      }
    }

    [Obsolete("Use static methods instead.")]
    public AssemblyName AssemblyName
    {
      get
      {
        return this._assemblyName;
      }
    }

    [Obsolete("Use static methods instead.")]
    public SerializationMethod Method
    {
      get
      {
        return this._method;
      }
      set
      {
        if (value != SerializationMethod.Array && value != SerializationMethod.Map)
          throw new ArgumentOutOfRangeException(nameof (value));
        this._method = value;
      }
    }

    [Obsolete("Use static methods instead.")]
    public SerializerGenerator(AssemblyName assemblyName)
    {
      if (assemblyName == null)
        throw new ArgumentNullException(nameof (assemblyName));
      this._assemblyName = assemblyName;
      this._targetTypes = new HashSet<Type>();
      this._method = SerializationMethod.Array;
    }

    [Obsolete("Use static methods instead.")]
    public SerializerGenerator(Type rootType, AssemblyName assemblyName)
      : this(assemblyName)
    {
      if (rootType == (Type) null)
        throw new ArgumentNullException(nameof (rootType));
      this._targetTypes.Add(rootType);
    }

    [Obsolete("Use static GenerateAssembly method instead.")]
    public void GenerateAssemblyFile()
    {
      this.GenerateAssemblyFile(Path.GetFullPath("."));
    }

    [Obsolete("Use static GenerateAssembly method instead.")]
    public void GenerateAssemblyFile(string directory)
    {
      if (!Directory.Exists(directory))
        Directory.CreateDirectory(directory);
      SerializerGenerator.GenerateAssembly(new SerializerAssemblyGenerationConfiguration()
      {
        AssemblyName = this._assemblyName,
        OutputDirectory = directory,
        SerializationMethod = this._method
      }, (IEnumerable<Type>) this._targetTypes);
    }

    public static string GenerateAssembly(
      SerializerAssemblyGenerationConfiguration configuration,
      params Type[] targetTypes)
    {
      return SerializerGenerator.GenerateAssembly(configuration, (IEnumerable<Type>) targetTypes);
    }

    public static string GenerateAssembly(
      SerializerAssemblyGenerationConfiguration configuration,
      IEnumerable<Type> targetTypes)
    {
      return SerializerGenerator.GenerateSerializerCodeAssembly(configuration, targetTypes).FirstOrDefault<SerializerCodeGenerationResult>()?.FilePath;
    }

    public static IEnumerable<SerializerCodeGenerationResult> GenerateSerializerCodeAssembly(
      SerializerAssemblyGenerationConfiguration configuration,
      params Type[] targetTypes)
    {
      return SerializerGenerator.GenerateSerializerCodeAssembly(configuration, (IEnumerable<Type>) targetTypes);
    }

    public static IEnumerable<SerializerCodeGenerationResult> GenerateSerializerCodeAssembly(
      SerializerAssemblyGenerationConfiguration configuration,
      IEnumerable<Type> targetTypes)
    {
      return new SerializerGenerator.SerializerAssemblyGenerationLogic().Generate(targetTypes, configuration);
    }

    public static IEnumerable<string> GenerateCode(params Type[] targetTypes)
    {
      return SerializerGenerator.GenerateCode((SerializerCodeGenerationConfiguration) null, (IEnumerable<Type>) targetTypes);
    }

    public static IEnumerable<string> GenerateCode(IEnumerable<Type> targetTypes)
    {
      return SerializerGenerator.GenerateCode((SerializerCodeGenerationConfiguration) null, targetTypes);
    }

    public static IEnumerable<string> GenerateCode(
      SerializerCodeGenerationConfiguration configuration,
      params Type[] targetTypes)
    {
      return SerializerGenerator.GenerateSerializerSourceCodes(configuration, targetTypes).Select<SerializerCodeGenerationResult, string>((Func<SerializerCodeGenerationResult, string>) (r => r.FilePath));
    }

    public static IEnumerable<string> GenerateCode(
      SerializerCodeGenerationConfiguration configuration,
      IEnumerable<Type> targetTypes)
    {
      return SerializerGenerator.GenerateSerializerSourceCodes(configuration, targetTypes).Select<SerializerCodeGenerationResult, string>((Func<SerializerCodeGenerationResult, string>) (r => r.FilePath));
    }

    public static IEnumerable<SerializerCodeGenerationResult> GenerateSerializerSourceCodes(
      params Type[] targetTypes)
    {
      return SerializerGenerator.GenerateSerializerSourceCodes((SerializerCodeGenerationConfiguration) null, (IEnumerable<Type>) targetTypes);
    }

    public static IEnumerable<SerializerCodeGenerationResult> GenerateSerializerSourceCodes(
      IEnumerable<Type> targetTypes)
    {
      return SerializerGenerator.GenerateSerializerSourceCodes((SerializerCodeGenerationConfiguration) null, targetTypes);
    }

    public static IEnumerable<SerializerCodeGenerationResult> GenerateSerializerSourceCodes(
      SerializerCodeGenerationConfiguration configuration,
      params Type[] targetTypes)
    {
      return SerializerGenerator.GenerateSerializerSourceCodes(configuration, (IEnumerable<Type>) targetTypes);
    }

    public static IEnumerable<SerializerCodeGenerationResult> GenerateSerializerSourceCodes(
      SerializerCodeGenerationConfiguration configuration,
      IEnumerable<Type> targetTypes)
    {
      return new SerializerGenerator.SerializerCodesGenerationLogic().Generate(targetTypes, configuration ?? new SerializerCodeGenerationConfiguration());
    }

    private abstract class SerializerGenerationLogic<TConfig> where TConfig : class, ISerializerGeneratorConfiguration
    {
      protected abstract EmitterFlavor EmitterFlavor { get; }

      public IEnumerable<SerializerCodeGenerationResult> Generate(
        IEnumerable<Type> targetTypes,
        TConfig configuration)
      {
        if (targetTypes == null)
          throw new ArgumentNullException(nameof (targetTypes));
        if ((object) configuration == null)
          throw new ArgumentNullException(nameof (configuration));
        configuration.Validate();
        SerializationContext context = new SerializationContext()
        {
          EmitterFlavor = this.EmitterFlavor,
          GeneratorOption = SerializationMethodGeneratorOption.CanDump,
          EnumSerializationMethod = configuration.EnumSerializationMethod,
          SerializationMethod = configuration.SerializationMethod
        };
        IEnumerable<Type> source = !configuration.IsRecursive ? targetTypes.Where<Type>((Func<Type, bool>) (t => !SerializationTarget.BuiltInSerializerExists((ISerializerGeneratorConfiguration) configuration, t, t.GetCollectionTraits()))) : targetTypes.SelectMany<Type, Type>((Func<Type, IEnumerable<Type>>) (t => SerializerGenerator.SerializerGenerationLogic<TConfig>.ExtractElementTypes(context, (ISerializerGeneratorConfiguration) configuration, t)));
        ISerializerCodeGenerationContext generationContext = this.CreateGenerationContext(context, configuration);
        Func<Type, ISerializerCodeGenerator> generatorFactory = this.CreateGeneratorFactory();
        foreach (Type type in source.Distinct<Type>())
        {
          ISerializerCodeGenerator serializerCodeGenerator = generatorFactory(type);
          Type concreteType = (Type) null;
          if (type.GetIsInterface() || type.GetIsAbstract())
            concreteType = context.DefaultCollectionTypes.GetConcreteType(type);
          serializerCodeGenerator.BuildSerializerCode(generationContext, concreteType, (PolymorphismSchema) null);
        }
        Directory.CreateDirectory(configuration.OutputDirectory);
        return generationContext.Generate();
      }

      private static IEnumerable<Type> ExtractElementTypes(
        SerializationContext context,
        ISerializerGeneratorConfiguration configuration,
        Type type)
      {
        if (!SerializationTarget.BuiltInSerializerExists(configuration, type, type.GetCollectionTraits()))
        {
          yield return type;
          if (!type.GetIsEnum())
          {
            foreach (Type type1 in SerializationTarget.Prepare(context, type).Members.SelectMany<SerializingMember, Type>((Func<SerializingMember, IEnumerable<Type>>) (m => SerializerGenerator.SerializerGenerationLogic<TConfig>.ExtractElementTypes(context, configuration, m.Member.GetMemberValueType()))))
              yield return type1;
          }
        }
        if (type.IsArray)
        {
          Type elementType = type.GetElementType();
          if (!SerializationTarget.BuiltInSerializerExists(configuration, elementType, elementType.GetCollectionTraits()))
          {
            foreach (Type elementType1 in SerializerGenerator.SerializerGenerationLogic<TConfig>.ExtractElementTypes(context, configuration, elementType))
              yield return elementType1;
            yield return elementType;
          }
        }
        else
        {
          if (type.IsGenericType)
          {
            foreach (Type type1 in ((IEnumerable<Type>) type.GetGenericArguments()).SelectMany<Type, Type>((Func<Type, IEnumerable<Type>>) (g => SerializerGenerator.SerializerGenerationLogic<TConfig>.ExtractElementTypes(context, configuration, g))))
              yield return type1;
          }
          if (configuration.WithNullableSerializers && type.GetIsValueType() && Nullable.GetUnderlyingType(type) == (Type) null)
            yield return typeof (Nullable<>).MakeGenericType(type);
        }
      }

      protected abstract ISerializerCodeGenerationContext CreateGenerationContext(
        SerializationContext context,
        TConfig configuration);

      protected abstract Func<Type, ISerializerCodeGenerator> CreateGeneratorFactory();
    }

    private sealed class SerializerAssemblyGenerationLogic : SerializerGenerator.SerializerGenerationLogic<SerializerAssemblyGenerationConfiguration>
    {
      protected override EmitterFlavor EmitterFlavor
      {
        get
        {
          return EmitterFlavor.FieldBased;
        }
      }

      protected override ISerializerCodeGenerationContext CreateGenerationContext(
        SerializationContext context,
        SerializerAssemblyGenerationConfiguration configuration)
      {
        return (ISerializerCodeGenerationContext) new AssemblyBuilderCodeGenerationContext(context, AppDomain.CurrentDomain.DefineDynamicAssembly(configuration.AssemblyName, AssemblyBuilderAccess.RunAndSave, configuration.OutputDirectory), configuration);
      }

      protected override Func<Type, ISerializerCodeGenerator> CreateGeneratorFactory()
      {
        return (Func<Type, ISerializerCodeGenerator>) (type => ReflectionExtensions.CreateInstancePreservingExceptionType<ISerializerCodeGenerator>(typeof (AssemblyBuilderSerializerBuilder<>).MakeGenericType(type)));
      }
    }

    private sealed class SerializerCodesGenerationLogic : SerializerGenerator.SerializerGenerationLogic<SerializerCodeGenerationConfiguration>
    {
      protected override EmitterFlavor EmitterFlavor
      {
        get
        {
          return EmitterFlavor.CodeDomBased;
        }
      }

      protected override ISerializerCodeGenerationContext CreateGenerationContext(
        SerializationContext context,
        SerializerCodeGenerationConfiguration configuration)
      {
        return (ISerializerCodeGenerationContext) new CodeDomContext(context, configuration);
      }

      protected override Func<Type, ISerializerCodeGenerator> CreateGeneratorFactory()
      {
        return (Func<Type, ISerializerCodeGenerator>) (type => ReflectionExtensions.CreateInstancePreservingExceptionType<ISerializerCodeGenerator>(typeof (CodeDomSerializerBuilder<>).MakeGenericType(type)));
      }
    }
  }
}
