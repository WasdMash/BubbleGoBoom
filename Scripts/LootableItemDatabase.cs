using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootableItemDatabase : MonoBehaviour
{
    public static LootableItemDatabase Instance { get; private set; }
    [SerializeField] private LootableItem[] allItems; // drag every LootableItem asset in here

    private Dictionary<string, LootableItem> lookup;

    void Awake()
    {
        Instance = this;
        lookup = new Dictionary<string, LootableItem>();
        foreach (var item in allItems) lookup[item.getId()] = item;
    }

    public LootableItem GetById(string id) => lookup.TryGetValue(id, out var item) ? item : null;
}
