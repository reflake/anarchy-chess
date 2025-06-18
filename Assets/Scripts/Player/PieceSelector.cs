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
					UnselectCurrentPiece(true);
					return;
				}
				
				var piece = hit.transform.gameObject.GetComponent<Piece>();
				if (piece == null)
				{
					UnselectCurrentPiece(true);
					return;
				}
				
				SelectPiece(piece);
			}
		}

		private void SelectPiece(Piece piece)
		{
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