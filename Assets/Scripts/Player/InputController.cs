using Entity;
using Player.Data;
using UnityEngine;

namespace Player
{
	public class InputController : MonoBehaviour
	{
		
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
					SendMessage("OnRaycastHit", new RaycastInfo());
					return;
				}
				
				var board = hit.transform.gameObject.GetComponent<Board>();
				if (board != null)
				{
					SendMessage("OnRaycastHit", new RaycastInfo(board, hit.point));
					return;
				}
				
				var piece = hit.transform.gameObject.GetComponent<Piece>();
				if (piece != null)
				{
					SendMessage("OnRaycastHit", new RaycastInfo(piece));
					return;
				}
				
				SendMessage("OnRaycastHit", new RaycastInfo());
			}
		}
	}
}