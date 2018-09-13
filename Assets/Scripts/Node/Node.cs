using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{   // Index of type
    public int type;
    // Cost to enter here
    public int entryCost;
    // Cost to get here
    public int pathCost;
    // Where here is
    public int column;
    public int row;
    // Constructor
    public Node(int t, int col, int r, int c)
    {
        type = t;
        entryCost = c;
        column = col;
        row = r;
        pathCost = int.MaxValue;
    }
    public Node(Node n)
    {
        entryCost = n.entryCost;
        pathCost = int.MaxValue;
        column = n.column;
        row = n.row;
    }
    // comparition methods
    public bool isSame(Node other)
    {   // Check if it is in same place
        if (other.column == column 
            && other.row == row)
            return true;
        else
            return false;
    }
}