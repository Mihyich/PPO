using MetroGid.Core.Models;
using MetroGid.Core.Utilities.Validators.Interfaces;

namespace MetroGid.Core.Utilities.Validators.Handlers
{
    public class ThrowableDomainAttribsValidator : IDomainValidatorVisitor
    {
        public void Visit(Chart chart)
        {
            throw new NotImplementedException();
        }
        
        public void Visit(Branch branch)
        {
            throw new NotImplementedException();
        }

        public void Visit(Station station)
        {
            throw new NotImplementedException();
        }

        public void Visit(Railway railway)
        {
            throw new NotImplementedException();
        }

        public void Visit(Transition transition)
        {
            throw new NotImplementedException();
        }
    }
}