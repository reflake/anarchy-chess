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
		[SerializeField] private bool isKing = false;
		[SerializeField] private PieceColor color;
		[SerializeField, EnumToggleButtons] private MovePattern pattern;
		[SerializeField, HideInEditMode] private Board board = null;
		[SerializeField, HideInEditMode] private bool onInitialPosition = true;
		
		public bool IsKing => isKing;
		public PieceColor Color => color;
		public Board Board => board;
		public bool CanBeMoved => color == board.CurrentTurnPlayerColor;
		public bool Captured { get; private set; } = false;

		private GameRules _gameRules;

		private void Awake()
		{
			_gameRules = FindFirstObjectByType<GameRules>();
		}

		public void SetBoard(Board board)
		{
			this.board = board;
		}

		public Vector2Int GetPositionOnBoard()
		{
			if (board == null)
			{
				throw new NullReferenceException("Piece is not bound to any Board");
			}
			
			return board.GetPositionOnBoard(this);
		}

		public IEnumerable<Vector2Int> GetPossibleMoves(bool validate)
		{
			MoveBuilder builder = new(this, board);

			if (pattern.HasFlag(MovePattern.Pawn))
			{
				var moveDirection = color == PieceColor.White ? Vector2Int.up : Vector2Int.down;
				
				builder.Options(MoveOption.CannotCapture)
						.AddStep(moveDirection, false);

				if (onInitialPosition)
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
			
			if (validate)
				return builder.GetMoves()
								.Where(x => _gameRules.IsMoveValid(this, x, board));

			return builder.GetMoves();
		}

		public bool IsPieceEnemy(Piece piece) => piece != null && color != piece.color;

		public void MoveTo(Vector2Int targetPoint)
		{
			// Try to capture piece occupying target point
			Piece pieceOccupyingCell = board.GetCellPiece(targetPoint);
			if (pieceOccupyingCell != null)
			{
				if (IsPieceEnemy(pieceOccupyingCell))
				{
					CapturePiece(pieceOccupyingCell);
				}
				else
				{
					return;
				}
			}

			onInitialPosition = false;
			
			transform.position = board.LocalToWorld(targetPoint);

			board.InvalidateGrid();
			board.NextPlayerTurn();
		}

		public bool CanCapture(Piece piece)
		{
			var moves = GetPossibleMoves(false);

			if (!IsPieceEnemy(piece))
				return false;
			
			var piecePosition = piece.GetPositionOnBoard();
			return moves.Contains(piecePosition);
		}

		private void CapturePiece(Piece other)
		{
			other.Captured = true;
			other.gameObject.SetActive(false);
		}
	}
}