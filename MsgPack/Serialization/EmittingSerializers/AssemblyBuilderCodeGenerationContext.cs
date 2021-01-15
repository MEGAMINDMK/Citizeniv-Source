// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.EmittingSerializers.AssemblyBuilderCodeGenerationContext
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using MsgPack.Serialization.AbstractSerializers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace MsgPack.Serialization.EmittingSerializers
{
  internal class AssemblyBuilderCodeGenerationContext : ISerializerCodeGenerationContext
  {
    private readonly SerializationContext _context;
    private readonly SerializationMethodGeneratorManager _generatorManager;
    private readonly AssemblyBuilder _assemblyBuilder;
    private readonly string _directory;
    private readonly List<SerializerSpecification> _generatedSerializers;

    public AssemblyBuilderCodeGenerationContext(
      SerializationContext context,
      AssemblyBuilder assemblyBuilder,
      SerializerAssemblyGenerationConfiguration configuration)
    {
      this._context = context;
      this._assemblyBuilder = assemblyBuilder;
      DefaultSerializationMethodGeneratorManager.SetUpAssemblyBuilderAttributes(assemblyBuilder, false);
      this._generatorManager = SerializationMethodGeneratorManager.Get(assemblyBuilder);
      this._directory = configuration.OutputDirectory;
      this._generatedSerializers = new List<SerializerSpecification>();
    }

    public AssemblyBuilderEmittingContext CreateEmittingContext(
      Type targetType,
      CollectionTraits targetTypeCollectionTraits,
      Type serializerBaseClass)
    {
      string serializerTypeName;
      string serializerTypeNamespace;
      DefaultSerializerNameResolver.ResolveTypeName((Assembly) this._assemblyBuilder == (Assembly) null, targetType, typeof (AssemblyBuilderCodeGenerationContext).Namespace, out serializerTypeName, out serializerTypeNamespace);
      SerializerSpecification spec = new SerializerSpecification(targetType, targetTypeCollectionTraits, serializerTypeName, serializerTypeNamespace);
      this._generatedSerializers.Add(spec);
      return new AssemblyBuilderEmittingContext(this._context, targetType, (Func<SerializerEmitter>) (() => this._generatorManager.CreateEmitter(spec, serializerBaseClass, EmitterFlavor.FieldBased)), (Func<EnumSerializerEmitter>) (() => this._generatorManager.CreateEnumEmitter(this._context, spec, EmitterFlavor.FieldBased)));
    }

    public IEnumerable<SerializerCodeGenerationResult> Generate()
    {
      string str = this._assemblyBuilder.GetName().Name + ".dll";
      this._assemblyBuilder.Save(str);
      string assemblyFilePath = Path.GetFullPath(Path.Combine(this._directory, str));
      return this._generatedSerializers.Select<SerializerSpecification, SerializerCodeGenerationResult>((Func<SerializerSpecification, SerializerCodeGenerationResult>) (s => new SerializerCodeGenerationResult(s.TargetType, assemblyFilePath, s.SerializerTypeFullName, s.SerializerTypeNamespace, s.SerializerTypeName)));
    }
  }
}
