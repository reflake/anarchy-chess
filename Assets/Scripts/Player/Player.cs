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
				
				if (hit.BoardHit && possibleMoves.Contains(hit.BoardPoint))
				{
					_selectedPiece.MoveTo(hit.BoardPoint, true);
				}
				else if (hit.PieceHit && possibleMoves.Contains(hit.PieceHit.GetPositionOnBoard()))
				{
					_selectedPiece.MoveTo(hit.PieceHit.GetPositionOnBoard(), true);
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