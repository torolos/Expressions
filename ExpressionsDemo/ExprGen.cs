using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ExpressionsDemo
{
    /* 
     * For more information on how to create dynamic expressions see http://msdn.microsoft.com/en-us/library/bb882637.aspx.
     * This is just an example on how to create expression trees when dealing with clr types or POCO's.
     */
    public class ExprGen
    {
        private static readonly
            string expr = "x";

        public static Expression<Func<T, bool>> WhereExpression<T>(object criteria)
        {
            //Reflect criteria object type and get properties
            Type type = criteria.GetType();
            PropertyInfo[] propertyInfo = null;
            // If anomymous type, get all properties, otherwise get all public/instance ones
            if (type.IsAnonymousType())
                propertyInfo = type.GetProperties();
            else
                propertyInfo = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            // Store property names and values
            Dictionary<string, object> values = new Dictionary<string, object>();
            foreach (PropertyInfo info in propertyInfo)
            {
                object val = info.GetValue(criteria, null);
                if (val != null && val.ToString() != "")
                    values.Add(info.Name, info.GetValue(criteria, null));
            }
            return CreateWhereExpression<T>(values);
        }

        private static Expression<Func<T, bool>> CreateWhereExpression<T>(IDictionary<string, object> criteria)
        {
            //Get the type of T
            Type complex = typeof(T);

            //Create the expression that will represent the predicate parameter
            ParameterExpression pEx = Expression.Parameter(typeof(T), expr);
            //Create a placeholder for all expressions
            List<Expression> exps = new List<Expression>();
            // Get all instance properties of T
            PropertyInfo[] props = complex.FindInstanceProperties();
            if (props.Any())
            {
                foreach (KeyValuePair<string, object> item in criteria)
                {
                    // Find corresponding instance property of T
                    PropertyInfo prop = props.FindProperty(item.Key);
                    if (prop != null)
                    {
                        // Create left part of expression, e.g. for Customer -> x.FirstName
                        Expression left = Expression.Property(pEx, prop);

                        Type argumentType = null;
                        bool isNullable = false;

                        // Check whether instance property is nullable
                        if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                        {
                            isNullable = true;
                            // If nullable store the generic argument type
                            argumentType = prop.PropertyType.GetGenericArguments()[0];
                            // Modify the expression to look into the value of the nullable
                            left = Expression.Property(left, prop.PropertyType.GetProperty("Value"));
                        }
                        else
                            argumentType = prop.PropertyType;

                        if (left != null)
                        {
                            // Set the right part of the expression
                            Expression right = null;
                            if (isNullable)
                            {
                                //if the argumentType and the item value are of the same type
                                // no parsing needed, otherwise proceed with parsing
                                if (argumentType.Equals(item.Value.GetType()))
                                    right = Expression.Constant(item.Value);
                                else
                                    right = Expression.Constant(item.Value.InvokeParse(argumentType));
                            }
                            else
                            {
                                // If instance property is a value type then call Parse
                                // else (string) set it directly
                                if (prop.PropertyType.IsValueType && !prop.PropertyType.Equals(item.Value.GetType()))
                                    right = Expression.Constant(item.Value.InvokeParse(prop.PropertyType));
                                else
                                    right = Expression.Constant(item.Value);
                            }

                            // Calling equal generates the full expression
                            // e.g. for customer, c.FirstName == [value/variable]
                            exps.Add(Expression.Equal(left, right));
                        }
                    }
                }
            }
            return CreateWhereExpression<T>(exps, new ParameterExpression[] { pEx });
        }

        private static Expression<Func<T, bool>> CreateWhereExpression<T>(
            IList<Expression> expressions, ParameterExpression[] parameters)
        {
            Expression predicate = null;
            if (expressions.Any())
            {
                // build the predicate expression
                foreach (Expression e in expressions)
                {
                    if (expressions.IndexOf(e) == 0)
                    {
                        predicate = e;
                    }
                    else
                    {
                        predicate = Expression.And(predicate, e);
                    }
                }
                // return constructed lambda
                return Expression.Lambda<Func<T, bool>>(predicate, parameters);
            }
            else
                throw new Exception("No fields found");
        }
    }
}
