using System;
using System.Linq;

namespace Naif.Core.Tests.Helpers
{

    public class ServiceImpl : IService
    {
        private static readonly Random rnd = new Random();
        private readonly int id = rnd.Next();

        public int Id 
        { 
            get { return id; } 
        }
    }
}
