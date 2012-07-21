using System;
using System.Linq;

using Moq;
using NUnit.Framework;
using Naif.Core.Caching;
using Naif.Core.Data;
using Raven.Client;
using Naif.TestUtilities;
using Naif.TestUtilities.Models;

namespace Naif.Data.RavenDB.Tests
{
    [TestFixture]
    public class RavenDbDataContextTests
    {
        private readonly string connectionStringName = "RavenDB";
        private RavenDbDataContext context;
         
        #region Constructor Tests

        [Test]
        public void RavenDbDataContext_Constructor_Throws_On_Null_ICacheProvider()
        {
            //Arrange
            ICacheProvider cache = null;

            //Act, Assert
            Assert.Throws<ArgumentNullException>(() => new RavenDbDataContext(cache));
        }

        [Test]
        public void RavenDbDataContextFactory_Constructor_Throws_On_Null_ConnectionString()
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();

            //Act, Assert
            Assert.Throws<ArgumentException>(() => new RavenDbDataContext(null, mockCache.Object));
        }

        [Test]
        public void RavenDbDataContextFactory_Constructor_Throws_On_Empty_ConnectionString()
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();

            //Act, Assert
            Assert.Throws<ArgumentException>(() => new RavenDbDataContext(String.Empty, mockCache.Object));
        }

        [Test]
        public void RavenDbDataContext_Constructor_Initialises_IDocumentSession_Field()
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();

            //Act
            context = new RavenDbDataContext(connectionStringName, mockCache.Object);

            //Assert
            Assert.IsInstanceOf<IDocumentSession>(Util.GetPrivateMember<RavenDbDataContext, IDocumentSession>(context, "_documentSession"));
        }

        #endregion

        #region GetRepository Tests

        [Test]
        public void RavenDbDataContext_GetRepository_Returns_Repository()
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();
            var context = new RavenDbDataContext(connectionStringName, mockCache.Object);

            //Act
            var repo = context.GetRepository<Dog>();

            //Assert
            Assert.IsInstanceOf<IRepository<Dog>>(repo);
        }
        #endregion
    }
}
