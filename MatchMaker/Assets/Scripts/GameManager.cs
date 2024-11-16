using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private Wanderer wandererPrefab;

    [SerializeField] private List<Sprite> sprites = new List<Sprite>();

    [SerializeField] private List<Wanderer> wanderers = new List<Wanderer>();

    public Vector2 areaSize = new Vector2(10f, 10f);

    private void Start() {
        if (Instance == null) Instance = this;

        CreateWanderers();
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
    private void OnDrawGizmosSelected() {
        // area
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(areaSize.x, areaSize.y, 0));
    }
}
