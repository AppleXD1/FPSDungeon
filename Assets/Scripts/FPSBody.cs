using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class FPSBody : MonoBehaviour
{
    public PlayerInput playerInput;
    private InputAction LeftClick;
    public float Health = 10;
    public float MaxHealth = 10;
    public GameObject equip;
    public bool gotKey;
    public bool lvlComplete;
    public bool hasHit;
    public bool isEquiped;
    public GameObject equippedSword;
    public bool isAttacking;

    public Animator animator;
    public TorchLight TorchLight;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (equip != null)
            animator = equip.GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
        TorchLight = GameObject.FindWithTag("Torch").GetComponent<TorchLight>();
        LeftClick = playerInput.actions.FindAction("Attack");
        lvlComplete = false;
    }

    // Update is called once per frame
    void Update()
    {
        isEquiped = false;
        if (equippedSword != null && LeftClick.triggered && !isAttacking)
        {
            StartCoroutine(SwingAttack());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Key"))
        {
            gotKey = true;
            Destroy(other.gameObject);
        }

        if (other.gameObject.CompareTag("Door") && gotKey)
        {
            Debug.Log("level complete");
            lvlComplete=true;
        }

        if(other.gameObject.CompareTag("Sword") && !isEquiped)
        {
            PickupSword(other.gameObject);
            isEquiped = true;
        }
    }

    void PickupSword(GameObject sword)
    {

        equippedSword = sword;

        sword.transform.SetParent(equip.transform);
        sword.transform.localPosition = new Vector3(0.2f, -0.15f, 0.35f);
        sword.transform.localRotation = Quaternion.Euler(0f, 45f, 45f);

        Rigidbody rb = sword.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.useGravity = false;
            rb.isKinematic = true;
        }

        Collider[] cols = sword.GetComponentsInChildren<Collider>();
        foreach (Collider c in cols)
        {
            if (!c.isTrigger)
                c.enabled = false;
        }
    }



    IEnumerator SwingAttack()
    {
        hasHit = false;
        isAttacking = true;
        animator.SetBool("isSwing", true);
        yield return new WaitForSeconds(0.5f);
        animator.SetBool("isSwing", false);
        isAttacking = false;
    }

    

}
