using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartPoolManager : MonoBehaviour
{
    public static PartPoolManager Instance;

    [Space(10)]
    [Header("Variables")]
    [SerializeField] private int poolSize;

    [Space(10)]
    [Header("References")]
    [SerializeField] private List<Part> parts = new List<Part>();

    private Dictionary<string, PartPool> partPools = new Dictionary<string, PartPool>();

    private void Start()
    {
        Instance = this;
        CreatePartPools();
    }

    private void CreatePartPools()
    {
        foreach (Part partPrefab in parts)
        {
            PartPool partPool = new PartPool(partPrefab, poolSize);
            partPools.Add(partPrefab.partName, partPool);
        }
    }

    public List<Part> GetEachPartType()
    {
        return parts;
    }

    public Part GetPart(string partName, Vector2 position)
    {
        if (partPools.ContainsKey(partName))
        {
            return partPools[partName].GetInstance(position);
        }
        else
        {
            Debug.LogError($"No pool found for part type: {partName}");
            return null;
        }
    }

    public void ReturnPart(Part part)
    {
        if (partPools.ContainsKey(part.partName))
        {
            partPools[part.partName].ReturnInstance(part);
        }
        else
        {
            Debug.LogError($"No pool found for part type: {part.partName}");
        }
    }

}

public class PartPool
{
    private readonly List<Part> parts;
    private readonly Part partPrefab;
    private Transform container;

    public PartPool(Part partPrefab, int initialSize)
    {
        this.partPrefab = partPrefab;
        parts = new List<Part>(initialSize);
        CreateContainer();

        for (int i = 0; i < initialSize; i++)
        {
            CreateInstance(partPrefab.partName);
        }
    }

    private void CreateContainer()
    {
        container = new GameObject($"{partPrefab.partName} Pool").transform;
        container.SetParent(PartPoolManager.Instance.transform);
    }

    public Part GetInstance(Vector2 position)   
    {
        Part part;

        if (parts.Count > 0)
        {
            part = parts[parts.Count - 1];
            parts.RemoveAt(parts.Count - 1);
        }
        else
        {
            part = CreateInstance(partPrefab.partName);
        }

        part.transform.parent = null;
        part.transform.position = position;
        part.gameObject.SetActive(true);

        return part;
    }

    public void ReturnInstance(Part part)
    {
        part.gameObject.SetActive(false);
        part.Reset();
        part.transform.SetParent(container);

        parts.Add(part);
    }

    private Part CreateInstance(string containerName)
    {
        Part part = Object.Instantiate(partPrefab);
        part.Initialize();
        part.gameObject.SetActive(false);
        part.transform.SetParent(container);
        return part;
    }
}
