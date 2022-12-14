using game.assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    private Vector3 frozenPosition;
    private Quaternion frozenRotation;

    public Scene mainMenuScene; 

    public void toggleEnable()
    {
        enabled = !enabled;
    }

    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        NetworkedGameManager.Get().playerCharacterFrozen = false;
        transform.parent.GetComponent<CharacterViewHandler>().isActive = true;
    }

    private void OnEnable()
    {
        frozenPosition = transform.position;
        frozenRotation = transform.rotation;
        Cursor.lockState = CursorLockMode.None;

        NetworkedGameManager.Get().playerCharacterFrozen = true;

        transform.parent.GetComponent<CharacterViewHandler>().isActive = false;
    }

    private void OnGUI()
    {
        if (enabled)
        {
            int xCenter = Screen.width / 2;
            int yCenter = Screen.height / 2;
            int buttonWidth = 200;
            int buttonHeight = 40;
            int padding = 10;
            var textStyle = new GUIStyle();
            textStyle.alignment = (TextAnchor)TextAlignment.Center;
            //var oldColor = GUI.backgroundColor;
            GUI.Box(new Rect(xCenter - buttonWidth / 2 - padding, yCenter - buttonHeight - buttonHeight / 2 - padding * 2 - buttonHeight - padding, buttonWidth + 2 * padding, buttonHeight * 3 + padding * 4 + 5), "");

            if (GUI.Button(new Rect(xCenter - buttonWidth / 2, yCenter - buttonHeight - padding - buttonHeight / 2 - buttonHeight - padding, buttonWidth, buttonHeight), "Resume"))
            {
                enabled = false;
            }
            if (GUI.Button(new Rect(xCenter - buttonWidth / 2, yCenter - buttonHeight - padding - buttonHeight / 2, buttonWidth, buttonHeight), "Return to Main Menu"))
            {
                NetworkedGameManager.Get().QuitToMainMenu();   

            }
            if (GUI.Button(new Rect(xCenter - buttonWidth / 2, yCenter - buttonHeight / 2, buttonWidth, buttonHeight), "Exit Game"))
            {
                NetworkedGameManager.Get().Quit();
            }
            /*
            GUI.Label(new Rect(xCenter - buttonWidth / 2 + 40, yCenter - buttonHeight / 2 + buttonHeight + padding, buttonWidth, buttonHeight), new GUIContent("Enter a room to join:"));
            enteredSessionName = GUI.TextField(new Rect(xCenter - buttonWidth / 2, yCenter - buttonHeight / 2 + buttonHeight + padding * 3.5f, buttonWidth, buttonHeight), enteredSessionName, 64, textStyle);*/
        }
    }
}
