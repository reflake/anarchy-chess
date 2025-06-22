using System;
using System.Collections.Generic;
using System.Linq;
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

		public PieceColor Color => color;
		public Board Board => _board;
		public bool CanBeMoved => color == _board.CurrentTurnPlayerColor;

		private Board _board = null;
		private bool _onInitialPosition = true;
		private GameRules _gameRules;

		private void Awake()
		{
			_gameRules = FindFirstObjectByType<GameRules>();
		}

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

			if (pattern.HasFlag(MovePattern.Pawn))
			{
				var moveDirection = color == PieceColor.White ? Vector2Int.up : Vector2Int.down;
				
				builder.Options(MoveOption.CannotCapture)
						.AddStep(moveDirection, false);

				if (_onInitialPosition)
					builder.AddStep(moveDirection * 2);

				builder.Options(MoveOption.MustCapture)
					   .AddSteps(moveDirection + Vector2Int.left, 
								 moveDirection + Vector2Int.right);
			}

			builder.Options(MoveOption.CanCapture);

			if (pattern.HasFlag(MovePattern.Knight))
			{
				builder.AddSteps(
					new(2, 1), new(1, 2),
					new(-2, 1), new(1, -2),
					new(-2, -1), new(-1, -2),
					new(2, -1), new(-1, 2));
			}

			builder.Loop(!pattern.HasFlag(MovePattern.OneStep));
			
			if (pattern.HasFlag(MovePattern.Straight))
			{
				builder.AddSteps(
					Vector2Int.up, 
					Vector2Int.down, 
					Vector2Int.left, 
					Vector2Int.right);
			}

			if (pattern.HasFlag(MovePattern.Diagonal))
			{
				builder.AddSteps(
					Vector2Int.up + Vector2Int.left,
					Vector2Int.up + Vector2Int.right,
					Vector2Int.down + Vector2Int.left,
					Vector2Int.down + Vector2Int.right);
			}
			
			return builder.GetValidatedMoves()
				.Where(x => _gameRules.IsMoveValid(this, x, _board));
		}

		public void MoveTo(Vector2Int point)
		{
			if (!GetPossibleMoves().Contains(point))
				return;

			_onInitialPosition = false;
			
			transform.position = _board.LocalToWorld(point);

			_board.NextPlayerTurn();
		}
		
		public bool CanCapture(Piece piece) => piece != null && color != piece.color;
	}
}