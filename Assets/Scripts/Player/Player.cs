using Entity;
using UnityEngine;

namespace Player
{
	public class Player : MonoBehaviour
	{
		public void OnSelectPiece(Piece piece)
		{
			SendMessage(nameof(MoveVisualizer.ShowMovesOfPiece), piece);
		}

		public void OnUnselectPiece(Piece _)
		{
			SendMessage(nameof(MoveVisualizer.ClearMarkers));
		}
	}
}