// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace Lueben.Templates.Microservice.Data.CosmosDb.Extensions
{
    [ExcludeFromCodeCoverage]
    public class ParameterRebinder : ExpressionVisitor
    {
        private readonly IDictionary<ParameterExpression, ParameterExpression> _map;

        internal ParameterRebinder(IDictionary<ParameterExpression, ParameterExpression> map) =>
            _map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();

        internal static Expression ReplaceParameters(
            IDictionary<ParameterExpression, ParameterExpression> map, Expression exp) =>
            new ParameterRebinder(map).Visit(exp);

        protected override Expression VisitParameter(ParameterExpression parameter)
        {
            if (_map.TryGetValue(parameter, out ParameterExpression replacement))
            {
                parameter = replacement;
            }

            return base.VisitParameter(parameter);
        }
    }
}