using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reflection4Net.Accessor;
using Reflection4Net.Test.Model;
using System;

namespace Reflection4Net.Test
{
    [TestClass]
    public class LateBoundAccessorBuilderTest
    {
        private Func<DataModel, string, object> dataModelPropertyGetter;
        private Func<DataModel, string, object, bool> dataModelPropertySetter;

        [TestInitialize]
        public void Startup()
        {
            var builder = new LateBoundPropertyAccessorDelegateBuilder();

            dataModelPropertyGetter = builder.BuildPropertyGetter<DataModel>();
            dataModelPropertySetter = builder.BuildPropertySetter<DataModel>();
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void LateBoundPropertyGetterTest()
        {
            var name = "xailjg";
            var model = new DataModel() { Name = name };

            var value = dataModelPropertyGetter(model, "Name");
            Assert.AreEqual(name, value);
            value = dataModelPropertyGetter(model, "Title");
            Assert.AreEqual(null, value);
            value = dataModelPropertyGetter(model, "CategoryNameNotExists");
            Assert.AreEqual(null, value);
        }

        [TestMethod]
        public void LateBoundPropertySetterTest()
        {
            var name = "asfawe";
            var model = new DataModel();
            Assert.AreEqual(false, dataModelPropertySetter(model, "Title", name));
            Assert.AreEqual(true, dataModelPropertySetter(model, "Name", name));
            Assert.AreEqual(name, model.Name);
        }
    }
}
