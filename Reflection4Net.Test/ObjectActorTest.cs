using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reflection4Net.Builder;
using Reflection4Net.Test.Model;

namespace Reflection4Net.Test
{
    [TestClass]
    public class ObjectActorTest
    {
        [TestMethod]
        public void EqualsTest()
        {
            var actor = new ObjectActor<DataModel>();
            var equals = actor.GenerateEquals();

            Assert.IsTrue(equals(new DataModel(), new DataModel()));
            Assert.IsTrue(equals(null, null));
            Assert.IsFalse(equals(null, new DataModel()));
            Assert.IsFalse(equals(new DataModel(), null));

            var dataModel = new DataModel();
            Assert.IsTrue(equals(dataModel, dataModel));
            dataModel.Id = 2;
            Assert.IsFalse(equals(new DataModel(), dataModel));
        }

        [TestMethod]
        public void GetHashCodeTest()
        {
            var actor = new ObjectActor<DataModel>();
            var getHashCode = actor.GenerateGetHash();

            Assert.AreEqual(getHashCode(new DataModel()), getHashCode(new DataModel()));
            var dataModel = new DataModel();
            dataModel.Id = 2;
            Assert.AreNotEqual(getHashCode(dataModel), getHashCode(new DataModel()));
        }
    }
}
