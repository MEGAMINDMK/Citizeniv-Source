// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.EmittingSerializers.ILConstruct
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using MsgPack.Serialization.AbstractSerializers;
using MsgPack.Serialization.Reflection;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;

namespace MsgPack.Serialization.EmittingSerializers
{
  internal abstract class ILConstruct : ICodeConstruct
  {
    public static readonly ILConstruct[] NoArguments = new ILConstruct[0];
    private readonly Type _contextType;

    public Type ContextType
    {
      get
      {
        return this._contextType;
      }
    }

    public virtual bool IsTerminating
    {
      get
      {
        return false;
      }
    }

    protected ILConstruct(Type contextType)
    {
      this._contextType = contextType;
    }

    public virtual void Evaluate(TracingILGenerator il)
    {
      throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "'{0}' does not define stand alone instruction.", (object) this));
    }

    public virtual void LoadValue(TracingILGenerator il, bool shouldBeAddress)
    {
      throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "'{0}' does not define load value instruction.", (object) this));
    }

    public virtual void StoreValue(TracingILGenerator il)
    {
      throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "'{0}' does not define store value instruction.", (object) this));
    }

    public virtual void Branch(TracingILGenerator il, Label @else)
    {
      throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "'{0}' does not define branch instruction.", (object) this));
    }

    public static ILConstruct LoadField(ILConstruct instance, FieldInfo field)
    {
      return (ILConstruct) new LoadFieldILConstruct(instance, field);
    }

    public static ILConstruct StoreField(
      ILConstruct instance,
      FieldInfo field,
      ILConstruct value)
    {
      return (ILConstruct) new StoreFieldILConstruct(instance, field, value);
    }

    public static ILConstruct StoreLocal(ILConstruct variable, ILConstruct value)
    {
      return (ILConstruct) new StoreVariableILConstruct(variable, value);
    }

    public static ILConstruct Instruction(
      string description,
      Type contextType,
      bool isTerminating,
      Action<TracingILGenerator> instructions)
    {
      return (ILConstruct) new SinglelStepILConstruct(contextType, description, isTerminating, instructions);
    }

    public static ILConstruct Argument(int index, Type type, string name)
    {
      return (ILConstruct) new VariableILConstruct(name, type, index);
    }

    public static ILConstruct IfThenElse(
      ILConstruct conditionExpression,
      ILConstruct thenExpression,
      ILConstruct elseExpression)
    {
      return (ILConstruct) new ConditionalILConstruct(conditionExpression, thenExpression, elseExpression);
    }

    public static ILConstruct AndCondition(IList<ILConstruct> conditionExpressions)
    {
      return (ILConstruct) new AndConditionILConstruct(conditionExpressions);
    }

    public static ILConstruct UnaryOperator(
      string @operator,
      ILConstruct input,
      Action<TracingILGenerator, ILConstruct> operation)
    {
      return (ILConstruct) new UnaryOperatorILConstruct(@operator, input, operation);
    }

    public static ILConstruct UnaryOperator(
      string @operator,
      ILConstruct input,
      Action<TracingILGenerator, ILConstruct> operation,
      Action<TracingILGenerator, ILConstruct, Label> branchOperation)
    {
      return (ILConstruct) new UnaryOperatorILConstruct(@operator, input, operation, branchOperation);
    }

    public static ILConstruct BinaryOperator(
      string @operator,
      Type resultType,
      ILConstruct left,
      ILConstruct right,
      Action<TracingILGenerator, ILConstruct, ILConstruct> operation,
      Action<TracingILGenerator, ILConstruct, ILConstruct, Label> branchOperation)
    {
      return (ILConstruct) new BinaryOperatorILConstruct(@operator, resultType, left, right, operation, branchOperation);
    }

    public static ILConstruct Invoke(
      ILConstruct target,
      MethodInfo method,
      IEnumerable<ILConstruct> arguments)
    {
      return (ILConstruct) new InvocationILConsruct(method, target, arguments);
    }

    internal static ILConstruct NewObject(
      ILConstruct variable,
      ConstructorInfo constructor,
      IEnumerable<ILConstruct> arguments)
    {
      return (ILConstruct) new InvocationILConsruct(constructor, variable, arguments);
    }

    public static ILConstruct Sequence(
      Type contextType,
      IEnumerable<ILConstruct> statements)
    {
      return (ILConstruct) new SequenceILConstruct(contextType, statements);
    }

    public static ILConstruct Composite(ILConstruct before, ILConstruct context)
    {
      return (ILConstruct) new StatementExpressionILConstruct(before, context);
    }

    public static ILConstruct Literal<T>(
      Type type,
      T literalValue,
      Action<TracingILGenerator> instruction)
    {
      return (ILConstruct) new SinglelStepILConstruct(type, "literal " + ((object) literalValue == null ? "(null)" : literalValue.ToString()), false, instruction);
    }

    public static ILConstruct Variable(Type type, string name)
    {
      return (ILConstruct) new VariableILConstruct(name, type);
    }

    protected static void ValidateContextTypeMatch(ILConstruct left, ILConstruct right)
    {
      if (ILConstruct.GetNormalizedType(left.ContextType) != ILConstruct.GetNormalizedType(right.ContextType))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Right type '{1}' does not equal to left type '{0}'.", (object) left.ContextType, (object) right.ContextType), nameof (right));
    }

    private static Type GetNormalizedType(Type type)
    {
      if (!type.IsPrimitive)
        return type;
      if (type == typeof (sbyte) || type == typeof (short) || (type == typeof (int) || type == typeof (byte)) || (type == typeof (ushort) || type == typeof (uint)))
        return typeof (long);
      return type == typeof (float) ? typeof (double) : type;
    }
  }
}
