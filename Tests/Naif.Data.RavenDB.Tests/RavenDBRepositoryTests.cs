using System;
using System.Collections.Generic;
using System.Linq;

using Moq;
using NUnit.Framework;
using Naif.Core.Caching;
using Naif.TestUtilities.Models;
using Raven.Client;
using System.Threading;
using Raven.Client.Linq;
using Naif.TestUtilities;

namespace Naif.Data.RavenDB.Tests
{
    [TestFixture]
    public class RavenDBRepositoryTests
    {
        private readonly string connectionStringName = "RavenDB";
        private readonly string[] _dogAges = TestConstants.PETAPOCO_DogAges.Split(',');
        private readonly string[] _dogNames = TestConstants.PETAPOCO_DogNames.Split(',');

        [SetUp]
        public void SetUp()
        {
            if (!RavenDBDocumentStore.IsInitialized)
            {
                RavenDBDocumentStore.Initialize(connectionStringName);
            }
        }

        [TearDown]
        public void TearDown()
        {
            Thread.Sleep(5000);
            using (IDocumentSession session = RavenDBDocumentStore.Instance.OpenSession())
            {
                foreach (Dog dog in session.Query<Dog>())
                {
                    session.Delete<Dog>(dog);
                }
                session.SaveChanges();
            }
        }

        #region Constructor Tests

        [Test]
        public void RavenDBRepository_Constructor_Throws_On_Null_ICacheProvider()
        {
            //Arrange
            IDocumentSession _ravenDB = RavenDBDocumentStore.Instance.OpenSession();

            //Act, Assert
            Assert.Throws<ArgumentNullException>(() => new RavenDBRepository<Dog>(_ravenDB, null));
        }

        [Test]
        public void RavenDBRepository_Constructor_Throws_On_Null_Database()
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();

            //Act, Assert
            Assert.Throws<ArgumentNullException>(() => new RavenDBRepository<Dog>(null, mockCache.Object));
        }

        #endregion

        #region GetAll Tests

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(5)]
        public void RavenDBRepository_GetAll_Returns_All_Rows(int count)
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();
            SetUpDatabase(count);

            IEnumerable<Dog> dogs;
            using (IDocumentSession _ravenDB = RavenDBDocumentStore.Instance.OpenSession())
            {
                var repository = new RavenDBRepository<Dog>(_ravenDB, mockCache.Object);

                //Act
                dogs = repository.GetAll();
            }

            //Assert
            Thread.Sleep(5000);
            Assert.AreEqual(count, dogs.Count());
        }

        [Test]
        public void RavenDBRepository_GetAll_Returns_List_Of_Models()
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();
            SetUpDatabase(5);

            IEnumerable<Dog> dogs;
            using (IDocumentSession _ravenDB = RavenDBDocumentStore.Instance.OpenSession())
            {
                var repository = new RavenDBRepository<Dog>(_ravenDB, mockCache.Object);

                //Act
                dogs = repository.GetAll();
            }

            //Assert
            Thread.Sleep(5000);
            var listOfDogs = dogs.ToList();
            for (int i = 0; i < listOfDogs.Count(); i++)
            {
                Assert.IsInstanceOf<Dog>(listOfDogs[i]);
            }
        }

        [Test]
        public void RavenDBRepository_GetAll_Returns_Models_With_Correct_Properties()
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();
            SetUpDatabase(5);

            IEnumerable<Dog> dogs;
            using (IDocumentSession _ravenDB = RavenDBDocumentStore.Instance.OpenSession())
            {
                var repository = new RavenDBRepository<Dog>(_ravenDB, mockCache.Object);

                //Act
                dogs = repository.GetAll();
            }

            //Assert
            Thread.Sleep(5000);
            var dog = dogs.First();
            Assert.AreEqual(_dogAges[0], dog.Age.ToString());
            Assert.AreEqual(_dogNames[0], dog.Name);
        }

        #endregion

        #region GetById Tests

        [Test]
        public void RavenDBRepository_GetById_Returns_Null_If_InValid_Id()
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();
            SetUpDatabase(5);

            Dog dog;
            using (IDocumentSession _ravenDB = RavenDBDocumentStore.Instance.OpenSession())
            {
                var repository = new RavenDBRepository<Dog>(_ravenDB, mockCache.Object);

                //Act
                dog = repository.GetById(TestConstants.PETAPOCO_InvalidDogId);
            }

            //Assert
            Thread.Sleep(5000);
            Assert.IsNull(dog);
        }

        [Test]
        public void RavenDBRepository_GetById_Returns_Model_With_Correct_Properties()
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();
            SetUpDatabase(5);

            Dog dog;
            using (IDocumentSession _ravenDB = RavenDBDocumentStore.Instance.OpenSession())
            {
                var repository = new RavenDBRepository<Dog>(_ravenDB, mockCache.Object);

                //Act
                dog = repository.GetById(TestConstants.PETAPOCO_ValidDogId);
            }

            //Assert
            Thread.Sleep(5000);
            Assert.AreEqual(TestConstants.PETAPOCO_ValidDogAge, dog.Age.ToString());
            Assert.AreEqual(TestConstants.PETAPOCO_ValidDogName, dog.Name);
        }

        #endregion

        #region GetByProperty Tests

        [Test]
        [TestCase("Spot", 2)]
        [TestCase("Buddy", 1)]
        public void RavenDBRepository_GetByProperty_Returns_List_Of_Models_If_Valid_Property(string dogName, int count)
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();
            SetUpDatabase(5);

            IEnumerable<Dog> dogs;
            using (IDocumentSession _ravenDB = RavenDBDocumentStore.Instance.OpenSession())
            {
                var repository = new RavenDBRepository<Dog>(_ravenDB, mockCache.Object);

                //Act
                dogs = repository.GetByProperty<string>("Name", dogName);
            }

            //Assert
            Assert.IsInstanceOf<IEnumerable<Dog>>(dogs);
            Assert.AreEqual(count, dogs.Count());
        }

        [Test]
        public void RavenDBRepository_GetByProperty_Returns_Instance_Of_Model_If_Valid_Property()
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();
            SetUpDatabase(5);
            var dogName = _dogNames[2];

            Dog dog;
            using (IDocumentSession _ravenDB = RavenDBDocumentStore.Instance.OpenSession())
            {
                var repository = new RavenDBRepository<Dog>(_ravenDB, mockCache.Object);

                //Act
                dog = repository.GetByProperty<string>("Name", dogName).FirstOrDefault();
            }


            //Assert
            Assert.IsInstanceOf<Dog>(dog);
        }

        [Test]
        public void RavenDBRepository_GetByProperty_Returns_Empty_List_If_InValid_Proeprty()
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();
            SetUpDatabase(5);
            var dogName = "Invalid";

            IEnumerable<Dog> dogs;
            using (IDocumentSession _ravenDB = RavenDBDocumentStore.Instance.OpenSession())
            {
                var repository = new RavenDBRepository<Dog>(_ravenDB, mockCache.Object);

                //Act
                dogs = repository.GetByProperty<string>("Name", dogName);
            }

            //Assert
            Assert.IsInstanceOf<IEnumerable<Dog>>(dogs);
            Assert.AreEqual(0, dogs.Count());
        }

        [Test]
        [TestCase("Spot")]
        [TestCase("Buddy")]
        public void RavenDBRepository_GetByProperty_Returns_Models_With_Correct_Properties(string dogName)
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();
            SetUpDatabase(5);

            IEnumerable<Dog> dogs;
            using (IDocumentSession _ravenDB = RavenDBDocumentStore.Instance.OpenSession())
            {
                var repository = new RavenDBRepository<Dog>(_ravenDB, mockCache.Object);

                //Act
                dogs = repository.GetByProperty<string>("Name", dogName);
            }

            //Assert
            foreach (Dog dog in dogs)
            {
                Assert.AreEqual(dogName, dog.Name);
            }
        }


        #endregion

        #region Add Tests

        [Test]
        public void RavenDBRepository_Add_Inserts_Item_Into_DataBase()
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();
            SetUpDatabase(TestConstants.PETAPOCO_RecordCount);

            using (IDocumentSession _ravenDB = RavenDBDocumentStore.Instance.OpenSession())
            {
                var repository = new RavenDBRepository<Dog>(_ravenDB, mockCache.Object);
                var dog = new Dog
                {
                    Age = TestConstants.PETAPOCO_InsertDogAge,
                    Name = TestConstants.PETAPOCO_InsertDogName
                };

                //Act
                repository.Add(dog);

                //The change is not persisted until the session's changes are saved
                _ravenDB.SaveChanges();
            }

            //Assert
            Thread.Sleep(5000);
            int actualCount;
            using (IDocumentSession session = RavenDBDocumentStore.Instance.OpenSession())
            {
                actualCount = session.Query<Dog>().Count();
            }
            Assert.AreEqual(TestConstants.PETAPOCO_RecordCount + 1, actualCount);
        }

        [Test]
        public void RavenDBRepository_Add_Inserts_Item_Into_DataBase_With_Correct_ColumnValues()
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();
            SetUpDatabase(TestConstants.PETAPOCO_RecordCount);

            using (IDocumentSession _ravenDB = RavenDBDocumentStore.Instance.OpenSession())
            {
                var repository = new RavenDBRepository<Dog>(_ravenDB, mockCache.Object);
                var dog = new Dog
                {
                    Age = TestConstants.PETAPOCO_InsertDogAge,
                    Name = TestConstants.PETAPOCO_InsertDogName
                };

                //Act
                repository.Add(dog);

                //The change is not persisted until the session's changes are saved
                _ravenDB.SaveChanges();
            }

            //Assert
            Thread.Sleep(5000);
            Dog newDog = GetLastItemAdded();
            Assert.AreEqual(TestConstants.PETAPOCO_InsertDogAge, newDog.Age);
            Assert.AreEqual(TestConstants.PETAPOCO_InsertDogName, newDog.Name);
        }

        #endregion

        #region Delete Tests

        [Test]
        public void RavenDBRepository_Delete_Deletes_Item_From_DataBase()
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();
            SetUpDatabase(TestConstants.PETAPOCO_RecordCount);

            using (IDocumentSession _ravenDB = RavenDBDocumentStore.Instance.OpenSession())
            {
                var repository = new RavenDBRepository<Dog>(_ravenDB, mockCache.Object);
                var dog = new Dog
                {
                    ID = TestConstants.PETAPOCO_DeleteDogId,
                    Age = TestConstants.PETAPOCO_DeleteDogAge,
                    Name = TestConstants.PETAPOCO_DeleteDogName
                };

                //Act
                repository.Delete(dog);
            }

            //Assert
            //int actualCount = DataUtil.GetRecordCount(TestConstants.PETAPOCO_DatabaseName,
            //    TestConstants.PETAPOCO_TableName);
            //Assert.AreEqual(TestConstants.PETAPOCO_RecordCount - 1, actualCount);
            Assert.IsTrue(false);
        }

        [Test]
        public void RavenDBRepository_Delete_Deletes_Item_From_DataBase_With_Correct_ID()
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();
            SetUpDatabase(TestConstants.PETAPOCO_RecordCount);

            using (IDocumentSession _ravenDB = RavenDBDocumentStore.Instance.OpenSession())
            {
                var repository = new RavenDBRepository<Dog>(_ravenDB, mockCache.Object);
                var dog = new Dog
                {
                    ID = TestConstants.PETAPOCO_DeleteDogId,
                    Age = TestConstants.PETAPOCO_DeleteDogAge,
                    Name = TestConstants.PETAPOCO_DeleteDogName
                };

                //Act
                repository.Delete(dog);
            }

            //Assert
            //DataTable table = DataUtil.GetTable(TestConstants.PETAPOCO_DatabaseName, TestConstants.PETAPOCO_TableName);
            //foreach (DataRow row in table.Rows)
            //{
            //    Assert.IsFalse((int)row["ID"] == TestConstants.PETAPOCO_DeleteDogId);
            //}
            Assert.IsTrue(false);
        }

        [Test]
        public void RavenDBRepository_Delete_Does_Nothing_With_Invalid_ID()
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();
            SetUpDatabase(TestConstants.PETAPOCO_RecordCount);

            using (IDocumentSession _ravenDB = RavenDBDocumentStore.Instance.OpenSession())
            {
                var repository = new RavenDBRepository<Dog>(_ravenDB, mockCache.Object);
                var dog = new Dog
                {
                    ID = TestConstants.PETAPOCO_DeleteDogId,
                    Age = TestConstants.PETAPOCO_DeleteDogAge,
                    Name = TestConstants.PETAPOCO_DeleteDogName
                };

                //Act
                repository.Delete(dog);
            }

            //Assert
            //int actualCount = DataUtil.GetRecordCount(TestConstants.PETAPOCO_DatabaseName,
            //    TestConstants.PETAPOCO_TableName);
            //Assert.AreEqual(TestConstants.PETAPOCO_RecordCount, actualCount);
            Assert.IsTrue(false);
        }

        #endregion

        #region Update Tests

        [Test]
        public void RavenDBRepository_Update_Updates_Item_In_DataBase()
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();
            SetUpDatabase(TestConstants.PETAPOCO_RecordCount);

            using (IDocumentSession _ravenDB = RavenDBDocumentStore.Instance.OpenSession())
            {
                var repository = new RavenDBRepository<Dog>(_ravenDB, mockCache.Object);
                var dog = new Dog
                {
                    ID = TestConstants.PETAPOCO_UpdateDogId,
                    Age = TestConstants.PETAPOCO_UpdateDogAge,
                    Name = TestConstants.PETAPOCO_UpdateDogName
                };

                //Act
                repository.Update(dog);
            }

            //Assert
            //int actualCount = DataUtil.GetRecordCount(TestConstants.PETAPOCO_DatabaseName,
            //    TestConstants.PETAPOCO_TableName);
            //Assert.AreEqual(TestConstants.PETAPOCO_RecordCount, actualCount);
            Assert.IsTrue(false);
        }

        [Test]
        public void RavenDBRepository_Update_Updates_Item_With_Correct_ID()
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();
            SetUpDatabase(TestConstants.PETAPOCO_RecordCount);

            using (IDocumentSession _ravenDB = RavenDBDocumentStore.Instance.OpenSession())
            {
                var repository = new RavenDBRepository<Dog>(_ravenDB, mockCache.Object);
                var dog = new Dog
                {
                    ID = TestConstants.PETAPOCO_UpdateDogId,
                    Age = TestConstants.PETAPOCO_UpdateDogAge,
                    Name = TestConstants.PETAPOCO_UpdateDogName
                };

                //Act
                repository.Update(dog);
            }

            //Assert
            //DataTable table = DataUtil.GetTable(TestConstants.PETAPOCO_DatabaseName, TestConstants.PETAPOCO_TableName);
            //foreach (DataRow row in table.Rows)
            //{
            //    if ((int)row["ID"] == TestConstants.PETAPOCO_UpdateDogId)
            //    {
            //        Assert.AreEqual(row["Age"], TestConstants.PETAPOCO_UpdateDogAge);
            //        Assert.AreEqual(row["Name"], TestConstants.PETAPOCO_UpdateDogName);
            //    }
            //}
            Assert.IsTrue(false);
        }

        #endregion

        private void SetUpDatabase(int count)
        {
            using (IDocumentSession _ravenDB = RavenDBDocumentStore.Instance.OpenSession())
            {
                for (int i = 0; i < count; i++)
                {
                    var dog = new Dog
                    {
                        ID = i + 1,
                        Age = Int32.Parse(_dogAges[i]),
                        Name = _dogNames[i]
                    };
                    _ravenDB.Store(dog);
                }
                _ravenDB.SaveChanges();
            }
        }

        private Dog GetLastItemAdded()
        {
            Dog dog = null;
            using (IDocumentSession session = RavenDBDocumentStore.Instance.OpenSession())
            {
                int count = session.Query<Dog>().Count();
                int i = 0;
                foreach (var d in session.Query<Dog>().OrderBy(d => d.ID))
                {
                    i += 1;
                    if (i == count)
                    {
                        dog = d;
                    }
                }
            }
            return dog;
        }
    }
}
