using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static game.assets.utilities.GameUtils;
using game.assets;
public class InitializeGameOnStart : MonoBehaviour
{
    [Tooltip("Player spawn location")]
    public Vector3 playerSpawn;
    public string scenePath = "Assets/Scenes/TestScenes/Manual Test Scenes/TestLoadGameAndPlay/TestPlayDontUseThisDirectly.unity";

    void Start()
    {
        GameManager gameManager = GameObject.Find(MagicWords.GameObjectNames.GameManager).GetComponent<LocalGameManager>();
        gameManager.Initialize(scenePath, new Vector3[] { playerSpawn });
    }
}
