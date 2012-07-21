using System;

namespace Naif.TestUtilities.Models
{
    public class Person //: IIdentifiable
    {
        public virtual int Id { get; set; }

        public virtual string Name { get; set; }
        public virtual DateTime Birthdate { get; set; }

        public bool IsNew
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}
