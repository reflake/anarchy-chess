using Data;
using Entity;
using UnityEditor;
using UnityEngine;

namespace Editor
{
	[CustomEditor(typeof(BoardState))]
	public class BoardStateEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			
			var boardState = (BoardState)target;

			if (boardState == null)
				return;

			if (GUILayout.Button("Read Board"))
			{
				ReadBoard(boardState);
			}

			if (GUILayout.Button("Place Pieces"))
			{
				PlacePieces(boardState);
			}
		}

		private void ReadBoard(BoardState boardState)
		{
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
				if (!board.IsPieceOnBoard(piece))
					continue;

				var prefab = PrefabUtility.GetCorrespondingObjectFromSource(piece);
					
				if (prefab == null)
					continue;
					
				var positionOnBoard = board.GetPositionOnBoard(piece);
				boardState.PlacePiece(positionOnBoard, prefab);
			}
		}

		private void PlacePieces(BoardState boardState)
		{
			var board = GameObject.FindFirstObjectByType<Board>();

			if (board == null)
			{
				Debug.Log("No board to place pieces on");
				return;
			}
			
			var previousPieces = GameObject.FindObjectsOfType<Piece>();

			foreach (var piece in previousPieces)
			{
				if (board.IsPieceOnBoard(piece))
				{
					DestroyImmediate(piece.gameObject);
				}
			}

			foreach (var position in boardState.PiecePositions)
			{
				var piece = Instantiate(position.Prefab);
				
				board.Put(piece, position.LocalPosition);
			}
		}
	}
}