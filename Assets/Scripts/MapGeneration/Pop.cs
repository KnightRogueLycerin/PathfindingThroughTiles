using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/********************************************************************************\
**  Used to describe the board without being conected nodes or to prefabs       **
**  No point in generating the prefabs until we know we got someting worthwhile **
**          Notation: PopMatrix[int Column][int Row] = int Value                **
\********************************************************************************/
using PopMatrix = System.Collections.Generic.Dictionary<
    int, System.Collections.Generic.Dictionary<int, Node>>;

public class Pop {
    private int myWidth = 0;
    private int myHeight = 0;
    
    protected PopMatrix genes = new PopMatrix();
    public PopMatrix getGenes()
    {   // Genes = 2D int matrix
        return genes;
    }

    protected int value = 0;
    public int getValue()
    {   // Value = shortest path
        return value;
    }

    public Pop(int width, int height, List<TileSettings> range)
    {   // Fresh pop
        randomFill(width, height, range);
        learnSelf();
    }
	public Pop(PopMatrix g)
    {   // Breed pop
        genes = g;
        learnSelf();
    }
    private void randomFill(int width, int height, List<TileSettings> range)
    {   // Fill the PopMatrix with random Value
        for (int i = 0; i < width; i++)
        {
            genes[i] = new Dictionary<int, Node>();
            for (int j = 0; j < height; j++)
            {
                int rng = Random.Range(1, range.Count + 1);
                genes[i][j] = new Node(rng, i, j, range[rng-1].Cost);
            }
        }
    }
    private void learnSelf()
    {
        // Easy accsess dimention
        myHeight = genes[0].Count;  // Each row is the same lenght
        myWidth = genes.Count;
        // Self evaluation
        calucalteValue();
    }
    /********************************************\
    **  Using BFS to find the shortest path,    **
    **  however any node in fisrt column counts **
    **  as a initial node and any in the last   **
    **  column counts as the goal               **
    \********************************************/
    private void calucalteValue()   // TODO: Finish!
    {   // Set up
        Queue<Node> queue = new Queue<Node>();  // FIFO, to be checked
        List<Node> visted = new List<Node>();   // Where we been
        // initialize queue with first column
        for(int root = 0; root < genes[0].Count; root++)
        {   // Asaign initial pathCost
            genes[0][root].pathCost = genes[0][root].entryCost;
            // Enqueue
            queue.Enqueue(genes[0][root]);
            // Add to visited
            visted.Add(genes[0][root]);
            // BFS-esq
            while (queue.Count != 0)
            {   // Dequeue
                Node n = queue.Dequeue();
                visted.Add(n);
                // Check neighbours
                for (int i = 0; i < 4; i++)
                {
                    int currentSpending = n.pathCost;   // Get cost to get here
                    Node other = null;
                    switch (i)
                    {   // Fetch neighbour, with boundry checks
                        case 0: // Up
                            if (n.column - 1 >= 0)
                            {
                                other = genes[n.column - 1][n.row];
                            }
                            break;
                        case 1: // Down
                            if (n.column + 1 < myWidth)
                            {
                                other = genes[n.column + 1][n.row];
                            }
                            break;
                        case 2: // Left
                            if (n.row - 1 >= 0)
                            {
                                other = genes[n.column][n.row - 1];
                            }
                            break;
                        case 3: // Rigth
                            if (n.row + 1 < myHeight)
                            {
                                other = genes[n.column][n.row + 1];
                            }
                            break;
                    }
                    if (other != null)
                    {   // Get cost to visist node
                        currentSpending += other.entryCost;
                        // Check if previously vistited
                        bool found = false;
                        foreach (Node v in visted)
                        {
                            if (v.isSame(other))
                            {   // We fount it
                                found = true;
                                // TODO: FIX!
                                if (v.pathCost > currentSpending)
                                {   // And we found a cheaper way here
                                    v.pathCost = currentSpending;
                                    queue.Enqueue(v);
                                }
                                break;  // No need to continue
                            }
                        }
                        if (!found)
                        {   // Not been visisted before
                            other.pathCost = currentSpending;
                            queue.Enqueue(other);
                        }
                    }
                }

            }
        }
        // Search in the last column for lowet path cost
        int val = genes[myWidth - 1][0].pathCost;
        for (int i = 1; i < myHeight; i++)
        {   // Find shortest path
            if (genes[myWidth - 1][i].pathCost < val)
                val = genes[myWidth - 1][i].pathCost;
        }
        value = val;
    }
}
