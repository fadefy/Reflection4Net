using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reflection4Net.Extension;
using Reflection4Net.Test.Model;

namespace Reflection4Net.Test
{
    /// <summary>
    /// Summary description for AdaptedAccessorFactoryTest
    /// </summary>
    [TestClass]
    public class AdaptedAccessorFactoryTest
    {
        [TestMethod]
        public void GetPropertyByExtensionTest()
        {
            var name = "xailjg";
            object model = new DataModel() { Name = name };

            Assert.AreEqual(null, model.GetProperty("Name"));
            Assert.AreEqual(name, model.GetProperty("Title"));
        }

        [TestMethod]
        public void GetComplexOpdsPropertyTest()
        {
            var model = new DataEntry();

            Assert.AreEqual(model.AuthorInfo.Name, model.GetProperty("AuthorName"));
            Assert.AreEqual(model.AuthorInfo.Email, model.GetProperty("AuthorEmail"));
            Assert.AreEqual(model.AuthorInfo.Address.Country, model.GetProperty("AuthorAddress"));

            // When a property is null, the adapted properties in this property will be assumed null.
            // No exception thrown.
            model.AuthorInfo = null;
            Assert.AreEqual(null, model.GetProperty("AuthorName"));
            Assert.AreEqual(null, model.GetProperty("AuthorAddress"));
        }

        [TestMethod]
        public void SetComplexOpdsPropertyTest()
        {
            var author = "author";
            var model = new DataEntry();

            Assert.AreEqual(true, model.SetProperty("AuthorName", author));
            Assert.AreEqual(author, model.AuthorInfo.Name);
        }

        [TestMethod]
        public void GetOPdsPropertyFromObjectsTest()
        {
            var name = "asdfaf";
            var objs = new object[]
            {
                new DataModel()
                {
                    Name = name
                },
                new CategoryInfo()
                {
                    Name = name
                }
            };

            var result = objs.GetProperty("Name");
            Assert.IsInstanceOfType(result, typeof(string));
            Assert.AreEqual(name, result);
            Assert.AreEqual(name, objs.GetProperty("Title"));
            var category = objs.GetProperty("CategoryInfo");
            Assert.IsNotNull(category);
            Assert.IsInstanceOfType(category, typeof(CategoryInfo));
            Assert.AreEqual(name, objs.GetProperty("CategoryInfo").GetProperty("Name"));
        }

        [TestMethod]
        public void GetOpdsArrayPropertyFromObjectsTest()
        {
            var name = "asdfaf";
            var objs = new object[]
            {
                new DataModel()
                {
                    Name = name
                },
                new [] {
                    new CategoryInfo()
                    {
                        Name = name
                    },
                    new CategoryInfo()
                    {
                        Name = name
                    }
                }
            };

            var categores = objs.GetProperty("CategoryInfo");
            Assert.IsNotNull(categores);
            Assert.IsInstanceOfType(categores, typeof(IEnumerable));
            Assert.AreEqual(2, (categores as IEnumerable<object>).Count());
            var category = (categores as IEnumerable<object>).First();
            Assert.AreEqual(name, category.GetProperty("Name"));
        }
    }
}
