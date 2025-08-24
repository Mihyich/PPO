using MetroGid.Core.Models.Concrete;

namespace MetroGid.Core.Utilities.Validators.Interfaces;

public interface IDomainValidatorVisitor
{
    void Visit(Client client);
    void Visit(Chart chart);
    void Visit(Branch branch);
    void Visit(Station station);
    void Visit(Railway railway);
    void Visit(Transition transition);
    void Visit(Route route);
}