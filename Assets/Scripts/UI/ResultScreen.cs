using Entity;
using TMPro;
using UnityEngine;

namespace UI
{
	public class ResultScreen : MonoBehaviour
	{
		[SerializeField] private Color whiteColor, blackColor, stalemateColor;
		[SerializeField] private TMP_Text resultLabel;
		[SerializeField] private string whiteWinsText, blackWinsText, stalemateText;
		
		private GameRules _gameRules;
		private CanvasGroup _canvasGroup;

		private void Awake()
		{
			_gameRules = FindFirstObjectByType<GameRules>();
			_gameRules.OnGameEnd += ShowResult;

			_canvasGroup = GetComponent<CanvasGroup>();
		}

		private void OnDestroy()
		{
			_gameRules.OnGameEnd -= ShowResult;
		}

		private void ShowResult(GameResult result)
		{
			_canvasGroup.alpha = 1f;
			_canvasGroup.blocksRaycasts = true;
			
			switch (result.Type)
			{
				case ResultType.Stalemate:
					resultLabel.text = stalemateText;
					resultLabel.color = stalemateColor;
					break;
				
				case ResultType.Checkmate:

					switch (result.VictorColor)
					{
						case PieceColor.White:
							resultLabel.text = whiteWinsText;
							resultLabel.color = whiteColor;
							break;
						
						case PieceColor.Black:
							resultLabel.text = blackWinsText;
							resultLabel.color = blackColor;
							break;
					}
					break;
			}
		}
	}
}