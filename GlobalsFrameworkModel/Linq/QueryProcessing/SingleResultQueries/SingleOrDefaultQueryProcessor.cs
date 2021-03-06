﻿using System;
using System.Collections;
using System.Linq.Expressions;
using GlobalsFramework.Linq.ExpressionProcessing;
using GlobalsFramework.Utils.InstanceCreation;

namespace GlobalsFramework.Linq.QueryProcessing.SingleResultQueries
{
    internal sealed class SingleOrDefaultQueryProcessor : SingleResultQueryProcessor
    {
        public override bool CanProcess(MethodCallExpression query)
        {
            return query.Method.Name == "SingleOrDefault";
        }

        protected override ProcessingResult ProcessQuery(IEnumerator enumerator, Type elementType)
        {
            if (!enumerator.MoveNext())
                return new ProcessingResult(true, InstanceCreator.GetDefaultValue(elementType), true);

            var result = enumerator.Current;

            if (enumerator.MoveNext())
                throw new InvalidOperationException("Sequence contains more than one element");

            return new ProcessingResult(true, result, true);
        }
    }
}
