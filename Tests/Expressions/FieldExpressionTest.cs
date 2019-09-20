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
        public class Complex
        {
            public Dummy dum { get; set; }
            
            [JsonProperty(PropertyName = "renamed")]
            public Dummy asd { get; set; }
            
            public List<Dummy> list { get; set; }
        }
        public interface IComplex
        {
            Dummy dum { get; }
        }
        
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
    }
}