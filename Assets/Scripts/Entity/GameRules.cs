using Data;
using UnityEngine;

namespace Entity
{
	public class GameRules : MonoBehaviour
	{
		[SerializeField] private BoardConfiguration boardConfiguration;

		private void Awake()
		{
			if (boardConfiguration == null)
				return;
			
			var board = FindObjectOfType<Board>();

			if (board == null)
				return;
			
			board.PlacePieces(boardConfiguration);
		}

		public bool IsMoveValid(Piece piece, Vector2Int move, Board board)
		{
			if (piece.Color != board.CurrentTurnPlayerColor)
				return false;
			
			if (!board.IsPositionOnBoard(move))
				return false;

			//if (!piece.CanMoveToPosition(move))
			//	return false;

			return true;
		}
	}
}