using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;



public class PlayerController : MonoBehaviour {

    
    public float speed;
    public float startEffectDelay;
    public ElementEffectIn elementEffect;

    // Used to let know other elements which one is picked up now
    private Dictionary<ElementEffectIn.ElementType, GameObject> elementsOnPlayer;

    private Rigidbody rb;
    private int count;
    private int numPickUps;
    private GameObject previousEffect;
    


  
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        count = 0;
        // Get how many pickUp are depend on the script of rotator
        numPickUps = FindObjectsOfType<Rotator>().Length;

        SetCountText();

        elementsOnPlayer = new Dictionary<ElementEffectIn.ElementType, GameObject>();
    }
    
    private void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0, moveVertical);

        rb.AddForce(movement * speed * Time.fixedDeltaTime, ForceMode.Acceleration);

        if(this.transform.position.y < 0)
        {
            FindObjectOfType<GameManager>().GameOver();
        }
    }


    void OnTriggerEnter(Collider other)
    {
        string currTag = other.gameObject.tag;
        
        switch (currTag)
        {
            // Do a dictionary for each element have it's color
            case "Pick Up Water":
                PointTaken(other);
                CreateElectricOrb(ElementEffectIn.ElementType.Water);
                break;
            case "Pick Up Fire":
                PointTaken(other);
                CreateElectricOrb(ElementEffectIn.ElementType.Fire);
                break;
            case "Pick Up Electricity":
                PointTaken(other);
                CreateElectricOrb(ElementEffectIn.ElementType.Electricity);
                break;
            case "Pick Up Wood":
                PointTaken(other);
                CreateElectricOrb(ElementEffectIn.ElementType.Wood);
                break;
            case "Pick Up Wing":
                PointTaken(other);
                CreateElectricOrb(ElementEffectIn.ElementType.Wing);
                break;
            case "Pick Up Hole":
                PointTaken(other);
                CreateElectricOrb(ElementEffectIn.ElementType.Hole);
                break;
            case "Pick Up":
                PointTaken(other);
                break;
        }
    }

    void SetCountText()
    {
        //countText.text = "Count: " + count.ToString();
        
        if (count >= numPickUps)
        {
            FindObjectOfType<GameManager>().GameWon();
        }
    }

    void CreateElectricOrb(ElementEffectIn.ElementType element)
    {
        GameObject elementEffect = null;



        //If the player already have an effect on him, remove it
        //if (previousEffect != null)
        //{
        //    Destroy(previousEffect, startEffectDelay);
        //}
        Debug.Log("Picked up " + element);
        
        switch (element)
        {
            case ElementEffectIn.ElementType.Electricity:
                elementEffect = this.elementEffect.ElectricityCreate(elementsOnPlayer);
                break;
            case ElementEffectIn.ElementType.Fire:
                elementEffect = this.elementEffect.FireCreate(elementsOnPlayer);
                break;
            case ElementEffectIn.ElementType.Water:
                elementEffect = this.elementEffect.WaterCreate(elementsOnPlayer);
                break;
            case ElementEffectIn.ElementType.Wood:
                elementEffect = this.elementEffect.WoodCreate(elementsOnPlayer);
                break;
            case ElementEffectIn.ElementType.Wing:
                elementEffect = this.elementEffect.WingCreate(elementsOnPlayer);
                break;
            case ElementEffectIn.ElementType.Hole:
                GameObject ground = GameObject.FindGameObjectWithTag("Ground");
                this.elementEffect.HoleCreate(elementsOnPlayer, ground); // Should create a hole in the ground
                break;
        }


        //Didn't find any orb with that type of color
        if (elementEffect == null)
        {
            return;
        }

        // Save the refference for the next time it will get another effect and cancel the last one
        previousEffect = elementEffect;

        elementsOnPlayer.Add(element, elementEffect);
    }

    void PointTaken(Collider other)
    {
        other.gameObject.SetActive(false);
        count++;
        SetCountText();
    }

    public void StopMovement()
    {
        this.enabled = false;
    }

    public void StartMovement()
    {
        this.enabled = true;
    }

    public void CleanElement(ElementEffectIn.ElementType element)
    {
        GameObject elementRemove;
        // Get the element to clean/remove, make it disappear and remove from the game
        elementsOnPlayer.TryGetValue(element, out elementRemove);
        elementEffect.RemoveEffect(element, elementRemove);
        
        
        elementsOnPlayer.Remove(element);
    }

    // Game over Phase 
    public void Die()
    {
        this.gameObject.SetActive(false);
        CleanAfterDeath();
        FindObjectOfType<GameManager>().GameOver();

    }

    void CleanAfterDeath()
    {
        foreach(KeyValuePair<ElementEffectIn.ElementType, GameObject> effect in elementsOnPlayer)
        {
            if (effect.Value != null)
            {
                effect.Value.SetActive(false);
            }
        }
        
    }
    


    
}
