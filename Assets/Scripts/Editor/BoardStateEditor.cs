using Data;
using Entity;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Editor
{
	[CustomEditor(typeof(BoardState))]
	public class BoardStateEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			if (GUILayout.Button("Read Board"))
			{
				var boardState = (BoardState)target;

				if (boardState == null)
					return;
				
				boardState.Clear();
				
				var board = GameObject.FindFirstObjectByType<Board>();

				if (board == null)
				{
					Debug.Log("Place a board onto scene");
					return;
				}
				
				var pieces = GameObject.FindObjectsOfType<Piece>();

				foreach (var piece in pieces)
				{
					var localPosition = board.transform.InverseTransformPoint(piece.transform.position);

					if (!board.Bounds.Contains(localPosition))
						continue;
					
					var prefab = PrefabUtility.GetCorrespondingObjectFromSource(piece);
					
					if (prefab == null)
						continue;
					
					var positionOnBoard = board.GetPositionOnBoard(piece);
					boardState.PlacePiece(positionOnBoard, prefab);
				}
			}
		}
	}
}