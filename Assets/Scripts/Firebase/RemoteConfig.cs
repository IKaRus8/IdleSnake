using System;
using System.Collections.Generic;
using Firebase;
using UnityEngine;

public static class RemoteConfig
{
    public static event Action OnConfigUpdate;
    public static bool GetBool(string configName)
    {
        try
        {
            Debug.Log(configName + " = " + Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(configName).BooleanValue);
            return Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(configName).BooleanValue;
        }
        catch (Exception e)
        {
            Debug.Log("Remote config error. " + e.Message);
            return bool.Parse(DefaultValues.defaultValues[configName].ToString());
        }
    }
    public static long GetLong(string configName)
    {
        try
        {
            return Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(configName).LongValue;
        }
        catch (Exception e)
        {
            Debug.Log("Remote config error. " + e.Message + ".." + e.StackTrace);
            return long.Parse(DefaultValues.defaultValues[configName].ToString());
        }
    }
    public static double GetDouble(string configName)
    {
        try
        {
            return Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(configName).DoubleValue;
        }
        catch (Exception e)
        {
            Debug.Log("Remote config error. " + configName + " " + e.Message + ".." + e.StackTrace);
            return double.Parse(DefaultValues.defaultValues[configName].ToString());
        }
    }
    public static string GetString(string configName)
    {
        try
        {
            return Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(configName).StringValue;
        }
        catch (Exception e)
        {
            Debug.Log("Remote config error. " + e.Message + ".." + e.StackTrace);
            return DefaultValues.defaultValues[configName].ToString();
        }
    }
    public static IEnumerable<byte> GetByteArray(string configName)
    {
        try
        {
            return Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(configName).ByteArrayValue;
        }
        catch (Exception e)
        {
            Debug.Log("Remote config error. " + e.Message);
            var chars = DefaultValues.defaultValues[configName].ToString().ToCharArray();
            byte[] vs = new byte[chars.Length];
            chars.CopyTo(vs, 0);
            return vs;
        }
    }

    public static void UpdateConfig()
    {
        OnConfigUpdate?.Invoke();
    }
}
