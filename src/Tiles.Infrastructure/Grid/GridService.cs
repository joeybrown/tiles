using System;
using System.Collections.Generic;

namespace Tiles.Infrastructure.Grid
{
    public interface IGridService
    {
        Grid Slice(byte[] layout);
    }

    public class GridService : IGridService
    {
        private readonly IGridServiceSettings _settings;

        public GridService(IGridServiceSettings settings)
        {
            _settings = settings;
        }

        public Grid Slice(byte[] layout)
        {
            var elements = new[] { new GridElement(0, 0, new byte[0]) };
            return new Grid(elements);
        }
    }
}