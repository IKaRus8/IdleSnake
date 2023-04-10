using System.Collections;
using UnityEngine;
using Google.Play.AppUpdate;
#if !UNITY_EDITOR
using Google.Play.Common;
#endif

public class CheckVersion : MonoBehaviour
{
    private AppUpdateManager _appUpdateManager;
    private AppUpdateInfo _appUpdateInfoResult;
    private AppUpdateOptions _appUpdateOptions;

    private void Awake()
    {
       StartCoroutine(CheckForUpdate(this));
    }

    private IEnumerator CheckForUpdate(MonoBehaviour mono)
    {
        Debug.Log("BeginChecking");
#if !UNITY_EDITOR
        _appUpdateManager = new AppUpdateManager();
        PlayAsyncOperation<AppUpdateInfo, AppUpdateErrorCode> appUpdateInfoOperation =
          _appUpdateManager.GetAppUpdateInfo();

        // Wait until the asynchronous operation completes.
        yield return appUpdateInfoOperation;
        Debug.Log("appUpdateInfoOperation IsSuccessful: "+appUpdateInfoOperation.IsSuccessful);
        Debug.Log("appUpdateInfoOperation IsDone: "+appUpdateInfoOperation.IsDone);
        if (appUpdateInfoOperation.IsSuccessful)
        {
            _appUpdateInfoResult = appUpdateInfoOperation.GetResult();

            _appUpdateOptions = AppUpdateOptions.ImmediateAppUpdateOptions();

            Debug.Log("appUpdateInfoResult AvailableVersionCode: "+ _appUpdateInfoResult.AvailableVersionCode);
            Debug.Log("appUpdateInfoResult UpdatePriority: "+_appUpdateInfoResult.UpdatePriority);
            Debug.Log("appUpdateInfoResult AppUpdateStatus: "+_appUpdateInfoResult.AppUpdateStatus);
            Debug.Log("appUpdateInfoResult UpdateAvailability: "+_appUpdateInfoResult.UpdateAvailability);
            Debug.Log("appUpdateInfoResult IsUpdateTypeAllowed: "+_appUpdateInfoResult.IsUpdateTypeAllowed(_appUpdateOptions));
            Debug.Log("appUpdateOptions AppUpdateType: "+_appUpdateOptions.AppUpdateType);
            Debug.Log("appUpdateOptions AllowAssetPackDeletion: "+_appUpdateOptions.AllowAssetPackDeletion);
            if(_appUpdateInfoResult.UpdateAvailability == UpdateAvailability.UpdateAvailable)
                mono.StartCoroutine(StartImmediateUpdate());
            // Check AppUpdateInfo's UpdateAvailability, UpdatePriority,
            // IsUpdateTypeAllowed(), etc. and decide whether to ask the user
            // to start an in-app update.
        }
        else
        {
            // Log appUpdateInfoOperation.Error.
        }
#else
        yield return null;
#endif
    }
#if !UNITY_EDITOR
    private IEnumerator StartImmediateUpdate()
    {
        Debug.Log("Start Update");
        // Creates an AppUpdateRequest that can be used to monitor the
        // requested in-app update flow.
        var startUpdateRequest = _appUpdateManager.StartUpdate(
          // The result returned by PlayAsyncOperation.GetResult().
          _appUpdateInfoResult,
          // The AppUpdateOptions created defining the requested in-app update
          // and its parameters.
          _appUpdateOptions);
        yield return startUpdateRequest;

        // If the update completes successfully, then the app restarts and this line
        // is never reached. If this line is reached, then handle the failure (for
        // example, by logging result.Error or by displaying a message to the user).
    }
#endif
}