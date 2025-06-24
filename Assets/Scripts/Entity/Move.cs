using UnityEngine;

namespace Entity
{
	public abstract class Move
	{
		public Vector2Int Position { get; protected set; }
		public Board Board { get; set; }

		public Move(Board board, Vector2Int position)
		{
			Board = board;
			Position = position;
		}

		public void Perform()
		{
			PerformHandle(Board);
			Board.NextPlayerTurn(true);
		}

		public void Perform(Board board)
		{
			PerformHandle(board);
			board.NextPlayerTurn(false);
		}
		
		protected abstract void PerformHandle(Board board);
	}

	public class Step : Move
	{
		public Vector2Int PiecePosition { get; private set; }
		
		public Step(Piece piece, Vector2Int stepPosition) : base(piece.Board, stepPosition)
		{
			PiecePosition = piece.Position;
		}

		protected override void PerformHandle(Board board)
		{
			Piece piece = board.GetCellPiece(PiecePosition);
			piece.MoveTo(Position);
		}
	}

	public class Capture : Move
	{
		public Vector2Int PiecePosition { get; private set; }
		public Vector2Int CapturedPosition { get; private set; }

		public Capture(Piece piece, Piece capturedPiece, Vector2Int position) : base(piece.Board, position)
		{
			PiecePosition = piece.Position;
			CapturedPosition = capturedPiece.Position;
		}

		protected override void PerformHandle(Board board)
		{
			var piece = board.GetCellPiece(PiecePosition);
			var capturedPiece = board.GetCellPiece(CapturedPosition);
			
			piece.CapturePiece(capturedPiece);
			piece.MoveTo(Position);
		}
	}
	
	public class Castling : Move
	{
		public Vector2Int KingPosition { get; private set; }
		public Vector2Int RookPosition { get; private set; }
		public Vector2Int RookDirection { get; private set; }

		public Castling(Piece king, Piece rook, Vector2Int rookDirection, Vector2Int position) : base(king.Board, position)
		{
			KingPosition = king.Position;
			RookPosition = rook.Position;
			RookDirection = rookDirection;
		}
		
		protected override void PerformHandle(Board board)
		{
			var king = board.GetCellPiece(KingPosition);
			var rook = board.GetCellPiece(RookPosition);
			
			king.MoveTo(Position);
			rook.MoveTo(Position + RookDirection);
		}
	}
}