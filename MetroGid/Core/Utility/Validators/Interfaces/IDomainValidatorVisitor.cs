using MCMC = MetroGid.Core.Models.Concrete;

namespace MetroGid.Core.Utility.Validators.Interfaces;

public interface IDomainValidatorVisitor
{
    void Visit(MCMC.Client client);
    void Visit(MCMC.Chart chart);
    void Visit(MCMC.Branch branch);
    void Visit(MCMC.Station station);
    void Visit(MCMC.Railway railway);
    void Visit(MCMC.Transition transition);
    void Visit(MCMC.Route route);
}