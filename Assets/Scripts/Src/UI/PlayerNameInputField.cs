using System.Collections;

using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(InputField))]
public class PlayerNameInputField : MonoBehaviour
{
	const string playerNamePrefKey = "Player";
    // Start is called before the first frame update
    void Start () {
        string defaultName = string.Empty;
        InputField _inputField = this.GetComponent<InputField>();
        if (_inputField!=null)
        {
            if (PlayerPrefs.HasKey(playerNamePrefKey)) {
                defaultName = PlayerPrefs.GetString(playerNamePrefKey);
                _inputField.text = defaultName;
            }
        }
    }

    public void SetPlayerName(string value) {
        // #Important
        if (string.IsNullOrEmpty(value))
        {
            Debug.LogError("Player Name is null or empty");
            return;
        }

        PlayerPrefs.SetString(playerNamePrefKey,value);
    }
}
