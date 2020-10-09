using UnityEngine;
using SimpleLocalizator;

public class TestItemText : MonoBehaviour
{
    [SerializeField] private ItemData item;
    [SerializeField] private MultiLangTextUIDef text;

    void Awake()
    {
        text.TranslateString = item.data.name;
    }
}
