using UnityEngine;
using UnityEngine.Serialization;

namespace Entity
{
	public enum PieceColor
	{
		Custom = -1,
		Undefined = 0,
		White,
		Black
	}
	
	public abstract class Piece : MonoBehaviour
	{
		[SerializeField] private PieceColor color;
		
		public Board Board => _board;
		
		private Board _board = null;

		public void SetBoard(Board board)
		{
			_board = board;
		}
	}
}