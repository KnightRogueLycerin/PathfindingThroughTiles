using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class BoardSetting
{
    [Header("Dimensions")]
    public int Width = 4;
    public int Height = 4;
    [Header("Scaling")]
    public float Distance = 1f;
    [Header("Camera Setting")]
    public float PositionCorrection = -.5f;
    public float SizeCorrection = 1f;
    [Header("Tiles")]
    public GameObject Tile = null;
    public GameObject Ends = null;
    [Header("Balance")]
    public int Generations = 1;
    public int Populations = 2;
    public int BreedFrom = 2;
    public int Budget = 100;
    public List<TileSettings> Range = new List<TileSettings>();
}

public class Manager : MonoBehaviour {
    [Header("Debug & Development")]
    public Text output;
    private Pop pop;

    [Header("Game Settings")]
    public BoardSetting Setting = new BoardSetting();

    // Archive
    private Dictionary<int, Dictionary<int, GameObject>> board 
        = new Dictionary<int, Dictionary<int, GameObject>>();
    private GameObject BudgetOut = null;
    private int origninalBudget = 0;
    
    // Initialize
    void Start () {
        pop = new Pop(
            Setting.Width,
            Setting.Height,
            Setting.Range
            );
        //printMatrix(pop); // for debuging
        createBoard(pop);
    }
    // Listening functions
    public void tileClick(TileManager source)
    {   // Turning off
        if (source.getState())
        {
            if(source.activeNeighbors() == 0 && source.Column == 0)
            {   // Edge case, okey to just turn off
                source.toggle();
                return;
            }
            else
            {   // Turn of source
                source.setState(false);
                // Need to ensure "pathing"
                List<TileManager> ToBeOn = new List<TileManager>();
                for(int i = 0; i < Setting.Height; i++)
                {   // Get initall "nodes" to find leagely on tiles
                    if (board[0][i].GetComponent<TileManager>().getState())
                    {
                        ToBeOn.Add(board[0][i].GetComponent<TileManager>());
                        searchForActiveNeighbors(
                            ToBeOn,
                            board[0][i].GetComponent<TileManager>());
                    }
                }
                // Turn off everything
                turnOffBoard();
                // Turn on 'saved' tiles
                foreach(TileManager tm in ToBeOn)
                {
                    tm.setState(true);
                }
                return;
            }
        }
        // Turning on
        if (source.Column == 0 || source.activeNeighbors() > 0)
        {   // first column or have at least on active neighbor
            if(Setting.Budget >= source.Settings.Cost)
            {   // Checking if player can aford to activate
                source.toggle();
                // Was it the wining tile?
                if (source.Column == Setting.Width - 1)
                    WIN();
            }
            return;
        }
    }
    // Special secret sause victory functions
    private void WIN()
    {
        if(output != null)
            output.text = "YOU WON!";
        Debug.Log("Loading  victory scene");
        SceneManager.LoadScene("Victory");
    }
    // Help functions for tile clicks
    public bool changeBudget(int value)
    {   // Sanity
        if(Setting.Budget >= -value)
        {   // More sanity
            if(origninalBudget < Setting.Budget)
            {   // Somehow the budget inflatet
                Debug.Log("Budget overflow!");
                return false;
            }
            else if(Setting.Budget < 0)
            {   // Somehow budget got into the negative
                Debug.Log("Budget under lower bound!");
                return false;
            }
            Setting.Budget += value;
            updateBudget();
            return true;
        }
        Debug.Log("Failed budget call: " + Setting.Budget + " >= " + value);
        return false;
    }
    private void updateBudget()
    {
        BudgetOut.GetComponent<Endblock>().setText("Budget: " + Setting.Budget + " / " + origninalBudget);
    }
    // Help functions for board controll
    public void resetBoard()
    {
        turnOffBoard();
    }
    private bool addNeighborToList(List<TileManager> l, TileManager tm)
    {   // Ensure that there are only uniqe TileManagers in the list
        foreach(TileManager n in l)
        {
            if(tm.Column == n.Column && tm.Row == n.Row)
            {   // Already added
                return false;
            }
        }
        l.Add(tm);
        return true;
    }
    private void searchForActiveNeighbors(List<TileManager> l, TileManager tm)
    {   // Recursively find path
        foreach (TileManager n in tm.Neighborhood)
        {
            if (n.getState())
            {   // If neighbor is on and not previusly added: add to search.
                if(addNeighborToList(l,n))
                    searchForActiveNeighbors(l, n);
            }
        }
    }
    private void turnOffBoard()
    {
        for (int i = 0; i < Setting.Width; i++)
        {
            for (int j = 0; j < Setting.Height; j++)
            {
                if(board[i][j].GetComponent<TileManager>().getState())
                    board[i][j].GetComponent<TileManager>().toggle();
            }
        }
    }
    // Creation
    private void createBoard(Pop p)
    {
        if (Setting.Tile == null || Setting.Ends == null)
        {   // Sanity
            Debug.Log("Prefab missing!");
            return;
        }
        // Set budget to shortet path
        Setting.Budget = p.getValue();
        origninalBudget = Setting.Budget;
        // Board setup
        SetStarEnd();
        SetBoard(p);
        SetNeighborhood();
        SetInfoBlocks();
        SetCamera();
        // Clear dev/debug text
        output.text = null;
    }
    // Creation functions
    private void SetStarEnd()
    {   // Add Endblocks
        board[-1] = new Dictionary<int, GameObject>();              // Start end
        Vector3 pos = new Vector3(
                    Setting.Distance * -1,
                    Setting.Distance * (Setting.Height - 1) / 2f,
                    0f);
        board[-1][0] = Instantiate(Setting.Ends, pos, Quaternion.identity);
        Vector3 rot = new Vector3(
            0f,
            0f,
            90f);
        Vector3 scale = new Vector3(
            Setting.Distance * (Setting.Height - 1),
            1f,
            1f);
        board[-1][0].GetComponent<Endblock>().setMesh(rot, scale, Setting.Range[0].On);
        board[-1][0].GetComponent<Endblock>().setText("Start");
        board[Setting.Width] = new Dictionary<int, GameObject>();   // Goal end
        pos = new Vector3(
                    Setting.Distance * Setting.Width,
                    Setting.Distance * (Setting.Height - 1) / 2f,
                    0f);
        board[Setting.Width][0] = Instantiate(Setting.Ends, pos, Quaternion.identity);
        rot = new Vector3(
            0f,
            0f,
            -90f);
        scale = new Vector3(
            Setting.Distance * (Setting.Height - 1),
            1f,
            1f);
        board[Setting.Width][0].GetComponent<Endblock>().setMesh(rot, scale, Setting.Range[0].Off);
        board[Setting.Width][0].GetComponent<Endblock>().setText("Goal");

    }
    private void SetBoard(Pop p)
    {   // Create board
        Dictionary<int, Dictionary<int, Node>> matrix = p.getGenes();
        for (int i = 0; i < Setting.Width; i++)
        {
            board[i] = new Dictionary<int, GameObject>();
            for (int j = 0; j < Setting.Height; j++)
            {
                Vector3 pos = new Vector3(
                    Setting.Distance * i,
                    Setting.Distance * j,
                    0f);
                board[i][j] = Instantiate(Setting.Tile, pos, Quaternion.identity);
                board[i][j].GetComponent<TileManager>().Settings.Cost =
                    matrix[i][j].entryCost;
                board[i][j].GetComponent<TileManager>().applyColorSettings(
                    Setting.Range[matrix[i][j].type - 1]);
                board[i][j].GetComponent<TileManager>().Overlord = this;
                board[i][j].GetComponent<TileManager>().Column = i;
                board[i][j].GetComponent<TileManager>().Row = j;
            }
        }
    }
    private void SetNeighborhood()
    {   // Set up the Neighborhood
        for (int i = 0; i < Setting.Width; i++)
        {
            for (int j = 0; j < Setting.Height; j++)
            {
                if (i != 0)
                {  // Not Far left
                    board[i][j].GetComponent<TileManager>().Neighborhood.Add(
                        board[i - 1][j].GetComponent<TileManager>());
                }
                if (i != Setting.Width - 1)  // Not Far Right
                {
                    board[i][j].GetComponent<TileManager>().Neighborhood.Add(
                        board[i + 1][j].GetComponent<TileManager>());
                }

                if (j != 0)  // Not Top
                {
                    board[i][j].GetComponent<TileManager>().Neighborhood.Add(
                        board[i][j - 1].GetComponent<TileManager>());
                }
                if (j != Setting.Height - 1)  // Not Bottom
                {
                    board[i][j].GetComponent<TileManager>().Neighborhood.Add(
                        board[i][j + 1].GetComponent<TileManager>());
                }
            }
        }

    }
    private void SetInfoBlocks()
    {// Add game information
        Vector3 pos = new Vector3();
        for (int i = 0; i < Setting.Range.Count; i++)
        {   // Add informative Tile
            board[Setting.Width + 1] = new Dictionary<int, GameObject>();
            pos = new Vector3(
                        (Setting.Distance * (Setting.Width - 1) / (Setting.Range.Count - 1)) * i,
                        Setting.Distance * (Setting.Height * 1f + 0.5f),
                        0f);
            board[Setting.Width + 1][0] = Instantiate(Setting.Ends, pos, Quaternion.identity);
            Vector3 rot = new Vector3(
                0f,
                0f,
                0f);
            Vector3 scale = new Vector3(
                1f,
                1f,
                1f);
            board[Setting.Width + 1][0].GetComponent<Endblock>().setMesh(rot, scale, Setting.Range[i].On);
            board[Setting.Width + 1][0].GetComponent<Endblock>().setText(Setting.Range[i].Cost.ToString());
        }
        // Create point block
        pos = new Vector3(
                        (Setting.Distance * (Setting.Width - 1) / 2f),
                        Setting.Distance * (Setting.Height * 1f + 2f),
                        0f);
        BudgetOut = Instantiate(Setting.Ends, pos, Quaternion.identity);
        BudgetOut.GetComponent<Endblock>().setMesh(
            new Vector3(    // Rotation
                0f,
                0f,
                0f),
            new Vector3(    // Scale
                5f,
                1f,
                1f),
            new Color(      // RGBa color
                1f,
                1f,
                1f,
                1f)
            );
        updateBudget();
    }
    private void SetCamera()
    {   // Move the camera
        Vector3 camPos = new Vector3(
            Setting.Distance * (Setting.Width / 2f) + Setting.PositionCorrection,
            Setting.Distance * (Setting.Height / 2f) + Setting.PositionCorrection,
            -10f);
        Camera.main.transform.position = camPos;
        if (Setting.Height > Setting.Width)
            Camera.main.orthographicSize = (Setting.Height / 2f) + Setting.SizeCorrection;
        else
            Camera.main.orthographicSize = (Setting.Width / 2f) + Setting.SizeCorrection;
    }
    // Print out pop information
    private void printMatrix(Pop p)
    {
        Dictionary<int, Dictionary<int, Node>> matrix = p.getGenes();
        string text = "Board by type\n";
        for (int r = 0; r < Setting.Height; r++)
        {
            for (int c = 0; c < Setting.Width; c++)
            {
                if (c != 0)
                    text += "  " + matrix[c][r].type.ToString();
                else
                    text += matrix[c][r].type.ToString();
            }
            text += "\n";
        }
        text += "\nBoard by entry cost\n";
        for (int r = 0; r < Setting.Height; r++)
        {
            for (int c = 0; c < Setting.Width; c++)
            {
                if (c != 0)
                    text += "  " + matrix[c][r].entryCost.ToString();
                else
                    text += matrix[c][r].entryCost.ToString();
            }
            text += "\n";
        }
        text += "\nBoard by pathing cost\n";
        for (int r = 0; r < Setting.Height; r++)
        {
            for (int c = 0; c < Setting.Width; c++)
            {
                if (c != 0)
                    text += "  " + matrix[c][r].pathCost.ToString();
                else
                    text += matrix[c][r].pathCost.ToString();
            }
            text += "\n";
        }
        text += "\nRange: 1-" + Setting.Range.Count + "\n";
        text += " Shortest path: " + pop.getValue();
        output.text = text;
    }
}
