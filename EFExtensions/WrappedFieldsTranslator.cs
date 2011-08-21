using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace EFExtensions
{
	internal class ExpressionTranslator 
	{
		public static Expression ForExpression(Expression query)
		{
			return new WrappedPropertiesRewriter().Modify(query);
		}

		public static Expression<T> ForExpression<T>(Expression<T> query)
		{
			return (Expression<T>)(new WrappedPropertiesRewriter().Modify(query));
		}

		class WrappedPropertiesRewriter : ExpressionVisitor
		{
			public Expression Modify(Expression expression)
			{
				return Visit(expression);
			}
			
			protected override Expression VisitUnary(UnaryExpression node)
			{
				if (node.NodeType == ExpressionType.Convert && node.Operand.Type.Name == "DbBool")
				{
					var binaryBlock = Expression.MakeBinary(ExpressionType.Equal, node.Operand, Expression.Constant(true));
					return VisitBinary(binaryBlock);
				}
				else if (node.NodeType == ExpressionType.Not && node.Operand.Type.Name == "DbBool")
				{
					var binaryBlock = Expression.MakeBinary(ExpressionType.NotEqual, node.Operand, Expression.Constant(true));
					return VisitBinary(binaryBlock);
				}
				return base.VisitUnary(node);
			}

			protected override Expression VisitBinary(BinaryExpression node)
			{
				if (node.Left.Type.Name == "DbBool" || node.Right.Type.Name == "DbBool")
				{
					Expression leftMember = TransformForDbBool(node.Left);
					
					Expression rightMember = TransformForDbBool(node.Right);

					node = Expression.MakeBinary(node.NodeType, leftMember, rightMember);
				}
				else if (node.Left.Type.GetInterface("IDbEnum") != null || node.Right.Type.GetInterface("IDbEnum") != null)
				{
					Expression leftMember = TransformForDbEnum(node.Left);
					Expression rightMember = TransformForDbEnum(node.Right);

					node = Expression.MakeBinary(node.NodeType, leftMember, rightMember);

				}
				return base.VisitBinary(node);
			}

			private Expression TransformForDbBool(Expression expression)
			{
				if (expression.Type.Name == "DbBool")
				{
					//the only constant can be the null value which should be of type string
					if (expression.NodeType == ExpressionType.Constant)
					{
						return Expression.Constant((string)null);
					}

					return Expression.MakeMemberAccess(expression, expression.Type.GetMember("Value").Single());
				}
				else if(expression.Type.Name == "Boolean")
				{
					bool constantValue = (bool)(expression as ConstantExpression).Value;
					return Expression.Constant(constantValue ? "Y" : "N");
				}
				else
				{
					//not sure if this case can be hit ever, but throw an exception just in case
					throw new NotSupportedException();
				}
			}

			private Expression TransformForDbEnum(Expression expression)
			{
				if (expression.Type.GetInterface("IDbEnum") != null)
				{
					//the only constant can be the null value which should be of type string
					if (expression.NodeType == ExpressionType.Constant)
					{
						return Expression.Constant((string)null);
					}

					return Expression.MakeMemberAccess(expression, expression.Type.GetMember("Value").Single());
				}
				else if (expression.Type.IsEnum)
				{
					object constantValue = EnumMapping.TranslateFromEnum(expression.Type,
						(expression as ConstantExpression).Value);
					return Expression.Constant(constantValue);
				}
				else
				{
					//not sure if this case can be hit ever, but throw an exception just in case
					throw new NotSupportedException();
				}
			}
		}

	}
}
