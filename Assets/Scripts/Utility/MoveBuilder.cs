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
		private Piece[] _enPassantPawns = null;

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
				
				bool outOfBound = !_board.IsPositionOnBoard(newPosition);

				if (outOfBound)
					break;
				
				Piece occupyingPiece = _board.GetCellPiece(newPosition);

				if (!occupyingPiece && _enPassantPawns != null)
				{
					var attackingCell = newPosition - _piece.PawnMoveDirection();
					var pieceBeignAttacked = _board.GetCellPiece(attackingCell);
					
					if (pieceBeignAttacked && _enPassantPawns.Contains(pieceBeignAttacked))
						occupyingPiece = pieceBeignAttacked;
				}
				
				var isCellEmpty = AddStep(occupyingPiece, newPosition);

				// Taking next steps until hit an occupied cell
				if (!isCellEmpty)
					return this;
				
				length--;

			} while (length != 0);
			
			return this;
		}

		private bool AddStep(Piece occupyingPiece, Vector2Int newPosition)
		{
			bool isCellEmpty = !occupyingPiece;
			bool captures = _options.HasFlag(MoveOption.CanCapture) && _piece.IsPieceEnemy(occupyingPiece);
			bool mustCapture = _options.HasFlag(MoveOption.MustCapture);
				
			if (!mustCapture && (isCellEmpty || captures) || 
			    mustCapture && captures)
			{
				if (isCellEmpty)
					_probableMoves.Add(new Step(_piece, newPosition));
				else
					_probableMoves.Add(new Capture(_piece, occupyingPiece, newPosition));
			}

			return isCellEmpty;
		}

		public MoveBuilder AddCastling(Piece king, Piece rook)
		{
			var kingMoveDirection = rook.Position - king.Position;
			kingMoveDirection.x /= Mathf.Abs(kingMoveDirection.x);
			kingMoveDirection.y = 0;

			// Check if anything is in the way
			for (var i = king.Position + kingMoveDirection; i != rook.Position; i += kingMoveDirection)
			{
				if (!_board.IsPositionOnBoard(i))
					return this;
				
				if (_board.GetCellPiece(i))
					return this;
			}
			
			for(int i = 1; i <= 2; i++)
			{
				if (_board.IsCellUnderAttack(king.Color, king.Position + kingMoveDirection * i))
					return this;
			}
			
			_probableMoves.Add(new Castling(king, rook, -kingMoveDirection, king.Position + kingMoveDirection * 2));
			
			return this;
		}

		public MoveBuilder EnPassantPawns(Piece[] enPassantPawns)
		{
			_enPassantPawns = enPassantPawns;
			return this;
		}

		public IReadOnlyList<Move> GetMoves()
		{
			return _probableMoves;
		}
	}
}