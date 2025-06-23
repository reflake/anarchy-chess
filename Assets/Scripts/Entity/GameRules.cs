using System;
using Data;
using UnityEngine;
using Utility;

namespace Entity
{
	public enum ResultType
	{
		Checkmate, Stalemate
	}

	public struct GameResult
	{
		public ResultType Type;
		public PieceColor VictorColor;
	}
	
	public class GameRules : MonoBehaviour
	{
		[SerializeField] private BoardConfiguration boardConfiguration;

		public event Action<GameResult> OnGameEnd = null;

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

		public void HandleTurnEvent(Board board)
		{
			PieceColor opponentColor = board.CurrentTurnPlayerColor switch
			{
				PieceColor.White => PieceColor.Black,
				PieceColor.Black => PieceColor.White,
				_ =>  PieceColor.Undefined,
			};

			bool noMoves = !board.CanCurrentPlayerMove();
			
			if (board.IsCheckmate(board.CurrentTurnPlayerColor))
			{
				if (noMoves)
					OnGameEnd?.Invoke(new GameResult()
					{
						Type = ResultType.Checkmate, 
						VictorColor = opponentColor
					});
			}
			else if (noMoves)
			{
				OnGameEnd?.Invoke(new GameResult()
				{
					Type = ResultType.Stalemate
				});
			}
		}
	}
}