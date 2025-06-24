using System;
using System.Collections.Generic;
using Entity;
using UnityEngine;

namespace Utility
{
	public class Simulation : IDisposable
	{
		private Board _boardCopy = null;

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

		public void MakeMove(Move move)
		{
			// Perform move on copied board
			move.Perform(_boardCopy);
		}

		public bool IsCheckmate(PieceColor color)
		{
			return _boardCopy.IsCheckmate(color);
		}
	}
}