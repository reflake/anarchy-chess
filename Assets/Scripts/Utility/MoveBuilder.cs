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
		
		private List<Move> _probableMoves = new();
		private MoveOption _options = MoveOption.CannotCapture;
		private int _stepLength = 1;

		public MoveBuilder(Piece piece, Board board)
		{
			_piece = piece;
			_board = board;
			_myPosition = piece.Position;
		}

		public MoveBuilder Loop(bool loop)
		{
			if (loop)
			{
				// Move infinitely
				_stepLength = 0;
			}
			else
			{
				_stepLength = 1;
			}
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

		public MoveBuilder AddStep(Vector2Int moveDirection, int? length = null)
		{
			var newPosition = _myPosition;
			
			if (!length.HasValue)
				length = _stepLength;

			do
			{
				newPosition += moveDirection;
				
				Piece otherPiece = _board.GetCellPiece(newPosition);
				bool outOfBound = !_board.IsPositionOnBoard(newPosition);
				bool isCellEmpty = !otherPiece;
				bool captures = _options.HasFlag(MoveOption.CanCapture) && _piece.IsPieceEnemy(otherPiece);
				bool mustCapture = _options.HasFlag(MoveOption.MustCapture);

				if (outOfBound)
					break;
				
				if (!mustCapture && (isCellEmpty || captures) || 
				    mustCapture && captures)
				{
					if (isCellEmpty)
						_probableMoves.Add(new Step(_piece, newPosition));
					else
						_probableMoves.Add(new Capture(_piece, otherPiece, newPosition));
				}

				// Taking next steps until hit an occupied cell
				if (!isCellEmpty)
					return this;
				
				length--;

			} while (length != 0);
			
			return this;
		}

		public MoveBuilder AddCastling(Piece king, Piece rook)
		{
			var kingMoveDirection = rook.Position - king.Position;
			kingMoveDirection.x /= Mathf.Abs(kingMoveDirection.x);

			// Check if anything is in the way
			for (var i = king.Position + kingMoveDirection; i != rook.Position; i += kingMoveDirection)
			{
				if (!_board.IsPositionOnBoard(i))
					return this;
				
				if (_board.GetCellPiece(i))
					return this;
			}
			
			_probableMoves.Add(new Castling(king, rook, -kingMoveDirection, king.Position + kingMoveDirection * 2));
			
			return this;
		}

		public IReadOnlyList<Move> GetMoves()
		{
			return _probableMoves;
		}
	}
}