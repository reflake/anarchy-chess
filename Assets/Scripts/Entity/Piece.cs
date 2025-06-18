using System;
using System.Collections.Generic;
using TriInspector;
using UnityEngine;
using UnityEngine.Serialization;
using Utility;

namespace Entity
{
	public enum PieceColor
	{
		Custom = -1,
		Undefined = 0,
		White,
		Black
	}

	[Flags]
	public enum MovePattern
	{
		Pawn = 0x1,
		Knight = 0x2,
		Straight = 0x4,
		Diagonal = 0x8,
		OneStep = 0x10
	}
	
	public class Piece : MonoBehaviour
	{
		[SerializeField] private PieceColor color;
		[SerializeField, EnumToggleButtons] 
		private MovePattern pattern;
		
		public Board Board => _board;
		
		private Board _board = null;

		public void SetBoard(Board board)
		{
			_board = board;
		}

		public Vector2Int GetPositionOnBoard()
		{
			if (_board == null)
			{
				throw new NullReferenceException("Piece is not bound to any Board");
			}
			
			return _board.GetPositionOnBoard(this);
		}

		public IEnumerable<Vector2Int> GetPossibleMoves()
		{
			MoveBuilder builder = new(this, _board);

			builder.Add(Vector2Int.up, Vector2Int.left, Vector2Int.right);
			
			return builder.GetValidatedMoves();
		}
	}
}