using System.Collections.Generic;
using UnityEngine;

// Clase que representa una ubicación en el laberinto.
public class MapLocation
{
    public int x, z;

    public MapLocation(int _x, int _z)
    {
        x = _x;
        z = _z;
    }

    // Convierte las coordenadas a un vector 2D.
    public Vector2 ToVector() => new Vector2(x, z);

    // Sobrecarga del operador suma para MapLocation.
    public static MapLocation operator +(MapLocation a, MapLocation b) =>
        new MapLocation(a.x + b.x, a.z + b.z);

    public override bool Equals(object obj) =>
        obj is MapLocation loc && x == loc.x && z == loc.z;

    public override int GetHashCode() => (x, z).GetHashCode();
}

// Clase principal para generar y manipular un laberinto.
public class Maze : MonoBehaviour
{
    public List<MapLocation> directions = new()
    {
        new MapLocation(1, 0), // Derecha
        new MapLocation(0, 1), // Arriba
        new MapLocation(-1, 0), // Izquierda
        new MapLocation(0, -1)  // Abajo
    };

    public int width = 30, depth = 30; // Dimensiones del laberinto
    public byte[,] map; // Representación del laberinto: 1 = pared, 0 = camino.
    public int scale = 6; // Escala de los objetos en Unity.

    [HideInInspector]
    public float offsetX, offsetZ;

    void Start()
    {
        offsetX = (width - 1) * scale / 2f;
        offsetZ = (depth - 1) * scale / 2f;

        InitialiseMap();
        Generate();
        DrawMap();
    }

    // Inicializa el mapa como un laberinto lleno de paredes.
    void InitialiseMap()
    {
        map = new byte[width, depth];
        for (int z = 0; z < depth; z++)
        {
            for (int x = 0; x < width; x++)
            {
                map[x, z] = 1;
            }
        }
    }

    // Genera el laberinto de forma básica (aleatorio).
    public virtual void Generate()
    {
        for (int z = 0; z < depth; z++)
        {
            for (int x = 0; x < width; x++)
            {
                map[x, z] = Random.Range(0, 100) < 50 ? (byte)0 : (byte)1;
            }
        }
    }

    // Dibuja el laberinto en Unity usando cubos.
    void DrawMap()
    {
        for (int z = 0; z < depth; z++)
        {
            for (int x = 0; x < width; x++)
            {
                if (map[x, z] == 1) // Solo se dibujan paredes.
                {
                    Vector3 position = new Vector3(x * scale - offsetX, 0, z * scale - offsetZ);
                    GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    wall.transform.localScale = Vector3.one * scale;
                    wall.transform.position = position;
                }
            }
        }
    }

    // Cuenta vecinos adyacentes que son caminos.
    public int CountSquareNeighbours(int x, int z)
    {
        int count = 0;
        if (x <= 0 || x >= width - 1 || z <= 0 || z >= depth - 1) return 5; // Bordes son tratados como paredes.
        if (map[x - 1, z] == 0) count++;
        if (map[x + 1, z] == 0) count++;
        if (map[x, z + 1] == 0) count++;
        if (map[x, z - 1] == 0) count++;
        return count;
    }

    // Cuenta vecinos diagonales que son caminos.
    public int CountDiagonalNeighbours(int x, int z)
    {
        int count = 0;
        if (x <= 0 || x >= width - 1 || z <= 0 || z >= depth - 1) return 5; // Bordes son tratados como paredes.
        if (map[x - 1, z - 1] == 0) count++;
        if (map[x + 1, z + 1] == 0) count++;
        if (map[x - 1, z + 1] == 0) count++;
        if (map[x + 1, z - 1] == 0) count++;
        return count;
    }

    // Cuenta todos los vecinos (adyacentes y diagonales).
    public int CountAllNeighbours(int x, int z) =>
        CountSquareNeighbours(x, z) + CountDiagonalNeighbours(x, z);
}