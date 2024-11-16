using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private bool bCreateWanderers = true;

    [SerializeField] private Wanderer wandererPrefab;

    [SerializeField] private List<Sprite> sprites = new List<Sprite>();

    [SerializeField] private List<Wanderer> wanderers = new List<Wanderer>();

    private void Start() {
        if (Instance == null) Instance = this;

        if (bCreateWanderers) CreateWanderers();
    }

    private void CreateWanderers() {
        foreach (Sprite sprite in sprites) {
            Wanderer newWanderer = Instantiate(wandererPrefab, Vector3.zero, Quaternion.identity);

            SpriteRenderer renderer = newWanderer.GetComponentInChildren<SpriteRenderer>();
            if (renderer != null) {
                renderer.sprite = sprite;
            }

            wanderers.Add(newWanderer);
        }
    }
}
