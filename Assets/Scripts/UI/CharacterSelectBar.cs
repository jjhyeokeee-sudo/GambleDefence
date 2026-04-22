using System.Collections.Generic;
using UnityEngine;

// 하단에 캐릭터 소환 버튼들을 나열하는 바
public class CharacterSelectBar : MonoBehaviour
{
    [SerializeField] private List<CharacterData> availableCharacters;
    [SerializeField] private GameObject characterButtonPrefab;
    [SerializeField] private Transform buttonRoot;

    void Start()
    {
        foreach (var data in availableCharacters)
        {
            if (data == null) continue;
            var go = Instantiate(characterButtonPrefab, buttonRoot);
            go.GetComponent<CharacterButtonUI>()?.Setup(data);
        }
    }
}
