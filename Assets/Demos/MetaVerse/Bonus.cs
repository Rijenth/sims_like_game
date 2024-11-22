using Unity.VisualScripting;
using UnityEngine;

public class Bonus : MonoBehaviour {
    public LayerMask CollisionLayers;
    public int Points;
    private System.Random random = new System.Random();



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        ActivateBonus();
    }

    // Update is called once per frame
    void Update() {

    }

    private void ResetBonus() {
        gameObject.SetActive(true);
        Invoke("DesactivateBonus", random.Next(10, 15));
    }

    private void DesactivateBonus() {
        if (gameObject.activeSelf) {
            gameObject.SetActive(false);
            Invoke("ResetBonus", random.Next(10, 15));
        }
    }


    private void ActivateBonus() {

        Transform cube = transform.Find("Cube");
        if (cube != null) {
            switch (Points = random.Next(1, 25)) {
                case < 10:
                    cube.GetComponent<Renderer>().material.color = new Color(0, 0, 255);
                    break;
                case < 20:
                    cube.GetComponent<Renderer>().material.color = new Color(128, 0, 128);
                    break;
                case 25:
                    cube.GetComponent<Renderer>().material.color = new Color(255, 215, 0);
                    break;
            }
        }
        gameObject.SetActive(true);
        Invoke("DesactivateBonus", random.Next(10, 15));
    }


    private bool ShouldHandleObject(Collider other) {
        return (CollisionLayers.value & (1 << other.gameObject.layer)) > 0;
    }

    void OnTriggerEnter(Collider other) {
        if (!ShouldHandleObject(other)) { return; }
        LootLock bonusLock = new LootLock();

        if (bonusLock.IsLocked) {
            return;
        }
        bonusLock.Lock();
        CharacterScore cScore = other.gameObject.GetComponentInChildren<CharacterScore>();

        if (cScore != null) {
            cScore.AddScore(Points);
        }
        Debug.Log("Bonus Collected");
        gameObject.SetActive(false);
        Invoke("ResetBonus", random.Next(5, 15));
    }


    public class LootLock {

        public bool IsLocked { get; private set; }

        private readonly object _lock = new object();

        public void Lock() {
            lock (_lock) {
                IsLocked = true;
            }
        }
        public void Unlock() {
            lock (_lock) {
                IsLocked = false;
            }
        }
    }
}

