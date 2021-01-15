// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.ExpressionSerializers.ExpressionDumper
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.Globalization;
using System.IO;
using System.Linq.Expressions;

namespace MsgPack.Serialization.ExpressionSerializers
{
  internal class ExpressionDumper : ExpressionVisitor
  {
    private int _indentLevel;
    private readonly TextWriter _writer;

    public ExpressionDumper(TextWriter writer, int indentLevel)
    {
      this._writer = writer;
      this._indentLevel = indentLevel;
    }

    protected override Expression VisitBinary(BinaryExpression node)
    {
      bool flag = false;
      switch (node.NodeType)
      {
        case ExpressionType.AddChecked:
        case ExpressionType.ConvertChecked:
        case ExpressionType.MultiplyChecked:
        case ExpressionType.SubtractChecked:
        case ExpressionType.AddAssignChecked:
        case ExpressionType.MultiplyAssignChecked:
        case ExpressionType.SubtractAssignChecked:
          flag = true;
          break;
      }
      if (flag)
        this._writer.Write("checked( ");
      this.Visit(node.Left);
      switch (node.NodeType)
      {
        case ExpressionType.Add:
        case ExpressionType.AddChecked:
          this._writer.Write(" + ");
          break;
        case ExpressionType.And:
          this._writer.Write(" & ");
          break;
        case ExpressionType.AndAlso:
          this._writer.Write(" && ");
          break;
        case ExpressionType.ArrayIndex:
          this._writer.Write("[ ");
          break;
        case ExpressionType.Coalesce:
          this._writer.Write(" ?? ");
          break;
        case ExpressionType.Divide:
          this._writer.Write(" / ");
          break;
        case ExpressionType.Equal:
          this._writer.Write(" == ");
          break;
        case ExpressionType.ExclusiveOr:
          this._writer.Write(" ^ ");
          break;
        case ExpressionType.GreaterThan:
          this._writer.Write(" > ");
          break;
        case ExpressionType.GreaterThanOrEqual:
          this._writer.Write(" >= ");
          break;
        case ExpressionType.LeftShift:
          this._writer.Write(" << ");
          break;
        case ExpressionType.LessThan:
          this._writer.Write(" < ");
          break;
        case ExpressionType.LessThanOrEqual:
          this._writer.Write(" <= ");
          break;
        case ExpressionType.Modulo:
          this._writer.Write(" % ");
          break;
        case ExpressionType.Multiply:
        case ExpressionType.MultiplyChecked:
          this._writer.Write(" * ");
          break;
        case ExpressionType.NotEqual:
          this._writer.Write(" != ");
          break;
        case ExpressionType.Or:
          this._writer.Write(" | ");
          break;
        case ExpressionType.OrElse:
          this._writer.Write(" || ");
          break;
        case ExpressionType.Power:
          this._writer.Write(" `pow` ");
          break;
        case ExpressionType.RightShift:
          this._writer.Write(" >> ");
          break;
        case ExpressionType.Subtract:
        case ExpressionType.SubtractChecked:
          this._writer.Write(" - ");
          break;
        case ExpressionType.Assign:
          this._writer.Write(" = ");
          break;
        case ExpressionType.AddAssign:
        case ExpressionType.AddAssignChecked:
          this._writer.Write(" += ");
          break;
        case ExpressionType.AndAssign:
          this._writer.Write(" &= ");
          break;
        case ExpressionType.DivideAssign:
          this._writer.Write(" /= ");
          break;
        case ExpressionType.ExclusiveOrAssign:
          this._writer.Write(" ^= ");
          break;
        case ExpressionType.LeftShiftAssign:
          this._writer.Write(" <<= ");
          break;
        case ExpressionType.ModuloAssign:
          this._writer.Write(" %= ");
          break;
        case ExpressionType.MultiplyAssign:
        case ExpressionType.MultiplyAssignChecked:
          this._writer.Write(" *= ");
          break;
        case ExpressionType.OrAssign:
          this._writer.Write(" |= ");
          break;
        case ExpressionType.PowerAssign:
          this._writer.Write(" `pow`= ");
          break;
        case ExpressionType.RightShiftAssign:
          this._writer.Write(" >>= ");
          break;
        case ExpressionType.SubtractAssign:
        case ExpressionType.SubtractAssignChecked:
          this._writer.Write(" -= ");
          break;
        default:
          throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Binary operation {0}(NodeType:{1}) is not supported. Expression tree:{2}", (object) node.GetType().Name, (object) node.NodeType, (object) node));
      }
      this.Visit(node.Right);
      if (flag)
        this._writer.Write(" )");
      else if (node.NodeType == ExpressionType.ArrayIndex)
        this._writer.Write(" ]");
      return (Expression) node;
    }

    protected override Expression VisitBlock(BlockExpression node)
    {
      this.WriteIndent();
      this._writer.WriteLine('{');
      ++this._indentLevel;
      try
      {
        if (node.Variables.Count > 0)
        {
          this.WriteIndent();
          this._writer.Write("var : [");
          ++this._indentLevel;
          try
          {
            for (int index = 0; index < node.Variables.Count; ++index)
            {
              if (index == 0)
                this._writer.WriteLine();
              else
                this._writer.WriteLine(',');
              this.WriteIndent();
              this._writer.Write(node.Variables[index].Name);
              this._writer.Write(" : ");
              this._writer.Write((object) node.Variables[index].Type);
            }
          }
          finally
          {
            --this._indentLevel;
          }
          this._writer.WriteLine();
          this.WriteIndent();
          this._writer.Write(" ]");
          this._writer.WriteLine();
        }
        for (int index = 0; index < node.Expressions.Count; ++index)
        {
          if (index > 0)
            this._writer.WriteLine(';');
          this.WriteIndent();
          this.Visit(node.Expressions[index]);
          if (index == node.Expressions.Count - 1)
            this._writer.WriteLine();
        }
      }
      finally
      {
        --this._indentLevel;
      }
      this.WriteIndent();
      this._writer.Write("} -> ");
      this._writer.WriteLine((object) node.Type);
      this.WriteIndent();
      return (Expression) node;
    }

    protected override CatchBlock VisitCatchBlock(CatchBlock node)
    {
      this.ThrowUnsupportedNodeException<CatchBlock>(node);
      return base.VisitCatchBlock(node);
    }

    protected override Expression VisitConditional(ConditionalExpression node)
    {
      if (node.Type != typeof (void))
        this.VisitConditionalExpression(node);
      else if (node.IfFalse is DefaultExpression)
        this.VisitIfThenStatement(node);
      else
        this.VisitIfThenElseStatement(node);
      return (Expression) node;
    }

    private void VisitIfThenStatement(ConditionalExpression node)
    {
      this._writer.Write("if (");
      this.Visit(node.Test);
      this._writer.WriteLine(") {");
      ++this._indentLevel;
      try
      {
        this.WriteIndent();
        this.Visit(node.IfTrue);
      }
      finally
      {
        --this._indentLevel;
      }
      this._writer.WriteLine();
      this.WriteIndent();
      this._writer.Write("}");
    }

    private void VisitIfThenElseStatement(ConditionalExpression node)
    {
      this._writer.Write("if (");
      this.Visit(node.Test);
      this._writer.WriteLine(") {");
      ++this._indentLevel;
      try
      {
        this.WriteIndent();
        this.Visit(node.IfTrue);
      }
      finally
      {
        --this._indentLevel;
      }
      this._writer.WriteLine();
      this.WriteIndent();
      this._writer.WriteLine("}");
      this.WriteIndent();
      this._writer.WriteLine("else");
      this.WriteIndent();
      this._writer.WriteLine("{");
      ++this._indentLevel;
      try
      {
        this.WriteIndent();
        this.Visit(node.IfFalse);
      }
      finally
      {
        --this._indentLevel;
      }
      this._writer.WriteLine();
      this.WriteIndent();
      this._writer.Write("}");
    }

    private void VisitConditionalExpression(ConditionalExpression node)
    {
      this._writer.WriteLine('(');
      ++this._indentLevel;
      try
      {
        this.WriteIndent();
        this.Visit(node.Test);
      }
      finally
      {
        --this._indentLevel;
      }
      this._writer.WriteLine();
      this.WriteIndent();
      this._writer.WriteLine(") ? (");
      ++this._indentLevel;
      try
      {
        this.WriteIndent();
        this.Visit(node.IfTrue);
      }
      finally
      {
        --this._indentLevel;
      }
      this._writer.WriteLine();
      this.WriteIndent();
      this._writer.WriteLine(") : (");
      ++this._indentLevel;
      try
      {
        this.WriteIndent();
        this.Visit(node.IfFalse);
      }
      finally
      {
        --this._indentLevel;
      }
      this._writer.WriteLine();
      this.WriteIndent();
      this._writer.Write(" -> ");
      this._writer.Write((object) node.Type);
    }

    protected override Expression VisitConstant(ConstantExpression node)
    {
      this._writer.Write(node.ToString());
      return (Expression) node;
    }

    protected override Expression VisitDebugInfo(DebugInfoExpression node)
    {
      this.ThrowUnsupportedNodeException<DebugInfoExpression>(node);
      return base.VisitDebugInfo(node);
    }

    protected override Expression VisitDefault(DefaultExpression node)
    {
      this._writer.Write(node.ToString());
      return (Expression) node;
    }

    protected override Expression VisitDynamic(DynamicExpression node)
    {
      this.ThrowUnsupportedNodeException<DynamicExpression>(node);
      return base.VisitDynamic(node);
    }

    protected override ElementInit VisitElementInit(ElementInit node)
    {
      this.ThrowUnsupportedNodeException<ElementInit>(node);
      return base.VisitElementInit(node);
    }

    protected override Expression VisitExtension(Expression node)
    {
      this.ThrowUnsupportedNodeException(node);
      return base.VisitExtension(node);
    }

    protected override Expression VisitGoto(GotoExpression node)
    {
      this._writer.Write(node.ToString());
      return (Expression) node;
    }

    protected override Expression VisitIndex(IndexExpression node)
    {
      this.Visit(node.Object);
      this._writer.Write("[ ");
      ++this._indentLevel;
      try
      {
        for (int index = 0; index < node.Arguments.Count; ++index)
        {
          if (index > 0)
            this._writer.Write(", ");
          this.Visit(node.Arguments[index]);
        }
      }
      finally
      {
        --this._indentLevel;
      }
      this._writer.Write(" ]");
      return (Expression) node;
    }

    protected override Expression VisitInvocation(InvocationExpression node)
    {
      this.Visit(node.Expression);
      this._writer.Write('(');
      if (node.Arguments.Count > 0)
      {
        this._writer.WriteLine();
        ++this._indentLevel;
        try
        {
          for (int index = 0; index < node.Arguments.Count; ++index)
          {
            if (index > 0)
              this._writer.WriteLine(',');
            this.WriteIndent();
            this.Visit(node.Arguments[index]);
          }
        }
        finally
        {
          --this._indentLevel;
        }
        this._writer.WriteLine();
        this.WriteIndent();
      }
      this._writer.Write(") -> ");
      this._writer.Write((object) node.Type);
      return (Expression) node;
    }

    protected override Expression VisitLabel(LabelExpression node)
    {
      this._writer.WriteLine();
      this._writer.WriteLine((object) node);
      this.WriteIndent();
      return (Expression) node;
    }

    protected override LabelTarget VisitLabelTarget(LabelTarget node)
    {
      this.ThrowUnsupportedNodeException<LabelTarget>(node);
      return base.VisitLabelTarget(node);
    }

    protected override Expression VisitListInit(ListInitExpression node)
    {
      this.ThrowUnsupportedNodeException<ListInitExpression>(node);
      return base.VisitListInit(node);
    }

    protected override Expression VisitLambda<T>(Expression<T> node)
    {
      this._writer.Write('(');
      if (node.Parameters.Count > 0)
      {
        this._writer.WriteLine();
        ++this._indentLevel;
        try
        {
          for (int index = 0; index < node.Parameters.Count; ++index)
          {
            if (index > 0)
              this._writer.WriteLine(',');
            this.WriteIndent();
            this._writer.Write(node.Parameters[index].Name);
            this._writer.Write(" : ");
            this._writer.Write((object) node.Parameters[index].Type);
          }
        }
        finally
        {
          --this._indentLevel;
        }
        this._writer.WriteLine();
        this.WriteIndent();
      }
      this._writer.Write(") => (");
      ++this._indentLevel;
      try
      {
        this.Visit(node.Body);
      }
      finally
      {
        --this._indentLevel;
      }
      this._writer.Write(") -> ");
      this._writer.Write((object) node.ReturnType);
      return (Expression) node;
    }

    protected override Expression VisitLoop(LoopExpression node)
    {
      this._writer.WriteLine();
      this.WriteIndent();
      this._writer.Write("loop {");
      ++this._indentLevel;
      try
      {
        this.WriteIndent();
        this.Visit(node.Body);
      }
      finally
      {
        --this._indentLevel;
      }
      this._writer.Write("} -> ");
      this._writer.WriteLine((object) node.Type);
      return (Expression) node;
    }

    protected override Expression VisitMember(MemberExpression node)
    {
      this.Visit(node.Expression);
      this._writer.Write('.');
      this._writer.Write(node.Member.Name);
      return (Expression) node;
    }

    protected override MemberAssignment VisitMemberAssignment(MemberAssignment node)
    {
      this.ThrowUnsupportedNodeException<MemberAssignment>(node);
      return base.VisitMemberAssignment(node);
    }

    protected override MemberBinding VisitMemberBinding(MemberBinding node)
    {
      this.ThrowUnsupportedNodeException<MemberBinding>(node);
      return base.VisitMemberBinding(node);
    }

    protected override Expression VisitMemberInit(MemberInitExpression node)
    {
      this.ThrowUnsupportedNodeException<MemberInitExpression>(node);
      return base.VisitMemberInit(node);
    }

    protected override MemberListBinding VisitMemberListBinding(
      MemberListBinding node)
    {
      this.ThrowUnsupportedNodeException<MemberListBinding>(node);
      return base.VisitMemberListBinding(node);
    }

    protected override MemberMemberBinding VisitMemberMemberBinding(
      MemberMemberBinding node)
    {
      this.ThrowUnsupportedNodeException<MemberMemberBinding>(node);
      return base.VisitMemberMemberBinding(node);
    }

    protected override Expression VisitMethodCall(MethodCallExpression node)
    {
      if (node.Object == null)
      {
        if (node.Method.DeclaringType != (Type) null)
          this._writer.Write(node.Method.DeclaringType.Name);
      }
      else
        this.Visit(node.Object);
      this._writer.Write('.');
      this._writer.Write(node.Method.Name);
      this._writer.Write('(');
      if (node.Arguments.Count > 0)
      {
        this._writer.WriteLine();
        ++this._indentLevel;
        try
        {
          for (int index = 0; index < node.Arguments.Count; ++index)
          {
            if (index > 0)
              this._writer.WriteLine(',');
            this.WriteIndent();
            this.Visit(node.Arguments[index]);
          }
        }
        finally
        {
          --this._indentLevel;
        }
        this._writer.WriteLine();
        this.WriteIndent();
      }
      this._writer.Write(") -> ");
      this._writer.Write((object) node.Type);
      return (Expression) node;
    }

    protected override Expression VisitNew(NewExpression node)
    {
      this._writer.Write("new ");
      this._writer.Write((object) node.Type);
      this._writer.Write("( ");
      if (node.Arguments.Count > 0)
      {
        this._writer.WriteLine();
        ++this._indentLevel;
        for (int index = 0; index < node.Arguments.Count; ++index)
        {
          if (index > 0)
            this._writer.WriteLine(',');
          this.WriteIndent();
          this.Visit(node.Arguments[index]);
        }
        --this._indentLevel;
      }
      this._writer.Write(") ");
      return (Expression) node;
    }

    protected override Expression VisitNewArray(NewArrayExpression node)
    {
      this._writer.Write((object) node);
      return (Expression) node;
    }

    protected override Expression VisitParameter(ParameterExpression node)
    {
      this._writer.Write(node.ToString());
      return (Expression) node;
    }

    protected override Expression VisitRuntimeVariables(RuntimeVariablesExpression node)
    {
      this.ThrowUnsupportedNodeException<RuntimeVariablesExpression>(node);
      return base.VisitRuntimeVariables(node);
    }

    protected override Expression VisitSwitch(SwitchExpression node)
    {
      this._writer.Write("switch ( ");
      this._writer.Write((object) node.Comparison);
      this._writer.WriteLine(" ) {");
      foreach (SwitchCase node1 in node.Cases)
        this.VisitSwitchCase(node1);
      if (node.DefaultBody != null)
      {
        this._writer.WriteLine("default: ");
        this._writer.WriteLine(" ):");
        ++this._indentLevel;
        this.Visit(node.DefaultBody);
        --this._indentLevel;
      }
      this._writer.WriteLine("}");
      return base.VisitSwitch(node);
    }

    protected override SwitchCase VisitSwitchCase(SwitchCase node)
    {
      this._writer.Write("case ( ");
      this.Visit(node.TestValues);
      this._writer.WriteLine(" ):");
      ++this._indentLevel;
      this.Visit(node.Body);
      --this._indentLevel;
      return node;
    }

    protected override Expression VisitTry(TryExpression node)
    {
      this._writer.WriteLine("try (");
      if (node.Body != null)
      {
        ++this._indentLevel;
        try
        {
          this.WriteIndent();
          this.Visit(node.Body);
        }
        finally
        {
          --this._indentLevel;
        }
        this._writer.WriteLine();
        this.WriteIndent();
      }
      this._writer.Write(')');
      foreach (CatchBlock handler in node.Handlers)
      {
        this.WriteIndent();
        this._writer.Write(" catch[ ");
        this._writer.Write(handler.Variable.Name);
        this._writer.Write(" : ");
        this._writer.Write((object) handler.Test);
        this._writer.Write(" ] ");
        if (handler.Filter != null)
        {
          this._writer.WriteLine("when (");
          ++this._indentLevel;
          try
          {
            this.WriteIndent();
            this.Visit(handler.Filter);
            this._writer.WriteLine();
          }
          finally
          {
            --this._indentLevel;
          }
          this.WriteIndent();
          this._writer.Write(") handler (");
        }
        this._writer.WriteLine('(');
        ++this._indentLevel;
        try
        {
          this.WriteIndent();
          this.Visit(handler.Body);
          this._writer.WriteLine();
        }
        finally
        {
          --this._indentLevel;
        }
        this.WriteIndent();
        this._writer.Write(')');
      }
      if (node.Finally != null)
      {
        this._writer.WriteLine(" finally (");
        ++this._indentLevel;
        try
        {
          this.WriteIndent();
          this.Visit(node.Finally);
          this._writer.WriteLine();
        }
        finally
        {
          --this._indentLevel;
        }
        this.WriteIndent();
        this._writer.Write(')');
      }
      if (node.Fault != null)
      {
        this._writer.WriteLine(" fault (");
        ++this._indentLevel;
        try
        {
          this.WriteIndent();
          this.Visit(node.Fault);
          this._writer.WriteLine();
        }
        finally
        {
          --this._indentLevel;
        }
        this.WriteIndent();
        this._writer.Write(')');
      }
      return (Expression) node;
    }

    protected override Expression VisitTypeBinary(TypeBinaryExpression node)
    {
      this.ThrowUnsupportedNodeException<TypeBinaryExpression>(node);
      return base.VisitTypeBinary(node);
    }

    protected override Expression VisitUnary(UnaryExpression node)
    {
      switch (node.NodeType)
      {
        case ExpressionType.ArrayLength:
        case ExpressionType.TypeAs:
        case ExpressionType.TypeIs:
        case ExpressionType.PostIncrementAssign:
        case ExpressionType.PostDecrementAssign:
          this._writer.Write("( ");
          break;
        case ExpressionType.Convert:
        case ExpressionType.Unbox:
          this._writer.Write("( ");
          this._writer.Write((object) node.Type);
          this._writer.Write(" )");
          break;
        case ExpressionType.ConvertChecked:
          this._writer.Write("checked( ( ");
          this._writer.Write((object) node.Type);
          this._writer.Write(" )");
          break;
        case ExpressionType.Negate:
          this._writer.Write("( -( ");
          break;
        case ExpressionType.UnaryPlus:
          this._writer.Write("( +( ");
          break;
        case ExpressionType.NegateChecked:
          this._writer.Write("checked( -( ");
          break;
        case ExpressionType.Not:
          this._writer.Write("!( ");
          break;
        case ExpressionType.Decrement:
          this._writer.Write("`decr`( ");
          break;
        case ExpressionType.Increment:
          this._writer.Write("`incr`( ");
          break;
        case ExpressionType.Throw:
          this._writer.Write("throw ( ");
          break;
        case ExpressionType.PreIncrementAssign:
          this._writer.Write("++(");
          break;
        case ExpressionType.PreDecrementAssign:
          this._writer.Write("--(");
          break;
        case ExpressionType.OnesComplement:
          this._writer.Write("~(");
          break;
        case ExpressionType.IsTrue:
          this._writer.Write("`true`( ");
          break;
        case ExpressionType.IsFalse:
          this._writer.Write("`false`( ");
          break;
        default:
          throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Unary operation {0}(NodeType:{1}) is not supported. Expression tree:{2}", (object) node.GetType().Name, (object) node.NodeType, (object) node));
      }
      this.Visit(node.Operand);
      switch (node.NodeType)
      {
        case ExpressionType.ArrayLength:
          this._writer.Write(" ).Length");
          break;
        case ExpressionType.ConvertChecked:
        case ExpressionType.Not:
        case ExpressionType.Decrement:
        case ExpressionType.Increment:
        case ExpressionType.Throw:
        case ExpressionType.PreIncrementAssign:
        case ExpressionType.PreDecrementAssign:
        case ExpressionType.OnesComplement:
        case ExpressionType.IsTrue:
        case ExpressionType.IsFalse:
          this._writer.Write(" )");
          break;
        case ExpressionType.Negate:
        case ExpressionType.UnaryPlus:
        case ExpressionType.NegateChecked:
          this._writer.Write(" ) )");
          break;
        case ExpressionType.TypeAs:
          this._writer.Write(") as ");
          this._writer.Write((object) node.Type);
          break;
        case ExpressionType.TypeIs:
          this._writer.Write(") is ");
          this._writer.Write((object) node.Type);
          break;
        case ExpressionType.PostIncrementAssign:
          this._writer.Write(" )++");
          break;
        case ExpressionType.PostDecrementAssign:
          this._writer.Write(" )--");
          break;
      }
      return (Expression) node;
    }

    private void ThrowUnsupportedNodeException(Expression node)
    {
      this._writer.Write((object) node);
    }

    private void ThrowUnsupportedNodeException<T>(T node)
    {
      this._writer.Write((object) node);
    }

    private void WriteIndent()
    {
      ExpressionDumper.WriteIndent(this._writer, this._indentLevel);
    }

    internal static void WriteIndent(TextWriter writer, int indentLevel)
    {
      for (int index = 0; index < indentLevel; ++index)
      {
        writer.Write(' ');
        writer.Write(' ');
      }
    }
  }
}
