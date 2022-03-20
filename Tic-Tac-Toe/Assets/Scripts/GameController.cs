using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameController : MonoBehaviour
{
    [Header("Text References")]
    [SerializeField] TextMeshProUGUI gameOverText;
    [SerializeField] TextMeshProUGUI turnText;
    [SerializeField] TextMeshProUGUI[] buttonList;

    [Header("GameObject References")]
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] GameObject restartButton;

    [Header("")]
    [SerializeField] Player playerX;
    [SerializeField] Player playerO;
    [SerializeField] PlayerColor activePlayerColor;
    [SerializeField] PlayerColor inactivePlayerColor;

    [SerializeField] bool isAI;     //This will control the option between Player vs Player or Player vs AI (future)

    // Private Variables
    string playerOne = "X";
    string playerTwo = "O";

    string currentPlayer;   
    string switchStartingPlayer;
    string draw = "Draw!";

    string[] board;     //A string array that referernce the gameboard

	private void Start()
	{
        SetGameControllerReferenceOnButtons();

        board = new string[buttonList.Length];
        SetBoardToEmpty();

        gameOverPanel.SetActive(false);
        restartButton.SetActive(false);

        SetStartingPlayer(playerOne);
        //add switchStartingPlayer = PlayerTwo;
        //Star Game function?
    }


	void SetGameControllerReferenceOnButtons()
    {
        for (int i = 0; i < buttonList.Length; i++)
        {
            buttonList[i].GetComponentInParent<GridSpace>().SetGameControllerReference(this);
        }
    }
  
    void SetBoardToEmpty()
	{
        for(int i = 0; i < board.Length; i++)
		{
            buttonList[i].text = "";
            board[i] = "";
		}
	}

    public void HandleBoardUpdate()
	{
        for (int i = 0; i < board.Length; i++)
        {
            board[i] = buttonList[i].text;
        }
    }

    //possible we do not need this 
    public void SetStartingPlayer(string startingPlayer)
    {
        currentPlayer = startingPlayer;

        if (currentPlayer == "X")
        {
			SetPlayerColors(playerX, playerO);  //remove?
			switchStartingPlayer = "O";
        }
        else
        {
			SetPlayerColors(playerO, playerX); //remove?
			switchStartingPlayer = "X";
        }
        StartGame();
    }

    void StartGame()
    {
        SetBoardInteractable(true);

        //if Computer goes first
        if (isAI && GetPlayerTurn() == playerTwo)
            ComputerTurn();
    }

    public string GetPlayerTurn()
    {
        return currentPlayer;
    }

	public void EndTurn()
    {
        if (IsWinner(board, currentPlayer))
		{
            GameOver(currentPlayer);
            return;
        }
        else if (IsBoardFull(board))
		{
           GameOver(draw);
           return;
		}
        else
		{
            ChangeTurn();

            //Check if it is Computer's turn
            if (isAI && GetPlayerTurn() == playerTwo)
                ComputerTurn();
        }
    }

    void ChangeTurn()
    {
        currentPlayer = (currentPlayer == playerOne) ? playerTwo : playerOne;

        //might remove this if-statement
        if (currentPlayer == playerOne)
		{
            SetTurnTextDisplay(playerOne);
        }
        else
		{
            SetTurnTextDisplay(playerTwo);
        }
    }

    //Unused Method...
    void SetPlayerColors(Player newPlayer, Player oldPlayer)
    {
        //Do I really need this, since I removed panel
        newPlayer.panel.color = activePlayerColor.panelColor;
        newPlayer.text.color = activePlayerColor.textColor;
        oldPlayer.panel.color = inactivePlayerColor.panelColor;
        oldPlayer.text.color = inactivePlayerColor.textColor;
    }

    void SetTurnTextDisplay(string playerTurnText)
	{
        turnText.text = playerTurnText + "'s turn";
	}

    void GameOver(string winningPlayer)
    {
        SetBoardInteractable(false);

        if (winningPlayer == draw)
        {
            SetGameOverText(draw);
            //SetPlayerColorsInactive();  //Unused
        }
        else
            SetGameOverText($"{winningPlayer} wins!");

        turnText.text = "";
        restartButton.SetActive(true);
    }

    void SetGameOverText(string text)
    {
        gameOverPanel.SetActive(true);
        gameOverText.text = text;
    }

    public void RestartGame()
    {
        gameOverPanel.SetActive(false);
        restartButton.SetActive(false);
        //SetPlayerButton(true);    //Unused
        //SetPlayerColorsInactive();  //Unused
        SetBoardToEmpty();

        //Switch starting player
        SetStartingPlayer(switchStartingPlayer);
        SetTurnTextDisplay(switchStartingPlayer);
    }

    //Unused
    void SetPlayerButton(bool toogle)
    {
        playerX.button.interactable = toogle;
        playerO.button.interactable = toogle;
    }

    void SetBoardInteractable(bool toogle)
    {
        for (int i = 0; i < buttonList.Length; i++)
        {
            buttonList[i].GetComponentInParent<Button>().interactable = toogle;
        }
    }

    //Unused
    void SetPlayerColorsInactive()
    {
        playerX.panel.color = inactivePlayerColor.panelColor;
        playerX.text.color = inactivePlayerColor.textColor;
        playerO.panel.color = inactivePlayerColor.panelColor;
        playerO.text.color = inactivePlayerColor.textColor;
    }

    //AI method (Unbeatable AI)
    void ComputerTurn()
    {
        float bestScore = Mathf.NegativeInfinity;
        int bestMove = 0;

        for (int i = 0; i < board.Length; i++)
        {
            if (board[i] == "")
            {
                board[i] = playerTwo;
                float score = Minimax(board, 0, Mathf.NegativeInfinity, Mathf.Infinity, false);
                
                //Unused Data for controlling the AI difficult level   
                // + Random.Range(0f, 1.60f);
                //.35f Impossible Mode 
                //1.25f Hard Mode
                //1.65f Normal Mode
                //2.20f Easy Mode
                
                board[i] = "";
                    
                if (score > bestScore)
                {
                    bestScore = score;
                    bestMove = i;
                }
            }
        }
        
        buttonList[bestMove].GetComponentInParent<Button>().onClick.Invoke();
    }

    float Minimax(string [] board, int depth, float alpha, float beta, bool isMaximizing) 
    {
        //need to review my win-condition 
        if (IsWinner(board, playerOne))
            return -1;
        if (IsWinner(board, playerTwo))
            return 1;
        if (IsBoardFull(board))
            return 0;

      
        if(isMaximizing)
		{
            float bestScore = Mathf.NegativeInfinity;

            for (int i = 0; i < board.Length; i++)
            {
                if (board[i] == "")
                {
                    board[i] = playerTwo;
                    float score = Minimax(board, depth + 1, alpha, beta, false);
                    board[i] = "";
                    bestScore = Mathf.Max(score, bestScore);

                    alpha = Mathf.Max(alpha, score);
                    if (beta <= alpha)
                        break;
                }
            }
            return bestScore;
        }
        else
        {
            float bestScore = Mathf.Infinity;

            for (int i = 0; i < board.Length; i++)
            {
                if (board[i] == "")
                {
                    board[i] = playerOne;
                    float score = Minimax(board, depth + 1, alpha, beta, true);
                    board[i] = "";
                    bestScore = Mathf.Min(score, bestScore);

                    beta = Mathf.Min(beta, score);
                    if (beta <= alpha)
                        break;
                }
            }
            return bestScore;
        }
    }


    //Check for win-condition 
    bool IsWinner(string [] board, string playerSide)
    {
        // Check each row for a winner
        if (board[0] == playerSide && board[1] == playerSide && board[2] == playerSide) return true;
        else if (board[3] == playerSide && board[4] == playerSide && board[5] == playerSide) return true;
        else if (board[6] == playerSide && board[7] == playerSide && board[8] == playerSide) return true;

        // Check each column for a winner
        else if (board[0] == playerSide && board[3] == playerSide && board[6] == playerSide) return true;
        else if (board[1] == playerSide && board[4] == playerSide && board[7] == playerSide) return true;
        else if (board[2] == playerSide && board[5] == playerSide && board[8] == playerSide) return true;

        // Check each diagonal for a winner
        else if (board[0] == playerSide && board[4] == playerSide && board[8] == playerSide) return true;
        else if (board[2] == playerSide && board[4] == playerSide && board[6] == playerSide) return true;
        
        // return false if no winner
        else return false;
    }

    //Check the board for a draw 
    bool IsBoardFull(string[] board)
	{
        for (int i = 0; i < board.Length; i++)
        {
            if (board[i] == "")
                return false;
        }
        return true;
    }


    [System.Serializable]
    public class Player
    {
        public Image panel;
        public TextMeshProUGUI text;
        public Button button;
    }

    [System.Serializable]
    public class PlayerColor
    {
        public Color panelColor;
        public Color textColor;
    }
}
