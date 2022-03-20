using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GridSpace : MonoBehaviour
{
	[Header("Component References")]
	[SerializeField] Button button;						// Reference to this Button
	[SerializeField] TextMeshProUGUI buttonText;		// Reference to this button's text

	GameController gameController;						// Reference to the Game Controller (script)


	public void SetGameControllerReference(GameController gameController)
	{
		this.gameController = gameController; 
	}

	// This function is call when an interactive button is click.
	public void SetSpace()
	{
		buttonText.text = gameController.GetPlayerTurn();
		//button.image.sprite = gameController.GetSprite();	//set sprite on button image
		gameController.HandleBoardUpdate();
		button.interactable = false;
		gameController.EndTurn();
	}
}
