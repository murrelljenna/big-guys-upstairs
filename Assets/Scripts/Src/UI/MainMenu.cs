using game.assets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NetworkedGameManager))]
public class MainMenu : MonoBehaviour
{
    private NetworkedGameManager gameManager;

    private string enteredSessionName = "TestRoom";

    private PageState pageState = PageState.Main;

    public Texture backgroundBoxTexture;
    public Texture backgroundBoxTextureAlt;
    public Texture buttonTexture;
    public Texture buttonTexturePressed;

    private enum PageState
    {
        Main,
        Multiplayer,
        HostGame,
        JoinGame
    }

    private void Start()
    {
        gameManager = GetComponent<NetworkedGameManager>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && gameManager._runner == null)
        {
            goBack();
        }
    }

    private void goBack() {
        if (pageState == PageState.Main)
        {
            gameManager.Quit();
        }
        else if (pageState == PageState.Multiplayer)
        {
            OpenMainMenu();
        }
        else if (pageState == PageState.HostGame)
        {
            OpenMultiplayerMenu();
        }
        else if (pageState == PageState.JoinGame)
        {
            OpenMultiplayerMenu();
        }
    }

    public GameMap[] AllMaps;

    private string[] AllMapNames()
    {
        string[] mapNames = new string[AllMaps.Length];

        for (int i = 0; i < AllMaps.Length; i++)
        {
            mapNames[i] = AllMaps[i].sceneName;
        }

        return mapNames;
    }

    private Texture[] AllMapTextures()
    {
        Texture[] textures = new Texture[AllMaps.Length];

        for (int i = 0; i < AllMaps.Length; i++)
        {
            textures[i] = AllMaps[i].icon;
        }

        return textures;
    }

    private int selectedScene = 0;

    private void OnGUI()
    {
        /* 
         * Brace yourself. This is about to get ugly
         */
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

            GUIStyle style = new GUIStyle();
            style.stretchHeight = true;
            style.stretchWidth = true;

            Rect boxR = new Rect(xCenter - buttonWidth / 2 - padding, yCenter - buttonHeight / 2 + buttonHeight, buttonWidth + 2 * padding, buttonHeight * 2 + padding * 3);
            GUI.DrawTexture(boxR, backgroundBoxTexture, ScaleMode.StretchToFill);

            Rect buttonRect1 = new Rect(xCenter - buttonWidth / 2, yCenter - buttonHeight / 2 + buttonHeight + padding, buttonWidth, buttonHeight);
            Rect buttonRect2 = new Rect(xCenter - buttonWidth / 2, yCenter + buttonHeight * 2, buttonWidth, buttonHeight);
            Rect backButtonRect = new Rect(xCenter - buttonWidth / 2, yCenter + buttonHeight * 3 + padding * 1.5f, buttonWidth / 4, buttonHeight);

            if (pageState == PageState.Main)
            {
                GUI.Box(boxR, "", style);

                GUI.DrawTexture(buttonRect1, buttonTexture, ScaleMode.StretchToFill);
                GUI.DrawTexture(buttonRect2, buttonTexture, ScaleMode.StretchToFill);
                if (GUI.Button(buttonRect1, "Multiplayer"))
                {
                    OpenMultiplayerMenu();
                }
                if (GUI.Button(buttonRect2, "Quit Game"))
                {
                    gameManager.Quit();
                }
            }
            else if (pageState == PageState.Multiplayer)
            {
                GUI.Box(boxR, "", style);
                GUI.DrawTexture(buttonRect1, buttonTexture, ScaleMode.StretchToFill);
                GUI.DrawTexture(buttonRect2, buttonTexture, ScaleMode.StretchToFill);
                GUI.DrawTexture(backButtonRect, buttonTexture, ScaleMode.StretchToFill);
                if (GUI.Button(buttonRect1, "Host New Game"))
                {
                    OpenHostGameMenu();
                }
                if (GUI.Button(buttonRect2, "Join a Game"))
                {
                    OpenJoinGameMenu();
                }

                if (GUI.Button(backButtonRect, "Back"))
                {
                    goBack();
                }
            }
            else if (pageState == PageState.JoinGame)
            {
                GUI.Box(boxR, "", style);
                GUI.Label(new Rect(xCenter - buttonWidth / 2 + 27, yCenter - buttonHeight / 2 + buttonHeight + padding, buttonWidth, buttonHeight), new GUIContent("Enter server name to join:"));
                enteredSessionName = GUI.TextField(new Rect(xCenter - buttonWidth / 2, yCenter - buttonHeight / 2 + buttonHeight + padding * 3.5f, buttonWidth, buttonHeight), enteredSessionName, 64, textStyle);

                GUI.DrawTexture(buttonRect2, buttonTexture, ScaleMode.StretchToFill);
                GUI.DrawTexture(backButtonRect, buttonTexture, ScaleMode.StretchToFill);

                if (GUI.Button(buttonRect2, "Join"))
                {
                    gameManager.JoinGame(enteredSessionName, "FourPlayer");
                }

                if (GUI.Button(new Rect(xCenter - buttonWidth / 2, yCenter + buttonHeight * 3 + padding * 1.5f, buttonWidth / 4, buttonHeight), "Back"))
                {
                    goBack();
                }
            }
            else if (pageState == PageState.HostGame)
            {
                Rect largeBoxR = new Rect(xCenter - buttonWidth / 2 - padding, yCenter - buttonHeight - buttonHeight / 2 - padding * 4, buttonWidth + 2 * padding, buttonHeight * 4 + padding * 7 + 7);
                GUI.DrawTexture(largeBoxR, backgroundBoxTexture, ScaleMode.StretchToFill);

                Rect mapSelectionRect = new Rect(xCenter - buttonWidth / 2, yCenter - buttonHeight - buttonHeight / 2 - padding / 4, buttonWidth, buttonHeight * 2 + padding);

                Rect mapSelectionBackground = mapSelectionRect;
                mapSelectionBackground.x = mapSelectionRect.x - padding / 2;
                mapSelectionBackground.width = mapSelectionRect.width + (padding);
                mapSelectionBackground.y = mapSelectionRect.y - padding * 2.5f;
                mapSelectionBackground.height = mapSelectionRect.height + padding * 3;






                GUI.DrawTexture(mapSelectionBackground, backgroundBoxTextureAlt, ScaleMode.StretchToFill);
                GUI.Label(new Rect(xCenter - buttonWidth / 2, yCenter - buttonHeight - buttonHeight / 2 - padding * 2.25f, buttonWidth / 2, buttonHeight * 2 + padding), new GUIContent("Map:"));
                selectedScene = GUI.SelectionGrid(
                    mapSelectionRect,
                    selectedScene,
                    AllMapNames(),
                    1);

                Rect sceneBox1 = new Rect(xCenter - buttonWidth / 2, yCenter - buttonHeight - buttonHeight / 2 - padding / 4, buttonWidth, buttonHeight * 2 + padding);

                GUI.Box(largeBoxR, "", style);
                GUI.DrawTexture(buttonRect2, buttonTexture, ScaleMode.StretchToFill);

                var labelStyle = new GUIStyle();
                labelStyle.normal.textColor = Color.black;

                GUI.Label(new Rect(xCenter - buttonWidth / 2 + 5, yCenter - buttonHeight / 2 + buttonHeight + padding * 3.5f, buttonWidth / 2, buttonHeight), new GUIContent("Server Name:"), labelStyle);
                Rect sessionNameRect = new Rect(xCenter - 5, yCenter - buttonHeight / 2 + buttonHeight + padding * 3.5f - 2, buttonWidth / 2, buttonHeight / 2);

                GUI.DrawTexture(sessionNameRect, buttonTexturePressed, ScaleMode.StretchToFill);

                sessionNameRect.y = sessionNameRect.y + 3;
                enteredSessionName = GUI.TextField(sessionNameRect, enteredSessionName, 15, textStyle);

                if (GUI.Button(new Rect(xCenter - buttonWidth / 2, yCenter + buttonHeight * 2, buttonWidth, buttonHeight), "Start Server"))
                {
                    gameManager.InitGame(AllMaps[selectedScene], enteredSessionName);
                }
                GUI.DrawTexture(backButtonRect, buttonTexture, ScaleMode.StretchToFill);
                if (GUI.Button(backButtonRect, "Back"))
                {
                    goBack();
                }
            }
        }
    }

    private void DrawSelectionGridTextures()
    {

    }

    private void OpenMainMenu()
    {
        pageState = PageState.Main;
    }

    private void OpenMultiplayerMenu()
    {
        pageState = PageState.Multiplayer;
    }

    private void OpenHostGameMenu()
    {
        pageState = PageState.HostGame;
    }

    private void OpenJoinGameMenu()
    {
        pageState = PageState.JoinGame;
    }
}
