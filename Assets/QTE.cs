using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QTE : MonoBehaviour
{
    [Tooltip("Mouse over before pressing the keys.")]
    public bool mouseOver;

    public Wave[] waves;
    public GameObject keyBox;
    public GameObject container;
    public Text timerText;
    public Image timerBackground;

    private bool waveInProgress = false;
    private Text keyText;
    private int currentWave = 0;
    private bool success = false;
    private float timer;
    private Color timerBackgroundColor;

    protected void Start()
    {
        LaunchWave(currentWave);
        timer = waves[currentWave].timeToReact;
        timerBackgroundColor = timerBackground.color;
        ResetKeys();
    }

    protected void Update()
    {
        if(waveInProgress)
        {
            
            timer -= Time.deltaTime;
            timerText.text = timer.ToString("0.00");
            if (timer <= 0)           {
                waveInProgress = false;
                success = false;
                NextWave();
                return;
            }


            DetectForKeyPress();

            if(DetectForEndWave())
            {
                waveInProgress = false;
                success = true;
                NextWave();
                return;                               
            }
            
            
        }
    }  

    private IEnumerator ChangeColor(bool success)
    {
        if (success)
            timerBackground.color = Color.green;
        else timerBackground.color = Color.red;

        yield return new WaitForSeconds(0.2f);

        timerBackground.color = timerBackgroundColor;  
    }

    #region Keys Management
    private void ResetKeys()
    {
        for (int i = 0; i < waves.Length; i++)
        {
            foreach (Key key in waves[i].keys)
            {
                key.isPressed = false;
                key.timesToPressLeft = key.timesToPress;
            }
        }
    }

    private void DetectForKeyPress()
    {
        //Debug.Log("Detecting..");

        foreach (Key key in waves[currentWave].keys)
        {          
            if((mouseOver && DetectIfMouseOver(key)) || !mouseOver)
            {
                Debug.Log("passed");
                if(Input.GetKeyDown(key.keyCode))
                {
                    if (key.timesToPressLeft > 1)
                    {
                        key.timesToPressLeft--;
                        key.multiTouchText.text = key.timesToPressLeft.ToString();
                    }
                    else
                    {
                        key.isPressed = true;
                        key.multiTouchText.text = "✓";
                    }
                }
            }

        }
    }

    private bool DetectIfMouseOver(Key key)
    {
        DetectMouseOver detectMouseOver = key.keyContainer.GetComponent<DetectMouseOver>();
        return detectMouseOver.mouseOver;
    }
    #endregion

    #region Waves Management
    private void NextWave()
    {
        StartCoroutine(ChangeColor(success));

        currentWave++;

        if (currentWave >= waves.Length)
        {
            currentWave = 0;
            ResetKeys();
        }

        Debug.Log("Game ended...:" + success.ToString() + " Restarting..");

        for (int i = 0; i < container.transform.childCount; i++)
        {
            Destroy(container.transform.GetChild(i).gameObject);
        }

        //timer = waves[currentWave].timeToReact;

        LaunchWave(currentWave);
    }

    private void LaunchWave(int waveToLaunch)
    {
        Debug.Log("Launching wave " + waveToLaunch.ToString());
        timer = waves[currentWave].timeToReact;
        success = false;

        foreach (Key key in waves[waveToLaunch].keys)
        {
            GameObject keyBox_ = Instantiate(keyBox);
            keyBox_.transform.SetParent(container.transform);
            keyBox_.transform.localPosition = key.keyPosition;
            keyText = keyBox_.GetComponentInChildren<Text>();
            keyText.text = key.keyName;

            Transform multiTouchContainer = keyBox_.transform.GetChild(1);
            Text multiTouchContainerText = multiTouchContainer.GetComponentInChildren<Text>();
            multiTouchContainerText.text = key.timesToPress.ToString();
            key.multiTouchText = multiTouchContainerText;

            key.keyContainer = keyBox_;
        }

        waveInProgress = true;

        //StartCoroutine(WaveTimer());
    }

    private bool DetectForEndWave()
    {
        int keysPressed = 0;
        foreach (Key key in waves[currentWave].keys)
        {
            if (key.isPressed)
                keysPressed++;
        }

        if (keysPressed < waves[currentWave].keys.Length)
            return false;
        else return true;
    }
    #endregion

}

[System.Serializable]
public class Wave
{
    public Key[] keys;

    [Tooltip("In Seconds")]
    public float timeToReact;
    
}

[System.Serializable]
public class Key
{
    public KeyCode keyCode;
    public bool isPressed = false;
    public Vector3 keyPosition;
    public string keyName;
    public int timesToPress;

    [HideInInspector]
    public int timesToPressLeft;
    [HideInInspector]
    public Text multiTouchText;
    [HideInInspector]
    public GameObject keyContainer;
}