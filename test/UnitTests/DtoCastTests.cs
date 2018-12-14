using Borzoo.GraphQL;
using Framework;
using Xunit;

namespace UnitTests
{
    public class DtoCastTests
    {
        [OrderedFact]
        public void Should_Generate_Id_From_Title()
        {
            string id = IdGenerator.GetIdFromTitle("Foo Bar #0");
            Assert.Equal("Foo-Bar-0", id);
        }
    }
}
