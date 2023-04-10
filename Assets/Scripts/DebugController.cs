using UnityEngine;

public class DebugController : MonoBehaviour
{
    public static bool isDebug;
    // Start is called before the first frame update
    private void Awake()
    {
        if (Application.version.StartsWith('d'))
        {
            isDebug = true;
        }
        else
        {
            var debug = GameObject.FindGameObjectsWithTag("Debug");
            foreach(var objectDebug in debug){
                objectDebug.SetActive(false);
            }
        }
    }
}
