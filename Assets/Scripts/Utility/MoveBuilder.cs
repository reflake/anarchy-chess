using System;
using System.Collections.Generic;
using System.Linq;
using Entity;
using UnityEngine;

namespace Utility
{
	[Flags]
	public enum MoveOption
	{
		CannotCapture = 0x0,
		CanCapture = 0x1,
		MustCapture = 0x2 | CanCapture,
	}
	
	public class MoveBuilder
	{
		private readonly Piece _piece;
		private readonly Board _board;
		private readonly Vector2Int _myPosition;
		
		private List<Vector2Int> _probableMoves = new();
		private bool _loop = false;
		private MoveOption _options = MoveOption.CannotCapture;

		public MoveBuilder(Piece piece, Board board)
		{
			_piece = piece;
			_board = board;
			_myPosition = piece.GetPositionOnBoard();
		}

		public MoveBuilder Loop(bool loop)
		{
			_loop = loop;
			return this;
		}

		public MoveBuilder Options(MoveOption options)
		{
			_options = options;
			return this;
		}

		public MoveBuilder AddSteps(params Vector2Int[] moveDirections)
		{
			foreach (var move in moveDirections)
			{
				AddStep(move);
			}
			
			return this;
		}

		public MoveBuilder AddStep(Vector2Int moveDirection, bool? loop = null)
		{
			var newPosition = _myPosition;
			
			if (!loop.HasValue)
				loop = _loop;

			do
			{
				newPosition += moveDirection;
				
				Piece otherPiece = _board.GetCellPiece(newPosition);
				bool outOfBound = !_board.IsPositionOnBoard(newPosition);
				bool isCellEmpty = !otherPiece;
				bool captures = _options.HasFlag(MoveOption.CanCapture) && _piece.CanCapture(otherPiece);
				bool mustCapture = _options.HasFlag(MoveOption.MustCapture);

				if (outOfBound)
					break;
				
				if (!mustCapture && (isCellEmpty || captures) || 
				    mustCapture && captures)
				{
					_probableMoves.Add(newPosition);
				}

				// Taking next steps until hit an occupied cell
				loop = loop.Value && isCellEmpty;

			} while (loop.Value);
			
			return this;
		}

		public IEnumerable<Vector2Int> GetValidatedMoves()
		{
			return _probableMoves;
		}
	}
}