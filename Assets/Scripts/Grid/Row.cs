using System.Collections.Generic;

namespace Grid
{
    [System.Serializable]
    public class Row 
    {
        public List<Cell> cells = new List<Cell>();
        public Cell this[int index] => cells[index];
    }
}
