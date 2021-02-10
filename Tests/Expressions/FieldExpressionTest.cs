using System;
using System.Collections.Generic;
using System.Linq;
using ArangoDriver.Expressions;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Tests.Expressions
{
    public class FieldExpressionTest
    {   
        [Test]
        public void SimpleTest()
        {
            var expression = new FieldExpression<Dummy, string>(x => x.Foo);
            
            Assert.AreEqual("Foo", expression.Field);
        }

        [Test]
        public void ComplexTest()
        {
            var expression = new FieldExpression<Complex, int>(x => x.dum.Bar);
            
            Assert.AreEqual("dum.Bar", expression.Field);
        }

        [Test]
        public void InterfaceTest()
        {
            var expression = new FieldExpression<IComplex, int>(x => x.dum.Bar);
            
            Assert.AreEqual("dum.Bar", expression.Field);
        }

        [Test]
        public void RenamedTest()
        {
            var expression = new FieldExpression<Complex, int>(x => x.asd.Bar);
            
            Assert.AreEqual("renamed.Bar", expression.Field);
        }

        [Test]
        public void VariableNameTest()
        {
            var expression = new FieldExpression<Dummy, string>(x => x.Foo);
            
            Assert.AreEqual("x", expression.Name);
        }

        [Test]
        public void CountExpressionTest()
        {
            var expression = new FilterFieldExpression<Complex, int>(x => x.list.Count());
            
            Assert.AreEqual("COUNT(x.list)", expression.Field);
        }

        [Test]
        public void TypeExpressionTest()
        {
            var expression = new FilterFieldExpression<Complex, Type>(x => x.GetType());
            
            Assert.AreEqual("x.$type", expression.Field);
        }

        [Test]
        public void SubTypeExpressionTest()
        {
            var expression = new FilterFieldExpression<Complex, Type>(x => x.asd.GetType());
            
            Assert.AreEqual("x.renamed.$type", expression.Field);
        }

        [Test]
        public void DictionaryExpressionTest()
        {
            var expression = new FilterFieldExpression<Dictionary<string, double>, double>(x => x["asd"]);
            
            Assert.AreEqual("x.asd", expression.Field);
            
            var expression2 = new FilterFieldExpression<ComplexWithDictionary, double>(x => x.dic["asd"]);
            
            Assert.AreEqual("x.dic.asd", expression2.Field);
            
            var expression3 = new FilterFieldExpression<Dictionary<string, Complex>, int>(x => x["asd"].asd.Bar);
            
            Assert.AreEqual("x.asd.renamed.Bar", expression3.Field);

            var param = "qwe";
            var expression4 = new FilterFieldExpression<Dictionary<string, Complex>, int>(x => x[param].asd.Bar);
            
            Assert.AreEqual("x.qwe.renamed.Bar", expression4.Field);
        }
    }
}