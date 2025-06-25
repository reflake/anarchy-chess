using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Data;
using UnityEditor;
using UnityEngine;

namespace Entity
{
	public class Board : MonoBehaviour
	{
		[SerializeField] private PieceColor currentTurnPlayerColor;
		[SerializeField] private float width, length;
		[SerializeField] private int rows, columns;
		
		public Bounds Bounds => new Bounds(Vector3.zero, new Vector3(width, 1f, length));
		public PieceColor CurrentTurnPlayerColor => currentTurnPlayerColor;
		public int Rows => rows;
		public int Columns => columns;
		public IReadOnlyList<Piece> Pieces => GetComponentsInChildren<Piece>(false);

		private Piece[,] _cachedPieces = null;

		public Vector2Int GetPositionOnBoard(Piece piece)
		{
			return WorldToLocal(piece.transform.position);
		}

		public Vector2Int WorldToLocal(Vector3 point)
		{
			var localPosition = transform.InverseTransformPoint(point);
			
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

		public void Put(Piece piece, Vector2Int localPosition)
		{
			var worldPosition = LocalToWorld(localPosition);

			piece.transform.parent = transform;
			piece.transform.position = worldPosition;

			piece.SetBoard(this);
		}

		public Vector3 LocalToWorld(Vector2Int localPosition)
		{
			var worldPosition = new Vector3(localPosition.x, 0, localPosition.y);
			
			// Normalize coordinates
			worldPosition.x /= columns;
			worldPosition.z /= rows;
			worldPosition -= new Vector3(0.5f, 0, 0.5f);
			
			// Convert to world position relative to board
			worldPosition.x *= width;
			worldPosition.z *= length;
			worldPosition = transform.TransformPoint(worldPosition);
			
			// Put the piece in the center of a cell
			Vector3 offset = new Vector3(width / columns / 2, 0, length / rows / 2);
			
			return worldPosition + offset;
		}

		public void PlacePieces(BoardConfiguration boardConfiguration)
		{
			// Remove pieces which previously occupied board
			var previousPieces = FindObjectsOfType<Piece>(false);

			foreach (var piece in previousPieces)
			{
				if (piece.Board == this || IsPieceOnBoard(piece))
				{
					Destroy(piece.gameObject);
				}
			}
			
			foreach (var piecePosition in boardConfiguration.PiecePositions)
			{
				Piece piece = Instantiate(piecePosition.Prefab);
				Put(piece, piecePosition.LocalPosition);
			}
		}
		
		public bool IsPieceOnBoard(Piece piece)
		{
			var localPosition = transform.InverseTransformPoint(piece.transform.position);
			
			return Bounds.Contains(localPosition);
		}

		public bool IsPositionOnBoard(Vector2Int move)
		{
			return 0 <= move.x && move.x < columns && 
			       0 <= move.y && move.y < rows;
		}

		public void NextPlayerTurn(bool triggerEvent)
		{
			switch (currentTurnPlayerColor)
			{
				case PieceColor.White:
					currentTurnPlayerColor = PieceColor.Black;
					break;
				case PieceColor.Black:
					currentTurnPlayerColor = PieceColor.White;
					break;
			}
			
			if (triggerEvent)
				FindFirstObjectByType<GameRules>().HandleTurnEvent(this);
		}

		public Piece GetCellPiece(Vector2Int boardPos)
		{
			if (_cachedPieces == null)
			{
				_cachedPieces = new Piece[rows, columns];

				var pieces = GetComponentsInChildren<Piece>(false);
				foreach (var piece in pieces)
				{
					if (piece.Board != this)
						continue;

					if (!IsPieceOnBoard(piece))
						continue;
					
					var localPos = WorldToLocal(piece.transform.position);
					
					_cachedPieces[localPos.x, localPos.y] = piece;
				}
			}

			if (!IsPositionOnBoard(boardPos))
				return null;
			
			return _cachedPieces[boardPos.x, boardPos.y];
		}

		public bool IsCheckmate(PieceColor colorToCheck)
		{
			var pieces = GetComponentsInChildren<Piece>(false);
			var king = pieces.Single(x => x.IsKing && x.Color == colorToCheck);
			
			return IsCellUnderAttack(colorToCheck, king.Position);
		}

		public bool IsCellUnderAttack(PieceColor colorToCheck, Vector2Int position)
		{
			var pieces = GetComponentsInChildren<Piece>(false);
			foreach (var opponentPiece in pieces)
			{
				if (opponentPiece.Color == colorToCheck)
					continue;

				if (opponentPiece.IsAttackingCell(position))
					return true;
			}

			return false;
		}

		public bool CanCurrentPlayerMove()
		{
			var pieces = GetComponentsInChildren<Piece>(false);
			foreach (var piece in pieces)
			{
				if (piece.Color != currentTurnPlayerColor)
					continue;

				if (piece.GetPossibleMoves(true).Any())
					return true;
			}
			
			return false;
		}

		public void InvalidateGrid()
		{
			_cachedPieces = null;
		}

		private void OnDrawGizmosSelected()
		{
			using (new Handles.DrawingScope(transform.localToWorldMatrix))
			{
				Handles.color = Color.blue;
				Handles.DrawWireCube(Vector3.zero, new Vector3(width, 0.5f, length));
			}
		}

		private void LateUpdate()
		{
			_cachedPieces = null;
		}
	}
}