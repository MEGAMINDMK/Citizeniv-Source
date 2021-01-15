// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.ExpressionSerializers.ExpressionConstruct
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using MsgPack.Serialization.AbstractSerializers;
using System;
using System.IO;
using System.Linq.Expressions;

namespace MsgPack.Serialization.ExpressionSerializers
{
  internal sealed class ExpressionConstruct : ICodeConstruct
  {
    private readonly Expression _expression;
    private readonly bool _isSignificantReference;

    public Expression Expression
    {
      get
      {
        return this._expression;
      }
    }

    public bool IsSignificantReference
    {
      get
      {
        return this._isSignificantReference;
      }
    }

    public Type ContextType
    {
      get
      {
        return this._expression.Type;
      }
    }

    public ExpressionConstruct(Expression expression)
      : this(expression, false)
    {
    }

    public ExpressionConstruct(Expression expression, bool isSignificantReference)
    {
      this._expression = expression;
      this._isSignificantReference = isSignificantReference;
    }

    public static implicit operator ExpressionConstruct(Expression expression)
    {
      return expression != null ? new ExpressionConstruct(expression) : (ExpressionConstruct) null;
    }

    public static implicit operator Expression(ExpressionConstruct construct)
    {
      return construct?.Expression;
    }

    internal void ToString(TextWriter textWriter)
    {
      this.ToString(textWriter, 0);
    }

    private void ToString(TextWriter textWriter, int indentLevel)
    {
      new ExpressionDumper(textWriter, indentLevel).Visit(this.Expression);
    }
  }
}
