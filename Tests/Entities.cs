namespace Tests
{
    public interface IFoo
    {
        string Foo { get; set; }
    }
    
    public class Dummy : IFoo
    {
        public string Foo { get; set; }
        
        public int Bar { get; set; }
        
        public int Baz { get; set; }
    }
}
