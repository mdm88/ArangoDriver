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
    }
}