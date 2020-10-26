using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "BlockScripts/Dirt", fileName = nameof(DirtScript))]
public class DirtScript : BaseBlockScript
{
    public override void StartScript()
    {
        Init();
        //Debug.Log(_pos);
        CoroutineToInit = SetGrass;
        //coroutineToInit = SetGrass();
        //StartCoroutine(SetGrass());
    }

    public IEnumerator SetGrass()
    {
        while (true)
        {
            float n = Random.Range(0, 8);
            //Debug.Log(_pos);
            //Debug.Log("Coroutine Started");
            //yield return new WaitForSeconds(n);
            //Debug.Log("Tile: " + GetTile() + "/" + blockUnit.Data.tileVariables[0]);
            //Debug.Log("CanSet Grass: " + blockUnit.ChunkUnit.chunkBuilder.CanSetGrass(_pos));
            var builder = blockUnit.ChunkUnit.chunkBuilder;
            if (builder.CanSetGrass(_pos, blockUnit.Data))
            {
                //Debug.Log("Grass Seted: " + _pos);
                SetTile(blockUnit.Data.tileVariables[0]);
            }
            yield break;
        }
    }
}