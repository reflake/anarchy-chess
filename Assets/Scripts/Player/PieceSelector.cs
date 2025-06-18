using Entity;
using Player.Data;
using UnityEngine;

namespace Player
{
	public class PieceSelector : MonoBehaviour
	{
		[SerializeField] private Material _selectionMaterial;
		
		private Piece _selectedPiece;
		private Material _selectedPieceMaterial;
		
		public void OnRaycastHit(RaycastInfo info)
		{
			if (info.Hit == false || info.PieceHit == null)
			{
				UnselectCurrentPiece(true);
				return;
			}
			
			SelectPiece(info.PieceHit);
		}

		private void SelectPiece(Piece piece)
		{
			if (_selectedPiece == piece)
			{
				return;
			}
			
			if (piece != _selectedPiece && _selectedPiece != null)
			{
				UnselectCurrentPiece(false);
			}
			
			_selectedPiece = piece;
			
			SendMessage(nameof(Player.OnSelectPiece), piece);
			
			var pieceRenderer = _selectedPiece.GetComponent<Renderer>();
			if (pieceRenderer == null)
				return;
			
			_selectedPieceMaterial = pieceRenderer.material;
			
			pieceRenderer.material = new Material(_selectionMaterial)
			{
				mainTexture = _selectedPieceMaterial.mainTexture
			};
		}

		private void UnselectCurrentPiece(bool fireMessage)
		{
			if (_selectedPiece == null)
				return;

			if (fireMessage)
			{
				SendMessage(nameof(Player.OnUnselectPiece), _selectedPiece);
			}

			var pieceRenderer = _selectedPiece.GetComponent<Renderer>();
			if (pieceRenderer)
				pieceRenderer.material = _selectedPieceMaterial;
			_selectedPiece = null;
		}
	}
}