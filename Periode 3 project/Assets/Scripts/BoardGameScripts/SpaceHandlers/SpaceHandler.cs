using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;

public class SpaceHandler : MonoBehaviour
{
    public bool isYarnPlace;
    [SerializeField] protected GameObject coinPrefab;
    [SerializeField] private int coinAmount;
    [SerializeField] protected GameObject coinCollectCanvas;
    public enum SpaceKind
    {
        Normal,
        Special,
        YarnPlace
    }

    public SpaceKind spaceKind;

    public bool IsSpaceCountable()
    {
        if (spaceKind == SpaceKind.Normal) return true;

        return false;
    }

    public void SetSpaceAsYarnSpace()
    {
        isYarnPlace = true;
    }

    public virtual IEnumerator HandleLandedPlayer(Transform player)
    {
        if (coinAmount >= 0)
        {
            yield return StartCoroutine(CoinChange.Instance.WinCoins(player, coinAmount));
        }

        else
        {
            yield return StartCoroutine(CoinChange.Instance.LoseCoins(player, coinAmount));
        }
        
    }

    public virtual async Task HandleAsyncLandedPlayer(Transform player, int currentIndex)
    {
        await Task.Yield();
    }

    
}
