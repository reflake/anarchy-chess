using Entity;
using UnityEngine;

namespace Player
{
	public class PieceSelector : MonoBehaviour
	{
		[SerializeField] private Material _selectionMaterial;
		
		private Piece _selectedPiece;
		private Material _selectedPieceMaterial;
		private Camera _mainCamera;

		private void Awake()
		{
			_mainCamera = Camera.main;
		}
		
		private void Update()
		{
			if (Input.GetButtonDown("Fire1"))
			{
				var screenMousePos = Input.mousePosition;
				var mouseRay = _mainCamera.ScreenPointToRay(screenMousePos);

				if (!Physics.Raycast(mouseRay, out RaycastHit hit))
				{
					UnselectCurrentPiece();
					return;
				}
				
				var piece = hit.transform.gameObject.GetComponent<Piece>();
				if (piece == null)
				{
					UnselectCurrentPiece();
					return;
				}
				
				SelectPiece(piece);
			}
		}

		private void SelectPiece(Piece piece)
		{
			if (piece != _selectedPiece && _selectedPiece != null)
			{
				UnselectCurrentPiece();
			}
			
			_selectedPiece = piece;
			
			var pieceRenderer = _selectedPiece.GetComponent<Renderer>();
			if (pieceRenderer == null)
				return;
			
			_selectedPieceMaterial = pieceRenderer.material;
			
			pieceRenderer.material = new Material(_selectionMaterial)
			{
				mainTexture = _selectedPieceMaterial.mainTexture
			};
		}

		private void UnselectCurrentPiece()
		{
			if (_selectedPiece == null)
				return;

			var pieceRenderer = _selectedPiece.GetComponent<Renderer>();
			if (pieceRenderer)
				pieceRenderer.material = _selectedPieceMaterial;
			_selectedPiece = null;
		}
	}
}