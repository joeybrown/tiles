using System;
using System.Collections.Generic;

namespace Tiles.Infrastructure.Grid {
    public interface IGridService {
        IEnumerable<GridElement> Slice(byte[] layout);
    }

    public class GridService: IGridService {
        private readonly IGridServiceSettings _settings;

        public GridService(IGridServiceSettings settings){
            _settings = settings;
        }

        public IEnumerable<GridElement> Slice(byte[] layout) {
            return new [] {new GridElement()};
        }
    }
}