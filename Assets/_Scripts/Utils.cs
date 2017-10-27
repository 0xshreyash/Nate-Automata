using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnityEngine;


public abstract class GameState
{
    protected GameManager gameManager;
    public bool IsInitialized;

    public GameState(GameManager gameMng)
    {
        gameManager = gameMng;
        IsInitialized = false;
    }

    public virtual void StateInitialize()
    {
        this.IsInitialized = true;
    }
    public abstract void StateEntry();
    public abstract void StateUpdate();
    public abstract void StateExit();
}


public static class TransformDeepChildExtension
{
    public static Transform FindDeepChild(this Transform aParent, string aName)
    {
        // Find and return if nothing is found
        var result = aParent.Find(aName);
        if (result != null) return result;
        
        // Iterate through the child
        foreach (Transform child in aParent)
        {
            result = child.FindDeepChild(aName);
            if (result != null) return result;
        }
        return null;
    }

    public static T FindChildComponent<T>(this Transform aParent)
    {
        // Find and return if nothing is found
        var result = aParent.GetComponent<T>();
        if (result != null) return result;

        // Iterate through the child
        foreach (Transform child in aParent)
        {
            result = child.FindChildComponent<T>();
            if (result != null) return result;
        }
        return default(T);
    }

    public static List<T> FindChildComponents<T>(this Transform aParent)
    {
        var toReturn = new List<T>();
        var result = aParent.GetComponent<T>();
        
        // Add child into returning lists
        if (result != null) toReturn.Add(result);

        // Iterate the child
        foreach (Transform child in aParent)
        {
            var res = child.FindChildComponents<T>();
            if (res.Count > 0) toReturn.Add(res);
        }
        return toReturn;
    }
}

public static class ListExtensions
{
    public static void Add<T>(this List<T> list, params T[] item)
    {
        list.AddRange(item);
    }

    public static void Add<T>(this List<T> list, IEnumerable<T> item)
    {
        list.AddRange(item);
    }
}

public static class TransformExtensions
{
    public static void SetX(this Transform transform, float x)
    {
        transform.position = new Vector3(x, transform.position.y, transform.position.z);
    }

    public static void SetY(this Transform transform, float y)
    {
        transform.position = new Vector3(transform.position.x, y, transform.position.z);
    }

    public static void SetZ(this Transform transform, float z)
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, z);
    }

    public static void SetPosition(this Transform transform, float x, float y, float z)
    {
        transform.position = new Vector3(x, y, z);
    }
}

internal static class Utils
{
    public static GameObject InstantiateSafe(GameObject gO, Vector3 position)
    {
        if(gO == null)
        {
            StackFrame frame = new StackFrame(1);
            var method = frame.GetMethod();
            var type = method.DeclaringType;
            string name = method.Name;

            UnityEngine.Debug.LogError(string.Format("Can't load prefab at {0}.{1}", type, name));
            UnityEngine.Debug.Break();
        }

        GameObject result = (GameObject)GameObject.Instantiate(gO, position, Quaternion.identity);
        return result;
    }

    public static GameObject LoadGameObjectSafe(string filenameFromResources)
    {
        GameObject gameObjectToLoad = Resources.Load<GameObject>(filenameFromResources.Trim());
        if (gameObjectToLoad != null) return gameObjectToLoad;
        
        // if unable to load, report the bug
        UnityEngine.Debug.LogError("Can't load GameObject " + filenameFromResources);
        UnityEngine.Debug.Break();
        return null;
    }
}

