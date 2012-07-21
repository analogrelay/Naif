using System;
using System.Linq;
using System.Threading;
using Moq;
using NUnit.Framework;
using Naif.Core.ComponentModel;
using Naif.Core.Tests.Helpers;
using Microsoft.Practices.ServiceLocation;

namespace Naif.Core.Tests
{
    [TestFixture]
    public class ContainerTests
    {
        [Test]
        public void Container_Supports_Transient_Lifestyle()
        {
            //Arrange
            var container = new Container();

            //Act
            container.Register(typeof(IService), null, (c, t, k) => 
                                                                {
                                                                    var mock = new Mock<IService>();
                                                                    return mock.Object;
                                                                });

            var inst1 = container.GetInstance<IService>();
            var inst2 = container.GetInstance<IService>();

            //Assert
            Assert.AreNotSame(inst1, inst2);
        }

        [Test]
        public void Container_Supports_Singleton_Lifestyle()
        {
            //Arrange
            var container = new Container();
            var mock = new Mock<IService>();
            var inst = mock.Object;

            //Act
            container.Register(typeof(IService), null, (c, t, k) => inst);
            var inst1 = container.GetInstance<IService>();
            var inst2 = container.GetInstance<IService>();

            //Assert
            Assert.AreSame(inst, inst1);
            Assert.AreSame(inst, inst2);
        }

        [Test]
        public void Container_Supports_PerThread_Lifestyle()
        {
            //Arrange
            var container = new Container();

            //Act
            container.Register(typeof(IService), null, (c, t, k) => servicePerThread);
            var id = container.GetInstance<IService>().Id;
            var thread = new Thread(() =>
            {
                try
                {
                    var id2 = container.GetInstance<IService>().Id;

                    //Assert
                    Assert.AreNotEqual(id, id2);
                }
                catch (Exception ex)
                {
                    Assert.Fail(ex.ToString());
                }
            });
            thread.Start();
            thread.Join();
        }

        [Test]
        public void Container_Supports_Injection()
        {
            //Arrange
            var container = new Container();
            var mock = new Mock<IService>();
            var inst = mock.Object;

            //Act
            container.Register(typeof(IService2), null, (c, t, k) => new Service2Impl(c.GetInstance<IService>()));
            container.Register(typeof(IService), null, (c, t, k) => inst);
            var svc = container.GetInstance<IService2>();

            //Assert
            Assert.AreSame(inst, svc.Service);
        }

        [Test]
        [ExpectedException(typeof(ActivationException))]
        public void Container_Throws_If_Injection_Without_Dependency()
        {
            //Arrange
            var container = new Container();

            container.Register(typeof(IService2), null, (c, t, k) => new Service2Impl(c.GetInstance<IService>()));
            var svc = container.GetInstance<IService2>();
        }

        [Test]
        public void Container_Uses_Last_Builder_When_Multiple_Instances_Of_Same_Service_Registered()
        {
            //Arrange
            var container = new Container();
            var mock1 = new Mock<IService>();
            mock1.Setup(m => m.Id).Returns(1);
            var inst1 = mock1.Object;

            var mock2 = new Mock<IService>();
            mock2.Setup(m => m.Id).Returns(2);
            var inst2 = mock2.Object;

            //Act
            container.Register(typeof(IService), null, (c, t, k) => inst1);
            container.Register(typeof(IService), null, (c, t, k) => inst2);
            var inst3 = container.GetInstance<IService>();

            //Assert
            Assert.AreNotEqual(inst1.Id, inst3.Id);
            Assert.AreEqual(inst2.Id, inst3.Id);

        }

        [Test]
        public void Container_Returns_Correct_Instance_When_Using_Key()
        {
            //Arrange
            var container = new Container();
            var mocka = new Mock<IService>();
            var insta = mocka.Object;
            var mockb = new Mock<IService>();
            var instb = mockb.Object;

            //Act
            container.Register(typeof(IService), "keya", (c, t, k) => insta);
            container.Register(typeof(IService), "keyb", (c, t, k) => instb);
            var inst1 = container.GetInstance<IService>("keya");
            var inst2 = container.GetInstance<IService>("keyb");

            //Assert
            Assert.AreSame(insta, inst1);
            Assert.AreSame(instb, inst2);
            Assert.AreNotSame(instb, inst1);
            Assert.AreNotSame(insta, inst2);
        }

        [Test]
        [ExpectedException(typeof(ActivationException))]
        public void Container_Thows_When_Registered_With_Key_But_Accessed_With_No_Key()
        {
            //Arrange
            var container = new Container();
            var mock = new Mock<IService>();
            var inst = mock.Object;

            //Act
            container.Register(typeof(IService), "key", (c, t, k) => inst);
            var inst1 = container.GetInstance<IService>();
        }

        [Test]
        public void Container_Returns_Service_When_Registered_With_No_Key_But_Accessed_With_Key()
        {
            //Arrange
            var container = new Container();
            var mock = new Mock<IService>();
            var inst = mock.Object;

            //Act
            container.Register(typeof(IService), null, (c, t, k) => inst);
            var inst1 = container.GetInstance<IService>("key");

            //Assert
            Assert.AreSame(inst, inst1);
        }

        [ThreadStatic]
        private static IService service;

        private static IService servicePerThread
        {
            get
            {
                if (service == null)
                {
                    service = new ServiceImpl();
                }
                return service;
            }
        }
    }
}
