using MetroGid.Core.Exceptions.Handlers;
using MetroGidIntegrationTests.DBA.EFFixtures;

namespace MetroGidIntegrationTests.Services.EFFixtures;

public class EFServiceSignedFixture : EFServiceFixture
{
    public EFServiceSignedFixture() :
        base(
            new PassThroughHandlerException(),
            new EFDataBaseSignedFixture(),
            new EFDataBasePostgresFixture()
        ) { }
}