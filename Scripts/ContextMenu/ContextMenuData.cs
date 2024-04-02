using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContextMenuData
{
    private Dictionary<string, object> parameters = new Dictionary<string, object>();

    public void AddParameter(string paramName, object paramValue)
    {
        parameters[paramName] = paramValue;
    }

    public T GetParameter<T>(string paramName)
    {
        if (parameters.ContainsKey(paramName))
        {
            return (T)parameters[paramName];
        }
        else
        {
            Debug.LogError($"Parameter with name {paramName} not found.");
            return default(T);
        }
    }

    public ContextMenuData Clone()
    {
        ContextMenuData clone = new ContextMenuData();
        foreach (var entry in parameters)
        {
            clone.AddParameter(entry.Key, entry.Value);
        }
        return clone;
    }
}