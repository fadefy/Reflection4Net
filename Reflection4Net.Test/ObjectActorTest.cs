using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reflection4Net.Actors;
using Reflection4Net.Test.Model;
using Reflection4Net.Extensions;
using System.Collections.Generic;

namespace Reflection4Net.Test
{
    [TestClass]
    public class ObjectActorTest
    {
        private ITypeMappingInfoProvider infoProvider;

        [TestInitialize]
        public void Init()
        {
            infoProvider = new DelegatedTypeMappingInfoProvider(t => t.GetProperties());
        }

        [TestMethod]
        public void EqualsTest()
        {
            var actor = new Equals<DataModel>();
            var equals = actor.Build(infoProvider);

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
            var actor = new Hashes<DataModel>();
            var getHashCode = actor.Build(infoProvider);

            Assert.AreEqual(getHashCode(new DataModel()), getHashCode(new DataModel()));
            var dataModel = new DataModel();
            dataModel.Id = 2;
            Assert.AreNotEqual(getHashCode(dataModel), getHashCode(new DataModel()));
        }

        [TestMethod]
        public void CopyToTest()
        {
            var actor = new CopyTo<DataModel>();

            var source = new DataModel();
            var target = new DataModel();

            source.Id = 3;
            source.Name = "Hugo";
            source.UpdateTime = DateTime.Now;

            var copyTo = actor.Build(infoProvider);
            copyTo(source, target);

            Assert.AreEqual(source.Id, target.Id);
            Assert.AreEqual(source.Name, target.Name);
            Assert.AreEqual(source.UpdateTime, target.UpdateTime);
        }

        [TestMethod]
        public void MapPropertyTest()
        {
            var actor = new MapProperty<DataModel>();
            var map = actor.Build(infoProvider);

            var target = new DataModel();
            var source = new Dictionary<string, object>();

            source.Add("Id", "3");
            source.Add("Name", "Hugo");
            source.Add("UpdateTime", DateTime.Now.Date.ToString("O"));

            map(source, target);

            Assert.AreEqual(3, target.Id);
            Assert.AreEqual("Hugo", target.Name);
            Assert.AreEqual(DateTime.Now.Date, target.UpdateTime);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CopyToNullTest()
        {
            var actor = new CopyTo<DataModel>();
            var copyTo = actor.Build(infoProvider);
            copyTo(new DataModel(), null);
        }
    }
}
