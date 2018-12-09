﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 public class ElementEffectIn : MonoBehaviour
 {

    public PlayerController player;
    [Header("Elements")]
    public GameObject electricity;
    public GameObject fire;
    public GameObject water;
    public Material wood;
    public GameObject wing;
    public GameObject smoke;
    public GameObject spark;
    public GameObject ash;
    [Header("Element Settings")]
    public int shocksToDie;
    public int secondsEachShock;
    public int fireDOT;


    public enum ElementType
        {
            Electricity,
            Spark,
            Fire,
            Smoke,
            Water,
            Wood,
            Wing,
            Hole,
            Ash
        }

    private Dictionary<ElementType, IEnumerator> currCoroutineEffects = new Dictionary<ElementType, IEnumerator>();

    /** Electricity **/
    /** 
     * When the player gains the electricity effect, it would shock him every /secondsEachShock/.
     * After numerous shocks /shocksToDie/, the player  would die.
     * If the player already have the effect of water, player would die immediatly :(
     * If the player already have the effect of wood, player nulifies the effect of shock :)
     * If the player have water & wood, player would die from shock.
     * 
     * WEAK - WATER
     * STRONG - WOOD
     **/
    public GameObject ElectricityCreate(Dictionary<ElementType, GameObject> elementsOnPlayer)
    {
        GameObject elementEffect = CreateEffect(ElementType.Electricity);

        // Start doing all the magic =]
        IEnumerator coroutine = ElectricityEffect(elementEffect);
        currCoroutineEffects.Add(ElementType.Electricity, coroutine);
        StartCoroutine(coroutine);
        

        if (elementsOnPlayer != null && elementsOnPlayer.ContainsKey(ElementType.Water))
        {
            StartCoroutine(AshCreate());
        }
        else if (elementsOnPlayer.ContainsKey(ElementType.Wood))
        { // The player is protected by wood, electricity doesn't take effect
            // Create after effect
            StartCoroutine(SparkCreate(ElementType.Electricity, elementEffect));
        }

        return elementEffect;
    }

    public IEnumerator ElectricityEffect(GameObject element)
    {
        for (int i = 0; i < shocksToDie; i++)
        {
            // Gain a random number between 1-secondsEachShock
            int shockTime = Random.Range(1, secondsEachShock);
            // Wait that x seconds
            yield return StartCoroutine(ShockWait(shockTime));
            // Stop movement for 1 second
            player.StopMovement();
            // TODO - Add shocked sound
            yield return StartCoroutine(ShockStop(1));
            // Start movement again
            player.StartMovement();
        }
        
        player.Die();
        

    }

    IEnumerator ShockWait(int time)
    {
        yield return new WaitForSeconds(time);
        Debug.Log("Shock wait, " + time);
    }

    IEnumerator ShockStop(int time)
    {
        yield return new WaitForSeconds(time);
        Debug.Log("Shock stop, " + time);
    }
    /** End Electricity **/


    /** Wood **/
    /** 
     * When the player gains the wood effect, it would protect the player from electricity.
     * If the player already have the effect of fire, player would die immediatly :(
     * If the player already have the effect of electricity, player nulifies the effect of shock :)
     * 
     * WEAK - FIRE
     * STRONG - ELECTRICITY
     **/
    public GameObject WoodCreate(Dictionary<ElementType, GameObject> elementsOnPlayer)
    {
        GameObject elementEffect = new GameObject();
        
        player.GetComponent<Renderer>().material = Material.Instantiate(wood);
        // Start doing all the magic =]
        WoodEffect(elementsOnPlayer);

        
        if (elementsOnPlayer != null && elementsOnPlayer.ContainsKey(ElementType.Fire))
        {
            StartCoroutine(AshCreate());
        }

        return elementEffect;
    }

    void WoodEffect( Dictionary<ElementType, GameObject> elementsOnPlayer)
    {
        if (elementsOnPlayer.ContainsKey(ElementType.Electricity))
        {
            // Create after effect
            StartCoroutine(SparkCreate(ElementType.Electricity, null));


        }
    }
    /** End Wood **/


    /** Fire **/
    /** 
     * When the player gains the fire effect, it would burn the player.
     * After fireDOT is over the player will die.
     * If the player already have the effect of wood, player would die immediatly :(
     * If the player already have the effect of water, player nulifies the effect of fire :)
     * If the player have wood & water, it would save the player from fire.
     * 
     * WEAK - WOOD
     * STRONG - WATER
     **/
    public GameObject FireCreate(Dictionary<ElementType, GameObject> elementsOnPlayer)
    {
        GameObject elementEffect = CreateEffect(ElementType.Fire);

        // Start doing all the magic =]
        FireEffect(elementsOnPlayer);

        
        if (elementsOnPlayer.ContainsKey(ElementType.Water))
        { // The player is soaked in water, the water evaporated and save from fire
            StartCoroutine(SmokeCreate(ElementType.Water, elementEffect, ElementType.Fire));
        }
        else if (elementsOnPlayer != null && elementsOnPlayer.ContainsKey(ElementType.Wood))
        {
            StartCoroutine(AshCreate());
        }
        return elementEffect;
    }

    void FireEffect(Dictionary<ElementType, GameObject> elementsOnPlayer)
    {
        IEnumerator coroutine = Burn();
        currCoroutineEffects.Add(ElementType.Fire, coroutine);
        StartCoroutine(coroutine);
    }

    IEnumerator Burn()
    {
        yield return new WaitForSeconds(fireDOT);

        player.Die();
    }
    /** End Fire **/


    /** Water **/
    /** 
     * When the player gains the water effect, it would soak the player.
     * If the player already have the effect of electricity, player would die immediatly :(
     * If the player already have the effect of fire, player nulifies the effect of fire :)
     * 
     * WEAK - ELECTRICITY
     * STRONG - FIRE
     **/
    public GameObject WaterCreate(Dictionary<ElementType, GameObject> elementsOnPlayer)
    {
        GameObject elementEffect = CreateEffect(ElementType.Water);
        
        
        if (elementsOnPlayer != null && elementsOnPlayer.ContainsKey(ElementType.Electricity))
        {
            StartCoroutine(AshCreate());
        }
        else if (elementsOnPlayer.ContainsKey(ElementType.Fire))
        { // The player is burning, the water evaporated and save from fire
           
            StartCoroutine(SmokeCreate(ElementType.Fire, elementEffect, ElementType.Water));
        }
        return elementEffect;
    }
    /** End Water **/


    /** Wing **/
    /** 
     * When the player gains the wing effect, it would protect the player from drop down from hole.
     * If the player already have the effect of fire, fire would eliminate the wings :(
     * 
     * WEAK - FIRE
     * STRONG - HOLE
     **/
    public GameObject WingCreate(Dictionary<ElementType, GameObject> elementsOnPlayer)
    {
       
        GameObject elementEffect = CreateEffect(ElementType.Wing);

        if (elementsOnPlayer != null && elementsOnPlayer.ContainsKey(ElementType.Fire))
        {
            player.CleanElement(ElementType.Wing);
        }

        return elementEffect;
    }
    /** End Wing **/


    /** Hole **/
    /** 
     * When the player takes the hole effect, the whole ground disappear
     * If the player don't the effect of wing, player would die immediatly :(
     *
     * WEAK - WING
     **/
    public void HoleCreate(Dictionary<ElementType, GameObject> elementsOnPlayer, GameObject ground)
    {

        if (elementsOnPlayer.ContainsKey(ElementType.Wing))
        { // The player have wings, won't fall down
            player.GetComponent<Rigidbody>().useGravity = false;
            // Player won't float up 
            player.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY;
        }

        // Start doing all the magic =]
        HoleEffect(ground);
    }

    void HoleEffect(GameObject ground)
    {
        ground.SetActive(false); // Muahahaha >:D
    }
    /** End Hole **/

    /** After States **/

    /* Smoke */
    /** Water <-> Fire **/
    IEnumerator SmokeCreate(ElementType cleanElementType, GameObject removeElement, ElementType removeElementType)
    {
        yield return new WaitForSeconds(1);
        // Disable contradiction elements
        player.CleanElement(cleanElementType);
        RemoveEffect(removeElementType, removeElement);

        // Create after effect
        GameObject elementEffect = CreateEffect(ElementType.Smoke);

        StartCoroutine(AfterEffect(elementEffect));
    }

    /* Spark */
    /** Wood <-> Electricity **/
    IEnumerator SparkCreate(ElementType elementType, GameObject removeElement)
    {
        yield return new WaitForSeconds(1);
        // Disable contradiction elements
        // Depend if it's already on the player or not
        if(removeElement == null)
        {
            player.CleanElement(elementType);
        } else
        {
            RemoveEffect(elementType, removeElement);
        }

        // Create after effect
        GameObject elementEffect = CreateEffect(ElementType.Spark);

        StartCoroutine(AfterEffect(elementEffect));
    }

    /* Ashes */
    /** Death **/
    IEnumerator AshCreate()
    {
        yield return new WaitForSeconds(1);

        
        // Create after effect
        GameObject elementEffect = CreateEffect(ElementType.Ash);

        StartCoroutine(AfterEffect(elementEffect));

        player.Die();
    }



    IEnumerator AfterEffect(GameObject effect)
    {
        yield return new WaitForSeconds(2f);
        effect.SetActive(false);
        Destroy(effect);
    }
    

    /** End After States **/


    /**
     * Create an effect depend on the element enum
     **/
    GameObject CreateEffect(ElementType element)
    {
        GameObject createElement = null;
        switch (element)
        {
            case ElementType.Electricity:
            createElement = electricity;
            break;
            case ElementType.Fire:
            createElement = fire;
            break;
            case ElementType.Water:
            createElement = water;
            break;
            case ElementType.Wing:
            createElement = wing;
            break;
            case ElementType.Smoke:
            createElement = smoke;
            break;
            case ElementType.Spark:
            createElement = spark;
            break;
            case ElementType.Ash:
            createElement = ash;
            break;
            
        }

        // Finally Create it ~!
        Vector3 vect = new Vector3(0, 0, 0);
        GameObject elementEffect = Instantiate(createElement, vect, Quaternion.identity);
        elementEffect.GetComponent<EffectCameraController>().player = player.gameObject;

        return elementEffect;
    }

    public void RemoveEffect(ElementType element, GameObject effect)
    {

        IEnumerator currCoroutine = null;
        switch (element)
        {
            case ElementType.Electricity:
                currCoroutineEffects.TryGetValue(ElementType.Electricity, out currCoroutine);
                StopCoroutine(currCoroutine);
                break;
            case ElementType.Fire:
                currCoroutineEffects.TryGetValue(ElementType.Fire, out currCoroutine);
                StopCoroutine(currCoroutine);
                break;
            case ElementType.Water:
                break;
            case ElementType.Wood:
                break;
            case ElementType.Wing:
                break;
        }

        effect.SetActive(false);
        Destroy(effect);
        // In case the shock stopped him
        player.StartMovement();
        
    }
}
