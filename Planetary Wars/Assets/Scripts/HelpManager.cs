using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HelpManager : MonoBehaviour
{
    public static HelpManager instance;

    [Header("Help Panel Settings")]
    [SerializeField] private GameObject helpPanel; // Panel de ayuda
    [SerializeField] private Button helpButton;    // Botón para activar el panel de ayuda
    [SerializeField] private Button closeButton;   // Botón para cerrar el panel de ayuda
    [SerializeField] private GameObject pauseButton; // Botón de pausa

    private bool isHelpActive = false; // Indica si el panel de ayuda está activo

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Hace que el HelpManager persista entre escenas
        }
        else
        {
            Destroy(gameObject); // Evita duplicados
        }
    }

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded; // Escuchar el evento de carga de escenas
        InitializeReferences(); // Inicializa las referencias
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // Dejar de escuchar el evento al destruirse
    }

    private void Update()
    {
        // Si el panel de ayuda está activo, cierra el panel con la tecla ESC
        if (isHelpActive && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseHelpPanel();
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitializeReferences(); // Reasignar referencias al cargar una nueva escena
    }

    private void InitializeReferences()
    {
        // Reasignar el panel de ayuda y botones tras cargar una escena (incluye objetos inactivos)
        if (helpPanel == null)
            helpPanel = FindInactiveObjectByName("HelpPanel");

        if (helpButton == null)
        {
            GameObject helpButtonObj = FindInactiveObjectByName("HelpButton");
            if (helpButtonObj != null)
                helpButton = helpButtonObj.GetComponent<Button>();
        }

        if (closeButton == null)
        {
            GameObject closeButtonObj = FindInactiveObjectByName("CloseButton");
            if (closeButtonObj != null)
                closeButton = closeButtonObj.GetComponent<Button>();
        }

        if (pauseButton == null)
            pauseButton = FindInactiveObjectByName("ButtonPause");

        // Configurar los botones si están asignados
        if (helpButton != null)
        {
            helpButton.onClick.RemoveAllListeners();
            helpButton.onClick.AddListener(ToggleHelpPanel);
        }

        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(CloseHelpPanel);
        }

        // Asegurarse de que el panel de ayuda esté inicialmente desactivado
        if (helpPanel != null)
            helpPanel.SetActive(false);
    }

    private GameObject FindInactiveObjectByName(string name)
    {
        Transform[] allTransforms = Resources.FindObjectsOfTypeAll<Transform>();
        foreach (Transform t in allTransforms)
        {
            if (t.hideFlags == HideFlags.None && t.name == name)
            {
                return t.gameObject;
            }
        }
        return null;
    }

    public void ToggleHelpPanel()
    {
        if (helpPanel == null) return;

        isHelpActive = !isHelpActive;
        helpPanel.SetActive(isHelpActive);

        // Bloquear el botón de pausa mientras el panel de ayuda está activo
        if (pauseButton != null)
            pauseButton.SetActive(!isHelpActive);

        // Bloquear el botón de ayuda mientras está activo
        if (helpButton != null)
            helpButton.interactable = !isHelpActive;

        Debug.Log($"PanelHelp {(isHelpActive ? "activado" : "desactivado")}.");
    }

    public void CloseHelpPanel()
    {
        if (helpPanel == null) return;

        isHelpActive = false;
        helpPanel.SetActive(false);

        // Reactivar el botón de pausa
        if (pauseButton != null)
            pauseButton.SetActive(true);

        // Reactivar el botón de ayuda
        if (helpButton != null)
            helpButton.interactable = true;

        Debug.Log("PanelHelp cerrado.");
    }
}