using System;
using System.Linq;
using System.Linq.Expressions;
using ArangoDriver.Exceptions;
using Newtonsoft.Json;

namespace ArangoDriver.Expressions
{
    internal abstract class FieldExpressionBase
    {
        protected string _field;
        protected string _name;
        
        protected virtual void Parse(Expression expression)
        {
            _field = "";

            while (expression != null)
            {
                string field;
                if (expression is MethodCallExpression methodCallExpression)
                {
                    if (methodCallExpression.Method.Name == "get_Item")
                    {
                        var argument = methodCallExpression.Arguments.FirstOrDefault();
                        if (argument is ConstantExpression constantArgument)
                        {
                            field = constantArgument.Value as string;
                        }
                        else
                        {
                            field = Expression.Lambda<Func<string>>(argument).Compile().Invoke();
                        }
                        expression = methodCallExpression.Object;
                    }
                    else
                    {
                        throw new ExpressionInvalidException(expression.ToString());
                    }
                }
                else if (expression is MemberExpression memberExpression)
                {
                    JsonPropertyAttribute attr = memberExpression.Member.GetCustomAttributes(typeof(JsonPropertyAttribute), true).FirstOrDefault() as JsonPropertyAttribute;
                    
                    field = attr?.PropertyName ?? memberExpression.Member.Name;
                    expression = memberExpression.Expression;
                }
                else if (expression is UnaryExpression unaryExpression)
                {
                    MemberExpression memberExpression2 = unaryExpression.Operand as MemberExpression;
                    JsonPropertyAttribute attr = memberExpression2.Member.GetCustomAttributes(typeof(JsonPropertyAttribute), true).FirstOrDefault() as JsonPropertyAttribute;
                    
                    field = attr?.PropertyName ?? memberExpression2.Member.Name;
                    expression = memberExpression2.Expression;
                }
                else if (expression is ParameterExpression)
                {
                    break;
                }
                else
                {
                    throw new ExpressionInvalidException(expression.ToString());
                }
                
                _field = field + (!String.IsNullOrEmpty(_field) ? "." + _field : "");
            }
        }
    }
}