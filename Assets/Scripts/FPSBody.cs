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
    }

    // Update is called once per frame
    void Update()
    {
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
        }

        if(other.gameObject.CompareTag("Sword"))
        {
            PickupSword(other.gameObject);
        }
    }

    void PickupSword(GameObject sword)
    {
        if (equippedSword != null)
            return;

        if (equip == null)
        {
            Debug.LogWarning("Equip is not assigned in Inspector!");
            return;
        }

        equippedSword = sword;

        sword.transform.SetParent(equip.transform);
        sword.transform.localPosition = Vector3.zero;
        sword.transform.localRotation = Quaternion.identity;

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
        isAttacking = true;
        animator.SetBool("isSwing", true);
        yield return new WaitForSeconds(0.5f);
        animator.SetBool("isSwing", false);
        isAttacking = false;
    }

    void GameOver()
    {
        if(Health <= 0 || TorchLight.isDead)
        {

        }
    }

}
