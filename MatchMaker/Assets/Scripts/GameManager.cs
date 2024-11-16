using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private bool bCreateWanderers = true;

    [SerializeField] private Wanderer wandererPrefab;

    [SerializeField] private List<Sprite> sprites = new List<Sprite>();

    public List<Wanderer> wanderers = new List<Wanderer>();

    private void Start() {
        if (Instance == null) Instance = this;

        if (bCreateWanderers) CreateWanderers();
    }

    private void CreateWanderers() {
        foreach (Sprite sprite in sprites) {
            float x = Random.Range(-wandererPrefab.areaSize.x / 2 + wandererPrefab.areaOffset.x, wandererPrefab.areaSize.x / 2 + wandererPrefab.areaOffset.x);
            float y = Random.Range(-wandererPrefab.areaSize.y / 2 + wandererPrefab.areaOffset.y, wandererPrefab.areaSize.y / 2 + wandererPrefab.areaOffset.y);
           
            Wanderer newWanderer = Instantiate(wandererPrefab, new Vector2(x, y), Quaternion.identity);

            SpriteRenderer renderer = newWanderer.GetComponentInChildren<SpriteRenderer>();
            if (renderer != null) {
                renderer.sprite = sprite;
            }

            wanderers.Add(newWanderer);
        }
    }
}
