using System;
using System.Linq;

namespace Naif.Core.Tests.Helpers
{
    public class Service2Impl : IService2
    {
        private readonly IService _service;

        public Service2Impl(IService service)
        {
            _service = service;
        }

        public IService Service
        {
            get
            {
                return this._service;
            }
        }
    }
}
