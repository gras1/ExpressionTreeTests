using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressionTreeTests
{
    [TestClass]
    public class UnitTests
    {
        private GrandParent _myGrandParent { get; set; }

        [TestInitialize]
        public void Init()
        {
            Child myChild1 = new Child
            {
                StringValue = "myChild1 Hello World",
                StringValues = new List<string> {"Newt", "Toad", "Frog", "Stickleback"},
                IntegerValue = 2346,
                IntegerValues = new List<int> {23, 879, 890, 12, 444}
            };
            Child myChild2 = new Child
            {
                StringValue = "myChild2 Hello World",
                StringValues = new List<string> {"Bald Headed Eagle", "Red Kite", "Perregrine Falcon", "Oyster Catcher"},
                IntegerValue = 9,
                IntegerValues = new List<int> {10285, 2234, 94210, 876}
            };
            Child myChild3 = new Child
            {
                StringValue = "myChild3 Hello World",
                StringValues = new List<string> {"Newt", "Toad", "Frog", "Stickleback"},
                IntegerValue = 7890,
                IntegerValues = new List<int> {73, 929, 940, 62, 494}
            };
            Child myChild4 = new Child
            {
                StringValue = "myChild4 Hello World",
                StringValues = new List<string> {"Spider", "Wasp", "Snake", "Birds"},
                IntegerValue = 999,
                IntegerValues = new List<int> { 409, 687130, 19871, 16, 4823 }
            };
            Parent myParent1 = new Parent
            {
                StringValue = "myParent1 Hello World",
                StringValues = new List<string> {"Spratt", "Mackerel", "Cod", "Hallibut"},
                IntegerValue = 9340354,
                IntegerValues = new List<int> {2, 43215, 56832, 123, 8456},
                Child = myChild1,
                Children = new List<Child> {myChild1, myChild2}
            };
            Parent myParent2 = new Parent
            {
                StringValue = "myParent2 Hello World",
                StringValues = new List<string> {"Whelk", "Cockel", "Winkle", "Crab"},
                IntegerValue = 248569,
                IntegerValues = new List<int> {9809135, 8964659, 651089, 6574984, 19493},
                Child = myChild3,
                Children = new List<Child> {myChild3, myChild4}
            };
            _myGrandParent = new GrandParent
            {
                StringValue = "myGrandParent Hello World",
                StringValues = new List<string> {"Whelk", "Cockel", "Winkle", "Crab"},
                IntegerValue = 9331,
                IntegerValues = new List<int> {5406, 89706, 98430, 980331, 71170},
                Parent = myParent1,
                Parents = new List<Parent> {myParent1, myParent2}
            };
        }

        [TestMethod]
        public void TestMethod1()
        {
            //arrange

            //act
            var valueToCheck = _myGrandParent.NullSafeGetValue(o => o.IntegerValue, 0);

            //assert
            Assert.IsTrue(valueToCheck == 9331);
        }

        [TestMethod]
        public void TestMethod2()
        {
            //arrange

            //act
            var valueToCheck = _myGrandParent.NullSafeGetValue(o => o.Parent.IntegerValue, 0);

            //assert
            Assert.IsTrue(valueToCheck == 9340354);
        }

        [TestMethod]
        public void TestMethod3()
        {
            //arrange

            //act
            Func<int> GetIntegerValue = () => _myGrandParent.Parents[0].Children[1].IntegerValue;

            //assert
            Assert.IsTrue(GetIntegerValue() == 9);
        }

        [TestMethod]
        public void TestMethod4()
        {
            //arrange

            //act
            Expression<Func<int>> expr = () => _myGrandParent.Parents[0].Children[1].IntegerValue;
            Func<int> deleg = expr.Compile();

            //assert
            Assert.IsTrue(deleg() == 9);
        }

        [TestMethod]
        public void TestMethod5()
        {
            //arrange

            //act
            var valueToCheck = _myGrandParent.NullSafeGetValue(o => o.Parents[0].Children[1].IntegerValue, 0);

            //assert
            Assert.IsTrue(valueToCheck == 9);
        }

        [TestMethod]
        public void TestMethod6()
        {
            //arrange

            //act
            var valueToCheck = _myGrandParent.NullSafeGetValue(o => o.Parents[0].Children[1].StringValues[3], "");

            //assert
            Assert.IsTrue(valueToCheck == "Oyster Catcher");
        }

        [TestMethod]
        public void TestMethod7()
        {
            //arrange

            //act
            var valueToCheck = _myGrandParent.NullSafeGetValue((Expression<Func<GrandParent, int>>)ExpressionOperator.CreateExpression(typeof(GrandParent), "Parent.IntegerValue"), 0);

            //assert
            Assert.IsTrue(valueToCheck == 9340354);
        }

        [TestMethod]
        public void TestMethod8()
        {
            //arrange

            //act
            var valueToCheck = _myGrandParent.NullSafeGetValue((Expression<Func<GrandParent, int>>)ExpressionOperator.CreateExpression(typeof(GrandParent), "Parents[1].IntegerValue"), 0);

            //assert
            Assert.IsTrue(valueToCheck == 248569);
        }

        [TestMethod]
        public void TestMethod9()
        {
            //arrange

            //act
            var valueToCheck = _myGrandParent.NullSafeGetValue((Expression<Func<GrandParent, int>>)ExpressionOperator.CreateExpression(typeof(GrandParent), "Parents[0].Children[1].IntegerValue"), 0);

            //assert
            Assert.IsTrue(valueToCheck == 9);
        }

        [TestMethod]
        public void TestMethod10()
        {
            //arrange

            //act
            var valueToCheck = _myGrandParent.NullSafeGetValue((Expression<Func<GrandParent, string>>)ExpressionOperator.CreateExpression(typeof(GrandParent), "Parents[0].Children[1].StringValues[3]"), "");

            //assert
            Assert.IsTrue(valueToCheck == "Oyster Catcher");
        }
    }
}