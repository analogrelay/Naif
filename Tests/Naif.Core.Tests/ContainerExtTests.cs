using System;
using System.Linq;
using NUnit.Framework;
using Naif.Core.ComponentModel;
using Naif.Core.Tests.Helpers;
using Moq;

namespace Naif.Core.Tests
{
    [TestFixture]
    public class ContainerExtTests
    {
        [Test]
        public void Container_Register_T_Supports_Transient_Lifestyle()
        {
            //Arrange
            var container = new Container();

            //Act
            container.Register<IService>("key", (c, t, k) =>
                                                        {
                                                            var mock = new Mock<IService>();
                                                            return mock.Object;
                                                        });

            var inst1 = container.GetInstance<IService>("key");
            var inst2 = container.GetInstance<IService>("key");

            //Assert
            Assert.AreNotSame(inst1, inst2);
        }

        [Test]
        public void Container_Register_T_Supports_Singleton_Lifestyle()
        {
            //Arrange
            var container = new Container();
            var mock = new Mock<IService>();
            var inst = mock.Object;

            //Act
            container.Register<IService>("key", (c, t, k) => inst);
            var inst1 = container.GetInstance<IService>("key");
            var inst2 = container.GetInstance<IService>("key");

            //Assert
            Assert.AreSame(inst, inst1);
            Assert.AreSame(inst, inst2);
        }

        [Test]
        public void Container_Register_T_Overload_Supports_Transient_Lifestyle()
        {
            //Arrange
            var container = new Container();

            //Act
            container.Register<IService>((c, t, k) =>
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
        public void Container_Register_T_Overload_Supports_Singleton_Lifestyle()
        {
            //Arrange
            var container = new Container();
            var mock = new Mock<IService>();
            var inst = mock.Object;

            //Act
            container.Register<IService>((c, t, k) => inst);
            var inst1 = container.GetInstance<IService>();
            var inst2 = container.GetInstance<IService>();

            //Assert
            Assert.AreSame(inst, inst1);
            Assert.AreSame(inst, inst2);
        }

        [Test]
        public void Container_RegisterInstance_Returns_Same_Instance()
        {
            //Arrange
            var container = new Container();
            var mock = new Mock<IService>();
            var inst = mock.Object;

            //Act
            container.RegisterInstance(typeof(IService), "key", inst);
            var inst1 = container.GetInstance<IService>("key");
            var inst2 = container.GetInstance<IService>("key");

            //Assert
            Assert.AreSame(inst, inst1);
            Assert.AreSame(inst, inst2);
        }

        [Test]
        public void Container_RegisterInstance_T_Returns_Same_Instance()
        {
            //Arrange
            var container = new Container();
            var mock = new Mock<IService>();
            var inst = mock.Object;

            //Act
            container.RegisterInstance<IService>("key", inst);
            var inst1 = container.GetInstance<IService>("key");
            var inst2 = container.GetInstance<IService>("key");

            //Assert
            Assert.AreSame(inst, inst1);
            Assert.AreSame(inst, inst2);
        }

        [Test]
        public void Container_RegisterInstance_T_Overload_Returns_Same_Instance()
        {
            //Arrange
            var container = new Container();
            var mock = new Mock<IService>();
            var inst = mock.Object;

            //Act
            container.RegisterInstance<IService>(inst);
            var inst1 = container.GetInstance<IService>();
            var inst2 = container.GetInstance<IService>();

            //Assert
            Assert.AreSame(inst, inst1);
            Assert.AreSame(inst, inst2);
        }

        [Test]
        public void Container_RegisterSingleton_Returns_Same_Instance()
        {
            //Arrange
            var container = new Container();
            var mock = new Mock<IService>();
            var inst = mock.Object;

            //Act
            container.RegisterSingleton(typeof(IService), "key", inst);
            var inst1 = container.GetInstance<IService>("key");
            var inst2 = container.GetInstance<IService>("key");

            //Assert
            Assert.AreSame(inst, inst1);
            Assert.AreSame(inst, inst2);
        }

        [Test]
        public void Container_RegisterSingleton_T_Returns_Same_Instance()
        {
            //Arrange
            var container = new Container();
            var mock = new Mock<IService>();
            var inst = mock.Object;

            //Act
            container.RegisterSingleton<IService>("key", inst);
            var inst1 = container.GetInstance<IService>("key");
            var inst2 = container.GetInstance<IService>("key");

            //Assert
            Assert.AreSame(inst, inst1);
            Assert.AreSame(inst, inst2);
        }

        [Test]
        public void Container_RegisterSingleton_T_Overload_Returns_Same_Instance()
        {
            //Arrange
            var container = new Container();
            var mock = new Mock<IService>();
            var inst = mock.Object;

            //Act
            container.RegisterSingleton<IService>(inst);
            var inst1 = container.GetInstance<IService>();
            var inst2 = container.GetInstance<IService>();

            //Assert
            Assert.AreSame(inst, inst1);
            Assert.AreSame(inst, inst2);
        }

        [Test]
        public void Container_RegisterSingleton_Returns_Null_If_Container_Disposed()
        {
            //Arrange
            var container = new Container();
            var mock = new Mock<IDisposable>();
            var inst = mock.Object;

            //Act
            container.RegisterSingleton(typeof(IDisposable), "key", inst);
            container.Dispose();
            var inst1 = container.GetInstance<IDisposable>("key");

            //Assert
            Assert.IsNull(inst1);
        }

        [Test]
        public void Container_RegisterType_Returns_Instance()
        {
            //Arrange
            var container = new Container();
            var mock = new Mock<IService>();
            var inst = mock.Object;
            container.Register<IService>((c, t, k) => inst);

            //Act
            container.RegisterType(typeof(IService2), null, typeof(Service2Impl));
            var svc = container.GetInstance<IService2>();

            //Assert
            Assert.AreSame(inst, svc.Service);
        }

    }
}
