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
			if (!hit.Hit || !hit.BoardHit)
				return;

			if (_selectedPiece)
			{
				_selectedPiece.MoveTo(hit.BoardPoint);
			}
			else
			{
				Debug.LogWarning("Cannot make move, piece is not selected");
			}
		}
	}
}