using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ModeToggler : MonoBehaviour
{
    [SerializeField] private TMP_Text modeText;
    [SerializeField] private bool isProMode = false;


    private void Update(){
        TextModeHandler();
    }

    private void TextModeHandler()
    {
        if (SceneManager.GetActiveScene().name.ToLower() == "main" && isProMode)
        {
            var redProText = "\"PRO\" MODE\n <color=red>DISABLED</color>";
            modeText.text = redProText;
            isProMode = false;
            
        }
        else if(SceneManager.GetActiveScene().name.ToLower() != "main" && !isProMode)
        {
            var greenFunText = "\"PRO\" MODE\n <color=green>ENABLED</color>";
            modeText.text = greenFunText;
            isProMode = true;
        }
    }

    [ContextMenu("Toggle Debug")]
    private void ToggleDebug(){
        Debug.Log("Toggle Debug");
        ToggleProMode();
    }

    public void ToggleProModeButton(){
        ToggleProMode();
    }

    private void ToggleProMode(){
        var nextScene = SceneManager.GetActiveScene().buildIndex + 1;
        
        if(nextScene >= SceneManager.sceneCountInBuildSettings){
            nextScene = 0;
        }

        SceneManager.LoadScene(nextScene);
    }
}
