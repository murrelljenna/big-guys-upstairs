using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    private Vector3 frozenPosition;
    private Quaternion frozenRotation;
    public void toggleEnable()
    {
        enabled = !enabled;

        if (enabled)
        {
            frozenPosition = transform.position;
            frozenRotation = transform.rotation;
            Cursor.lockState = CursorLockMode.None;

            GetComponent<CharacterController>().enabled = false;
            GetComponent<CharacterViewHandler>().isActive = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            GetComponent<CharacterController>().enabled = true;
            GetComponent<CharacterViewHandler>().isActive = true;
        }
    }

    private void Update()
    {
        if (enabled)
        {
            transform.position = frozenPosition;
            transform.rotation = frozenRotation;
        }
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
                //
            }
            if (GUI.Button(new Rect(xCenter - buttonWidth / 2, yCenter - buttonHeight - padding - buttonHeight / 2, buttonWidth, buttonHeight), "Return to Main Menu"))
            {
                //
            }
            if (GUI.Button(new Rect(xCenter - buttonWidth / 2, yCenter - buttonHeight / 2, buttonWidth, buttonHeight), "Exit Game"))
            {
                //
            }
            /*
            GUI.Label(new Rect(xCenter - buttonWidth / 2 + 40, yCenter - buttonHeight / 2 + buttonHeight + padding, buttonWidth, buttonHeight), new GUIContent("Enter a room to join:"));
            enteredSessionName = GUI.TextField(new Rect(xCenter - buttonWidth / 2, yCenter - buttonHeight / 2 + buttonHeight + padding * 3.5f, buttonWidth, buttonHeight), enteredSessionName, 64, textStyle);*/
        }
    }
}
