using System;
using System.Collections.Generic;
using Entity;
using UnityEngine;

namespace Utility
{
	public class Simulation : IDisposable
	{
		private Board _boardCopy = null;
		private Dictionary<Piece, Piece> _pieceCopiesMap = new();

		public Simulation(Board board)
		{
			// Some random arbitrary position to hide copied board from player
			var position = new Vector3(200, 146, 700);
			
			_boardCopy = GameObject.Instantiate(board, position, Quaternion.identity);
		}

		public void Dispose()
		{
			GameObject.Destroy(_boardCopy.gameObject);
		}

		public void PieceMoveTo(Piece piece, Vector2Int move)
		{
			if (_pieceCopiesMap.ContainsKey(piece))
			{
				_pieceCopiesMap[piece].MoveTo(move);
				return;
			}
			
			// Link original piece with its copy
			var piecePosition = piece.GetPositionOnBoard();
			var pieceCopy = _boardCopy.GetCellPiece(piecePosition);
			
			_pieceCopiesMap.Add(piece, pieceCopy);
			
			pieceCopy.MoveTo(move);
		}

		public bool IsCheckmate(PieceColor color)
		{
			return _boardCopy.IsCheckmate(color);
		}
	}
}