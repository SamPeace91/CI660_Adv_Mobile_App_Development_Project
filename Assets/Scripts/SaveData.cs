using UnityEngine;
using Firebase;
using Firebase.Database;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class SaveData : MonoBehaviour
{
    DatabaseReference reference;
    public Text coinText;
    public TextMeshProUGUI potionText;
    Scene currentScene;
    public SceneManagement sceneManager;
    public GameObject menuScreen;
    public GameObject HUD;
    public GameObject saveToast;
    float toastTimer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        // Get the root reference location of the database.
        reference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    // Update is called once per frame
    void Update()
    {
        //Sets time limit for the toast popup
        if (saveToast.activeSelf)
        {
            toastTimer += 0.016f;
            if(toastTimer >= 1f)
            {
                saveToast.SetActive(false);
                toastTimer = 0f;
            }
        }
    }

    public void saveData()
    {
        //Saves the data to the database as a child of "User 1" which is a child of "Users"
        reference.Child("Users").Child("User 1").Child("Coin Count").SetValueAsync(sceneManager.coinCount);
        reference.Child("Users").Child("User 1").Child("Coin Text").SetValueAsync(coinText.text.ToString());
        reference.Child("Users").Child("User 1").Child("Potion Count").SetValueAsync(sceneManager.potionCount);
        reference.Child("Users").Child("User 1").Child("Potion Text").SetValueAsync(potionText.text.ToString());
        reference.Child("Users").Child("User 1").Child("Current Health").SetValueAsync(sceneManager.player.healthComponent.curHealth);
        reference.Child("Users").Child("User 1").Child("Fill Amount").SetValueAsync(sceneManager.player.healthComponent.fillAmount.fillAmount);
        
        //Sets the 'Scene' variable to the active scene, so we can get its name below
        currentScene = SceneManager.GetActiveScene();
        reference.Child("Users").Child("User 1").Child("Scene").SetValueAsync(currentScene.name.ToString());
        saveToast.SetActive(true);
    }

    public void loadData()
    {
        //Loads the data from the "Users" branch (including its children) and any changes
        FirebaseDatabase.DefaultInstance.GetReference("Users").ValueChanged += SaveData_ValueChanged;
    }

    private void SaveData_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        //Converts Strings back into numbers using parsing
        sceneManager.coinCount = int.Parse(e.Snapshot.Child("User 1").Child("Coin Count").GetValue(true).ToString());
        coinText.text = e.Snapshot.Child("User 1").Child("Coin Text").GetValue(true).ToString();
        sceneManager.potionCount = int.Parse(e.Snapshot.Child("User 1").Child("Potion Count").GetValue(true).ToString());
        potionText.text = e.Snapshot.Child("User 1").Child("Potion Text").GetValue(true).ToString();
        sceneManager.player.healthComponent.curHealth = float.Parse(e.Snapshot.Child("User 1").Child("Current Health").GetValue(true).ToString());
        sceneManager.player.healthComponent.fillAmount.fillAmount = float.Parse(e.Snapshot.Child("User 1").Child("Fill Amount").GetValue(true).ToString());
        SceneManager.LoadScene(e.Snapshot.Child("User 1").Child("Scene").GetValue(true).ToString());
        
        //Other functions are performed during loading too
        menuScreen.SetActive(false);
        HUD.SetActive(true);
        sceneManager.player.transform.position = new Vector3(-7.15f, -4.02f, 0);
        sceneManager.player.restartGame();
    }
}
