using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private bool bCreateWanderers = true;

    [SerializeField] private Wanderer wandererPrefab;

    [SerializeField] private List<Sprite> sprites = new List<Sprite>();

    public List<Wanderer> wanderers = new List<Wanderer>();
    public Wanderer match1;
    public Wanderer match2;

    public Image match1Sprite;
    public Image match2Sprite;

    private void Start() {
        if (Instance == null) Instance = this;

        if (bCreateWanderers)
        {
            CreateWanderers();
            CreateMatch();
        }
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

    private void CreateMatch()
    {
        int randomIndex = Random.Range(0, wanderers.Count);
        match1Sprite.sprite = wanderers[randomIndex].GetComponentInChildren<SpriteRenderer>().sprite;
        match1 = wanderers[randomIndex];

        // Get a random index
        randomIndex = Random.Range(0, wanderers.Count);
        match2Sprite.sprite = wanderers[randomIndex].GetComponentInChildren<SpriteRenderer>().sprite;
        match2 = wanderers[randomIndex];
    }
}
