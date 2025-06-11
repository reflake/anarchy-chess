using UnityEditor;
using UnityEngine;

namespace Entity
{
	public class Board : MonoBehaviour
	{
		[SerializeField] private float width, length;
		[SerializeField] private int rows, columns;
		
		public Bounds Bounds => new Bounds(Vector3.zero, new Vector3(width, 1f, length));
		public int Rows => rows;
		public int Columns => columns;

		private void OnDrawGizmosSelected()
		{
			using (new Handles.DrawingScope())
			{
				Handles.color = Color.blue;
				Handles.DrawWireCube(Vector3.zero, new Vector3(width, 0.5f, length));
			}
		}

		public Vector2Int GetPositionOnBoard(Piece piece)
		{
			var localPosition = transform.InverseTransformPoint(piece.transform.position);
			
			// Normalize coordinates of the piece
			localPosition += new Vector3(width / 2, 0, length / 2);
			localPosition.x /= width;
			localPosition.z /= length;
			
			// Try find at which row and column the piece is residing
			localPosition.x *= columns;
			localPosition.z *= rows;

			return new Vector2Int(
				Mathf.FloorToInt(localPosition.x), 
				Mathf.FloorToInt(localPosition.z));
		}
	}
}