// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.CodeDomSerializers.CodeDomSerializerBuilder`1
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using MsgPack.Serialization.AbstractSerializers;
using MsgPack.Serialization.CollectionSerializers;
using MsgPack.Serialization.Metadata;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security;

namespace MsgPack.Serialization.CodeDomSerializers
{
  internal class CodeDomSerializerBuilder<TObject> : SerializerBuilder<CodeDomContext, CodeDomConstruct, TObject>
  {
    protected override void EmitMethodPrologue(CodeDomContext context, SerializerMethod method)
    {
      context.ResetMethodContext();
    }

    protected override void EmitMethodPrologue(CodeDomContext context, EnumSerializerMethod method)
    {
      context.ResetMethodContext();
    }

    protected override void EmitMethodPrologue(
      CodeDomContext context,
      CollectionSerializerMethod method,
      MethodInfo declaration)
    {
      context.ResetMethodContext();
    }

    protected override void EmitMethodEpilogue(
      CodeDomContext context,
      SerializerMethod method,
      CodeDomConstruct construct)
    {
      if (construct == null)
        return;
      CodeMemberMethod codeMemberMethod1;
      switch (method)
      {
        case SerializerMethod.PackToCore:
          CodeMemberMethod codeMemberMethod2 = new CodeMemberMethod();
          codeMemberMethod2.Name = "PackToCore";
          codeMemberMethod1 = codeMemberMethod2;
          codeMemberMethod1.Parameters.Add(context.Packer.AsParameter());
          codeMemberMethod1.Parameters.Add(context.PackToTarget.AsParameter());
          break;
        case SerializerMethod.UnpackFromCore:
          CodeMemberMethod codeMemberMethod3 = new CodeMemberMethod();
          codeMemberMethod3.Name = "UnpackFromCore";
          codeMemberMethod3.ReturnType = new CodeTypeReference(typeof (TObject));
          codeMemberMethod1 = codeMemberMethod3;
          codeMemberMethod1.Parameters.Add(context.Unpacker.AsParameter());
          break;
        case SerializerMethod.UnpackToCore:
          CodeMemberMethod codeMemberMethod4 = new CodeMemberMethod();
          codeMemberMethod4.Name = "UnpackToCore";
          codeMemberMethod1 = codeMemberMethod4;
          codeMemberMethod1.Parameters.Add(context.Unpacker.AsParameter());
          codeMemberMethod1.Parameters.Add(context.UnpackToTarget.AsParameter());
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof (method));
      }
      codeMemberMethod1.Attributes = (MemberAttributes) ((context.IsInternalToMsgPackLibrary ? 16384 : 12288) | 4);
      codeMemberMethod1.Statements.AddRange(construct.AsStatements().ToArray<CodeStatement>());
      context.DeclaringType.Members.Add((CodeTypeMember) codeMemberMethod1);
    }

    protected override void EmitMethodEpilogue(
      CodeDomContext context,
      EnumSerializerMethod method,
      CodeDomConstruct construct)
    {
      if (construct == null)
        return;
      CodeMemberMethod codeMemberMethod1;
      switch (method)
      {
        case EnumSerializerMethod.PackUnderlyingValueTo:
          CodeMemberMethod codeMemberMethod2 = new CodeMemberMethod();
          codeMemberMethod2.Name = "PackUnderlyingValueTo";
          codeMemberMethod1 = codeMemberMethod2;
          codeMemberMethod1.Parameters.Add(context.Packer.AsParameter());
          codeMemberMethod1.Parameters.Add(new CodeParameterDeclarationExpression(typeof (TObject), "enumValue"));
          break;
        case EnumSerializerMethod.UnpackFromUnderlyingValue:
          CodeMemberMethod codeMemberMethod3 = new CodeMemberMethod();
          codeMemberMethod3.Name = "UnpackFromUnderlyingValue";
          codeMemberMethod3.ReturnType = new CodeTypeReference(typeof (TObject));
          codeMemberMethod1 = codeMemberMethod3;
          codeMemberMethod1.Parameters.Add(new CodeParameterDeclarationExpression(typeof (MessagePackObject), "messagePackObject"));
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof (method));
      }
      codeMemberMethod1.Attributes = (MemberAttributes) ((context.IsInternalToMsgPackLibrary ? 16384 : 12288) | 4);
      codeMemberMethod1.Statements.AddRange(construct.AsStatements().ToArray<CodeStatement>());
      context.DeclaringType.Members.Add((CodeTypeMember) codeMemberMethod1);
    }

    protected override void EmitMethodEpilogue(
      CodeDomContext context,
      CollectionSerializerMethod method,
      CodeDomConstruct construct)
    {
      if (construct == null)
        return;
      CodeMemberMethod codeMemberMethod1;
      switch (method)
      {
        case CollectionSerializerMethod.AddItem:
          CodeMemberMethod codeMemberMethod2 = new CodeMemberMethod();
          codeMemberMethod2.Name = "AddItem";
          codeMemberMethod1 = codeMemberMethod2;
          codeMemberMethod1.Parameters.Add(new CodeParameterDeclarationExpression(typeof (TObject), "collection"));
          if (SerializerBuilder<CodeDomContext, CodeDomConstruct, TObject>.CollectionTraitsOfThis.DetailedCollectionType == CollectionDetailedKind.GenericDictionary || SerializerBuilder<CodeDomContext, CodeDomConstruct, TObject>.CollectionTraitsOfThis.DetailedCollectionType == CollectionDetailedKind.GenericReadOnlyDictionary)
          {
            codeMemberMethod1.Parameters.Add(new CodeParameterDeclarationExpression(SerializerBuilder<CodeDomContext, CodeDomConstruct, TObject>.CollectionTraitsOfThis.ElementType.GetGenericArguments()[0], "key"));
            codeMemberMethod1.Parameters.Add(new CodeParameterDeclarationExpression(SerializerBuilder<CodeDomContext, CodeDomConstruct, TObject>.CollectionTraitsOfThis.ElementType.GetGenericArguments()[1], "value"));
          }
          else
            codeMemberMethod1.Parameters.Add(new CodeParameterDeclarationExpression(SerializerBuilder<CodeDomContext, CodeDomConstruct, TObject>.CollectionTraitsOfThis.ElementType, "item"));
          codeMemberMethod1.Attributes = MemberAttributes.Family | MemberAttributes.Override;
          break;
        case CollectionSerializerMethod.CreateInstance:
          CodeMemberMethod codeMemberMethod3 = new CodeMemberMethod();
          codeMemberMethod3.Name = "CreateInstance";
          codeMemberMethod3.ReturnType = new CodeTypeReference(typeof (TObject));
          codeMemberMethod1 = codeMemberMethod3;
          codeMemberMethod1.Parameters.Add(new CodeParameterDeclarationExpression(typeof (int), "initialCapacity"));
          codeMemberMethod1.Attributes = MemberAttributes.Family | MemberAttributes.Override;
          break;
        case CollectionSerializerMethod.RestoreSchema:
          CodeMemberMethod codeMemberMethod4 = new CodeMemberMethod();
          codeMemberMethod4.Name = "RestoreSchema";
          codeMemberMethod4.ReturnType = new CodeTypeReference(typeof (PolymorphismSchema));
          codeMemberMethod1 = codeMemberMethod4;
          codeMemberMethod1.Attributes = MemberAttributes.Static | MemberAttributes.Private;
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof (method));
      }
      codeMemberMethod1.Statements.AddRange(construct.AsStatements().ToArray<CodeStatement>());
      context.DeclaringType.Members.Add((CodeTypeMember) codeMemberMethod1);
    }

    protected override CodeDomConstruct MakeNullLiteral(
      CodeDomContext context,
      Type contextType)
    {
      return (CodeDomConstruct) CodeDomConstruct.Expression(contextType, (CodeExpression) new CodePrimitiveExpression((object) null));
    }

    protected override CodeDomConstruct MakeByteLiteral(
      CodeDomContext context,
      byte constant)
    {
      return (CodeDomConstruct) CodeDomConstruct.Expression(typeof (byte), (CodeExpression) new CodePrimitiveExpression((object) constant));
    }

    protected override CodeDomConstruct MakeSByteLiteral(
      CodeDomContext context,
      sbyte constant)
    {
      return (CodeDomConstruct) CodeDomConstruct.Expression(typeof (sbyte), (CodeExpression) new CodePrimitiveExpression((object) constant));
    }

    protected override CodeDomConstruct MakeInt16Literal(
      CodeDomContext context,
      short constant)
    {
      return (CodeDomConstruct) CodeDomConstruct.Expression(typeof (short), (CodeExpression) new CodePrimitiveExpression((object) constant));
    }

    protected override CodeDomConstruct MakeUInt16Literal(
      CodeDomContext context,
      ushort constant)
    {
      return (CodeDomConstruct) CodeDomConstruct.Expression(typeof (ushort), (CodeExpression) new CodePrimitiveExpression((object) constant));
    }

    protected override CodeDomConstruct MakeInt32Literal(
      CodeDomContext context,
      int constant)
    {
      return (CodeDomConstruct) CodeDomConstruct.Expression(typeof (int), (CodeExpression) new CodePrimitiveExpression((object) constant));
    }

    protected override CodeDomConstruct MakeUInt32Literal(
      CodeDomContext context,
      uint constant)
    {
      return (CodeDomConstruct) CodeDomConstruct.Expression(typeof (uint), (CodeExpression) new CodePrimitiveExpression((object) constant));
    }

    protected override CodeDomConstruct MakeInt64Literal(
      CodeDomContext context,
      long constant)
    {
      return (CodeDomConstruct) CodeDomConstruct.Expression(typeof (long), (CodeExpression) new CodePrimitiveExpression((object) constant));
    }

    protected override CodeDomConstruct MakeUInt64Literal(
      CodeDomContext context,
      ulong constant)
    {
      return (CodeDomConstruct) CodeDomConstruct.Expression(typeof (ulong), (CodeExpression) new CodePrimitiveExpression((object) constant));
    }

    protected override CodeDomConstruct MakeReal32Literal(
      CodeDomContext context,
      float constant)
    {
      return (CodeDomConstruct) CodeDomConstruct.Expression(typeof (float), (CodeExpression) new CodePrimitiveExpression((object) constant));
    }

    protected override CodeDomConstruct MakeReal64Literal(
      CodeDomContext context,
      double constant)
    {
      return (CodeDomConstruct) CodeDomConstruct.Expression(typeof (double), (CodeExpression) new CodePrimitiveExpression((object) constant));
    }

    protected override CodeDomConstruct MakeBooleanLiteral(
      CodeDomContext context,
      bool constant)
    {
      return (CodeDomConstruct) CodeDomConstruct.Expression(typeof (bool), (CodeExpression) new CodePrimitiveExpression((object) constant));
    }

    protected override CodeDomConstruct MakeCharLiteral(
      CodeDomContext context,
      char constant)
    {
      return (CodeDomConstruct) CodeDomConstruct.Expression(typeof (char), (CodeExpression) new CodePrimitiveExpression((object) constant));
    }

    protected override CodeDomConstruct MakeEnumLiteral(
      CodeDomContext context,
      Type type,
      object constant)
    {
      string source = constant.ToString();
      return '0' <= source[0] && source[0] <= '9' || source.Contains<char>(',') ? (CodeDomConstruct) CodeDomConstruct.Expression(type, (CodeExpression) new CodeCastExpression(type, (CodeExpression) new CodePrimitiveExpression((object) ulong.Parse(((Enum) constant).ToString("D"), (IFormatProvider) CultureInfo.InvariantCulture)))) : (CodeDomConstruct) CodeDomConstruct.Expression(type, (CodeExpression) new CodeFieldReferenceExpression((CodeExpression) new CodeTypeReferenceExpression(type), constant.ToString()));
    }

    protected override CodeDomConstruct MakeDefaultLiteral(
      CodeDomContext context,
      Type type)
    {
      return (CodeDomConstruct) CodeDomConstruct.Expression(type, (CodeExpression) new CodeDefaultValueExpression(new CodeTypeReference(type)));
    }

    protected override CodeDomConstruct MakeStringLiteral(
      CodeDomContext context,
      string constant)
    {
      return (CodeDomConstruct) CodeDomConstruct.Expression(typeof (string), (CodeExpression) new CodePrimitiveExpression((object) constant));
    }

    protected override CodeDomConstruct EmitThisReferenceExpression(
      CodeDomContext context)
    {
      return (CodeDomConstruct) CodeDomConstruct.Expression(typeof (MessagePackSerializer<>).MakeGenericType(typeof (TObject)), (CodeExpression) new CodeThisReferenceExpression());
    }

    protected override CodeDomConstruct EmitBoxExpression(
      CodeDomContext context,
      Type valueType,
      CodeDomConstruct value)
    {
      return (CodeDomConstruct) CodeDomConstruct.Expression(typeof (object), (CodeExpression) new CodeCastExpression(typeof (object), value.AsExpression()));
    }

    protected override CodeDomConstruct EmitUnboxAnyExpression(
      CodeDomContext context,
      Type targetType,
      CodeDomConstruct value)
    {
      return (CodeDomConstruct) CodeDomConstruct.Expression(targetType, (CodeExpression) new CodeCastExpression(targetType, value.AsExpression()));
    }

    protected override CodeDomConstruct EmitNotExpression(
      CodeDomContext context,
      CodeDomConstruct booleanExpression)
    {
      return (CodeDomConstruct) CodeDomConstruct.Expression(typeof (bool), (CodeExpression) new CodeBinaryOperatorExpression(booleanExpression.AsExpression(), CodeBinaryOperatorType.ValueEquality, (CodeExpression) new CodePrimitiveExpression((object) false)));
    }

    protected override CodeDomConstruct EmitEqualsExpression(
      CodeDomContext context,
      CodeDomConstruct left,
      CodeDomConstruct right)
    {
      return (CodeDomConstruct) CodeDomConstruct.Expression(typeof (bool), (CodeExpression) new CodeBinaryOperatorExpression(left.AsExpression(), CodeBinaryOperatorType.ValueEquality, right.AsExpression()));
    }

    protected override CodeDomConstruct EmitGreaterThanExpression(
      CodeDomContext context,
      CodeDomConstruct left,
      CodeDomConstruct right)
    {
      return (CodeDomConstruct) CodeDomConstruct.Expression(typeof (bool), (CodeExpression) new CodeBinaryOperatorExpression(left.AsExpression(), CodeBinaryOperatorType.GreaterThan, right.AsExpression()));
    }

    protected override CodeDomConstruct EmitLessThanExpression(
      CodeDomContext context,
      CodeDomConstruct left,
      CodeDomConstruct right)
    {
      return (CodeDomConstruct) CodeDomConstruct.Expression(typeof (bool), (CodeExpression) new CodeBinaryOperatorExpression(left.AsExpression(), CodeBinaryOperatorType.LessThan, right.AsExpression()));
    }

    protected override CodeDomConstruct EmitIncrement(
      CodeDomContext context,
      CodeDomConstruct int32Value)
    {
      return (CodeDomConstruct) CodeDomConstruct.Statement((CodeStatement) new CodeAssignStatement(int32Value.AsExpression(), (CodeExpression) new CodeBinaryOperatorExpression(int32Value.AsExpression(), CodeBinaryOperatorType.Add, (CodeExpression) new CodePrimitiveExpression((object) 1))));
    }

    protected override CodeDomConstruct EmitTypeOfExpression(
      CodeDomContext context,
      Type type)
    {
      return (CodeDomConstruct) CodeDomConstruct.Expression(typeof (Type), (CodeExpression) new CodeTypeOfExpression(type));
    }

    protected override CodeDomConstruct EmitFieldOfExpression(
      CodeDomContext context,
      FieldInfo field)
    {
      return (CodeDomConstruct) CodeDomConstruct.Expression(typeof (FieldInfo), (CodeExpression) new CodeFieldReferenceExpression((CodeExpression) new CodeThisReferenceExpression(), context.GetCachedFieldInfoName(field)));
    }

    protected override CodeDomConstruct EmitMethodOfExpression(
      CodeDomContext context,
      MethodBase method)
    {
      return (CodeDomConstruct) CodeDomConstruct.Expression(typeof (MethodBase), (CodeExpression) new CodeFieldReferenceExpression((CodeExpression) new CodeThisReferenceExpression(), context.GetCachedMethodBaseName(method)));
    }

    protected override CodeDomConstruct EmitSequentialStatements(
      CodeDomContext context,
      Type contextType,
      IEnumerable<CodeDomConstruct> statements)
    {
      return (CodeDomConstruct) CodeDomConstruct.Statement(statements.SelectMany<CodeDomConstruct, CodeStatement>((Func<CodeDomConstruct, IEnumerable<CodeStatement>>) (s => s.AsStatements())));
    }

    protected override CodeDomConstruct DeclareLocal(
      CodeDomContext context,
      Type type,
      string name)
    {
      return CodeDomConstruct.Variable(type, context.GetUniqueVariableName(name));
    }

    protected override CodeDomConstruct ReferArgument(
      CodeDomContext context,
      Type type,
      string name,
      int index)
    {
      return (CodeDomConstruct) CodeDomConstruct.Parameter(type, name);
    }

    protected override CodeDomConstruct EmitCreateNewObjectExpression(
      CodeDomContext context,
      CodeDomConstruct variable,
      ConstructorInfo constructor,
      params CodeDomConstruct[] arguments)
    {
      return (CodeDomConstruct) CodeDomConstruct.Expression(constructor.DeclaringType, (CodeExpression) new CodeObjectCreateExpression(constructor.DeclaringType, ((IEnumerable<CodeDomConstruct>) arguments).Select<CodeDomConstruct, CodeExpression>((Func<CodeDomConstruct, CodeExpression>) (a => a.AsExpression())).ToArray<CodeExpression>()));
    }

    protected override CodeDomConstruct EmitInvokeVoidMethod(
      CodeDomContext context,
      CodeDomConstruct instance,
      MethodInfo method,
      params CodeDomConstruct[] arguments)
    {
      return (CodeDomConstruct) CodeDomConstruct.Statement((CodeStatement) new CodeExpressionStatement((CodeExpression) new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(instance == null ? (CodeExpression) new CodeTypeReferenceExpression(method.DeclaringType) : instance.AsExpression(), method.Name, method.IsGenericMethod ? ((IEnumerable<Type>) method.GetGenericArguments()).Select<Type, CodeTypeReference>((Func<Type, CodeTypeReference>) (t => new CodeTypeReference(t))).ToArray<CodeTypeReference>() : CodeDomSerializerBuilder.EmptyGenericArguments), ((IEnumerable<CodeDomConstruct>) arguments).Select<CodeDomConstruct, CodeExpression>((Func<CodeDomConstruct, CodeExpression>) (a => a.AsExpression())).ToArray<CodeExpression>())));
    }

    protected override CodeDomConstruct EmitInvokeMethodExpression(
      CodeDomContext context,
      CodeDomConstruct instance,
      MethodInfo method,
      IEnumerable<CodeDomConstruct> arguments)
    {
      return (CodeDomConstruct) CodeDomConstruct.Expression(method.ReturnType, (CodeExpression) new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(instance == null ? (CodeExpression) new CodeTypeReferenceExpression(method.DeclaringType) : instance.AsExpression(), method.Name, method.IsGenericMethod ? ((IEnumerable<Type>) method.GetGenericArguments()).Select<Type, CodeTypeReference>((Func<Type, CodeTypeReference>) (t => new CodeTypeReference(t))).ToArray<CodeTypeReference>() : CodeDomSerializerBuilder.EmptyGenericArguments), arguments.Select<CodeDomConstruct, CodeExpression>((Func<CodeDomConstruct, CodeExpression>) (a => a.AsExpression())).ToArray<CodeExpression>()));
    }

    protected override CodeDomConstruct EmitGetPropertyExpression(
      CodeDomContext context,
      CodeDomConstruct instance,
      PropertyInfo property)
    {
      return (CodeDomConstruct) CodeDomConstruct.Expression(property.PropertyType, (CodeExpression) new CodePropertyReferenceExpression(instance == null ? (CodeExpression) new CodeTypeReferenceExpression(property.DeclaringType) : instance.AsExpression(), property.Name));
    }

    protected override CodeDomConstruct EmitGetFieldExpression(
      CodeDomContext context,
      CodeDomConstruct instance,
      FieldInfo field)
    {
      return (CodeDomConstruct) CodeDomConstruct.Expression(field.FieldType, (CodeExpression) new CodeFieldReferenceExpression(instance == null ? (CodeExpression) new CodeTypeReferenceExpression(field.DeclaringType) : instance.AsExpression(), field.Name));
    }

    protected override CodeDomConstruct EmitSetProperty(
      CodeDomContext context,
      CodeDomConstruct instance,
      PropertyInfo property,
      CodeDomConstruct value)
    {
      return (CodeDomConstruct) CodeDomConstruct.Statement((CodeStatement) new CodeAssignStatement((CodeExpression) new CodePropertyReferenceExpression(instance == null ? (CodeExpression) new CodeTypeReferenceExpression(property.DeclaringType) : instance.AsExpression(), property.Name), value.AsExpression()));
    }

    protected override CodeDomConstruct EmitSetField(
      CodeDomContext context,
      CodeDomConstruct instance,
      FieldInfo field,
      CodeDomConstruct value)
    {
      return (CodeDomConstruct) CodeDomConstruct.Statement((CodeStatement) new CodeAssignStatement((CodeExpression) new CodeFieldReferenceExpression(instance == null ? (CodeExpression) new CodeTypeReferenceExpression(field.DeclaringType) : instance.AsExpression(), field.Name), value.AsExpression()));
    }

    protected override CodeDomConstruct EmitLoadVariableExpression(
      CodeDomContext context,
      CodeDomConstruct variable)
    {
      return (CodeDomConstruct) CodeDomConstruct.Expression(variable.ContextType, variable.AsExpression());
    }

    protected override CodeDomConstruct EmitStoreVariableStatement(
      CodeDomContext context,
      CodeDomConstruct variable,
      CodeDomConstruct value)
    {
      return (CodeDomConstruct) CodeDomConstruct.Statement((CodeStatement) new CodeAssignStatement(variable.AsExpression(), value.AsExpression()));
    }

    protected override CodeDomConstruct EmitThrowExpression(
      CodeDomContext context,
      Type expressionType,
      CodeDomConstruct exceptionExpression)
    {
      return (CodeDomConstruct) CodeDomConstruct.Statement((CodeStatement) new CodeThrowExceptionStatement(exceptionExpression.AsExpression()));
    }

    protected override CodeDomConstruct EmitTryFinally(
      CodeDomContext context,
      CodeDomConstruct tryStatement,
      CodeDomConstruct finallyStatement)
    {
      return (CodeDomConstruct) CodeDomConstruct.Statement((CodeStatement) new CodeTryCatchFinallyStatement(tryStatement.AsStatements().ToArray<CodeStatement>(), CodeDomContext.EmptyCatches, finallyStatement.AsStatements().ToArray<CodeStatement>()));
    }

    protected override CodeDomConstruct EmitCreateNewArrayExpression(
      CodeDomContext context,
      Type elementType,
      int length)
    {
      return (CodeDomConstruct) CodeDomConstruct.Expression(elementType.MakeArrayType(), (CodeExpression) new CodeArrayCreateExpression(elementType, (CodeExpression) new CodePrimitiveExpression((object) length)));
    }

    protected override CodeDomConstruct EmitCreateNewArrayExpression(
      CodeDomContext context,
      Type elementType,
      int length,
      IEnumerable<CodeDomConstruct> initialElements)
    {
      return (CodeDomConstruct) CodeDomConstruct.Expression(elementType.MakeArrayType(), (CodeExpression) new CodeArrayCreateExpression(elementType, initialElements.Select<CodeDomConstruct, CodeExpression>((Func<CodeDomConstruct, CodeExpression>) (i => i.AsExpression())).ToArray<CodeExpression>())
      {
        Size = length
      });
    }

    protected override CodeDomConstruct EmitSetArrayElementStatement(
      CodeDomContext context,
      CodeDomConstruct array,
      CodeDomConstruct index,
      CodeDomConstruct value)
    {
      return (CodeDomConstruct) CodeDomConstruct.Statement((CodeStatement) new CodeAssignStatement((CodeExpression) new CodeArrayIndexerExpression(array.AsExpression(), new CodeExpression[1]
      {
        index.AsExpression()
      }), value.AsExpression()));
    }

    protected override CodeDomConstruct EmitGetSerializerExpression(
      CodeDomContext context,
      Type targetType,
      SerializingMember? memberInfo,
      PolymorphismSchema itemsSchema)
    {
      return (CodeDomConstruct) CodeDomConstruct.Expression(typeof (MessagePackSerializer<>).MakeGenericType(targetType), (CodeExpression) new CodeFieldReferenceExpression((CodeExpression) new CodeThisReferenceExpression(), context.GetSerializerFieldName(targetType, !memberInfo.HasValue ? EnumMemberSerializationMethod.Default : memberInfo.Value.GetEnumMemberSerializationMethod(), !memberInfo.HasValue ? DateTimeMemberConversionMethod.Default : memberInfo.Value.GetDateTimeMemberConversionMethod(), itemsSchema ?? PolymorphismSchema.Create(targetType, memberInfo))));
    }

    protected override CodeDomConstruct EmitConditionalExpression(
      CodeDomContext context,
      CodeDomConstruct conditionExpression,
      CodeDomConstruct thenExpression,
      CodeDomConstruct elseExpression)
    {
      return elseExpression != null ? (!(thenExpression.ContextType == typeof (void)) && !thenExpression.IsStatement ? (CodeDomConstruct) CodeDomConstruct.Expression(thenExpression.ContextType, (CodeExpression) new CodeMethodInvokeExpression((CodeExpression) null, "__Conditional", new CodeExpression[3]
      {
        conditionExpression.AsExpression(),
        thenExpression.AsExpression(),
        elseExpression.AsExpression()
      })) : (CodeDomConstruct) CodeDomConstruct.Statement((CodeStatement) new CodeConditionStatement(conditionExpression.AsExpression(), thenExpression.AsStatements().ToArray<CodeStatement>(), elseExpression.AsStatements().ToArray<CodeStatement>()))) : (CodeDomConstruct) CodeDomConstruct.Statement((CodeStatement) new CodeConditionStatement(conditionExpression.AsExpression(), thenExpression.AsStatements().ToArray<CodeStatement>()));
    }

    protected override CodeDomConstruct EmitAndConditionalExpression(
      CodeDomContext context,
      IList<CodeDomConstruct> conditionExpressions,
      CodeDomConstruct thenExpression,
      CodeDomConstruct elseExpression)
    {
      return (CodeDomConstruct) CodeDomConstruct.Statement((CodeStatement) new CodeConditionStatement(conditionExpressions.Select<CodeDomConstruct, CodeExpression>((Func<CodeDomConstruct, CodeExpression>) (c => c.AsExpression())).Aggregate<CodeExpression>((Func<CodeExpression, CodeExpression, CodeExpression>) ((l, r) => (CodeExpression) new CodeBinaryOperatorExpression(l, CodeBinaryOperatorType.BooleanAnd, r))), thenExpression.AsStatements().ToArray<CodeStatement>(), elseExpression.AsStatements().ToArray<CodeStatement>()));
    }

    protected override CodeDomConstruct EmitStringSwitchStatement(
      CodeDomContext context,
      CodeDomConstruct target,
      IDictionary<string, CodeDomConstruct> cases,
      CodeDomConstruct defaultCase)
    {
      return (CodeDomConstruct) CodeDomConstruct.Statement((CodeStatement) cases.Aggregate<KeyValuePair<string, CodeDomConstruct>, CodeConditionStatement>((CodeConditionStatement) null, (Func<CodeConditionStatement, KeyValuePair<string, CodeDomConstruct>, CodeConditionStatement>) ((current, caseStatement) =>
      {
        CodeBinaryOperatorExpression operatorExpression = new CodeBinaryOperatorExpression(target.AsExpression(), CodeBinaryOperatorType.ValueEquality, (CodeExpression) new CodePrimitiveExpression((object) caseStatement.Key));
        CodeStatement[] array = caseStatement.Value.AsStatements().ToArray<CodeStatement>();
        CodeStatement[] falseStatements;
        if (current != null)
          falseStatements = new CodeStatement[1]
          {
            (CodeStatement) current
          };
        else
          falseStatements = defaultCase.AsStatements().ToArray<CodeStatement>();
        return new CodeConditionStatement((CodeExpression) operatorExpression, array, falseStatements);
      })));
    }

    protected override CodeDomConstruct EmitRetrunStatement(
      CodeDomContext context,
      CodeDomConstruct expression)
    {
      return (CodeDomConstruct) CodeDomConstruct.Statement((CodeStatement) new CodeMethodReturnStatement(expression.AsExpression()));
    }

    protected override CodeDomConstruct EmitForLoop(
      CodeDomContext context,
      CodeDomConstruct count,
      Func<SerializerBuilder<CodeDomContext, CodeDomConstruct, TObject>.ForLoopContext, CodeDomConstruct> loopBodyEmitter)
    {
      string uniqueVariableName = context.GetUniqueVariableName("i");
      CodeDomConstruct counter = CodeDomConstruct.Variable(typeof (int), uniqueVariableName);
      SerializerBuilder<CodeDomContext, CodeDomConstruct, TObject>.ForLoopContext forLoopContext = new SerializerBuilder<CodeDomContext, CodeDomConstruct, TObject>.ForLoopContext(counter);
      return (CodeDomConstruct) CodeDomConstruct.Statement((CodeStatement) new CodeIterationStatement((CodeStatement) new CodeVariableDeclarationStatement(typeof (int), uniqueVariableName, (CodeExpression) new CodePrimitiveExpression((object) 0)), (CodeExpression) new CodeBinaryOperatorExpression(counter.AsExpression(), CodeBinaryOperatorType.LessThan, count.AsExpression()), (CodeStatement) new CodeAssignStatement(counter.AsExpression(), (CodeExpression) new CodeBinaryOperatorExpression(counter.AsExpression(), CodeBinaryOperatorType.Add, (CodeExpression) new CodePrimitiveExpression((object) 1))), loopBodyEmitter(forLoopContext).AsStatements().ToArray<CodeStatement>()));
    }

    protected override CodeDomConstruct EmitForEachLoop(
      CodeDomContext context,
      CollectionTraits collectionTraits,
      CodeDomConstruct collection,
      Func<CodeDomConstruct, CodeDomConstruct> loopBodyEmitter)
    {
      string uniqueVariableName1 = context.GetUniqueVariableName("enumerator");
      string uniqueVariableName2 = context.GetUniqueVariableName("current");
      bool flag = typeof (IDisposable).IsAssignableFrom(collectionTraits.GetEnumeratorMethod.ReturnType);
      CodeDomConstruct codeDomConstruct1 = CodeDomConstruct.Variable(collectionTraits.GetEnumeratorMethod.ReturnType, uniqueVariableName1);
      CodeDomConstruct codeDomConstruct2 = CodeDomConstruct.Variable(collectionTraits.ElementType, uniqueVariableName2);
      StatementCodeDomConstruct codeDomConstruct3 = CodeDomConstruct.Statement((CodeStatement) new CodeIterationStatement((CodeStatement) new CodeSnippetStatement(string.Empty), (CodeExpression) new CodeMethodInvokeExpression(codeDomConstruct1.AsExpression(), "MoveNext", new CodeExpression[0]), (CodeStatement) new CodeSnippetStatement(string.Empty), ((IEnumerable<CodeStatement>) new CodeStatement[1]
      {
        (CodeStatement) new CodeAssignStatement(codeDomConstruct2.AsExpression(), (CodeExpression) new CodePropertyReferenceExpression(codeDomConstruct1.AsExpression(), collectionTraits.GetEnumeratorMethod.ReturnType == typeof (IDictionaryEnumerator) ? "Entry" : "Current"))
      }).Concat<CodeStatement>(loopBodyEmitter(codeDomConstruct2).AsStatements()).ToArray<CodeStatement>()));
      List<CodeDomConstruct> codeDomConstructList = new List<CodeDomConstruct>()
      {
        (CodeDomConstruct) CodeDomConstruct.Statement((CodeStatement) new CodeVariableDeclarationStatement(codeDomConstruct1.ContextType, uniqueVariableName1, (CodeExpression) new CodeMethodInvokeExpression(collection.AsExpression(), "GetEnumerator", new CodeExpression[0]))),
        (CodeDomConstruct) CodeDomConstruct.Statement((CodeStatement) new CodeVariableDeclarationStatement(codeDomConstruct2.ContextType, uniqueVariableName2))
      };
      if (flag)
        codeDomConstructList.Add((CodeDomConstruct) CodeDomConstruct.Statement((CodeStatement) new CodeTryCatchFinallyStatement(codeDomConstruct3.AsStatements().ToArray<CodeStatement>(), CodeDomContext.EmptyCatches, new CodeStatement[1]
        {
          (CodeStatement) new CodeExpressionStatement((CodeExpression) new CodeMethodInvokeExpression(codeDomConstruct1.AsExpression(), "Dispose", new CodeExpression[0]))
        })));
      else
        codeDomConstructList.Add((CodeDomConstruct) codeDomConstruct3);
      return this.EmitSequentialStatements(context, typeof (void), codeDomConstructList.ToArray());
    }

    protected override CodeDomConstruct EmitEnumFromUnderlyingCastExpression(
      CodeDomContext context,
      Type enumType,
      CodeDomConstruct underlyingValue)
    {
      return (CodeDomConstruct) CodeDomConstruct.Expression(enumType, (CodeExpression) new CodeCastExpression(enumType, underlyingValue.AsExpression()));
    }

    protected override CodeDomConstruct EmitEnumToUnderlyingCastExpression(
      CodeDomContext context,
      Type underlyingType,
      CodeDomConstruct enumValue)
    {
      return (CodeDomConstruct) CodeDomConstruct.Expression(underlyingType, (CodeExpression) new CodeCastExpression(underlyingType, enumValue.AsExpression()));
    }

    protected override void BuildSerializerCodeCore(
      ISerializerCodeGenerationContext context,
      Type concreteType,
      PolymorphismSchema itemSchema)
    {
      if (!(context is CodeDomContext context1))
        throw new ArgumentException("'context' was not created with CreateGenerationContextForCodeGeneration method.", nameof (context));
      context1.Reset(typeof (TObject), SerializerBuilder<CodeDomContext, CodeDomConstruct, TObject>.BaseClass);
      if (!typeof (TObject).GetIsEnum())
        this.BuildSerializer(context1, concreteType, itemSchema);
      else
        this.BuildEnumSerializer(context1);
      this.Finish(context1, typeof (TObject).GetIsEnum());
    }

    protected override Func<SerializationContext, MessagePackSerializer<TObject>> CreateSerializerConstructor(
      CodeDomContext codeGenerationContext,
      PolymorphismSchema schema)
    {
      this.Finish(codeGenerationContext, false);
      ParameterExpression parameterExpression;
      return Expression.Lambda<Func<SerializationContext, MessagePackSerializer<TObject>>>((Expression) Expression.New(((IEnumerable<ConstructorInfo>) CodeDomSerializerBuilder<TObject>.PrepareSerializerConstructorCreation(codeGenerationContext).GetConstructors()).Single<ConstructorInfo>(), (Expression) parameterExpression), parameterExpression).Compile();
    }

    protected override Func<SerializationContext, MessagePackSerializer<TObject>> CreateEnumSerializerConstructor(
      CodeDomContext codeGenerationContext)
    {
      this.Finish(codeGenerationContext, true);
      ParameterExpression parameterExpression;
      return Expression.Lambda<Func<SerializationContext, MessagePackSerializer<TObject>>>((Expression) Expression.New(((IEnumerable<ConstructorInfo>) CodeDomSerializerBuilder<TObject>.PrepareSerializerConstructorCreation(codeGenerationContext).GetConstructors()).Single<ConstructorInfo>((Func<ConstructorInfo, bool>) (c => c.GetParameters().Length == 1)), (Expression) parameterExpression), parameterExpression).Compile();
    }

    [SecuritySafeCritical]
    private static Type PrepareSerializerConstructorCreation(
      CodeDomContext codeGenerationContext)
    {
      if (!SerializerDebugging.OnTheFlyCodeDomEnabled)
        throw new NotSupportedException();
      CodeCompileUnit cu = codeGenerationContext.CreateCodeCompileUnit();
      CompilerResults cr;
      using (CodeDomProvider provider = CodeDomProvider.CreateProvider("cs"))
      {
        if (SerializerDebugging.DumpEnabled)
        {
          SerializerDebugging.TraceEvent("Compile {0}", (object) codeGenerationContext.DeclaringType.Name);
          provider.GenerateCodeFromCompileUnit(cu, SerializerDebugging.ILTraceWriter, new CodeGeneratorOptions());
          SerializerDebugging.FlushTraceData();
        }
        cr = provider.CompileAssemblyFromDom(new CompilerParameters(SerializerDebugging.CodeDomSerializerDependentAssemblies.ToArray<string>()), cu);
        if (cr.Errors.OfType<CompilerError>().Where<CompilerError>((Func<CompilerError, bool>) (e => !e.IsWarning)).ToArray<CompilerError>().Length > 0)
        {
          if (SerializerDebugging.TraceEnabled)
          {
            provider.GenerateCodeFromCompileUnit(cu, SerializerDebugging.ILTraceWriter, new CodeGeneratorOptions());
            SerializerDebugging.FlushTraceData();
          }
          throw new SerializationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Failed to compile assembly. Details:{0}{1}", (object) Environment.NewLine, (object) CodeDomSerializerBuilder<TObject>.BuildCompilationError(cr)));
        }
      }
      if (SerializerDebugging.TraceEnabled)
        SerializerDebugging.TraceEvent("Build assembly '{0}' from dom.", (object) cr.PathToAssembly);
      SerializerDebugging.AddCompiledCodeDomAssembly(cr.PathToAssembly);
      return ((IEnumerable<Type>) cr.CompiledAssembly.GetTypes()).SingleOrDefault<Type>((Func<Type, bool>) (t => t.Namespace == cu.Namespaces[0].Name && t.Name == codeGenerationContext.DeclaringType.Name));
    }

    private static string BuildCompilationError(CompilerResults cr)
    {
      return string.Join(Environment.NewLine, cr.Errors.OfType<CompilerError>().Select<CompilerError, string>((Func<CompilerError, int, string>) ((error, i) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[{0}]{1}:{2}:(File:{3}, Line:{4}, Column:{5}):{6}", (object) i, error.IsWarning ? (object) "Warning" : (object) "Error   ", (object) error.ErrorNumber, (object) error.FileName, (object) error.Line, (object) error.Column, (object) error.ErrorText))).ToArray<string>());
    }

    private void Finish(CodeDomContext context, bool isEnum)
    {
      foreach (KeyValuePair<SerializerFieldKey, string> dependentSerializer in context.GetDependentSerializers())
      {
        Type typeFromHandle = Type.GetTypeFromHandle(dependentSerializer.Key.TypeHandle);
        context.DeclaringType.Members.Add((CodeTypeMember) new CodeMemberField(typeof (MessagePackSerializer<>).MakeGenericType(typeFromHandle), dependentSerializer.Value));
      }
      foreach (KeyValuePair<RuntimeFieldHandle, CodeDomContext.CachedFieldInfo> cachedFieldInfo in context.GetCachedFieldInfos())
        context.DeclaringType.Members.Add((CodeTypeMember) new CodeMemberField(typeof (FieldInfo), cachedFieldInfo.Value.StorageFieldName));
      foreach (KeyValuePair<RuntimeMethodHandle, CodeDomContext.CachedMethodBase> cachedMethodBase in context.GetCachedMethodBases())
        context.DeclaringType.Members.Add((CodeTypeMember) new CodeMemberField(typeof (MethodBase), cachedMethodBase.Value.StorageFieldName));
      if (isEnum)
      {
        CodeConstructor codeConstructor1 = new CodeConstructor();
        codeConstructor1.Attributes = MemberAttributes.Public;
        CodeConstructor codeConstructor2 = codeConstructor1;
        codeConstructor2.Parameters.Add(new CodeParameterDeclarationExpression(typeof (SerializationContext), nameof (context)));
        codeConstructor2.ChainedConstructorArgs.Add((CodeExpression) new CodeArgumentReferenceExpression(nameof (context)));
        codeConstructor2.ChainedConstructorArgs.Add((CodeExpression) new CodeFieldReferenceExpression((CodeExpression) new CodeTypeReferenceExpression(typeof (EnumSerializationMethod)), EnumMessagePackSerializerHelpers.DetermineEnumSerializationMethod(context.SerializationContext, typeof (TObject), EnumMemberSerializationMethod.Default).ToString()));
        context.DeclaringType.Members.Add((CodeTypeMember) codeConstructor2);
        CodeConstructor codeConstructor3 = new CodeConstructor();
        codeConstructor3.Attributes = MemberAttributes.Public;
        CodeConstructor codeConstructor4 = codeConstructor3;
        codeConstructor4.Parameters.Add(new CodeParameterDeclarationExpression(typeof (SerializationContext), nameof (context)));
        codeConstructor4.Parameters.Add(new CodeParameterDeclarationExpression(typeof (EnumSerializationMethod), "enumSerializationMethod"));
        codeConstructor4.BaseConstructorArgs.Add((CodeExpression) new CodeArgumentReferenceExpression(nameof (context)));
        codeConstructor4.BaseConstructorArgs.Add((CodeExpression) new CodeArgumentReferenceExpression("enumSerializationMethod"));
        context.DeclaringType.Members.Add((CodeTypeMember) codeConstructor4);
      }
      else
      {
        CodeConstructor codeConstructor1 = new CodeConstructor();
        codeConstructor1.Attributes = MemberAttributes.Public;
        CodeConstructor codeConstructor2 = codeConstructor1;
        codeConstructor2.Parameters.Add(new CodeParameterDeclarationExpression(typeof (SerializationContext), nameof (context)));
        CodeArgumentReferenceExpression referenceExpression = new CodeArgumentReferenceExpression(nameof (context));
        codeConstructor2.BaseConstructorArgs.Add((CodeExpression) referenceExpression);
        if (((IEnumerable<ConstructorInfo>) SerializerBuilder<CodeDomContext, CodeDomConstruct, TObject>.BaseClass.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)).Any<ConstructorInfo>((Func<ConstructorInfo, bool>) (c => ((IEnumerable<ParameterInfo>) c.GetParameters()).Select<ParameterInfo, Type>((Func<ParameterInfo, Type>) (p => p.ParameterType)).SequenceEqual<Type>((IEnumerable<Type>) CollectionSerializerHelpers.CollectionConstructorTypes))))
          codeConstructor2.BaseConstructorArgs.Add((CodeExpression) new CodeMethodInvokeExpression(new CodeMethodReferenceExpression((CodeExpression) new CodeTypeReferenceExpression(context.DeclaringType.Name), "RestoreSchema"), new CodeExpression[0]));
        int num = -1;
        foreach (KeyValuePair<SerializerFieldKey, string> dependentSerializer in context.GetDependentSerializers())
        {
          Type typeFromHandle = Type.GetTypeFromHandle(dependentSerializer.Key.TypeHandle);
          if (typeFromHandle.GetIsEnum())
            codeConstructor2.Statements.Add((CodeStatement) new CodeAssignStatement((CodeExpression) new CodeFieldReferenceExpression((CodeExpression) new CodeThisReferenceExpression(), dependentSerializer.Value), (CodeExpression) new CodeMethodInvokeExpression(new CodeMethodReferenceExpression((CodeExpression) referenceExpression, "GetSerializer", new CodeTypeReference[1]
            {
              new CodeTypeReference(typeFromHandle)
            }), new CodeExpression[1]
            {
              (CodeExpression) new CodeMethodInvokeExpression((CodeExpression) new CodeTypeReferenceExpression(typeof (EnumMessagePackSerializerHelpers)), _EnumMessagePackSerializerHelpers.DetermineEnumSerializationMethodMethod.Name, new CodeExpression[3]
              {
                (CodeExpression) referenceExpression,
                (CodeExpression) new CodeTypeOfExpression(typeFromHandle),
                (CodeExpression) new CodeFieldReferenceExpression((CodeExpression) new CodeTypeReferenceExpression(typeof (EnumMemberSerializationMethod)), dependentSerializer.Key.EnumSerializationMethod.ToString())
              })
            })));
          else if (DateTimeMessagePackSerializerHelpers.IsDateTime(typeFromHandle))
          {
            codeConstructor2.Statements.Add((CodeStatement) new CodeAssignStatement((CodeExpression) new CodeFieldReferenceExpression((CodeExpression) new CodeThisReferenceExpression(), dependentSerializer.Value), (CodeExpression) new CodeMethodInvokeExpression(new CodeMethodReferenceExpression((CodeExpression) referenceExpression, "GetSerializer", new CodeTypeReference[1]
            {
              new CodeTypeReference(typeFromHandle)
            }), new CodeExpression[1]
            {
              (CodeExpression) new CodeMethodInvokeExpression((CodeExpression) new CodeTypeReferenceExpression(typeof (DateTimeMessagePackSerializerHelpers)), _DateTimeMessagePackSerializerHelpers.DetermineDateTimeConversionMethodMethod.Name, new CodeExpression[2]
              {
                (CodeExpression) referenceExpression,
                (CodeExpression) new CodeFieldReferenceExpression((CodeExpression) new CodeTypeReferenceExpression(typeof (DateTimeMemberConversionMethod)), dependentSerializer.Key.DateTimeConversionMethod.ToString())
              })
            })));
          }
          else
          {
            CodeExpression codeExpression;
            if (dependentSerializer.Key.PolymorphismSchema == null)
            {
              codeExpression = (CodeExpression) new CodePrimitiveExpression((object) null);
            }
            else
            {
              ++num;
              string str = "schema" + (object) num;
              CodeDomConstruct storage = this.DeclareLocal(context, typeof (PolymorphismSchema), str);
              codeConstructor2.Statements.AddRange(storage.AsStatements().ToArray<CodeStatement>());
              codeConstructor2.Statements.AddRange(this.EmitConstructPolymorphismSchema(context, storage, dependentSerializer.Key.PolymorphismSchema).SelectMany<CodeDomConstruct, CodeStatement>((Func<CodeDomConstruct, IEnumerable<CodeStatement>>) (st => st.AsStatements())).ToArray<CodeStatement>());
              codeExpression = (CodeExpression) new CodeVariableReferenceExpression(str);
            }
            codeConstructor2.Statements.Add((CodeStatement) new CodeAssignStatement((CodeExpression) new CodeFieldReferenceExpression((CodeExpression) new CodeThisReferenceExpression(), dependentSerializer.Value), (CodeExpression) new CodeMethodInvokeExpression(new CodeMethodReferenceExpression((CodeExpression) referenceExpression, "GetSerializer", new CodeTypeReference[1]
            {
              new CodeTypeReference(typeFromHandle)
            }), new CodeExpression[1]{ codeExpression })));
          }
        }
        foreach (KeyValuePair<RuntimeFieldHandle, CodeDomContext.CachedFieldInfo> cachedFieldInfo in context.GetCachedFieldInfos())
        {
          FieldInfo target = cachedFieldInfo.Value.Target;
          codeConstructor2.Statements.Add((CodeStatement) new CodeAssignStatement((CodeExpression) new CodeFieldReferenceExpression((CodeExpression) new CodeThisReferenceExpression(), cachedFieldInfo.Value.StorageFieldName), (CodeExpression) new CodeMethodInvokeExpression(new CodeMethodReferenceExpression((CodeExpression) new CodeTypeOfExpression(target.DeclaringType), "GetField"), new CodeExpression[2]
          {
            (CodeExpression) new CodePrimitiveExpression((object) target.Name),
            (CodeExpression) new CodeBinaryOperatorExpression((CodeExpression) new CodeFieldReferenceExpression((CodeExpression) new CodeTypeReferenceExpression(typeof (BindingFlags)), "Instance"), CodeBinaryOperatorType.BitwiseOr, (CodeExpression) new CodeBinaryOperatorExpression((CodeExpression) new CodeFieldReferenceExpression((CodeExpression) new CodeTypeReferenceExpression(typeof (BindingFlags)), "Public"), CodeBinaryOperatorType.BitwiseOr, (CodeExpression) new CodeFieldReferenceExpression((CodeExpression) new CodeTypeReferenceExpression(typeof (BindingFlags)), "NonPublic")))
          })));
        }
        foreach (KeyValuePair<RuntimeMethodHandle, CodeDomContext.CachedMethodBase> cachedMethodBase in context.GetCachedMethodBases())
        {
          MethodBase target = cachedMethodBase.Value.Target;
          codeConstructor2.Statements.Add((CodeStatement) new CodeAssignStatement((CodeExpression) new CodeFieldReferenceExpression((CodeExpression) new CodeThisReferenceExpression(), cachedMethodBase.Value.StorageFieldName), (CodeExpression) new CodeMethodInvokeExpression(new CodeMethodReferenceExpression((CodeExpression) new CodeTypeOfExpression(target.DeclaringType), "GetMethod"), new CodeExpression[5]
          {
            (CodeExpression) new CodePrimitiveExpression((object) target.Name),
            (CodeExpression) new CodeBinaryOperatorExpression((CodeExpression) new CodeFieldReferenceExpression((CodeExpression) new CodeTypeReferenceExpression(typeof (BindingFlags)), "Instance"), CodeBinaryOperatorType.BitwiseOr, (CodeExpression) new CodeBinaryOperatorExpression((CodeExpression) new CodeFieldReferenceExpression((CodeExpression) new CodeTypeReferenceExpression(typeof (BindingFlags)), "Public"), CodeBinaryOperatorType.BitwiseOr, (CodeExpression) new CodeFieldReferenceExpression((CodeExpression) new CodeTypeReferenceExpression(typeof (BindingFlags)), "NonPublic"))),
            (CodeExpression) new CodePrimitiveExpression((object) null),
            (CodeExpression) new CodeArrayCreateExpression(typeof (Type), (CodeExpression[]) ((IEnumerable<ParameterInfo>) target.GetParameters()).Select<ParameterInfo, CodeTypeOfExpression>((Func<ParameterInfo, CodeTypeOfExpression>) (pi => new CodeTypeOfExpression(pi.ParameterType))).ToArray<CodeTypeOfExpression>()),
            (CodeExpression) new CodePrimitiveExpression((object) null)
          })));
        }
        context.DeclaringType.Members.Add((CodeTypeMember) codeConstructor2);
      }
      CodeTypeParameter typeParameter = new CodeTypeParameter("T");
      CodeTypeReference type = new CodeTypeReference(typeParameter);
      CodeMemberMethod codeMemberMethod1 = new CodeMemberMethod();
      codeMemberMethod1.Name = "__Conditional";
      codeMemberMethod1.Attributes = MemberAttributes.Static | MemberAttributes.Private;
      codeMemberMethod1.ReturnType = type;
      CodeMemberMethod codeMemberMethod2 = codeMemberMethod1;
      codeMemberMethod2.TypeParameters.Add(typeParameter);
      codeMemberMethod2.Parameters.Add(new CodeParameterDeclarationExpression(typeof (bool), "condition"));
      codeMemberMethod2.Parameters.Add(new CodeParameterDeclarationExpression(type, "whenTrue"));
      codeMemberMethod2.Parameters.Add(new CodeParameterDeclarationExpression(type, "whenFalse"));
      codeMemberMethod2.Statements.Add((CodeStatement) new CodeConditionStatement((CodeExpression) new CodeArgumentReferenceExpression("condition"), new CodeStatement[1]
      {
        (CodeStatement) new CodeMethodReturnStatement((CodeExpression) new CodeArgumentReferenceExpression("whenTrue"))
      }, new CodeStatement[1]
      {
        (CodeStatement) new CodeMethodReturnStatement((CodeExpression) new CodeArgumentReferenceExpression("whenFalse"))
      }));
      context.DeclaringType.Members.Add((CodeTypeMember) codeMemberMethod2);
    }

    protected override CodeDomContext CreateCodeGenerationContextForSerializerCreation(
      SerializationContext context)
    {
      CodeDomContext codeDomContext = new CodeDomContext(context, new SerializerCodeGenerationConfiguration());
      codeDomContext.Reset(typeof (TObject), SerializerBuilder<CodeDomContext, CodeDomConstruct, TObject>.BaseClass);
      return codeDomContext;
    }
  }
}
