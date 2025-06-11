using Data;
using UnityEngine;

namespace Entity
{
	public class GameRules : MonoBehaviour
	{
		[SerializeField] private BoardState boardState;

		private void Awake()
		{
			if (boardState == null)
				return;
			
			var board = GameObject.FindObjectOfType<Board>();

			if (board == null)
				return;
			
			foreach (var piecePosition in boardState.PiecePositions)
			{
				Piece piece = Instantiate(piecePosition.Prefab);
				
				board.Put(piece, piecePosition.LocalPosition);
			}
		}
	}
}