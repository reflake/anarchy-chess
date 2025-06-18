using Entity;
using Player.Data;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Player
{
	public class Player : MonoBehaviour
	{
		private Camera _mainCamera;

		private void Awake()
		{
			_mainCamera = Camera.main;
		}
		
		public void OnSelectPiece(Piece piece)
		{
			SendMessage(nameof(MoveVisualizer.ShowMovesOfPiece), piece);
		}

		public void OnUnselectPiece(Piece _)
		{
			SendMessage(nameof(MoveVisualizer.ClearMarkers));
		}
		
		private void Update()
		{
			if (Input.GetButtonDown("Fire1"))
			{
				var screenMousePos = Input.mousePosition;
				var mouseRay = _mainCamera.ScreenPointToRay(screenMousePos);

				if (!Physics.Raycast(mouseRay, out RaycastHit hit))
				{
					SendMessage("OnRaycastHit", new RaycastInfo());
					return;
				}
				
				var piece = hit.transform.gameObject.GetComponent<Piece>();
				if (piece == null)
				{
					SendMessage("OnRaycastHit", new RaycastInfo());
					return;
				}
				
				SendMessage("OnRaycastHit", new RaycastInfo(piece));
			}
		}
	}
}