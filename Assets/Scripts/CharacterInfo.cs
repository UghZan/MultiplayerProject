using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Controls character clothing
public class CharacterInfo : NetworkBehaviour
{
    //controls body part states
    //these are synced across clients and use hooks to update visuals
    [SyncVar(hook = nameof(SetBodyState))]
    public int bodyIndex;
    [SyncVar(hook = nameof(SetLegsState))]
    public int legsIndex;
    [SyncVar(hook = nameof(SetFeetState))]
    public int feetIndex;

    //contains possible state objects
    //0 is naked, 1+ is dressed
    [SerializeField] GameObject[] bodyStates;
    [SerializeField] GameObject[] legsStates;
    [SerializeField] GameObject[] feetStates;

    [SerializeField] GameObject canvas;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        //set UI active for this player only
        canvas.SetActive(true);
    }

    //command, because we change values on server for everyone to see
    [Command]
    public void UpdateClothingState(int partIndex)
    {
        //update corresponding body part
        switch (partIndex)
        {
            case 0:
                bodyIndex++;
                if (bodyIndex >= bodyStates.Length) bodyIndex = 0;
                break;
            case 1:
                legsIndex++;
                if (legsIndex >= legsStates.Length) legsIndex = 0;
                break;
            case 2:
                feetIndex++;
                if (feetIndex >= feetStates.Length) feetIndex = 0;
                break;
            default:
                Debug.LogError("Unknown body part");
                break;
        }
    }

    //next methods are called whenever synced variable is changed on clients
    //so the body parts are updated correctly
    void SetBodyState(int oldValue, int newValue)
    {
        bodyIndex = newValue;
        for (int i = 0; i < bodyStates.Length; i++)
        {
            if (i == bodyIndex)
                bodyStates[i].SetActive(true);
            else
                bodyStates[i].SetActive(false);
        }
    }
    void SetLegsState(int oldValue, int newValue)
    {
        legsIndex = newValue;
        for (int i = 0; i < legsStates.Length; i++)
        {
            if (i == legsIndex)
                legsStates[i].SetActive(true);
            else
                legsStates[i].SetActive(false);
        }
    }

    void SetFeetState(int oldValue, int newValue)
    {
        feetIndex = newValue;
        for (int i = 0; i < feetStates.Length; i++)
        {
            if (i == feetIndex)
                feetStates[i].SetActive(true);
            else
                feetStates[i].SetActive(false);
        }
    }
}
