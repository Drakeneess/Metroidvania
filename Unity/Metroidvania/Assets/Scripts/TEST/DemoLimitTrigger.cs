using UnityEngine;

public class DemoLimitTrigger : MonoBehaviour
{
    [Header("Canvas que se activará al entrar")]
    public GameObject demoLimitCanvas;

    [Header("Opciones")]
    public bool hideOnExit = false;   // Si quieres que desaparezca al salir
    public bool triggerOnce = false;  // Si solo quieres que ocurra una vez

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggerOnce && hasTriggered) return;

        if (other.CompareTag("Player"))
        {
            demoLimitCanvas.SetActive(true);
            hasTriggered = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!hideOnExit) return;

        if (other.CompareTag("Player"))
        {
            demoLimitCanvas.SetActive(false);
        }
    }
}
