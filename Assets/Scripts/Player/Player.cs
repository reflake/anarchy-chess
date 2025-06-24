using System.Linq;
using Entity;
using Player.Data;
using UnityEngine;

namespace Player
{
	public class Player : MonoBehaviour
	{
		private Piece _selectedPiece;
		
		public void OnSelectPiece(Piece piece)
		{
			SendMessage(nameof(MoveVisualizer.ShowMovesOfPiece), piece);
			
			_selectedPiece = piece;
		}

		public void OnUnselectPiece(Piece _)
		{
			SendMessage(nameof(MoveVisualizer.ClearMarkers));

			_selectedPiece = null;
		}

		public void OnRaycastHit(RaycastInfo hit)
		{
			if (!hit.Hit)
				return;

			if (_selectedPiece)
			{
				var possibleMoves = _selectedPiece.GetPossibleMoves(true).ToList();
				
				if (hit.BoardHit)
				{
					var move = possibleMoves.FirstOrDefault(move => move.Position == hit.BoardPoint);
					move?.Perform();
				}
				else if (hit.PieceHit && possibleMoves.Any(move => move.Position == hit.PieceHit.Position))
				{
					var move = possibleMoves.FirstOrDefault(move => move.Position == hit.PieceHit.Position);
					move?.Perform();
				}
					
				SendMessage(nameof(MoveVisualizer.ClearMarkers));
				SendMessage(nameof(PieceSelector.UnselectCurrentPiece), true);
			}
			else
			{
				Debug.LogWarning("Cannot make move, piece is not selected");
			}
		}
	}
}