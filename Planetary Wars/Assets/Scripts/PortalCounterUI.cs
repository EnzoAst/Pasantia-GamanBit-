using UnityEngine;
using TMPro;
using System.Collections;

public class PortalCounterUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI portalCounterText;

    void Start()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.OnPortalsChanged += UpdateCounter;
            StartCoroutine(DelayedInitialUpdate());
        }
    }

    IEnumerator DelayedInitialUpdate()
    {
        yield return null; // Espera un frame para que los portales se registren
        if (GameManager.instance != null)
            UpdateCounter(
                GameManager.instance.TotalPortals - GameManager.instance.PortalsDestroyed,
                GameManager.instance.TotalPortals
            );
    }

    void OnDestroy()
    {
        if (GameManager.instance != null)
            GameManager.instance.OnPortalsChanged -= UpdateCounter;
    }

    public void UpdateCounter(int remaining, int total)
    {
        portalCounterText.text = $"PORTALS {remaining}/{total}";
    }
}