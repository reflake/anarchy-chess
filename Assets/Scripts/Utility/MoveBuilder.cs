using System.Collections.Generic;
using System.Linq;
using Entity;
using UnityEngine;

namespace Utility
{
	public class MoveBuilder
	{
		private readonly Piece _piece;
		private readonly Board _board;
		private readonly Vector2Int _myPosition;
		private readonly GameRules _gameRules = null;
		
		private List<Vector2Int> _probableMoves = new();

		public MoveBuilder(Piece piece, Board board)
		{
			_piece = piece;
			_board = board;
			_myPosition = piece.GetPositionOnBoard();
			_gameRules = GameObject.FindFirstObjectByType<GameRules>();
		}

		public MoveBuilder Add(params Vector2Int[] moveDirections)
		{
			foreach (var move in moveDirections)
			{
				_probableMoves.Add(_myPosition + move);
			}
			return this;
		}

		public IEnumerable<Vector2Int> GetValidatedMoves()
		{

			return _probableMoves.Where(IsMoveValid);
		}

		private bool IsMoveValid(Vector2Int move)
		{
			return _gameRules.IsMoveValid(_piece, move, _board);
		}
	}
}