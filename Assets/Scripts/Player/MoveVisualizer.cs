using System.Collections.Generic;
using Entity;
using UnityEngine;

namespace Player
{
	public class MoveVisualizer : MonoBehaviour
	{
		[SerializeField] private GameObject markerPrefab;
		
		private List<GameObject> _markers = new();

		public void ShowMovesOfPiece(Piece piece)
		{
			var possibleMoves = piece.GetPossibleMoves(true);
			var board = piece.Board;

			ClearMarkers();
			
			foreach (var move in possibleMoves)
			{
				var position = board.LocalToWorld(move);
				var offset = markerPrefab.transform.localPosition;
				var newMarker = Instantiate(markerPrefab, position + offset, markerPrefab.transform.rotation);
				
				_markers.Add(newMarker);
			}
		}

		public void ClearMarkers()
		{
			foreach (var marker in _markers)
			{
				Destroy(marker);
			}
			
			_markers.Clear();
		}
	}
}