using MetroGid.Core.Exceptions.Handlers;
using MetroGidIntegrationTests.DBA.EFFixtures;

namespace MetroGidIntegrationTests.Services.EFFixtures;

public class EFServiceUnsignedFixture : EFServiceFixture
{
    public EFServiceUnsignedFixture() :
        base(
            new PassThroughHandlerException(),
            new EFDataBaseUnsignedFixture(),
            new EFDataBasePostgresFixture()
        ) { }
}