using UnityEngine;

public class Shard : MonoBehaviour
{
    public int shardId;
    private ShardItemInteractor itemInteractor;
    private Transform shardControllerTransform;
    private Vector3 originalPosition;
    private Vector3 originalScale;
    private ShardTool shardTool;
    public ShardTool ShardTool { get { return shardTool; } set { shardTool = value; } }

    void Awake()
    {
        shardControllerTransform = transform.parent;

        originalPosition = transform.localPosition;
        originalScale = transform.localScale;
    }

    void Start()
    {
        // Validación: si la herramienta ya está desbloqueada, el fragmento no tiene que hacer nada.
        if (shardTool != null && shardTool.GetUnlocked())
        {
            return;
        }

        // Buscar el interactor correspondiente
        ShardItemInteractor[] allInteractors = FindObjectsOfType<ShardItemInteractor>();
        foreach (var interactor in allInteractors)
        {
            if (interactor.shardId == shardId)
            {
                itemInteractor = interactor;
                SetInInteractor();
                break;
            }
        }

        // Si no se encuentra interactor, desactivar el fragmento
        if (itemInteractor == null)
        {
            SetOut();
            return;
        }

        // Colocar el fragmento en su interactor
    }


    public void SetInMirror()
    {
        itemInteractor.Shard=null;
        transform.SetParent(shardControllerTransform);
        transform.localScale = originalScale;
        transform.localPosition = originalPosition;
    }

    public void SetInInteractor()
    {
        if(itemInteractor != null){
            itemInteractor.Shard=this;
            Transform interactorTransform = itemInteractor.transform;
            transform.SetParent(interactorTransform);
            transform.position = new Vector3(interactorTransform.position.x,interactorTransform.position.y+1f,interactorTransform.position.z);
            transform.localScale *= 3f;
        }
    }

    public void SetOut()
    {
        if(itemInteractor == null){
            gameObject.SetActive(false);
        }
    }
}
