using Firebase;
using Firebase.Analytics;
using UnityEngine;

public class FirebaseInit : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //Initialise Firebase when the app starts
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
        });
    }
}
