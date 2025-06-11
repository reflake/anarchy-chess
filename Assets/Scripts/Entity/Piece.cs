using UnityEngine;

namespace Entity
{
	public class Piece : MonoBehaviour
	{
		public Board Board => _board;
		
		private Board _board = null;

		public void SetBoard(Board board)
		{
			_board = board;
		}
	}
}