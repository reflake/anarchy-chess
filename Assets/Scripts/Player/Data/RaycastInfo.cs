using Entity;
using UnityEngine;

namespace Player.Data
{
	public class RaycastInfo
	{
		public Piece PieceHit = null;
		public Board BoardHit = null;
		public Vector2Int BoardPoint;
		public bool Hit = false;

		public RaycastInfo()
		{
		}

		public RaycastInfo(Piece piece)
		{
			Hit = true;
			PieceHit = piece;
		}

		public RaycastInfo(Board board, Vector3 point)
		{
			Hit = true;
			BoardHit = board;
			BoardPoint = board.WorldToLocal(point);
		}
	}
}