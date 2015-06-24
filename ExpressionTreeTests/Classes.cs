using System.Collections.Generic;

namespace ExpressionTreeTests
{
    public class Foo
    {
        public string StringValue { get; set; }
        public IList<string> StringValues { get; set; }
        public int IntegerValue { get; set; }
        public IList<int> IntegerValues { get; set; }
    }

    public class GrandParent : Foo
    {
        public Parent Parent { get; set; }
        public IList<Parent> Parents { get; set; }
    }

    public class Parent : Foo
    {
        public Child Child { get; set; }
        public IList<Child> Children { get; set; }
    }

    public class Child : Foo
    {
    }
}