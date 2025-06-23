using Data;
using UnityEngine;
using Utility;

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

			// Check for invalid move that threatens king
			using var simulation = new Simulation(board);
			simulation.PieceMoveTo(piece, move);

			if (simulation.IsCheckmate(piece.Color))
				return false;

			return true;
		}
	}
}