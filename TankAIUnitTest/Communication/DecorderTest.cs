using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TankAI.Communication;
using TankAI.AI;

using System.Collections.Generic;

namespace TankAIUnitTest.Communication
{
    [TestClass]
    public class DecorderTest
    {


        [TestMethod]
        public void startTest()
        {
            Decorder dc = new Decorder("S:P0;0,0;0:P1;0,9;0:P2;9,0;0:P3;9,9;0:P4;5,5;0");

            dc.Start();

            Assert.AreEqual(1, 1, "Wrong decoding of StartMessage");

        }

        public void listTest()
        {
            List<int> l = new List<int>();
            l.Add(1);
            l.Add(2);
            l.Add(3);

            List<int>.Enumerator en = l.GetEnumerator();
            en.MoveNext();
            Assert.AreEqual(en.Current, 1, "list is reversed");
        }
    }
}
