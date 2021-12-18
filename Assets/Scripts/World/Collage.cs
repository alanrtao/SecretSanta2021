using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Collage : MonoBehaviour
{
    public static Collage Instance;
    List<Texture2D> pictures = new List<Texture2D>();

    private void Awake()
    {

        Instance = this;
        DontDestroyOnLoad(gameObject);

    }

    // Start is called before the first frame update
    void Start()
    {
        SceneManager.activeSceneChanged += (scene1, scene2) =>
        {
            if (scene2.buildIndex == 1)
            {
                print("displaying collage...");
                StartCoroutine(DisplayCollage());
            }
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddPicture(Texture2D pic) { pictures.Add(pic); }
    [SerializeField] private GameObject picture_prototype;
    IEnumerator DisplayCollage()
    {
        GameObject go = GameObject.Find("Canvas");
        foreach(Texture2D p in pictures)
        {
            GameObject obj = GameObject.Instantiate(picture_prototype);
            RectTransform rect = obj.transform as RectTransform;
            rect.anchoredPosition = new Vector3(Random.value * 400 + 200, Random.value * 400 + 100, 0);
            rect.localRotation = Quaternion.Euler(0, 0, Random.value * 30 - 15);

            UnityEngine.UI.Image img = obj.GetComponent<UnityEngine.UI.Image>();
            Color tint = Color.HSVToRGB(Random.value, 0.1f, 0.9f);
            img.color = tint;
            img.material.SetFloat("_t", 0);

            yield return new WaitForSeconds(0.5f);
        }
        DisplayUI();
    }

    void DisplayUI()
    {
        GameObject.Find("Panel").SetActive(true);
    }
}
