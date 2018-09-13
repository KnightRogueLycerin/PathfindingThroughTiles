using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using PopMatrix = System.Collections.Generic.Dictionary<
    int, System.Collections.Generic.Dictionary<int, Node>>;

public class Generation{
    
    public Pop EvolutionaryGeneration(BoardSetting bs)
    {
        return new Pop( bs.Width, bs.Height, bs.Range);
    }
    private List<Pop> frechBatch(BoardSetting bs)
    {
        return new List<Pop>();
    }
    private List<Pop> breedBatch(BoardSetting bs, List<Pop> stock)
    {
        return new List<Pop>();
    }
    private PopMatrix mixGenes(PopMatrix a, PopMatrix b)
    {
        return new PopMatrix();
    }
    private PopMatrix mutate(PopMatrix gene)
    {
        return new PopMatrix();
    }
}
