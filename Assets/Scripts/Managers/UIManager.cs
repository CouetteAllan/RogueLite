using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class UIManager : Singleton<UIManager>
{
    [Header("EventSystem Related")]
    [SerializeField] private EventSystem eventSystem;
    [Tooltip("Le premier game object selectionné dans l'UI à l'ouverture du panel Pause")]
    [SerializeField] private GameObject firstGameObjectPauseMenu;
    private Button pauseMenuButton;

    [Tooltip("Le premier game object selectionné dans l'UI à l'ouverture du panel Select")]
    [SerializeField] private GameObject firstGameObjectSelectMenu;
    private Button selectMenuButton;

    [Space]
    [SerializeField] private Slider h_Slider;
    [SerializeField] private Gradient h_Gradient;
    [SerializeField] private Image h_Fill;
    [SerializeField] private TextMeshProUGUI h_Txt;

    [Space]
    [Header("Menus Canvas/Panel")]
    [SerializeField] private Canvas menuCanvas;
    [SerializeField] private GameObject selectParentGameObject;
    [SerializeField] private GameObject pauseParentGameObject;



    private float playerHealth;
    private float playerMaxHealth;
    private Coroutine fillBarCoroutine;
    private Coroutine txtCoroutine;
    private delegate void SetActivePanel(bool active);
    private SetActivePanel activeMenu;


    private MainCharacterScript3D player = null;

    protected override void Awake()
    {
        base.Awake();
        pauseMenuButton = firstGameObjectPauseMenu.GetComponent<Button>();
        selectMenuButton = firstGameObjectSelectMenu.GetComponent<Button>();
    }

    public void SetMaxHealth(float health, MainCharacterScript3D player = null)
    {
        h_Slider.maxValue = health;
        playerMaxHealth = health;


        if (player != null)
        {
            playerHealth = health;
            h_Fill.color = h_Gradient.Evaluate(1);
            this.player = player;
            MainCharacterScript3D.playerChangeHealth += SetUIHealth;
        }

    }

    public void SetUIHealth(float health)
    {

        //h_Slider.value = health;
        if (fillBarCoroutine != null)
            StopCoroutine(fillBarCoroutine);
        fillBarCoroutine = StartCoroutine(LerpHealthBar(health,h_Slider.value,0.5f));

        if (txtCoroutine != null)
            StopCoroutine(txtCoroutine);
        fillBarCoroutine = StartCoroutine(LerpHealthTxt(health,playerHealth,1f));


        h_Fill.color = h_Gradient.Evaluate(h_Slider.normalizedValue);

    }


    IEnumerator LerpHealthBar(float targetFill, float start,float timeToMove)
    {
        float t = 0;
        while (t < 1)
        {
            h_Slider.value = Mathf.Lerp(start, targetFill, t);
            t = t + Time.deltaTime / timeToMove;
            yield return null;
        }
        h_Slider.value = targetFill;
    }
    
    IEnumerator LerpHealthTxt(float target, float start, float timeToMove)
    {
        float t = 0;
        while (t < 1)
        {
            playerHealth = Mathf.Clamp(Mathf.Lerp(start, target, t), 0, playerMaxHealth);
            h_Txt.text = playerHealth.ToString("#") + "/" + playerMaxHealth.ToString("#");
            t = t + Time.deltaTime / timeToMove;
            yield return null;
        }
        playerHealth = target;
    }

    public void Resume()
    {
        GameManager.Instance.ActualGameState = GameState.InGame;
    }

    ///<summary> Active le canvas de pause et séléctionne la panel à afficher selon le game state
    ///</summary>
    public void SetActiveMenu(bool active, GameState state)
    {
        menuCanvas.gameObject.SetActive(active);
        SetMenu(state);
        activeMenu(active);
    }

    //Séléctionne le menu Pause ou bien le menu Select selon l'état du jeu
    private void SetMenu(GameState state)
    {
        if (state == GameState.Pause)
            activeMenu = PauseMenuActive;
        else if(state == GameState.InSelect)
            activeMenu = SelectMenuActive;
    }

    private void PauseMenuActive(bool active) {
        pauseParentGameObject.SetActive(active);
        selectParentGameObject.SetActive(false);
        eventSystem.firstSelectedGameObject = firstGameObjectPauseMenu;
        pauseMenuButton.Select();
    }

    private void SelectMenuActive(bool active)
    {
        selectParentGameObject.SetActive(active);
        pauseParentGameObject.SetActive(false);
        eventSystem.firstSelectedGameObject = firstGameObjectSelectMenu;
        selectMenuButton.Select();
    }


}
