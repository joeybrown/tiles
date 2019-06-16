using System.Linq;
using FluentAssertions;
using Moq;
using Tiles.Infrastructure.Grid;
using Xunit;

namespace Tiles.Infrastructure.UnitTest.Grid {
    public class GridServiceTests {
        [Fact]
        public void Slice_Layout_Larger_Than_Tile_Should_Result_In_Single_GridElement(){
            var layout = new byte[0];
            var settings = new Mock<IGridServiceSettings>();

            var sut = new GridService(settings.Object);
            var result = sut.Slice(layout);
            var elements = result.GetElements();

            var expectedValue = new byte[0];
            elements.Should().HaveCount(1);
            elements.First().CoordX.Should().Be(0);
            elements.First().CoordY.Should().Be(0);
            elements.First().Value.Should().BeEquivalentTo(expectedValue);
        }
    }
}