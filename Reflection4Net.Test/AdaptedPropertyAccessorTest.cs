using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reflection4Net.Accessor;
using Reflection4Net.Test.Model;
using System;

namespace Reflection4Net.Test
{
    [TestClass]
    public class AdaptedPropertyAccessorTest
    {
        [TestMethod]
        public void StringNullOperationTest()
        {
            Assert.AreEqual(String.Empty, String.Empty + null);
        }

        [TestMethod]
        public void GetPropertyTest()
        {
            var name = "xailjg";
            var model = new DataModel() { Name = name };

            var value = AdaptedPropertyAccessor<DataModel>.GetProperty(model, "Name");
            Assert.AreEqual(null, value);
            value = AdaptedPropertyAccessor<DataModel>.GetProperty(model, "Title");
            Assert.AreEqual(name, value);
            value = AdaptedPropertyAccessor<DataModel>.GetProperty(model, "CategoryNameNotExists");
            Assert.AreEqual(null, value);
        }

        [TestMethod]
        public void SetPropertyTest()
        {
            var name = "asfawe";
            var model = new DataModel();
            Assert.AreEqual(true, AdaptedPropertyAccessor<DataModel>.SetProperty(model, "Title", name));
            Assert.AreEqual(name, model.Name);
            // Set on orignal name not work
            Assert.AreEqual(false, AdaptedPropertyAccessor<DataModel>.SetProperty(model, "Name", null));
            Assert.AreEqual(name, model.Name);
        }
    }
}
