// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.CodeDomSerializers.CodeDomConstruct
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using MsgPack.Serialization.AbstractSerializers;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Globalization;

namespace MsgPack.Serialization.CodeDomSerializers
{
  internal class CodeDomConstruct : ICodeConstruct
  {
    private readonly Type _contextType;

    public Type ContextType
    {
      get
      {
        return this._contextType;
      }
    }

    public virtual bool IsArgument
    {
      get
      {
        return false;
      }
    }

    public virtual bool IsExpression
    {
      get
      {
        return false;
      }
    }

    public virtual bool IsStatement
    {
      get
      {
        return false;
      }
    }

    public virtual CodeParameterDeclarationExpression AsParameter()
    {
      throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot get '{0}' as parameter declaration.", (object) this));
    }

    public virtual CodeArgumentReferenceExpression AsArgument()
    {
      throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot get '{0}' as argument reference expression.", (object) this));
    }

    public virtual CodeExpression AsExpression()
    {
      throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot get '{0}' as expression.", (object) this));
    }

    public virtual IEnumerable<CodeStatement> AsStatements()
    {
      throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot get '{0}' as statements.", (object) this));
    }

    public virtual void AddStatements(CodeStatementCollection collection)
    {
      throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot get '{0}' as statements.", (object) this));
    }

    protected CodeDomConstruct(Type contextType)
    {
      this._contextType = contextType;
    }

    public static ParameterCodeDomConstruct Parameter(
      Type type,
      string name)
    {
      return new ParameterCodeDomConstruct(type, name);
    }

    public static StatementCodeDomConstruct Statement(
      params CodeStatement[] statement)
    {
      return new StatementCodeDomConstruct((IEnumerable<CodeStatement>) statement);
    }

    public static StatementCodeDomConstruct Statement(
      IEnumerable<CodeStatement> statements)
    {
      return new StatementCodeDomConstruct(statements);
    }

    public static ExpressionCodeDomConstruct Expression(
      Type contextType,
      CodeExpression expression)
    {
      return new ExpressionCodeDomConstruct(contextType, expression);
    }

    public static CodeDomConstruct Variable(Type type, string name)
    {
      return (CodeDomConstruct) new VariableCodeDomConstruct(type, name);
    }
  }
}
