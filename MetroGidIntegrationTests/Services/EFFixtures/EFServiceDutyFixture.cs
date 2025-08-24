using MetroGid.Core.Exceptions.Handlers;
using MetroGidIntegrationTests.DBA.EFFixtures;

namespace MetroGidIntegrationTests.Services.EFFixtures;

public class EFServiceDutyFixture : EFServiceFixture
{
    public EFServiceDutyFixture() :
        base(
            new PassThroughHandlerException(),
            new EFDataBaseDutyFixture(),
            new EFDataBasePostgresFixture()
        ) { }
}