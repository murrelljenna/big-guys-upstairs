using game.assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NetworkedGameManager))]
public class MainMenu : MonoBehaviour
{
    private NetworkedGameManager gameManager;

    private string enteredSessionName = "TestRoom";

    private void Start()
    {
        gameManager = GetComponent<NetworkedGameManager>();
    }

    private void OnGUI()
    {
        if (gameManager._runner == null)
        {
            int xCenter = Screen.width / 2;
            int yCenter = Screen.height / 2;
            int buttonWidth = 200;
            int buttonHeight = 40;
            int padding = 10;
            var textStyle = new GUIStyle();
            textStyle.alignment = (TextAnchor)TextAlignment.Center;
            var oldColor = GUI.backgroundColor;
            GUI.Box(new Rect(xCenter - buttonWidth / 2 - padding, yCenter - buttonHeight - buttonHeight / 2 - padding * 2, buttonWidth + 2 * padding, buttonHeight * 4 + padding * 4 + 7), "");

            if (GUI.Button(new Rect(xCenter - buttonWidth / 2, yCenter - buttonHeight - padding - buttonHeight / 2, buttonWidth, buttonHeight), "Host"))
            {
                gameManager.InitGame("FourPlayer", enteredSessionName);
            }
            if (GUI.Button(new Rect(xCenter - buttonWidth / 2, yCenter - buttonHeight / 2, buttonWidth, buttonHeight), "Join"))
            {
                gameManager.JoinGame(enteredSessionName, "FourPlayer");
            }
            if (GUI.Button(new Rect(xCenter - buttonWidth / 2, yCenter + buttonHeight * 2, buttonWidth, buttonHeight), "Quit Game"))
            {
                gameManager.Quit();
            }
            GUI.Label(new Rect(xCenter - buttonWidth / 2 + 40, yCenter - buttonHeight / 2 + buttonHeight + padding, buttonWidth, buttonHeight), new GUIContent("Enter a room to join:"));
            enteredSessionName = GUI.TextField(new Rect(xCenter - buttonWidth / 2, yCenter - buttonHeight / 2 + buttonHeight + padding * 3.5f, buttonWidth, buttonHeight), enteredSessionName, 64, textStyle);
        }
    }
}
