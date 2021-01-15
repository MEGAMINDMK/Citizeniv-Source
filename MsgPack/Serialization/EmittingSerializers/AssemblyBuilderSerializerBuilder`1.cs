// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.EmittingSerializers.AssemblyBuilderSerializerBuilder`1
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using MsgPack.Serialization.AbstractSerializers;
using MsgPack.Serialization.Reflection;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace MsgPack.Serialization.EmittingSerializers
{
  internal class AssemblyBuilderSerializerBuilder<TObject> : ILEmittingSerializerBuilder<AssemblyBuilderEmittingContext, TObject>
  {
    protected override ILConstruct EmitGetSerializerExpression(
      AssemblyBuilderEmittingContext context,
      Type targetType,
      SerializingMember? memberInfo,
      PolymorphismSchema itemsSchema)
    {
      PolymorphismSchema realSchema = itemsSchema ?? PolymorphismSchema.Create(targetType, memberInfo);
      Action<TracingILGenerator, int> instructions = context.Emitter.RegisterSerializer(targetType, !memberInfo.HasValue ? EnumMemberSerializationMethod.Default : memberInfo.Value.GetEnumMemberSerializationMethod(), !memberInfo.HasValue ? DateTimeMemberConversionMethod.Default : memberInfo.Value.GetDateTimeMemberConversionMethod(), realSchema, (Func<IEnumerable<ILConstruct>>) (() => this.EmitConstructPolymorphismSchema(context, realSchema)));
      return ILConstruct.Instruction("getserializer", typeof (MessagePackSerializer<>).MakeGenericType(targetType), false, (Action<TracingILGenerator>) (il => instructions(il, 0)));
    }

    private IEnumerable<ILConstruct> EmitConstructPolymorphismSchema(
      AssemblyBuilderEmittingContext context,
      PolymorphismSchema currentSchema)
    {
      ILConstruct schema = this.DeclareLocal(context, typeof (PolymorphismSchema), "schema");
      yield return schema;
      foreach (ILConstruct ilConstruct in this.EmitConstructPolymorphismSchema(context, schema, currentSchema))
        yield return ilConstruct;
      yield return this.EmitLoadVariableExpression(context, schema);
    }

    protected override ILConstruct EmitFieldOfExpression(
      AssemblyBuilderEmittingContext context,
      FieldInfo field)
    {
      Action<TracingILGenerator, int> instructions = context.Emitter.RegisterField(field);
      return ILConstruct.Instruction("getfield", typeof (FieldInfo), false, (Action<TracingILGenerator>) (il => instructions(il, 0)));
    }

    protected override ILConstruct EmitMethodOfExpression(
      AssemblyBuilderEmittingContext context,
      MethodBase method)
    {
      Action<TracingILGenerator, int> instructions = context.Emitter.RegisterMethod(method);
      return ILConstruct.Instruction("getsetter", typeof (MethodBase), false, (Action<TracingILGenerator>) (il => instructions(il, 0)));
    }

    protected override AssemblyBuilderEmittingContext CreateCodeGenerationContextForSerializerCreation(
      SerializationContext context)
    {
      string serializerTypeName;
      string serializerTypeNamespace;
      DefaultSerializerNameResolver.ResolveTypeName(true, typeof (TObject), this.GetType().Namespace, out serializerTypeName, out serializerTypeNamespace);
      SerializerSpecification spec = new SerializerSpecification(typeof (TObject), SerializerBuilder<AssemblyBuilderEmittingContext, ILConstruct, TObject>.CollectionTraitsOfThis, serializerTypeName, serializerTypeNamespace);
      return new AssemblyBuilderEmittingContext(context, typeof (TObject), (Func<SerializerEmitter>) (() => SerializationMethodGeneratorManager.Get().CreateEmitter(spec, SerializerBuilder<AssemblyBuilderEmittingContext, ILConstruct, TObject>.BaseClass, EmitterFlavor.FieldBased)), (Func<EnumSerializerEmitter>) (() => SerializationMethodGeneratorManager.Get().CreateEnumEmitter(context, spec, EmitterFlavor.FieldBased)));
    }

    protected override void BuildSerializerCodeCore(
      ISerializerCodeGenerationContext context,
      Type concreteType,
      PolymorphismSchema itemSchema)
    {
      if (!(context is AssemblyBuilderCodeGenerationContext generationContext))
        throw new ArgumentException("'context' was not created with CreateGenerationContextForCodeGeneration method.", nameof (context));
      AssemblyBuilderEmittingContext emittingContext = generationContext.CreateEmittingContext(typeof (TObject), SerializerBuilder<AssemblyBuilderEmittingContext, ILConstruct, TObject>.CollectionTraitsOfThis, SerializerBuilder<AssemblyBuilderEmittingContext, ILConstruct, TObject>.BaseClass);
      if (!typeof (TObject).GetIsEnum())
      {
        this.BuildSerializer(emittingContext, concreteType, itemSchema);
        emittingContext.Emitter.CreateConstructor<TObject>();
      }
      else
      {
        this.BuildEnumSerializer(emittingContext);
        emittingContext.EnumEmitter.CreateConstructor<TObject>();
      }
    }
  }
}
