namespace Tiles.Infrastructure.UnitTest.Grid {
    public class GridServiceTests {
        [Fact]
        public void Slice_Layout_Larger_Than_Tile_Should_Result_In_Single_GridElement(){
            var layout = new byte[0];
            var settings = new Mock<IGridServiceSettings>();
            var sut = new GridService(settings.Object);
            var result = sut.Slice();
            result.Should().Be(1);
        }
    }
}