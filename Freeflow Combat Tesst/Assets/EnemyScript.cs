using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    [SerializeField] private CharacterStats stats;
    [SerializeField] private PlayerController player;
    public CharacterState state = CharacterState.alive;

    private float currentHealth;
    private float maxHealth;

    private bool isAttackable = true;

    [SerializeField] private List<Material> materials;
    [SerializeField] private MeshRenderer meshRenderer;
    private Material currentMaterial;

    private void Awake()
    {
        maxHealth = stats.Health;
        currentHealth = maxHealth;
        currentMaterial = meshRenderer.material;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z));
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        StartCoroutine(DamageFlash());
    }

    public bool GetIsAttackable()
    {
        return isAttackable;
    }

    private IEnumerator DamageFlash()
    {
        meshRenderer.material = materials[1];
        yield return new WaitForSeconds(0.5f);
        if (currentHealth <= 0)
        {
            state = CharacterState.dead;
            isAttackable = false;
        }
        else
            meshRenderer.material = materials[0];

    }
}
