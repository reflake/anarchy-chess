using System;
using System.Collections.Generic;
using Entity;
using UnityEngine;

namespace Data
{
	[CreateAssetMenu(fileName = "Board State", menuName = "Data/Chess/Board State", order = 0)]
	public class BoardConfiguration : ScriptableObject
	{
		[SerializeField] private List<PiecePosition> piecePositions = new();
		
		public IReadOnlyList<PiecePosition> PiecePositions => piecePositions;

		public void Clear()
		{
			piecePositions = new();
		}

		public void PlacePiece(Vector2Int position, Piece prefab)
		{
			piecePositions.Add(new PiecePosition(position, prefab));
		}
	}

	[Serializable]
	public class PiecePosition
	{
		[field: SerializeField] public Vector2Int LocalPosition { get; private set; }
		[field: SerializeField] public Piece Prefab { get; private set; }

		public PiecePosition(Vector2Int localPosition, Piece prefab)
		{
			LocalPosition = localPosition;
			Prefab = prefab;
		}
	}
}