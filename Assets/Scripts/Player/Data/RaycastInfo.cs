using Entity;

namespace Player.Data
{
	public class RaycastInfo
	{
		public Piece PieceHit = null;
		public bool Hit = false;

		public RaycastInfo()
		{
		}

		public RaycastInfo(Piece piece)
		{
			Hit = true;
			PieceHit = piece;
		}
	}
}