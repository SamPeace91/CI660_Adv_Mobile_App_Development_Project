using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour
{
    //Obtains the HUD elements
    public int coinCount;
    public int potionCount;
    public Text coinText;
    public TextMeshProUGUI potionText;
    public TextMeshProUGUI coinResult;

    public HeroKnight player;

    //Obtains all of the menus and overlays
    public GameObject settingsScreen;
    public GameObject pauseScreen;
    public GameObject HUD;
    public GameObject controls;
    public GameObject editControls;
    public GameObject menuScreen;
    public GameObject winScreen;
    public GameObject loseScreen;

    //Gyroscope
    public bool gyroActive = false;
    public Toggle settingsGyro;
    public Toggle pauseGyro;
    bool settingsGyroFlip = false;
    bool pauseGyroFlip = false;

    //Movable Controls
    public Image joystickImg;
    public Image jumpImg;
    public Image attackImg;
    public Image potionImg;
    public Image shieldImg;
    public MovableControls joystickScript;
    public MovableControls jumpScript;
    public MovableControls attackScript;
    public MovableControls potionScript;
    public MovableControls shieldScript;
    bool inGame;
    bool winGame;
    bool loseGame;

    private void Awake()
    {
        coinText.GetComponent<Text>();
        potionText.GetComponent<TextMeshProUGUI>();
        player.GetComponent<HeroKnight>();
    }

    // Start is called before the first frame update
    void Start()
    {
        coinCount = 0;
        potionCount = 0;
    }

    //Loads various scenes in the game
    public void LevelOne()
    {
        //Ensures all relevant screens (Canvases) are active/inactive when loading the first level
        SceneManager.LoadScene("Level 1");
        player.restartGame();
        player.transform.position = new Vector3(-7.15f, -4.02f, 0);
        menuScreen.SetActive(false);
        settingsScreen.SetActive(false);
        pauseScreen.SetActive(false);
        winScreen.SetActive(false);
        loseScreen.SetActive(false);

        //Sets up all of the HUD elements
        HUD.SetActive(true);
        coinCount = 0;
        coinText.text = coinCount.ToString();
        potionCount = 0;
        potionText.SetText("= " + potionCount.ToString());
        player.healthComponent.curHealth = 100f;
        player.healthComponent.fillAmount.fillAmount = player.healthComponent.curHealth;
    }

    public void LevelTwo()
    {
        SceneManager.LoadScene("Level 2");
        player.transform.position = new Vector3(-7.15f, -4.02f, 0);
    }

    public void LevelThree()
    {
        SceneManager.LoadScene("Level 3");
        player.transform.position = new Vector3(-7.15f, -4.02f, 0);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("Main Menu");
        Time.timeScale = 1;
        menuScreen.SetActive(true);
        HUD.SetActive(true);
        controls.SetActive(true);
        settingsScreen.SetActive(false);
        pauseScreen.SetActive(false);
        winScreen.SetActive(false);
        loseScreen.SetActive(false);

        //Turns off the gyroscope when returning to the Main Menu
        if (gyroActive)
        {
            settingsGyro.isOn = false;
            pauseGyro.isOn = false;
            Debug.Log("Settings & Pause Gyro reset");
            gyroActive = false;
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void WinScreen()
    {
        SceneManager.LoadScene("Win Screen");
        winScreen.SetActive(true);
        HUD.SetActive(false);
        controls.SetActive(false);
        player.transform.position = new Vector3(-7.15f, -4.02f, 0);
        coinResult.text = coinText.text; //Displays how many coins the player finished with
    }

    public void LoseScreen()
    {
        SceneManager.LoadScene("Lose Screen");
        loseScreen.SetActive(true);
        HUD.SetActive(false);
        controls.SetActive(false);
        player.transform.position = new Vector3(-7.15f, -4.02f, 0);
    }

    //Overlays

    public void SettingsScreen()
    {
        //The checks ensure the correct menus/overlays/Canvases are active/inactive depending on what screen Settings is chosen from
        if (winScreen.activeSelf)
        {
            settingsScreen.SetActive(true);
            winScreen.SetActive(false);
            winGame = true;
        }
        else if (loseScreen.activeSelf)
        {
            settingsScreen.SetActive(true);
            loseScreen.SetActive(false);
            loseGame = true;
        }
        else if (menuScreen.activeSelf)
        {
            settingsScreen.SetActive(true);
            menuScreen.SetActive(false);
        }
    }

    public void BackupSettings()
    {
        if (winGame)
        {
            settingsScreen.SetActive(false);
            winScreen.SetActive(true);
            winGame = false;
        }
        else if (loseGame)
        {
            settingsScreen.SetActive(false);
            loseScreen.SetActive(true);
            loseGame = false;
        }
        else if (!winGame && !loseGame)
        {
            settingsScreen.SetActive(false);
            menuScreen.SetActive(true);
        }
    }

    public void PauseScreen()
    {
        pauseScreen.SetActive(true);
        HUD.SetActive(false);
        controls.SetActive(false);
        Time.timeScale = 0; //Pauses most things like physics
    }

    public void BackupPause()
    {
        pauseScreen.SetActive(false);
        HUD.SetActive(true);
        controls.SetActive(true);
        Time.timeScale = 1;
    }

    // Item updates
    public void CoinUpdate()
    {
        coinCount += 1;
        coinText.text = coinCount.ToString();
    }

    public void PotionPickup()
    {
        potionCount += 1;
        potionText.SetText("= " + potionCount.ToString());
    }

    public void PotionUsed()
    {
        potionCount -= 1;
        potionText.SetText("= " + potionCount.ToString());
    }

    public void settingsGyroActivate()
    {
        //Need to make sure that the Gyro Control toggle is applied to both the Settings and Pause overlays
        if (gyroActive && !pauseGyroFlip)
        {
            gyroActive = false;
            settingsGyroFlip = true;
            pauseGyro.isOn = false;
            Debug.Log("Settings Gyro NOT ACTIVE");
            player.gyroControls();
            settingsGyroFlip = false;
        }
        else if (!gyroActive && !pauseGyroFlip)
        {
            gyroActive = true;
            settingsGyroFlip = true;
            pauseGyro.isOn = true;
            Debug.Log("Settings Gyro ACTIVE");
            player.gyroControls();
            settingsGyroFlip = false;
        }
        else
        {
            settingsGyroFlip = false;
        }
    }

    public void pauseGyroActivate()
    {
        if (gyroActive && !settingsGyroFlip)
        {
            gyroActive = false;
            pauseGyroFlip = true;
            settingsGyro.isOn = false;
            Debug.Log("Pause Gyro NOT ACTIVE");
            player.gyroControls();
            pauseGyroFlip = false;
        }
        else if (!gyroActive && !settingsGyroFlip)
        {
            gyroActive = true;
            pauseGyroFlip = true;
            settingsGyro.isOn = true;
            Debug.Log("Pause Gyro ACTIVE");
            player.gyroControls();
            pauseGyroFlip = false;
        }
        else
        {
            pauseGyroFlip = false;
        }
    }

    public void EditControls()
    {
        if (settingsScreen.activeSelf)
        {
            settingsScreen.SetActive(false);
            inGame = false;
            HUD.SetActive(false);
        }
        else if (pauseScreen.activeSelf)
        {
            pauseScreen.SetActive(false);
            inGame = true;
        }

        //Enables the Edit Controls visuals
        controls.SetActive(true);
        editControls.SetActive(true);
        joystickImg.enabled = true;
        jumpImg.enabled = true;
        attackImg.enabled = true;
        potionImg.enabled = true;
        shieldImg.enabled = true;
        joystickScript.enabled = true;
        jumpScript.enabled = true;
        attackScript.enabled = true;
        potionScript.enabled = true;
        shieldScript.enabled = true;
    }

    public void BackupEditControls()
    {
        if (!inGame)
        {
            settingsScreen.SetActive(true);
        }
        else if (inGame)
        {
            pauseScreen.SetActive(true);
            controls.SetActive(false);
        }

        editControls.SetActive(false);
        joystickImg.enabled = false;
        jumpImg.enabled = false;
        attackImg.enabled = false;
        potionImg.enabled = false;
        shieldImg.enabled = false;
        joystickScript.enabled = false;
        jumpScript.enabled = false;
        attackScript.enabled = false;
        potionScript.enabled = false;
        shieldScript.enabled = false;
    }
}