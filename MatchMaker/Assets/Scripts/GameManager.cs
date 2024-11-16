using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


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

    private Wanderer test;

    public TextMeshProUGUI timerText;
    public float timer;

    private void Start() {
        if (Instance == null) Instance = this;

        if (bCreateWanderers)
        {
            CreateWanderers();
            CreateMatch();
        }
    }

    private void CreateWanderers() {
        int i = 0;
        foreach (Sprite sprite in sprites) {
            float x = Random.Range(-wandererPrefab.areaSize.x / 2 + wandererPrefab.areaOffset.x, wandererPrefab.areaSize.x / 2 + wandererPrefab.areaOffset.x);
            float y = Random.Range(-wandererPrefab.areaSize.y / 2 + wandererPrefab.areaOffset.y, wandererPrefab.areaSize.y / 2 + wandererPrefab.areaOffset.y);
           
            Wanderer newWanderer = Instantiate(wandererPrefab, new Vector2(x, y), Quaternion.identity);

            SpriteRenderer renderer = newWanderer.GetComponentInChildren<SpriteRenderer>();
            if (renderer != null) {
                renderer.sprite = sprite;
            }

            wanderers.Add(newWanderer);
            
            // TEMP
            if (i == 0) {
                test = newWanderer;
                newWanderer.bWander = false;
                i++;
                continue;
            }
            newWanderer.wandererToAvoid = test;
            i++;
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

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            HandleClick();
        }
    }

    private void HandleClick() {
        // raycast from mouse to wanderer
    }

    // TODO: Finish this
    public bool IsIndexSelected(int index) {
        if (index == 0) {
            return true;
        }

        return false;
    }


    private void SelectWanderer() {
        //wanderer.bWander = false;
    }

    private void DeselectWanderer() {
        //wanderer.bWander = true;
    }
}
