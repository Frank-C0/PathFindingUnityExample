using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// Clase auxiliar para rastrear nodos en A*.
public class PathMarker
{
    public MapLocation location;
    public float G, H, F; // Costos: G (desde el inicio), H (heurístico), F = G + H.
    public GameObject marker;
    public PathMarker parent;

    public PathMarker(MapLocation loc, float g, float h, float f, GameObject mark, PathMarker par)
    {
        location = loc;
        G = g;
        H = h;
        F = f;
        marker = mark;
        parent = par;
    }

    public override bool Equals(object obj) =>
        obj is PathMarker marker && location.Equals(marker.location);

    public override int GetHashCode() => location.GetHashCode();
}

// Clase principal para implementar el algoritmo A* en el laberinto.
public class FindPathAStarSample : MonoBehaviour
{
    public Maze maze; // Referencia al laberinto.
    public Material closedMaterial, openMaterial; // Materiales para visualización.
    public GameObject start, end, pathP; // Prefabs para inicio, fin y el camino.

    private PathMarker startNode, goalNode, lastPos;
    private bool done = false, hasStarted = false;

    private List<PathMarker> open = new();    // Lista de nodos abiertos.
    private List<PathMarker> closed = new(); // Lista de nodos cerrados.

    // Elimina todos los marcadores visuales en la escena.
    void RemoveAllMarkers()
    {
        foreach (GameObject marker in GameObject.FindGameObjectsWithTag("marker"))
            Destroy(marker);
    }

    // Inicia la búsqueda A* seleccionando ubicaciones aleatorias para inicio y fin.
    void BeginSearch()
    {
        done = false;
        RemoveAllMarkers();

        // Busca celdas transitables y mezcla sus ubicaciones.
        List<MapLocation> locations = new();
        for (int z = 1; z < maze.depth - 1; z++)
            for (int x = 1; x < maze.width - 1; x++)
                if (maze.map[x, z] == 0)
                    locations.Add(new MapLocation(x, z));
        locations.Shuffle();

        // Configura el nodo inicial y el nodo objetivo.
        Vector3 startLocation = new Vector3(
            locations[0].x * maze.scale - maze.offsetX, 
            0.0f, 
            locations[0].z * maze.scale - maze.offsetZ
        );
        
        startNode = new PathMarker(locations[0], 0.0f, 0.0f, 0.0f, Instantiate(start, startLocation, Quaternion.identity), null);

        Vector3 endLocation = new Vector3(
            locations[1].x * maze.scale - maze.offsetX, 
            0.0f, 
            locations[1].z * maze.scale - maze.offsetZ
        );

        goalNode = new PathMarker(locations[1], 0.0f, 0.0f, 0.0f, Instantiate(end, endLocation, Quaternion.identity), null);

        open.Clear();
        closed.Clear();
        open.Add(startNode);
        lastPos = startNode;
    }

    // Realiza un paso del algoritmo A*.
    void Search(PathMarker thisNode)
    {
        if (thisNode.Equals(goalNode))
        {
            done = true;
            Debug.Log("Camino encontrado!");
            return;
        }

        foreach (MapLocation dir in maze.directions)
        {
            MapLocation neighbour = dir + thisNode.location;

            // Ignora vecinos que sean paredes o estén fuera de límites.
            if (maze.map[neighbour.x, neighbour.z] == 1 || neighbour.x < 1 || neighbour.x >= maze.width - 1 || neighbour.z < 1 || neighbour.z >= maze.depth - 1)
                continue;

            // Ignora vecinos que ya estén en la lista cerrada.
            if (IsClosed(neighbour)) continue;

            // Calcula costos G, H y F para el vecino.
            float g = Vector2.Distance(thisNode.location.ToVector(), neighbour.ToVector()) + thisNode.G;
            float h = Vector2.Distance(neighbour.ToVector(), goalNode.location.ToVector());
            float f = g + h;

            GameObject pathBlock = Instantiate(pathP, new Vector3(
                neighbour.x * maze.scale - maze.offsetX, 
                0.0f, 
                neighbour.z * maze.scale - maze.offsetZ
            ), Quaternion.identity);
            
            TextMesh[] values = pathBlock.GetComponentsInChildren<TextMesh>();
            values[0].text = $"G: {g:F2}";
            values[1].text = $"H: {h:F2}";
            values[2].text = $"F: {f:F2}";

            // Agrega el vecino a la lista abierta si no se actualiza un marcador existente.
            if (!UpdateMarker(neighbour, g, h, f, thisNode))
                open.Add(new PathMarker(neighbour, g, h, f, pathBlock, thisNode));
        }

        // Ordena la lista abierta por costo F y mueve el mejor nodo a la lista cerrada.
        open = open.OrderBy(p => p.F).ToList();
        PathMarker pm = open[0];
        closed.Add(pm);
        open.RemoveAt(0);
        pm.marker.GetComponent<Renderer>().material = closedMaterial;
        lastPos = pm;
    }

    // Actualiza un marcador si ya existe en la lista abierta.
    bool UpdateMarker(MapLocation pos, float g, float h, float f, PathMarker prt)
    {
        foreach (PathMarker p in open)
        {
            if (p.location.Equals(pos))
            {
                p.G = g;
                p.H = h;
                p.F = f;
                p.parent = prt;
                return true;
            }
        }
        return false;
    }

    // Verifica si una celda está en la lista cerrada.
    bool IsClosed(MapLocation marker) =>
        closed.Any(p => p.location.Equals(marker));

    // Visualiza el camino encontrado desde el nodo objetivo al nodo inicial.
    void GetPath()
    {
        RemoveAllMarkers();
        PathMarker current = lastPos;

        while (!startNode.Equals(current) && current != null)
        {
            Instantiate(pathP, new Vector3(
                current.location.x * maze.scale - maze.offsetX, 
                0.0f, 
                current.location.z * maze.scale - maze.offsetZ
            ), Quaternion.identity);
            current = current.parent;
        }

        Instantiate(pathP, new Vector3(
            startNode.location.x * maze.scale - maze.offsetX, 
            0.0f, 
            startNode.location.z * maze.scale - maze.offsetZ
        ), Quaternion.identity);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            BeginSearch();
            hasStarted = true;
        }

        if (hasStarted && Input.GetKeyDown(KeyCode.C) && !done)
        {
            Search(lastPos);
            Debug.Log("Buscando...");
        }

        if (Input.GetKeyDown(KeyCode.M)) GetPath();
    }
}