using System.Collections.Generic;
using UnityEngine;

namespace Grid
{
    [System.Serializable]
    public class Board : MonoBehaviour
    {
        [SerializeField] private List<Row> _board = new List<Row>();
        public Cell this[int x, int y] => _board[x][y];
    }
}
